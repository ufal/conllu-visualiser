namespace ConlluVisualiser
{
    /// <summary>
    /// Creates the sentence from the file by the concrete manner.
    /// </summary>
    public interface ISentenceFactory
    {
        /// <summary>
        /// Creates and returns the sentence according to the type of the factory.
        /// </summary>
        /// <param name="sentences">The list of all sentences.</param>
        /// <returns>The created sentence.</returns>
        ISentence GetSentence(ListOfSentences sentences);
    }

    /// <summary>
    /// Creates the sentences from the simple text format.
    /// Splits the file on lines and createst the sentence from the words on each line.
    /// </summary>
    class SimpleSentenceFactory : ISentenceFactory
    {
        /// <summary>
        /// The reader that holds the file and can read it.
        /// </summary>
        private readonly IReader Reader;

        /// <summary>
        /// The last used sent_id.
        /// </summary>
        private int idNum = 0;

        /// <summary>
        /// Initializes the factory with the reader that reads the file.
        /// </summary>
        /// <param name="reader">The reader with loaded file in it.</param>
        public SimpleSentenceFactory(IReader reader)
        {
            Reader = reader;
        }

        /// <summary>
        /// Creates and returns the next sentence from the file with automatically created id.
        /// Reads one line and creates the sentence from the words in it.
        /// </summary>
        /// <param name="sentences">The list of sentences which is created by this factory.</param>
        /// <returns>The new sentence or null, if there is no other sentence.</returns>
        public ISentence GetSentence(ListOfSentences sentences)
        {
            // If there is some sentence
            if(!Reader.EndOfFile)
            {
                // Reads the sentence and splits it to words (by the white spaces)
                string[] words = null;
                string line = Reader.ReadLine();
                if (line != null)
                {
                    words = line.Split();
                }
                // Creates the id as the character "s" and the index of the sentence in the file.
                ISentence sentence = new Sentence(words, "s" + idNum.ToString(), sentences);
                idNum++;
                return sentence;
            }
            return null;
        }
    }
}
