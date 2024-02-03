using System;
using System.Collections.Generic;

namespace ConlluVisualiser
{
    /// <summary>
    /// The word that makes a node in the basic sentence tree.
    /// </summary>
    public interface ITreeWord : IWord
    {
        /// <summary>
        /// The parent of the word in the sentence graph. The attribute Head of all children equals their parent's Id.
        /// </summary>
        ITreeWord Parent { get; set; }

        /// <summary>
        /// Set of all children of the word in the sentence graph.
        /// </summary>
        SortedList<int, ITreeWord> Children { get; } 

        /// <summary>
        /// Set of empty nodes which are situated right after this word.
        /// </summary>
        List<EmptyNodeWord> EmptyNodes { get; set; } 
        /// <summary>
        /// The multiword which contains this word, if it is inside multiword. Null otherwise.
        /// </summary>
        MultiWord JoinedWord { get; set; }
        /// <summary>
        /// Specifies if the word is inside a multiword.
        /// </summary>
        bool IsJoined { get; }
        /// <summary>
        /// Says if the word is the root of the sentence.
        /// </summary>
        bool IsRoot { get; }

        /// <summary>
        /// Add <paramref name="word"/> as the new child. 
        /// If <paramref name="removeFromCurrentParent"/> is true, removes the word from preceding parent's children.
        /// </summary>
        /// <param name="word">The new child</param>
        /// <param name="removeFromCurrentParent">
        /// Specifies, wheather the word should be removed from preceding parent's children list
        /// </param>
        void AddChild(ITreeWord word, bool removeFromCurrentParent);

        /// <summary>
        /// Specifies if the wor can be a parent of <paramref name="possibleChild"/>.
        /// </summary>
        /// <param name="possibleChild">The word which tries to be a new child of this word</param>
        /// <returns>true if the word can be a parent of the word in the parameter; otherwise, false</returns>
        bool CanBeParentOf(ITreeWord possibleChild);


        /// <summary>
        /// Removes the word from children. The word Id must be a number.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <param name="id">The id of the removed child</param>
        void RemoveChild(string id);

        /// <summary>
        /// Swaps the word with another according to direction.
        /// </summary>
        /// <example><paramref name="direction"/> : -1 = left sibling, 1 = right sibling</example>
        /// <param name="direction">
        /// Negative or positive number which determines the swapped word according to the word order in the sentence.
        /// </param>
        /// <param name="sentence">The sentence whose words are swapped.</param>
        /// <returns>true if the word can be swapped with the given another word; otherwise, false</returns>
        bool Swap(int direction, ISentence sentence);

        /// <summary>
        /// Adds the empt node behind this word.
        /// </summary>
        /// <param name="sentence">The sentence to which the new empty word will be added.</param>
        void AddEmptyNode(ISentence sentence);

    }
}
