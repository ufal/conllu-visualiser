using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using GraphVisualiser;

namespace ConlluVisualiser
{
    /// <summary>
    /// Any word in the sentence.
    /// </summary>
    public interface IWord
    {
        /// <summary>
        /// The parent of the word in the basic representation
        /// </summary>
       // IWord Parent { get; set; }

        /// <summary>
        /// The CoNLL-U attributes of the word.
        /// </summary>
        WordInfo Info { get; set; }

        /// <summary>
        /// The list of children in the basic representation.
        /// </summary>
       // SortedList<int, IWord> Children { get; }

        /// <summary>
        /// Add <paramref name="word"/> as a new child. 
        /// If <paramref name="removeFromCurrentParent"/> is true, removes the word from preceding parent's children.
        /// </summary>
        /// <param name="word">The new child</param>
        /// <param name="removeFromCurrentParent">
        /// Specifies, wheather the word should be removed from preceding parent's children list.
        /// </param>
       // void AddChild(IWord w, bool removeFromCurrentParent);

        /// <summary>
        /// Removes the child with given <paramref name="id"/> from the list of children.
        /// </summary>
        /// <param name="id"></param>
       // void RemoveChild(string id);

        /// <summary>
        /// The list of children in the enhanced representation.
        /// </summary>
        List<IWord> ChildrenEnhanced { get; }

        /// <summary>
        /// Adds the child to the list of children in the enhanced representation.
        /// </summary>
        /// <param name="word">The word that is being removed.</param>
        void RemoveChildEnhhanced(IWord word);

        /// <summary>
        /// The ID attribute of the word.
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// Returns the word point in the current representation.
        /// </summary>
        /// <returns></returns>
        WordPoint GetWordPoint();

        /// <summary>
        /// Increases or decreases the number by a parameter <paramref name="shift"/>
        /// </summary>
        /// <param name="shift">
        /// The number by which the id moves. 
        /// If the Id should decrease, the value must be negative.
        /// </param>
        void ShiftId(int shift);

        /// <summary>
        /// Shows all information about the word got from the CoNLL-U format.
        /// Shows the information about the sentence if the word is a sentence root.
        /// Sentence text can be modified.
        /// </summary>
        /// <param name="sentence">The sentence whose information are shown if the word is root. Text can be modified.</param>
        void ShowInfo(ISentence sentence);

        /// <summary>
        /// Returns the array with not empty values 
        /// <seealso cref="WordInfo.Lemma"/>, <seealso cref="WordInfo.Deprel"/>, <seealso cref="WordInfo.Upos"/>.
        /// If the word is a sentence root, returns the array with one value <paramref name="sent_id"/>.
        /// </summary>
        /// <param name="sent_id">The id of the sentence where the word lies.</param>
        /// <returns>The array with not empty values with basic information</returns>
        string[] GetWordBasicInfo(string sent_id);

        /// <summary>
        /// The multiword which contains this word, if it is inside multiword. Null otherwise.
        /// </summary>
        //MultiWord JoinedWord { get; set; }

        /// <summary>
        /// Set of empty nodes which are situated right after this word.
        /// </summary>
        //List<EmptyNodeWord> EmptyNodes { get; }

        /// <summary>
        /// Specifies if the word is inside a multiword.
        /// </summary>
        //bool IsJoined { get; }

        /// <summary>
        /// Specifies if the word is currently shown as active in the sentence graph.
        /// </summary>
        bool IsActive { get; set; }

       // bool IsRoot { get; }

        /// <summary>
        /// Specifies if the wor can be a parent of <paramref name="possibleChild"/>.
        /// </summary>
        /// <param name="possibleChild">The word which tries to be a new child of this word.</param>
        /// <returns>true if the word can be a parent of the word in the parameter; otherwise, false</returns>
      //  bool CanBeParentOf(IWord par);

        /// <summary>
        /// Saves the word into the file according to CoNLL-U format.
        /// </summary>
        /// <param name="stream">The stream where the word is written.</param>
        void SaveToFile(StreamWriter stream);

        /// <summary>
        /// Returns the form which will be on the word place in the sentence.
        /// </summary>
        /// <returns>The word which makes a part of the text of the sentence.</returns>
        string GetFormToSentence();

        /// <summary>
        /// Specifies if the word can lie as the first word of the sentence.
        /// </summary>
        /// <param name="reason">The reason, when the word can not be the first word of the sentence.</param>
        /// <returns>true if the word can be the first in a sentence; otherwise, false</returns>
        bool CanStartNewSentence(out string reason);

        /// <summary>
        /// Deletes the word and sets its id to null (for the case someone would try to use it).
        /// </summary>
        /// <param name="sentence">The sentence from which the word will be deleted.</param>
        void Delete(ISentence sentence);

        /// <summary>
        /// Method to design the word according to the representation - Applies the received visitor on this word
        /// </summary>
        /// <param name="visitor">The class which allows this word to make some operation.</param>
        /// <param name="g">The graphics where the word is visualised according to the visitor.</param>
        /// <returns>The return value of the visitor</returns>
        object Accept(IVisitor visitor, Graphics g);

        /// <summary>
        /// Gets the context menu of the word according to the representation and the type of the word.
        /// </summary>
        /// <param name="visitor">The visitor that manages the curent representation.</param>
        /// <param name="sentences">The list of sentences where the word lies in.</param>
        /// <param name="form">The struct to command the main form to change the active word or sentence.</param>
        /// <returns>The right context menu with all actions.</returns>
        ContextMenuStrip GetContextMenu(IGetMenuVisitor visitor, ListOfSentences sentences, CurrentState form);
    }
}
