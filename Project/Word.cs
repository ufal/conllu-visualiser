using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using GraphVisualiser;

namespace ConlluVisualiser
{
    public abstract class Word :IWord
    {
        /// <summary>
        /// Set of all information about the word which correspond to the CoNLL-U format.
        /// </summary>
        public WordInfo Info { get; set; } = new WordInfo();

        /// <summary>
        /// Set of all children of the word in the sentence graph.
        /// </summary>
        public List<IWord> ChildrenEnhanced { get; } = new List<IWord>();

        /// <summary>
        /// Removes <paramref name="word"/> from the enhanced graph representation. 
        /// </summary>
        /// <param name="word">The child in enhanced.</param>
        public virtual void RemoveChildEnhhanced(IWord word)
        {
            // Add the child
            ChildrenEnhanced.Remove(word);
            word.Info.Deps.Remove(Id);
        }

        /// <summary>
        /// The struct that holds the current point while the word is visualised.
        /// </summary>
        private readonly WordPoint Point = new WordPoint();
        /// <summary>
        /// Returns the point on the drawing space where the word is visualised
        /// </summary>
        public virtual WordPoint GetWordPoint()
        {
            return Point;
        }

        /// <summary>
        /// Specifies if the word is currently shown as active in the sentence graph.
        /// </summary>
        public bool IsActive { get; set; } = false;
        /// <summary>
        /// The Id is corresponding with the value in the word info
        /// </summary>
        public string Id
        {
            get => Info.Id;
            set => Info.Id = value;
        }

        /// <summary>
        /// Creates the new word.
        /// </summary>
        /// <param name="id">The id of the new word</param>
        public Word(string id)
        {
            Id = id;
        }

        /// <summary>
        /// Increases or decreases the number by a parameter <paramref name="shift"/>
        /// </summary>
        /// <param name="shift">
        /// The number by which the id moves. 
        /// If the Id should decrease, the value must be negative.
        /// </param>
        public abstract void ShiftId(int shift);


        /// <summary>
        /// Shows all information about the word got from the CoNLL-U format.
        /// Shows the information about the sentence if the word is a sentence root.
        /// Sentence text can be modified.
        /// </summary>
        /// <param name="sentence">The sentence whose information are shown if the word is root. Text can be modified.</param>
        public virtual void ShowInfo(ISentence sentence)
        {
            // Shows a form with the information. User can change the values.
            Info.ShowWordInfo(sentence);
            // Actualizes the text of the sentence. The text can be modified because of user's changes.
            sentence.MakeText();
        }

        /// <summary>
        /// Returns the array with not empty values 
        /// <seealso cref="WordInfo.Lemma"/>, <seealso cref="WordInfo.Deprel"/>, <seealso cref="WordInfo.Upos"/>.
        /// If the word is a sentence root, returns the array with one value <paramref name="sent_id"/>.
        /// </summary>
        /// <param name="sent_id">The id of the sentence where the word lies.</param>
        /// <returns>The array with not empty values with basic information</returns>
        public virtual string[] GetWordBasicInfo(string sent_id)
        {
            List<string> basics = new List<string>();
            // The array will contain only not empty values
            if (Info.Lemma != "") basics.Add(Info.Lemma);
            if (Info.Deprel != "") basics.Add(Info.Deprel);
            if (Info.Upos != "") basics.Add(Info.Upos);
            return basics.ToArray();
        }

        /// <summary>
        /// Deletes the word and sets its id to null (for the case someone would try to use it)
        /// </summary>
        /// <param name="sentence">The sentence from which the word will be deleted</param>
        public abstract void Delete(ISentence sentence);

        /// <summary>
        /// Returns the form which will be on the word place in the sentence
        /// </summary>
        /// <returns>The word which makes a part of the text of the sentence</returns>
        public abstract string GetFormToSentence();

        /// <summary>
        /// Specifies if the word can lie as the first word of the sentence.
        /// </summary>
        /// <param name="reason">The reason, when the word can not be the first word of the sentence.</param>
        /// <returns>true if the word can be the first in a sentence; otherwise, false</returns>
        public abstract bool CanStartNewSentence(out string reason);

        /// <summary>
        /// Saves the word into the file according to CoNLL-U format.
        /// </summary>
        /// <param name="stream">The stream where the word is written</param>
        public void SaveToFile(StreamWriter stream)
        {
            Info.SaveWord(stream);
        }

        /// <summary>
        /// Method to design the word according to the representation - Applies the received visitor on this word
        /// </summary>
        /// <param name="visitor">The class which allows this word to make some operation.</param>
        /// <param name="g">The graphics where the word is drawn to.</param>
        /// <returns>The return value of the visitor</returns>
        public abstract object Accept(IVisitor visitor, Graphics g);

        /// <summary>
        /// Gets the context menu of the word according to the representation.
        /// </summary>
        /// <param name="visitor">The visitor that manages the curent representation.</param>
        /// <param name="sentences">The list of sentences where the word lies in.</param>
        /// <param name="form">The struct to command the main form to change the active word or sentence.</param>
        /// <returns>The right context menu with all actions.</returns>
        public abstract ContextMenuStrip GetContextMenu(IGetMenuVisitor visitor, ListOfSentences sentences, CurrentState form);
    }
}
