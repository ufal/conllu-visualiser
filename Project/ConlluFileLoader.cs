using System.Drawing;
using System.Windows.Forms;

namespace ConlluVisualiser
{
    /// <summary>
    /// Loader that loads the file in CoNLL-U format and saves all the sentences to the list.
    /// </summary>
    class ConlluFileLoader : SimpleFileLoader, IFileLoader
    {
        /// <summary>
        /// Creates the CoNLL-U file loader.
        /// </summary>
        /// <param name="tp">The panel where it will be written the loading message to.</param>
        public ConlluFileLoader(Panel tp) : base(tp)
        {
        }

        /// <summary>
        /// Reads a file and saves it from the CoNLL-U format of sentences into the list <paramref name="sentences"/>.
        /// </summary>
        /// <param name="FileName">The name of the file</param>
        /// <param name="sentences">The list of sentences where the sentences will be saved to.</param>
        /// <returns>True, if the file was correctly loaded; otherwise, false.</returns>
        protected override bool ReadFile(string FileName, ListOfSentences sentences)
        {
            try
            {
                // Reads the whole file and saves it to the list of sentences.
                using (IReader reader = new Reader(FileName))
                {
                    // Uses the ConlluSentenceFactory to create the sentences.
                    sentences.AddSentences(new ConlluSentenceFactory(reader));
                    return true;
                }
            }
            catch
            {
                // Notices the user that the file has a bad format.
                MessageBox.Show("File is not in a CoNLL-U format.");
                return false;
            }
        }
    }
}
