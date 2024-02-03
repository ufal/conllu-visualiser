using System;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace ConlluVisualiser
{
    /// <summary>
    /// The reader that can read the file in any encoding.
    /// </summary>
    public interface IReader : IDisposable
    {
        /// <summary>
        /// Reads the line from a file.
        /// </summary>
        /// <returns>The string value of the line.</returns>
        string ReadLine();
        /// <summary>
        /// Says if the reading is at the end of the file.
        /// </summary>
        bool EndOfFile { get; set; }
    }

    /// <summary>
    /// The file reader that reads the file according to its encoding.
    /// </summary>
    public class Reader : IDisposable, IReader
    {
        /// <summary>
        /// The stream of the text.
        /// </summary>
        private TextReader File { get; }

        /// <summary>
        /// Creates the reader from the file.
        /// </summary>
        /// <param name="filename">The name of the file.</param>
        public Reader(string filename)
        {
            var encoding = EncodingDecoder.GetFileEncoding(filename);
            // Gets the encoding and initializes the stream.
            File = new StreamReader(new FileStream(filename, FileMode.Open), encoding);
        }

        /// <summary>
        /// Creates the reader from the textbox.
        /// </summary>
        /// <param name="textbox">The textbox whose text should be read.</param>
        public Reader(TextBox textbox)
        {
            File = new StringReader(textbox.Text);
        }

        /// <summary>
        /// Reads one line from the file.
        /// </summary>
        /// <returns>The string value of the line or null, if there is no other line.</returns>
        public string ReadLine()
        {
            string line = File.ReadLine();
            if (line == null)
            {
                EndOfFile = true;
                return null;
            }
            return line;
        }

        /// <summary>
        /// Says if the stram is at the end of the file.
        /// </summary>
        public bool EndOfFile { get; set; } = false;

        /// <summary>
        /// Disposes the object and closes the file stream, if it is open.
        /// </summary>
        public void Dispose()
        {
            if (File != null)
            {
                File.Close();
            }
        }
    }
}
