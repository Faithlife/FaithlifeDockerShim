using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Faithlife.DockerShim.Services
{
	/// <summary>
	/// Provides a way to handle process shutdown signals.
	/// </summary>
	internal interface ISignalService
	{
		/// <summary>
		/// Adds a handler to process shutdown signals. The handler receives the signal name; this is only for logging purposes. There is not currently a way to remove a handler.
		/// </summary>
		/// <param name="handler">The handler to add.</param>
		void AddHandler(Action<string> handler);
	}
}
