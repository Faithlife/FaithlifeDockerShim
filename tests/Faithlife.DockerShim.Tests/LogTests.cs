using System;
using System.Collections.Generic;
using System.IO;
using Faithlife.DockerShim.Logging;
using Faithlife.DockerShim.Tests.Util;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace Faithlife.DockerShim.Tests
{
	public class LogTests
	{
		[Test]
		public void ExceptionLogging_EscapesEolCharacters()
		{
			var settings = new StubbedSettings();
			DockerShimRunner.Main(settings, (Action<DockerShimContext>) (_ => throw new InvalidOperationException("test message")));
			Assert.That(settings.StubStringLog.Messages, Has.Some.Contains("test message"));
			Assert.That(settings.StubStringLog.Messages, Has.None.Contains("\n"));
		}

		[Test]
		public void EscapedConsoleStdout_EscapesEolCharacters()
		{
			var stdout = new StringWriter();
			var escapedStdout = Escaping.CreateEscapingTextWriter(stdout);
			escapedStdout.WriteLine("Test\nMessage");
			Assert.That(stdout.ToString(), Is.EqualTo("Test\\nMessage" + Environment.NewLine));
		}

		[Test]
		public void CustomFormatter_GetsStructuredState()
		{
			var actualState = new List<KeyValuePair<string, object>>();

			string TestFormatter(LogEvent logEvent)
			{
				actualState.AddRange(logEvent.State);
				return logEvent.Message;
			}

			var settings = new StubbedSettings
			{
				Formatter = TestFormatter,
			};
			var timestamp = new DateTime(2018, 06, 01, 0, 0, 0, DateTimeKind.Utc);
			DockerShimRunner.Main(settings,
				context => context.LoggerFactory.CreateLogger("testlogger")
					.LogInformation("Hello from {source} at {timestamp:o}!", "sourceValue", timestamp));

			Assert.That(settings.StubStringLog.Messages,
				Has.Some.Contains("Hello from sourceValue at 2018-06-01T00:00:00.0000000Z!"));
			Assert.That(actualState, Has.Some.EqualTo(new KeyValuePair<string, object>("source", "sourceValue")));
			Assert.That(actualState, Has.Some.EqualTo(new KeyValuePair<string, object>("timestamp", timestamp)));
		}

		[Test]
		public void CustomLogger_IsUsedForDockerShimLogging()
		{
			var settings = new StubbedSettings
			{
				StubLoggerProvider = new StubLoggerProvider(),
			};
			var exception = new InvalidOperationException("test message");

			DockerShimRunner.Main(settings, (Action<DockerShimContext>) (context => throw exception));

			Assert.That(settings.StubStringLog.Messages, Is.Empty);
			Assert.That(settings.StubLoggerProvider.Messages,
				Has.Some.Matches(Has.Property("CategoryName").EqualTo("DockerShim") & Has.Property("Exception").EqualTo(exception)));
		}

		[Test]
		public void CustomLogger_IsUsedForApplicationLogging()
		{
			var settings = new StubbedSettings
			{
				StubLoggerProvider = new StubLoggerProvider(),
			};
			var timestamp = new DateTime(2018, 06, 01, 0, 0, 0, DateTimeKind.Utc);

			DockerShimRunner.Main(settings,
				context => context.LoggerFactory.CreateLogger("testlogger")
					.LogInformation("Hello from {source} at {timestamp:o}!", "sourceValue", timestamp));

			Assert.That(settings.StubStringLog.Messages, Is.Empty);
			Assert.That(settings.StubLoggerProvider.Messages, Has.Some.Matches(
				Has.Property("CategoryName").EqualTo("testlogger") &
				Has.Property("Message").EqualTo("Hello from sourceValue at 2018-06-01T00:00:00.0000000Z!") &
				Has.Property("State").Some.EqualTo(new KeyValuePair<string, object>("source", "sourceValue")) &
				Has.Property("State").Some.EqualTo(new KeyValuePair<string, object>("timestamp", timestamp))));
		}

		[Test]
		public void CustomLogger_IsUsedForLoggingConsoleOutput()
		{
			var settings = new StubbedSettings
			{
				StubLoggerProvider = new StubLoggerProvider(),
			};

			DockerShimRunner.Main(settings, context => context.LoggingConsoleStdout.WriteLine("Hello there"));

			Assert.That(settings.StubStringLog.Messages, Is.Empty);
			Assert.That(settings.StubLoggerProvider.Messages, Has.Some.Matches(Has.Property("Message").EqualTo("Hello there")));
		}
	}
}
