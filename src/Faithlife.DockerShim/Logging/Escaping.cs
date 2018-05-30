using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Faithlife.DockerShim.Logging
{
	/// <summary>
	/// Utility class to backslash-escape EOL characters.
	/// </summary>
	internal static class Escaping
	{
		/// <summary>
		/// Creates a <see cref="TextWriter"/> that backslash-escapes EOL characters. You must explicitly request an EOL by calling one of the <c>WriteLine</c> methods.
		/// </summary>
		/// <param name="destination">The wrapped <see cref="TextWriter"/> that the escaping <see cref="TextWriter"/> writes to.</param>
		/// <returns></returns>
		public static TextWriter CreateEscapingTextWriter(TextWriter destination)
		{
			return new StringLogTextWriter(new EscapingStringLog(new TextWriterStringLog(destination)));
		}

		/// <summary>
		/// Backslash-escapes the EOL characters in a source string.
		/// </summary>
		/// <param name="source">The source string.</param>
		public static string BackslashEscape(string source)
		{
			if (source.IndexOfAny(s_backslashEscapeChars) == -1)
				return source;

			var sb = new StringBuilder(source.Length);
			foreach (var ch in source)
			{
				if (ch == '\\')
					sb.Append("\\\\");
				else if (ch == '\n')
					sb.Append("\\n");
				else if (ch == '\r')
					sb.Append("\\r");
				else
					sb.Append(ch);
			}

			return sb.ToString();
		}

		private static readonly char[] s_backslashEscapeChars = {'\\', '\n', '\r'};
	}
}
