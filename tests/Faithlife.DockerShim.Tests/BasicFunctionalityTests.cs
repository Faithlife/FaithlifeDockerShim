using System;
using Faithlife.DockerShim.Tests.Util;
using NUnit.Framework;

namespace Faithlife.DockerShim.Tests
{
	public class BasicFunctionalityTests
	{
		[Test]
		public void Main_ExecutesAction()
		{
			var executed = false;
			DockerShimRunner.Main(new StubbedSettings(), _ => { executed = true; });
			Assert.That(executed, Is.EqualTo(true));
		}

		[Test]
		public void ExitCode_WhenActionSpecifiesReturnValue_IsActionReturnValue()
		{
			var settings = new StubbedSettings();
			DockerShimRunner.Main(settings, _ => 13);
			Assert.That(settings.StubExitProcessService.ExitCode, Is.Not.Null);
			Assert.That(settings.StubExitProcessService.ExitCode, Is.EqualTo(13));
		}

		[Test]
		public void ExitCode_WhenActionHasNoReturnValue_IsZero()
		{
			var settings = new StubbedSettings();
			DockerShimRunner.Main(settings, _ => { });
			Assert.That(settings.StubExitProcessService.ExitCode, Is.Not.Null);
			Assert.That(settings.StubExitProcessService.ExitCode, Is.EqualTo(0));
		}

		[Test]
		public void Action_ThrowsException_IsTranslatedIntoExitCode64()
		{
			var settings = new StubbedSettings();
			DockerShimRunner.Main(settings, (Action<DockerShimContext>) (_ => throw new InvalidOperationException()));
			Assert.That(settings.StubExitProcessService.ExitCode, Is.Not.Null);
			Assert.That(settings.StubExitProcessService.ExitCode, Is.EqualTo(64));
		}

		[Test]
		public void Action_ThrowsException_IsLogged()
		{
			var settings = new StubbedSettings();
			DockerShimRunner.Main(settings, (Action<DockerShimContext>) (_ => throw new InvalidOperationException("Test message")));
			Assert.That(settings.StubStringLog.Messages, Has.Some.Contains("Test message"));
		}

		[Test]
		public void Action_ThrowsInnerException_IsLogged()
		{
			var settings = new StubbedSettings();
			DockerShimRunner.Main(settings,
				(Action<DockerShimContext>) (_ =>
					throw new InvalidOperationException("Outer message", new InvalidOperationException("Test message"))));
			Assert.That(settings.StubStringLog.Messages, Has.Some.Contains("Test message"));
		}
	}
}
