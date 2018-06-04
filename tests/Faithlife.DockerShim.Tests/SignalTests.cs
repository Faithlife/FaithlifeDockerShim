using System;
using System.Threading;
using System.Threading.Tasks;
using Faithlife.DockerShim.Tests.Util;
using NUnit.Framework;

// ReSharper disable ImplicitlyCapturedClosure
// ReSharper disable MethodSupportsCancellation

namespace Faithlife.DockerShim.Tests
{
	public class SignalTests
	{
		[Test]
		public void Signal_CancelsExitRequested()
		{
			var settings = new StubbedSettings();

			var ready = new ManualResetEvent(false);
			var finish = new ManualResetEvent(false);
			var cancellationTokenSourceSet = new ManualResetEvent(false);
			CancellationToken exitRequested;
			Task.Run(() => DockerShimRunner.Main(settings, context =>
			{
				exitRequested = context.ExitRequestedToken;
				ready.Set();
				finish.WaitOne();
			}));
			ready.WaitOne();
			exitRequested.Register(() => cancellationTokenSourceSet.Set());
			Task.Run(() => settings.StubSignalService.Invoke("testSignal"));

			cancellationTokenSourceSet.WaitOne();
			Assert.That(exitRequested.IsCancellationRequested, Is.True);

			finish.Set();
		}

		[Test]
		public async Task Signal_BlocksHandlerUntilAppExits()
		{
			var settings = new StubbedSettings();

			var ready = new ManualResetEvent(false);
			var finish = new ManualResetEvent(false);
			var mainTask = Task.Run(() => DockerShimRunner.Main(settings, context =>
			{
				ready.Set();
				finish.WaitOne();
			}));
			ready.WaitOne();

			// The signal handler should block until mainTask completes.
			var signalTask = Task.Run(() => settings.StubSignalService.Invoke("testSignal"));
			var signalTaskCompleted = signalTask.Wait(TimeSpan.FromMilliseconds(200));
			Assert.That(signalTaskCompleted, Is.False);

			finish.Set();
			await mainTask;
			await signalTask;
		}

		[Test]
		public async Task ExitRequestedCancellationException_WhenExitRequested_IsTreatedAsNormalException()
		{
			var settings = new StubbedSettings();
			var ready = new ManualResetEventSlim();
			var task = Task.Run(() => DockerShimRunner.Main(settings, async context =>
			{
				ready.Set();
				await Task.Delay(Timeout.InfiniteTimeSpan, context.ExitRequestedToken);
			}));
			ready.Wait();

			var _ = Task.Run(() => settings.StubSignalService.Invoke("testSignal"));

			await task;
			Assert.That(settings.StubExitProcessService.ExitCode, Is.Zero);
		}

		[Test]
		public async Task LinkedCancellationException_WhenExitRequested_IsTreatedAsNormalException()
		{
			var settings = new StubbedSettings();
			var ready = new ManualResetEventSlim();
			var task = Task.Run(() => DockerShimRunner.Main(settings, async context =>
			{
				ready.Set();
				var otherCts = new CancellationTokenSource();
				var cts = CancellationTokenSource.CreateLinkedTokenSource(context.ExitRequestedToken, otherCts.Token);
				await Task.Delay(Timeout.InfiniteTimeSpan, cts.Token);
			}));
			ready.Wait();

			var _ = Task.Run(() => settings.StubSignalService.Invoke("testSignal"));

			await task;
			Assert.That(settings.StubExitProcessService.ExitCode, Is.Zero);
		}

		[Test]
		public void Signal_WhenApplicationCodeTakesTooLong_ExitsProcessWithCode66()
		{
			var settings = new StubbedSettings();

			var ready = new ManualResetEventSlim();
			var finish = new ManualResetEventSlim();
			Task.Run(() => DockerShimRunner.Main(settings, context =>
			{
				ready.Set();
				finish.Wait();
			}));
			ready.Wait();

			Task.Run(() => settings.StubSignalService.Invoke("testSignal"));
			Thread.Sleep(settings.StubExitTimeout * 2);
			Assert.That(settings.StubExitProcessService.ExitCode, Is.EqualTo(66));

			finish.Set();
		}
	}
}
