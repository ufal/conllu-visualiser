using System.IO;
using System;

namespace ConlluVisualiser
{
    /// <summary>
    /// Provides the interface of sentence with words and operations with them.
    /// </summary>
    public interface ISentence
    {
        /// <summary>
        /// The extra root of the sentence.
        /// </summary>
        ITreeWord Root { get; }

        /// <summary>
        /// The index of the sentence in the order in the file.
        /// </summary>
        int PlaceInFile { get; set; }

        /// <summary>
        /// The count of the words in the sentence.
        /// </summary>
        int CountWords { get; }

        /// <summary>
        /// The sent_id attribute of the sentence. Must be unique in the file.
        /// </summary>
        string Sent_id { get; }

        /// <summary>
        /// The text of the sentence.
        /// </summary>
        string Text_attribute { get; }

        /// <summary>
        /// Adds the new word to the <paramref name="position"/> in the sentence and actualizes the text of the sentence.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">If the position is out of the range of the words.</exception>
        /// <param name="word">The new word that will be added to the sentence.</param>
        /// <param name="position">The position in the sentence where the word will lie.</param>
        void AddWord(IWord word, int position);

        /// <summary>
        /// Creates and inserts the new child to the sentence behind the <paramref name="preceding"/> word (or behind its empty node words).
        /// The new parent of the child will be the <paramref name="parent"/> parameter.
        /// </summary>
        /// <param name="parent">The parent of the new created word.</param>
        /// <param name="preceding">The word that precedes to the new word.</param>
        /// <returns>The new created word.</returns>
        IWord InsertChild(ITreeWord parent, ITreeWord preceding);

        /// <summary>
        /// Deletes the <paramref name="word"/> from the sentence words. 
        /// If the parameter <paramref name="shiftIds"/> is true, shifts all the following ids to be lower by one.
        /// </summary>
        /// <param name="word">The deleted word.</param>
        /// <param name="shiftIds">Specifies if the following ids should be reduced by one (true), or not (false).</param>
        void DeleteWord(IWord word, bool shiftIds);

        /// <summary>
        /// Returns the word from sentence that lies on the given <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index of the searched word.</param>
        /// <returns>The found word or null, if the index is out of range of the sentence words.</returns>
        IWord GetWord(int index);

        /// <summary>
        /// Returns the word from sentence that has the given id.
        /// </summary>
        /// <param name="id">The id of the searched word.</param>
        /// <returns>The found word or null, if there is no word with the given id.</returns>
        IWord GetWord(string id);

        /// <summary>
        /// Returns the index of the <paramref name="word"/>.
        /// </summary>
        /// <param name="word">The word whose index is searched for.</param>
        /// <returns>The index of the word or -1, if the word is not in the sentence.</returns>
        int GetIndexOf(IWord word);

        /// <summary>
        /// Swaps two words from the sentence. Is they have the empty node words, shifts them according to their main word.
        /// </summary>
        /// <param name="first">The first swapped word.</param>
        /// <param name="second">The second swapped word.</param>
        void Swap(IWord first, IWord second);

        /// <summary>
        /// Splits the sentence on two. The second sentence starts with the <paramref name="firstWord"/> 
        /// and has the sent_id attribue set to <paramref name="new_id"/>.
        /// </summary>
        /// <param name="firstWord">The word that starts the new sentence.</param>
        /// <param name="new_id">The id of the new sentence.</param>
        /// <param name="sentences">The list of sentences where the new sentence should be inserted.</param>
        /// <returns>The second part of the sentence.</returns>
        ISentence Split(IWord firstWord, string new_id, ListOfSentences sentences);

        /// <summary>
        /// Shows the form with the sentence information.
        /// </summary>
        void ShowInfo();

        /// <summary>
        /// Actualizes the text to match the words in the sentence.
        /// </summary>
        /// <returns>The new text of the sentence.</returns>
        string MakeText();

        /// <summary>
        /// Writes the sentence to the <paramref name="stream"/> in the right format.
        /// </summary>
        /// <param name="stream">The stream where the sentence will be written.</param>
        void SaveTo(StreamWriter stream);
    }
}