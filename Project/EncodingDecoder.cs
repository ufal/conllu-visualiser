using System.IO;
using System.Text;

namespace ConlluVisualiser
{
    class EncodingDecoder
    {

        /// <summary>
        /// Gets the file encoding.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <returns>The encoding of the file.</returns>
        public static Encoding GetFileEncoding(string path)
        {
            Encoding e;
            // Tries to find the encoding according to the byte order mark in the beginning of the file.
            using (var reader = new StreamReader(path, Encoding.Default, true))
            {
                if (reader.Peek() >= 0)
                    reader.Read();
                // Saves the encoding. If it is not posible to detect is, returns the default encoding.
                e = reader.CurrentEncoding;
            }
            // If the encoding was default, there probably was not the bom on the beginning of the file. 
            // Then tries to find out if the document can be in UTF8 or it returns the default encoding.
            return e == Encoding.Default ? DetectUtf8Encoding(path) : e;
        }

        /// <summary>
        /// Finds out whether the encoding can be UTF8, if there is no byte order mark in the beginning.
        /// </summary>
        /// <param name="filename">The name of the file whose encoding is searched for.</param>
        /// <returns>The encoding of the file.</returns>
        private static Encoding DetectUtf8Encoding(string filename)
        {
            int max_test_length = 1000;
            FileStream stream = new FileStream(filename, FileMode.Open);
            byte[] bytes = new byte[max_test_length];
            // Reads the bytes from the file.
            int length = stream.Read(bytes, 0, max_test_length);
            stream.Close();

            // It tests if the file is in the right utf8 format. The count of one-bites in the beginning of the byte sequence.
            // For the character code there must be used the shortest sequence of bytes.
            int i = 0;
            bool utf8 = false;
            while (i < length - 4)
            {
                // If the byte has a format 0xxxxxxx, it is still a valid utf8 format, but if all the bytes have only one byte, we will take it as the default encoding.
                if (bytes[i] <= 0x7F) { i += 1; }
                // If the next two bytes are 110xxxxx 10xxxxxx, it is still valid utf8.
                else if (bytes[i] >= 0xC2 && bytes[i] <= 0xDF && StartsBy10(bytes[i + 1])) { i += 2; utf8 = true; }
                // If the next bytes are 1110xxxx 10xxxxxx 10xxxxxx, it is still a valid utf8 format. 
                else if (bytes[i] >= 0xE0 && bytes[i] <= 0xF0 && StartsBy10(bytes[i + 1]) && StartsBy10(bytes[i + 2])) { i += 3; utf8 = true; }
                // If the next bytes are 11110xxx 10xxxxxx 10xxxxxx 10xxxxxx, it is still a valid utf8.
                else if (bytes[i] >= 0xF0 && bytes[i] <= 0xF4 && StartsBy10(bytes[i + 1]) && StartsBy10(bytes[i + 2]) && StartsBy10(bytes[i + 1])) { i += 4; utf8 = true; }
                // In all other cases the format is not valid utf8.
                else { utf8 = false; break; }
            }

            if (utf8 == true)
            {
                return Encoding.UTF8;
            }
            return Encoding.Default;
        }

        /// <summary>
        /// If the byte matches the mask 10xxxxxx, returns true, else returns false.
        /// </summary>
        /// <param name="b">The tested byte.</param>
        /// <returns>True, if the byte starts by two bits 1,0; otherwise, false.</returns>
        private static bool StartsBy10(byte b)
        {
            // 0x80 = 1000 0000, 0xC0 = 1100 0000
            return (b >= 0x80 && b < 0xC0);
        }
    }
}