using System;
using System.Collections.Generic;
using System.Text;
using Faithlife.DockerShim.Services;

namespace Faithlife.DockerShim.Tests.Util
{
	public sealed class StubExitProcessService : IExitProcessService
	{
		public int ExitCode { get; set; }

		public void Exit() { }
	}
}
