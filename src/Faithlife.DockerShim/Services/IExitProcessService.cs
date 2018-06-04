namespace Faithlife.DockerShim.Services
{
	/// <summary>
	/// Service that exits the entire process.
	/// </summary>
	internal interface IExitProcessService
	{
		/// <summary>
		/// Gets or sets the exit code for the process.
		/// </summary>
		int ExitCode { get; set; }

		/// <summary>
		/// Exits the process with exit code <see cref="ExitCode"/>. This method may or may not return.
		/// </summary>
		void Exit();
	}
}
