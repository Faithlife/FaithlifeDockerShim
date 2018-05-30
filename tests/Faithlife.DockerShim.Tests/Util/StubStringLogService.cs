using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Faithlife.DockerShim.Logging;

namespace Faithlife.DockerShim.Tests.Util
{
	public sealed class StubStringLog : IStringLog
	{
		public StubStringLog(StringWriter stringWriter)
		{
			StringWriter = stringWriter;
		}

		public List<string> Messages { get; } = new List<string>();

		public void WriteLine(string message)
		{
			Messages.Add(message);
			StringWriter.WriteLine(message);
		}

		public StringWriter StringWriter { get; }
	}
}
