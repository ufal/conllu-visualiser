using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GraphVisualiser;

namespace ConlluVisualiser
{
    /// <summary>
    /// Class representing a word that is omitted in the sentence by an ellipse
    /// </summary>
    public class EmptyNodeWord : Word, IWord
    { 
        /// <summary>
        /// The word before this group of empty words
        /// </summary>
        public ITreeWord MainWord { get; set; }

        /// <summary>
        /// Creates the empty node word. 
        /// </summary>
        /// <param name="id">The id of the empty word. Must be decimal.</param>
        public EmptyNodeWord(string id) : base(id)
        {
        }
        
        /// <summary>
        /// Shifts the first part of the id. Main word id is changed.
        /// </summary>
        /// <exception cref="ArgumentException">If the id is not in a forn x.y, where x and  are numbers.</exception>
        /// <exception cref="FormatException">Bad form of id.</exception>
        /// <param name="shift">The number by which the id is shifted.</param>
        public override void ShiftId(int shift)
        {
            string[] orig = Id.Split('.');
            if (orig.Length != 2)
                throw new ArgumentException();
            int int_new_id = int.Parse(orig[0]) + shift;
            string new_id = int_new_id + "." + orig[1];
            foreach (var childEnh in ChildrenEnhanced)
            {
                string dep = childEnh.Info.Deps[Id];
                childEnh.Info.Deps.Remove(Id);
                childEnh.Info.Deps[new_id] = dep;
            }
            Id = new_id;
            
        }
        
        /// <summary>
        /// Deletes the word and shift the ids of the following empty words
        /// </summary>
        /// <param name="sentence">The sentence where this word lies.</param>
        public override void Delete(ISentence sentence)
        {
            foreach (var childEnh in ChildrenEnhanced)
            {
                childEnh.Info.Deps.Remove(Id);
            }
            if (MainWord != null)
            {
                // The set of following empty nodes.
                var nextEmpty = MainWord.EmptyNodes.SkipWhile(x => x != this);
                foreach(var x in nextEmpty)
                {
                    // Decreases the second part of the id.
                    string[] splitted = x.Id.Split('.');
                    x.Id = splitted[0] + "." + (int.Parse(splitted[1]) - 1);
                }
                MainWord.EmptyNodes.Remove(this);
            }           
            // Makes this id null to signalize it is deleted
            Id = null;
            // Deletes the word from the sentence
            sentence.DeleteWord(this, false);
        }

        /// <summary>
        /// Swaps the empty node with the next empty node in this group according to the direction
        /// </summary>
        /// <param name="direction">The direction that specifies the shift.</param>
        /// <param name="sentence">The sentence where the word lies.</param>
        /// <returns>true if the words were swapped</returns>
        public bool Swap(int direction, ISentence sentence)
        {
            // The index of the empty word in the group of the empty words
            int idx = MainWord.EmptyNodes.IndexOf(this);
            // If there is no word which can be swapped with this word 
            if (MainWord.EmptyNodes.Count > idx + 1)
            {
                // Switches the nodes
                EmptyNodeWord sibling = MainWord.EmptyNodes[idx + 1];
                MainWord.EmptyNodes[idx] = sibling;
                MainWord.EmptyNodes[idx + 1] = this;
                string sibId = sibling.Id;
                sibling.Id = Id;
                Id = sibId;
                sentence.Swap(this, sibling);
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Returns the form which is a part of the sentence. 
        /// Returns allways the empty string because the empty word is omitted from the sentence.
        /// </summary>
        /// <returns>Returns allways the empty string.</returns>
        public override string GetFormToSentence()
        {
            return "";
        }

        /// <summary>
        /// Method to design the word according to the representation - Applies the received visitor on this word
        /// </summary>
        /// <param name="visitor">The class which allows this word to make some operation.</param>
        /// <param name="g">The graphics where the word is drawn to.</param>
        /// <returns>The return value of the visitor</returns>
        public override object Accept(IVisitor visitor, Graphics g)
        {
            return visitor.Visit(this, g);
        }

        /// <summary>
        /// Gets the context menu of the empty node word according to the representation.
        /// </summary>
        /// <param name="visitor">The visitor that manages the curent representation.</param>
        /// <param name="sentences">The list of sentences where the word lies in.</param>
        /// <param name="form">The struct to command the main form to change the active word or sentence.</param>
        /// <returns>The right context menu with all actions.</returns>
        public override ContextMenuStrip GetContextMenu(IGetMenuVisitor visitor, ListOfSentences sentences, CurrentState form)
        {
            return visitor.GetMenu(this, sentences, form);
        }

        public override bool CanStartNewSentence(out string reason)
        {
            reason = "";
            return false;
        }
    }
}
