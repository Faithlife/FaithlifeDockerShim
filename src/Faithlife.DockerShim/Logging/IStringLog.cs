namespace Faithlife.DockerShim.Logging
{
	/// <summary>
	/// Service that writes strings to a log.
	/// </summary>
	internal interface IStringLog
	{
		/// <summary>
		/// Writes a single message to the log.
		/// </summary>
		/// <param name="message">The message to write.</param>
		void WriteLine(string message);
	}
}
