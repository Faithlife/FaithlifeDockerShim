using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Faithlife.DockerShim.Logging;
using Faithlife.DockerShim.Services;
using Microsoft.Extensions.Logging;

namespace Faithlife.DockerShim.Tests.Util
{
	public sealed class StubbedSettings
	{
		public TimeSpan StubMaximumRuntime { get; set; } = Timeout.InfiniteTimeSpan;

		public TimeSpan StubExitTimeout { get; set; } = TimeSpan.FromSeconds(1);

		public StubLoggerProvider StubLoggerProvider { get; set; } = null;

		public Func<LogEvent, string> Formatter { get; set; } = null;

		public StubStringLog StubStringLog { get; } = new StubStringLog(new StringWriter());

		public StubExitProcessService StubExitProcessService { get; } = new StubExitProcessService();

		public StubSignalService StubSignalService { get; } = new StubSignalService();

		public static implicit operator DockerShimSettings(StubbedSettings stubs)
		{
			ILoggerFactory loggerFactory = null;
			if (stubs.StubLoggerProvider != null)
			{
				loggerFactory = new LoggerFactory();
				loggerFactory.AddProvider(stubs.StubLoggerProvider);
			}

			var result = new DockerShimSettings(stubs.StubStringLog, null, stubs.Formatter)
			{
				MaximumRuntime = stubs.StubMaximumRuntime,
				ExitTimeout = stubs.StubExitTimeout,
				ExitProcessService = stubs.StubExitProcessService,
				SignalService = stubs.StubSignalService,
			};
			if (loggerFactory != null)
				result.LoggerFactory = loggerFactory;

			return result;
		}
	}
}
