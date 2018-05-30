using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Faithlife.DockerShim.Logging;
using Faithlife.DockerShim.Services;
using Microsoft.Extensions.Logging;

namespace Faithlife.DockerShim
{
	/// <summary>
	/// Settings used to control DockerShim behavior.
	/// </summary>
	public sealed class DockerShimSettings
	{
		/// <summary>
		/// Creates a new instance of the settings class with default settings applied.
		/// </summary>
		/// <param name="loggerIsEnabledFilter">The filter used by the <see name="LoggerFactory"/>. Defaults to a filter that always logs all messages.</param>
		/// <param name="loggerFormatter">The formatter used by the <see name="LoggerFactory"/>. Defaults to a formatter that writes log messages one message per line.</param>
		public DockerShimSettings(Func<string, LogLevel, bool> loggerIsEnabledFilter = null, Func<LogEvent, string> loggerFormatter = null)
		: this(null, loggerIsEnabledFilter, loggerFormatter)
		{
		}

		/// <summary>
		/// The maximum amount of time the application will run until it is requested to exit. Defaults to infinite, but most apps should use a non-infinite time.
		/// </summary>
		public TimeSpan MaximumRuntime { get; set; } = Timeout.InfiniteTimeSpan;

		/// <summary>
		/// The core logging factory used by all structured logging. Defaults to a logging factory with a single provider that writes formatted text to the console.
		/// </summary>
		public ILoggerFactory LoggerFactory { get; set; }

		/// <summary>
		/// The amount of time application code has after it is requested to exit, before the process forcibly exits. Defaults to 10 seconds.
		/// </summary>
		public TimeSpan ExitTimeout { get; set; } = TimeSpan.FromSeconds(10);

		/// <summary>
		/// The amount of random fluction in <see cref="MaximumRuntime"/>.
		/// E.g., <c>0.10</c> is a 10% change; if <see cref="MaximumRuntime"/> is 30 minutes, then the actual maximum runtime would be a random value between 27 and 33 minutes.
		/// Defaults to 0.10 (10%).
		/// </summary>
		public double RandomMaximumRuntimeRelativeDelta { get; set; } = 0.10;

		/// <summary>
		/// A method that parses text written to stdout and logs it. Defaults to writing an info message for each message written to stdout.
		/// </summary>
		public Action<string, ILoggerFactory> StdoutParser { get; set; } = (message, provider) => provider.CreateLogger("App").LogInformation(message);

		internal DockerShimSettings(IStringLog consoleLog, Func<string, LogLevel, bool> loggerIsEnabledFilter, Func<LogEvent, string> loggerFormatter)
		{
			var loggerFactory = new LoggerFactory();
			consoleLog = consoleLog ?? new TextWriterStringLog(Console.Out);
			loggerFormatter = loggerFormatter ?? DockerShimFormatters.FormattedText;
			loggerIsEnabledFilter = loggerIsEnabledFilter ?? ((_, __) => true);
			var loggerProvider = new DockerShimLoggerProvider(consoleLog, loggerFormatter, loggerIsEnabledFilter);
			loggerFactory.AddProvider(loggerProvider);
			LoggerFactory = loggerFactory;

			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				SignalService = new WindowsSignalService();
			else
				SignalService = new UnixSignalService();
		}

		/// <summary>
		/// Service that exits the entire process.
		/// </summary>
		internal IExitProcessService ExitProcessService { get; set; } = new ExitProcessService();

		/// <summary>
		/// Service that hooks shutdown signals sent to the process.
		/// </summary>
		internal ISignalService SignalService { get; set; }
	}
}
