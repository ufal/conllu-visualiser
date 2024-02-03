using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GraphVisualiser;

namespace ConlluVisualiser
{
    /// <summary>
    /// Class representing a multiword token in the sentence
    /// </summary>
    public class MultiWord : Word, IWord
    {
        /// <summary>
        /// The set of the words inside the multiword token
        /// </summary>
        private List<ITreeWord> subWords = new List<ITreeWord>();
        /// <summary>
        /// The id of the first word in the multiword token
        /// </summary>
        public string From { get; set; } = null;
        /// <summary>
        /// The id of the last word in the multiword token
        /// </summary>
        public string To { get; set; } = null;
        public override WordPoint GetWordPoint()
        {
            return new WordPoint();
        }
        /// <summary>
        /// The floor, where the multiword is visualised in the sentence tree
        /// </summary>
        public int MinLevel
        {
            get
            {
                // The level is the minimum level of all subwords
                return subWords.Min(x => x.GetWordPoint().Level);
            }
        }

        /// <summary>
        /// The maximum level of the <seealso cref="subWords"/>
        /// </summary>
        public int GetMaxLevel()
        {
            return subWords.Max(x => x.GetWordPoint().Level);
        }
        /// <summary>
        /// Creates a multiword token with given word Form and id
        /// </summary>
        /// <param name="word">The Form attribute of the new multiword</param>
        /// <param name="id">The Id attribute of the new multiword</param>
        public MultiWord(string word, string id) :base(id)
        {
            // The part before the dash is the first id in the multiword token, the second part is the last id
            string[] splitted = id.Split('-');
            From = splitted[0];
            To = splitted[1];
            Info.Form = word;
        }
        /// <summary>
        /// Creates a multiword that joins two words.
        /// </summary>
        /// <param name="first">First word in the multiword</param>
        /// <param name="second">Second word in the multiword</param>
        /// <param name="sentence">The sentence where the words belong</param>
        public MultiWord(IWord first, IWord second, ISentence sentence) : base(first.Id + "-" + second.Id)
        {
            // Creates the default form so that joins the forms together
            Info.Form = first.GetFormToSentence() + second.GetFormToSentence();
            Info.Form.Trim();
            From = first.Id;
            To = second.Id;
            // Adds the subwords. If any of them is a multiword, moves its subwords to this multiword.
            AddSubWord(first, sentence);
            AddSubWord(second, sentence);
        }
        /// <summary>
        /// Adds the subword to the multiword token. Modifies the id, if it is needed.
        /// </summary>
        /// <param name="word">The new subword</param>
        /// <param name="sentence">The sentence where the subword lies</param>
        public void AddSubWord(IWord word, ISentence sentence)
        {
            // If the new word is multiword, removes its subwords and add them into this multiword. 
            if (word is MultiWord)
            {
                foreach (ITreeWord sub in (word as MultiWord).subWords)
                {
                    // Adds the subword which can not be a multiword
                    AddNotJoinedSubWord(sub);
                }
                // Removes the original multiword token. The multiword tokens can not cross
                sentence.DeleteWord(word, false);
                return;
            }
            // Adds the word which is not a multiword
            AddNotJoinedSubWord(word as ITreeWord);
        }
        /// <summary>
        /// Adds a single <paramref name="word"/> to the subwords. The <paramref name="word"/> can not be a multiword token.
        /// </summary>
        /// <param name="word"></param>
        private void AddNotJoinedSubWord(ITreeWord word)
        {
            subWords.Add(word);
            word.JoinedWord = this;
            // Actualizes the last id to be the maximum Id in the group of subwords
            To = subWords.Max(x => int.Parse(x.Id)).ToString();
            ActualizeId();
        }

        /// <summary>
        /// Returns the form which will be on the word place in the sentence
        /// </summary>
        /// <returns>The word which makes a part of the text of the sentence</returns>
        public override string GetFormToSentence()
        {
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
        /// Actualizes the id so that it corresponds with the values <seealso cref="From"/> and <seealso cref="To"/>
        /// </summary>
        private void ActualizeId()
        {
            Id = From + "-" + To;
        }

        /// <summary>
        /// Removes the multiword token. Does not remove the words inside a token.
        /// </summary>
        /// <param name="sentence">The sentence where the multiword token lies.</param>
        public override void Delete(ISentence sentence)
        {
            // Removes the multiword from all its subwords
            foreach(var word in subWords)
            {
                word.JoinedWord = null;
            }
            // Null id specifies that the word is deleted
            Id = null;
            // Deletes the token from the sentence, but does not shift ids of following words (the do not change)
            sentence.DeleteWord(this, false);
        }

        /// <summary>
        /// Deletes the word from the subwords. If there rests only one word, removes the multiword token.
        /// </summary>
        /// <param name="wordToDelete">The word which is going to be removed</param>
        /// <param name="sentence">The sentence where the multiword token lies</param>
        public void DeleteSubWord(ITreeWord wordToDelete, ISentence sentence)
        {
            subWords.Remove(wordToDelete);
            // Last id allways decrease by 1
            To = (int.Parse(To)-1).ToString();
            // If there resrs only the last subword, removes the whole token
            if(subWords.Count == 1)
            {
                Delete(sentence);
            }
            ActualizeId();
        }

        /// <summary>
        /// Returns the subword with the Id <paramref name="strId"/> or null, if not present
        /// </summary>
        /// <param name="strId">The id whose word is searched for</param>
        /// <returns>The found subword or null, if nothing found.</returns>
        public IWord GetSubWord(string strId)
        {
            return subWords.Where(x => x.Id == strId).SingleOrDefault();
        }

        /// <summary>
        /// Shifts the id by the <paramref name="shift"/> parameter. Modifies the <seealso cref="From"/> and <seealso cref="To"/> values.
        /// </summary>
        /// <param name="shift">The number by which the id is decreased (negative) or increased (positive).</param>
        public override void ShiftId(int shift)
        {
            From = (int.Parse(From) + shift).ToString();
            To = (int.Parse(To) + shift).ToString();
            ActualizeId();
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
        /// Gets the context menu of the multiword according to the representation.
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
            return true;
        }
    }
}
