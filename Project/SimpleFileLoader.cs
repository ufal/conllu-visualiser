using System;
using System.Drawing;
using System.Windows.Forms;

namespace ConlluVisualiser
{
    /// <summary>
    /// File loader that can load the file in the concrete format.
    /// </summary>
    public interface IFileLoader
    {
        /// <summary>
        /// Loads all the file and splits it to single sentences. Saves the created sentences into <paramref name="sentences"/>.
        /// </summary>
        /// <param name="sentences">The list of sentences. It should contain no existing sentences.</param>
        /// <returns>True if the file was correctly loaded; otherwise, false.</returns>
        bool LoadFile(ListOfSentences sentences);
    }

    /// <summary>
    /// The loader that can load and save the file in the simple text format.
    /// </summary>
    class SimpleFileLoader : IFileLoader
    {
        /// <summary>
        ///  The panel where the informative text about loading is shown.
        /// </summary>
        protected readonly Panel MessagePanel;

        /// <summary>
        /// Creates the file loadre that loads the file in the simple text format.
        /// </summary>
        /// <param name="messagePanel">The panel where the informative text about loading will be shown.</param>
        public SimpleFileLoader(Panel messagePanel)
        {
            MessagePanel = messagePanel;
        }
        
        /// <summary>
        /// Writes the message about loading on the panel.
        /// </summary>
        protected void InformAboutLoading()
        {
            Graphics g = MessagePanel.CreateGraphics();
            g.Clear(Color.White);
            string text = "Please, wait a moment...";
            Font font = new Font("Arial", 15);
            SizeF sizeText = g.MeasureString(text, font);
            PointF p = new PointF((MessagePanel.Width / 2) - (sizeText.Width / 2), (MessagePanel.Height / 2) - (sizeText.Height / 2));
            g.DrawString(text, font, Brushes.Black, p);
        }

        /// <summary>
        /// Opens the dialog to chose the right file.
        /// Loads the file in the simple text format, splits it to single sentences and saves them to the <paramref name="sentences"/>.
        /// </summary>
        /// <param name="sentences">The list of sentences, should be empty.</param>
        /// <returns>True if the file was correctly loaded; otherwise, false.</returns>
        public virtual bool LoadFile(ListOfSentences sentences)
        {
            // Opens the dialog to choose the file to open.
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                // Writes the message about loading to the message panel.
                InformAboutLoading();
                // Tries to read a file. If everything goes well, returns true.
                if(ReadFile(dialog.FileName, sentences))
                {
                    return true;
                }                
            }
            // In case there was some problem in loading or if the user did not choose any file,
            // clears the panel with the loading message and returns true.
            Graphics g = MessagePanel.CreateGraphics();
            g.Clear(Color.White);
            return false;
        }

        /// <summary>
        /// Reads a file and saves its lines as sentences into the <paramref name="sentences"/>.
        /// </summary>
        /// <param name="FileName">The name of the file</param>
        /// <param name="sentences">The list of sentences where the sentences will be saved to.</param>
        /// <returns>True, if the file was correctly loaded; otherwise, false.</returns>
        protected virtual bool ReadFile(string FileName, ListOfSentences sentences)
        {
            try
            {
                // Tries to load the file into the reader.
                using (IReader reader = new Reader(FileName))
                {
                    // Fills the list of sentences by the sentences from the file.
                    sentences.AddSentences(new SimpleSentenceFactory(reader));
                    // If the file was correctly loaded, returns true.
                    return true;
                }
            }
            // If there was some problem with loading or reading the file, shows the message to the user.
            catch (Exception ex)
            {
                MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                return false;
            }
        }

    }
}
