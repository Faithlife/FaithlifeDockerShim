using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;

namespace Faithlife.DockerShim.Services
{
	internal sealed class UnixSignalService : ISignalService
	{
		public void AddHandler(Action<string> handler)
		{
			Console.CancelKeyPress += (_, args) =>
			{
				if (args.SpecialKey == ConsoleSpecialKey.ControlC)
				{
					args.Cancel = true;
					handler("SIGINT");
				}
			};

			// See https://github.com/dotnet/coreclr/issues/7394 / http://www.webcitation.org/6z4UQa7nG
			var assemblyLoadContext = AssemblyLoadContext.GetLoadContext(Assembly.GetExecutingAssembly());
			assemblyLoadContext.Unloading += _ => handler("SIGTERM");
		}
	}
}
