using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using GraphVisualiser;

namespace ConlluVisualiser
{
    /// <summary>
    /// Class representing one word in the sentence. Implements IWord
    /// </summary>
    public class BasicWord : Word, ITreeWord
    {
        private ITreeWord parent;
        /// <summary>
        /// The parent of the word in the sentence graph. The attribute Head of all children equals their parent's Id.
        /// </summary>
        public ITreeWord Parent
        {
            get => parent;
            set
            {
                parent = value;
                Info.Head = parent.Id;
            }
        }
        /// <summary>
        /// Set of all children of the word in the sentence graph.
        /// </summary>
        public SortedList<int, ITreeWord> Children { get; } = new SortedList<int, ITreeWord>();
       
        /// <summary>
        /// Set of empty nodes which are situated right after this word.
        /// </summary>
        public List<EmptyNodeWord> EmptyNodes { get; set; } = new List<EmptyNodeWord>();
        /// <summary>
        /// The multiword which contains this word, if it is inside multiword. Null otherwise.
        /// </summary>
        public MultiWord JoinedWord { get; set; }
        /// <summary>
        /// Specifies if the word is inside a multiword.
        /// </summary>
        public bool IsJoined => JoinedWord != null;
        /// <summary>
        /// Says if the word is the root of the sentence.
        /// </summary>
        public bool IsRoot { get => Id == "0"; }

        /// <summary>
        /// Creates the new word and add it to the <paramref name="parent"/> as new child (if the parent is not null).
        /// </summary>
        /// <param name="parent">The parent of the new word</param>
        /// <param name="id">The id of the new word</param>
        public BasicWord(ITreeWord parent, string id) : base(id)
        {
            if (parent != null)
            {
                // Adds word to parent and removes the word from another possible parent (there is no other parent in this case)
                parent.AddChild(this, true);
            }
        }

        /// <summary>
        /// Add <paramref name="word"/> as the new child. 
        /// If <paramref name="removeFromCurrentParent"/> is true, removes the word from preceding parent's children.
        /// </summary>
        /// <param name="word">The new child</param>
        /// <param name="removeFromCurrentParent">
        /// Specifies, wheather the word should be removed from preceding parent's children list
        /// </param>
        public virtual void AddChild(ITreeWord word, bool removeFromCurrentParent)
        {
            // Word should be removed from the original list of children of the last parent
            if (removeFromCurrentParent && word.Parent != null) word.Parent.RemoveChild(word.Id);
            // Add the child
            Children[int.Parse(word.Id)] = word;
            // Change the parent
            word.Parent = this;
        }

        

        /// <summary>
        /// Removes the word from children. The word Id must be a number.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <param name="id">The id of the removed child</param>
        public void RemoveChild(string id)
        {
            // Check if the id is a number
            bool is_int = int.TryParse(id, out int int_id);
            if (!is_int)
                throw new ArgumentException();
            Children.Remove(int_id);
        }

        /// <summary>
        /// Increases or decreases the number by a parameter <paramref name="shift"/>
        /// </summary>
        /// <param name="shift">
        /// The number by which the id moves. 
        /// If the Id should decrease, the value must be negative.
        /// </param>
        public override void ShiftId(int shift)
        {
            // If the parent still contain this word as a child, the child will be removed
            if (Parent.Children.ContainsKey(int.Parse(Id)) && Parent.Children[int.Parse(Id)] == this)
                parent.RemoveChild(Id);
            // Changes the id with all consequences
            ChangeId((int.Parse(Id) + shift).ToString());
            // Add the child with the new id to the parent. 
            // The word is already removed from the parent's children list => false (will not be removed from parent again)
            parent.AddChild(this, false);
        }

        /// <summary>
        /// Changes the word id and solves the dependencies (children's Head attribute)
        /// </summary>
        /// <param name="id">The new id of the word.</param>
        private void ChangeId(string id)
        {
            foreach (var childEnh in ChildrenEnhanced)
            {
                string dep = childEnh.Info.Deps[Id];
                childEnh.Info.Deps.Remove(Id);
                childEnh.Info.Deps[id] = dep;
            }
            Id = id;
            // It automatically changes also their Head attribute
            foreach (var child in Children)
            {
                child.Value.Parent = this;
            }           
        }

        /// <summary>
        /// Specifies if the wor can be a parent of <paramref name="possibleChild"/>.
        /// </summary>
        /// <param name="possibleChild">The word which tries to be a new child of this word</param>
        /// <returns>true if the word can be a parent of the word in the parameter; otherwise, false</returns>
        public bool CanBeParentOf(ITreeWord possibleChild)
        {
            // The word can not be a parent of itself and the word can not have a parent inside its subtree.
            // If the word has no parent, it is not a part of the tree structure of the sentence.
            if (this == possibleChild || possibleChild.Parent == null)
                return false;
            if (parent != null)
                // Checks recursivelly, if this word is not in the subtree of the possible child
                return parent.CanBeParentOf(possibleChild);
            return true;
        }

        /// <summary>
        /// Shows all information about the word got from the CoNLL-U format.
        /// Shows the information about the sentence if the word is a sentence root.
        /// Sentence text can be modified.
        /// </summary>
        /// <param name="sentence">The sentence whose information are shown if the word is root. Text can be modified.</param>
        public override void ShowInfo(ISentence sentence)
        {
            // Root's id is allways 0.
            if (!IsRoot)
            {
                base.ShowInfo(sentence);
            }
            else
            {
                sentence.ShowInfo();
            }
        }

        /// <summary>
        /// Returns the array with not empty values 
        /// <seealso cref="WordInfo.Lemma"/>, <seealso cref="WordInfo.Deprel"/>, <seealso cref="WordInfo.Upos"/>.
        /// If the word is a sentence root, returns the array with one value <paramref name="sent_id"/>.
        /// </summary>
        /// <param name="sent_id">The id of the sentence where the word lies.</param>
        /// <returns>The array with not empty values with basic information</returns>
        public override string[] GetWordBasicInfo(string sent_id)
        {
            if (IsRoot)
            {
                // The array will contain only sent_id value
                return new string[] { sent_id };
            }
            return base.GetWordBasicInfo(sent_id);
        }

        /// <summary>
        /// Swaps the word with another according to direction.
        /// </summary>
        /// <example><paramref name="direction"/> : -1 = left sibling, 1 = right sibling</example>
        /// <param name="direction">
        /// Negative or positive number which determines the swapped word according to the word order in the sentence.
        /// </param>
        /// <param name="sentence">The sentence whose words are swapped.</param>
        /// <returns>true if the word can be swapped with the given another word; otherwise, false</returns>
        public virtual bool Swap(int direction, ISentence sentence)
        {
            // Returns true if the sibling exists and can be swapped
            if (GetSwappedSibling(direction, sentence, out ITreeWord swappedSibling))
            {
                // Shifts the word ids according to the direction
                swappedSibling.ShiftId(-direction);
                ShiftId(direction);
                // If the words have the Empty nodes, its necessary to shift their ids too
                foreach (var empty in EmptyNodes)
                    empty.ShiftId(direction);
                foreach (var empty in swappedSibling.EmptyNodes)
                    empty.ShiftId(-direction);
                // Swaps the words in the sentence words order
                sentence.Swap(this, swappedSibling);                
                return true;
            }
            // It was not possible to swap the words
            return false;
        }

        /// <summary>
        /// Finds the word which should be swapped according to the direstion and returns, if the words can be swapped
        /// </summary>
        /// <param name="direction">Number which determines the swapped word according to word order in the sentence.</param>
        /// <param name="sentence">The sentence whose words are being swapped</param>
        /// <param name="swappedSibling"></param>
        /// <returns>true if the words can be swapped; otherwise, false</returns>
        private bool GetSwappedSibling(int direction, ISentence sentence, out ITreeWord swappedSibling)
        {
            // Only words with numeric id can be swapped (multiword or empty node can not be swapped with the basic word)
            if (int.TryParse(Id, out int siblingId))
            {
                // Add the direction and creates the sibling id
                siblingId = siblingId + direction;
            }
            else
            {
                // Word with not numeric id can not be swapped with this word
                swappedSibling = null;
                return false;
            }
            // If there exists the sibling in the sentence
            if ((swappedSibling = (ITreeWord)sentence.GetWord(siblingId.ToString())) != null){
                // Word which lies inside of the multiword token can not be swapped, root can not be swapped.
                if (!IsJoined && !swappedSibling.IsJoined && !swappedSibling.IsRoot)
                {
                    return true;
                }
            }
            return false;            
        }

        /// <summary>
        /// Adds the empt node behind this word.
        /// </summary>
        /// <param name="sentence">The sentence to which the new empty word will be added.</param>
        public void AddEmptyNode(ISentence sentence)
        {
            // Creates the new empty node word and adds it to the end of the empty nodes of this word
            string lastPartId = (EmptyNodes.Count + 1).ToString();
            EmptyNodeWord neww = new EmptyNodeWord(Id + "." + lastPartId)
            {
                MainWord = this
            };
            EmptyNodes.Add(neww);
            // Determines the position in the sentence
            int position = sentence.GetIndexOf(this) + EmptyNodes.Count;
            sentence.AddWord(neww, position);
        }

        /// <summary>
        /// Deletes the word and sets its id to null (for the case someone would try to use it)
        /// </summary>
        /// <param name="sentence">The sentence from which the word will be deleted</param>
        public override void Delete(ISentence sentence)
        {
            // It is not possible to delete a root
            if (IsRoot)
                return;
            parent.RemoveChild(Id);
            // Adds all its children to its parent
            foreach (var child in Children.Values)
            {
                parent.AddChild(child, false);
            }
            // If the word is inside a multiword token, removes the word from it
            if (IsJoined)
            {
                JoinedWord.DeleteSubWord(this, sentence);
            }
            // Deletes the word from the sentence
            sentence.DeleteWord(this, true);
            // Deletes all the empty words which were associated with this word
            for(int i = EmptyNodes.Count-1; i >=0;i--)
            {
                EmptyNodes[i].Delete(sentence);
            }
            foreach(var childEnh in ChildrenEnhanced)
            {
                childEnh.Info.Deps.Remove(Id);
            }
            // Sets Id to null to symbolize that the word is deleted
            Id = null;
        }

        /// <summary>
        /// Returns the form which will be on the word place in the sentence
        /// </summary>
        /// <returns>The word which makes a part of the text of the sentence</returns>
        public override string GetFormToSentence()
        {
            // If the word is inside of the multiword token, it is not a part of the sentence text.
            if (IsJoined) return "";
            // Usually the text is the Form attribute of the word.
            string form = Info.Form;
            // If the space after is not cancelled, inserts a gap behind the word form.
            if (!Info.Misc.ContainsKey("SpaceAfter") || Info.Misc["SpaceAfter"] != "No")
            {
                return form + ' ';
            }
            return form;
        }

        /// <summary>
        /// Specifies if the word can lie as the first word of the sentence.
        /// </summary>
        /// <param name="reason">The reason, when the word can not be the first word of the sentence.</param>
        /// <returns>true if the word can be the first in a sentence; otherwise, false</returns>
        public override bool CanStartNewSentence(out string reason)
        {
            // If the word lies in a multiword, it can not start a sentence (only if it is the first word of the multiword)
            if (IsJoined && JoinedWord.From != Id)
            {
                reason = "You can not split sentence in the middle of the multiword token.";
                return false;
            }
            reason = "";
            return true;
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
        /// Gets the context menu of the word according to the representation.
        /// </summary>
        /// <param name="visitor">The visitor that manages the curent representation.</param>
        /// <param name="sentences">The list of sentences where the word lies in.</param>
        /// <param name="form">The struct to command the main form to change the active word or sentence.</param>
        /// <returns>The right context menu with all actions.</returns>
        public override ContextMenuStrip GetContextMenu(IGetMenuVisitor visitor, ListOfSentences sentences, CurrentState form)
        {
            return visitor.GetMenu(this, sentences, form);
        }
    }
}