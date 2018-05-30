using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Faithlife.DockerShim.Logging
{
	/// <summary>
	/// A text writer that writes to a string log only when an explicit <c>WriteLine</c>/<c>WriteLineAsync</c> is requested or <c>Flush</c>/<c>FlushAsync</c> is invoked.
	/// </summary>
	internal sealed class StringLogTextWriter : TextWriter
	{
		/// <summary>
		/// Creates a new text writer that writes to the specified string log only when an explicit <c>WriteLine</c> is requested.
		/// </summary>
		/// <param name="stringLog">The wrapped string log.</param>
		public StringLogTextWriter(IStringLog stringLog)
		{
			m_stringLog = stringLog;
			m_writer = new StringWriter();
		}

		/// <inheritdoc/>
		public override IFormatProvider FormatProvider => m_writer.FormatProvider;

		/// <inheritdoc/>
		public override string NewLine
		{
			get => m_writer.NewLine;
			set => m_writer.NewLine = value;
		}

		/// <inheritdoc/>
		public override Encoding Encoding => m_writer.Encoding;

		/// <inheritdoc/>
		public override void Flush()
		{
			var text = m_writer.ToString();
			m_writer.GetStringBuilder().Clear();
			if (text != "")
				m_stringLog.WriteLine(text);
		}

		/// <inheritdoc/>
		public override Task FlushAsync()
		{
			Flush();
			return Task.CompletedTask;
		}

		/// <inheritdoc/>
		public override void Write(char value) => m_writer.Write(value);

		/// <inheritdoc/>
		public override void Write(bool value) => m_writer.Write(value);

		/// <inheritdoc/>
		public override void Write(char[] buffer) => m_writer.Write(buffer);

		/// <inheritdoc/>
		public override void Write(char[] buffer, int index, int count) => m_writer.Write(buffer, index, count);

		/// <inheritdoc/>
		public override void Write(decimal value) => m_writer.Write(value);

		/// <inheritdoc/>
		public override void Write(double value) => m_writer.Write(value);

		/// <inheritdoc/>
		public override void Write(int value) => m_writer.Write(value);

		/// <inheritdoc/>
		public override void Write(long value) => m_writer.Write(value);

		/// <inheritdoc/>
		public override void Write(object value) => m_writer.Write(value);

		/// <inheritdoc/>
		public override void Write(float value) => m_writer.Write(value);

		/// <inheritdoc/>
		public override void Write(string value) => m_writer.Write(value);

		/// <inheritdoc/>
		public override void Write(string format, object arg0) => m_writer.Write(format, arg0);

		/// <inheritdoc/>
		public override void Write(string format, object arg0, object arg1) => m_writer.Write(format, arg0, arg1);

		/// <inheritdoc/>
		public override void Write(string format, object arg0, object arg1, object arg2) =>
			m_writer.Write(format, arg0, arg1, arg2);

		/// <inheritdoc/>
		public override void Write(string format, params object[] arg) => m_writer.Write(format, arg);

		/// <inheritdoc/>
		public override void Write(uint value) => m_writer.Write(value);

		/// <inheritdoc/>
		public override void Write(ulong value) => m_writer.Write(value);

		/// <inheritdoc/>
		public override Task WriteAsync(char value)
		{
			Write(value);
			return Task.CompletedTask;
		}

		/// <inheritdoc/>
		public override Task WriteAsync(char[] buffer, int index, int count)
		{
			Write(buffer, index, count);
			return Task.CompletedTask;
		}

		/// <inheritdoc/>
		public override Task WriteAsync(string value)
		{
			Write(value);
			return Task.CompletedTask;
		}

		/// <inheritdoc/>
		public override void WriteLine()
		{
			var text = m_writer.ToString();
			m_writer.GetStringBuilder().Clear();
			m_stringLog.WriteLine(text);
		}

		/// <inheritdoc/>
		public override void WriteLine(bool value)
		{
			Write(value);
			WriteLine();
		}

		/// <inheritdoc/>
		public override void WriteLine(char value)
		{
			Write(value);
			WriteLine();
		}

		/// <inheritdoc/>
		public override void WriteLine(char[] buffer)
		{
			Write(buffer);
			WriteLine();
		}

		/// <inheritdoc/>
		public override void WriteLine(char[] buffer, int index, int count)
		{
			Write(buffer, index, count);
			WriteLine();
		}

		/// <inheritdoc/>
		public override void WriteLine(decimal value)
		{
			Write(value);
			WriteLine();
		}

		/// <inheritdoc/>
		public override void WriteLine(double value)
		{
			Write(value);
			WriteLine();
		}

		/// <inheritdoc/>
		public override void WriteLine(int value)
		{
			Write(value);
			WriteLine();
		}

		/// <inheritdoc/>
		public override void WriteLine(long value)
		{
			Write(value);
			WriteLine();
		}

		/// <inheritdoc/>
		public override void WriteLine(object value)
		{
			Write(value);
			WriteLine();
		}

		/// <inheritdoc/>
		public override void WriteLine(float value)
		{
			Write(value);
			WriteLine();
		}

		/// <inheritdoc/>
		public override void WriteLine(string value)
		{
			Write(value);
			WriteLine();
		}

		/// <inheritdoc/>
		public override void WriteLine(string format, object arg0)
		{
			Write(format, arg0);
			WriteLine();
		}

		/// <inheritdoc/>
		public override void WriteLine(string format, object arg0, object arg1)
		{
			Write(format, arg0, arg1);
			WriteLine();
		}

		/// <inheritdoc/>
		public override void WriteLine(string format, object arg0, object arg1, object arg2)
		{
			Write(format, arg0, arg1, arg2);
			WriteLine();
		}

		/// <inheritdoc/>
		public override void WriteLine(string format, params object[] arg)
		{
			Write(format, arg);
			WriteLine();
		}

		/// <inheritdoc/>
		public override void WriteLine(uint value)
		{
			Write(value);
			WriteLine();
		}

		/// <inheritdoc/>
		public override void WriteLine(ulong value)
		{
			Write(value);
			WriteLine();
		}

		/// <inheritdoc/>
		public override Task WriteLineAsync()
		{
			WriteLine();
			return Task.CompletedTask;
		}

		/// <inheritdoc/>
		public override Task WriteLineAsync(char value)
		{
			Write(value);
			WriteLine();
			return Task.CompletedTask;
		}

		/// <inheritdoc/>
		public override Task WriteLineAsync(char[] buffer, int index, int count)
		{
			Write(buffer, index, count);
			WriteLine();
			return Task.CompletedTask;
		}

		/// <inheritdoc/>
		public override Task WriteLineAsync(string value)
		{
			Write(value);
			WriteLine();
			return Task.CompletedTask;
		}

		private readonly StringWriter m_writer;
		private readonly IStringLog m_stringLog;
	}
}
