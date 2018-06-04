using System;
using System.Collections.Generic;
using System.Text;

namespace Faithlife.DockerShim.Services
{
	internal sealed class ExitProcessService : IExitProcessService
	{
		public int ExitCode
		{
			get => Environment.ExitCode;
			set => Environment.ExitCode = value;
		}

		public void Exit() => Environment.Exit(ExitCode);
	}
}
