using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ConlluVisualiser
{
    /// <summary>
    /// The class that represents one sentence with words and their information in CoNLL-U format
    /// </summary>
    public class Sentence : ISentence
    {
        /// <summary>
        /// The extra node in the sentence. In visualisation the root is the root of the sentence tree and has no parent.
        /// Its id is allways '0'
        /// </summary>
        public ITreeWord Root { get; private set; }
        /// <summary>
        /// The position in the file according to order of all sentences in the file
        /// </summary>
        public int PlaceInFile { get; set; }
        /// <summary>
        /// The count of all words in the sentence including the root
        /// </summary>
        public int CountWords { get => AllWords.Count; }
        /// <summary>
        /// The unique id of the sentence
        /// </summary>
        public string Sent_id { get => SentenceInfo.Sent_id; } 
        /// <summary>
        /// The text of the sentence
        /// </summary>
        public string Text_attribute { get; private set; }
        /// <summary>
        /// The info attributes of the sentence. There is at least the sent_id attribute.
        /// </summary>
        private SentenceInfo SentenceInfo { get; }
        /// <summary>
        /// The list of all the words inside the sentence
        /// </summary>
        private List<IWord> AllWords { get; set; } = new List<IWord>();
        /// <summary>
        /// Creates a new sentence with words from array <paramref name="words"/> that do not have any additional information.
        /// The root will be a parent of all the words
        /// </summary>
        /// <param name="words">The array of words in the sentence</param>
        /// <param name="sent_id">The unique id of the sentence</param>
        public Sentence(string[] words, string sent_id, ListOfSentences sentences)
        {
            // Creates the root with null parent and id "0"
            Root = new BasicWord(null, "0");
            AllWords.Add(Root);
            if (words != null)
            {
                int id = 1;
                foreach (var word in words)
                {
                    // Adds all not empty words, creates their ids by the order in the sentence.
                    if (word != "")
                    {
                        BasicWord newWord = new BasicWord(Root, id.ToString());
                        newWord.Info.Form = word;
                        AllWords.Add(newWord);
                        id++;
                    }
                }
            }
            List<string> comment = new List<string>
            {
                // Adds only the sentence id to the sentence info
                "sent_id =" + sent_id
            };
            SentenceInfo = new SentenceInfo(this, comment, sentences);
            // Makes the text from the words and actualizes the sentence info
            MakeText();
        }

        /// <summary>
        /// Creates the new sentence with prepared words and information
        /// </summary>
        /// <param name="Root">The new root of the sentence</param>
        /// <param name="all">All words in the sentence in the same order as in the sentence</param>
        /// <param name="sent_id">The unique sentence id</param>
        /// <param name="info">The list of information about the sentence</param>
        public Sentence(BasicWord Root, List<IWord> all, List<string> info, ListOfSentences sentences)
        {
            AllWords = all;
            this.Root = Root;
            SentenceInfo = new SentenceInfo(this, info, sentences);
            // Actualizes the text of the sentence
            MakeText();
        }
        
        /// <summary>
        /// Changes the id
        /// </summary>
        /// <param name="new_id">The new sentence id</param>
        public void SetId(string new_id)
        {
            SentenceInfo.Sent_id = new_id;
        }

        /// <summary>
        /// Add the given word to the given position in the sentence and actualizes the text of the sentence.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">If the position is out of the range of the words</exception>
        /// <param name="word">The new word that will be added to the sentence</param>
        /// <param name="position">The position in the sentence where the word will lie</param>
        public void AddWord(IWord word, int position)
        {
            if (position > AllWords.Count || position < 0)
                throw new ArgumentOutOfRangeException();
            AllWords.Insert(position,word);
            // It is needed to actualize the text
            MakeText();
        }

        /// <summary>
        /// Saves all the sentence (the information about the whole sentence and the info about all words) to the file
        /// </summary>
        /// <param name="stream">The stream to which the information will be written</param>
        public void SaveTo(StreamWriter stream)
        {
            // Saves the information about the whole sentence
            SentenceInfo.SaveToFile(stream);
            // Saves the information about all words
            for (int i = 1; i < AllWords.Count; i++)
            {
                AllWords[i].SaveToFile(stream);
            }
        }

        /// <summary>
        /// Shows the form with information about the whole sentence.
        /// </summary>
        public void ShowInfo()
        {
            SentenceInfo.ShowInfo();
        }

        /// <summary>
        /// Deletes a word from the sentence
        /// </summary>
        /// <param name="word">Word which is going to be removed</param>
        /// <param name="shiftIds">True if the following ids should decrease after deleting the word; false otherwise</param>
        public void DeleteWord(IWord word, bool shiftIds)
        {
            // Removes the word wrom the list of all words
            int position = AllWords.IndexOf(word);
            AllWords.Remove(word);
            // If the ids should be shifted, decrease the following ids by 1
            if (shiftIds)
            {
                for (int i = position; i < AllWords.Count; i++)
                {
                    IWord w = AllWords[i];
                    w.ShiftId(-1);
                }
            }
            // Actualizes the text of the sentence
            Text_attribute = MakeText();
        }

        /// <summary>
        /// Creates or actualizes the text of the sentence according to all words in the sentence
        /// </summary>
        /// <returns>The created text of the sentence</returns>
        public string MakeText()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var word in AllWords)
            {
                // Returns the form of the word which should be inserted into sentence text
                sb.Append(word.GetFormToSentence());
            }
            // Actualizes the information in sentence info and the text attribute
            Text_attribute = sb.ToString().Trim();
            SentenceInfo.ChangeText(Text_attribute);
            return sb.ToString();
        }

        /// <summary>
        /// Returns the word on the given index in the sentence word order
        /// </summary>
        /// <param name="index">The index where the searched word lies</param>
        /// <returns>The word on the index in the sentence word order if any; otherwise, null</returns>
        public IWord GetWord(int index)
        {
            if (index >= AllWords.Count || index < 0) return null;
            else return AllWords[index];
        }

        /// <summary>
        /// Returns the word with the given Id in the sentence
        /// </summary>
        /// <param name="id">The Id of searched word</param>
        /// <returns>The word on with the given Id, if it is present; otherwise, null</returns>
        public IWord GetWord(string id)
        {
            return AllWords.SingleOrDefault(x => x.Id == id);
            //return AllWords.FirstOrDefault(x => x.Id == id);
        }

        /// <summary>
        /// Returns the index of the word according to the word order in the sentence.
        /// </summary>
        /// <param name="word">The word whose index is searched for</param>
        /// <returns>The index of the <paramref name="word"/> if present; otherwise -1</returns>
        public int GetIndexOf(IWord word)
        {
            if (!AllWords.Contains(word))
            {
                return -1;
            }
            else
            {
                return AllWords.IndexOf(word);
            }
        }

        /// <summary>
        /// Swaps two neighbouring words in the sentence. If the words have Empty nodes behind, shift the empty nodes with the word
        /// </summary>
        /// <param name="first">One of the swapped word</param>
        /// <param name="second">One of the swapped word</param>
        public void Swap(IWord first, IWord second)
        {
            // Finds out the order of the words and calls the same method again if the words are in downward order.
            int firstIndex = GetIndexOf(first);
            int secondIndex = GetIndexOf(second);
            if (firstIndex > secondIndex)
            {
                Swap(second, first);
                return;
            }
            // Moves the second word before the first word.
            AllWords.Remove(second);
            AllWords.Insert(firstIndex, second);
            if (second is ITreeWord)
            {
                int i = 1;
                // Moves all the empty nodes behind their main word.
                foreach (var nextEmpty in (second as ITreeWord).EmptyNodes)
                {
                    AllWords.Remove(nextEmpty);
                    AllWords.Insert(firstIndex + i, nextEmpty);
                    i++;
                }
            }
            // Actualizes the text of the sentence.
            MakeText();
        }

        /// <summary>
        /// Creates a new word and inserts it to the sentence after the 
        /// <paramref name="preceding"/> word with the given <paramref name="parent"/>
        /// </summary>
        /// <param name="parent">The parent of the new word.</param>
        /// <param name="preceding">The preceding word of the new word. Must be basic word with the numeric id.</param>
        /// <returns>The new inserted word or null, if it failed.</returns>
        public IWord InsertChild(ITreeWord parent, ITreeWord preceding)
        {
            // The preceding word must have the numeric id
            if (!int.TryParse(preceding.Id, out int prec_id))
            {
                return null;
            }
            // Gets the position of the word
            int position = GetIndexOf(GetWord((prec_id).ToString()));
            position += preceding.EmptyNodes.Count + 1;
            // Creates the new word with increased id and given parent.
            ITreeWord new_word = new BasicWord(parent, (prec_id + 1).ToString());
            AllWords.Insert(position, new_word);
            // Increase all ids in the sentence by one.
            for (int i = position + 1; i < AllWords.Count; i++)
            {
                IWord w = AllWords[i];
                w.ShiftId(1);
            }
            // If the preceding word is ińside of the multiword token, insert the new word into the token
            if (preceding.IsJoined)
            {
                preceding.JoinedWord.AddSubWord(new_word, this);
            }
            return new_word;
        }

        /// <summary>
        /// Splits the sentence into two sentences. The second sentence will start by the <paramref name="firstWord"/>.
        /// </summary>
        /// <param name="firstWord">The starting word of the second sentence</param>
        /// <param name="new_id">The id of the new sentence</param>
        /// <param name="sentences">The list where the splitted sentence lies</param>
        /// <returns>The second part of the original sentence or null, if it failed</returns>
        public ISentence Split(IWord firstWord, string new_id, ListOfSentences sentences)
        {
            // Checks if the word is able to be thestarting word. If not, shows the message with the reason and returns null.
            if (!firstWord.CanStartNewSentence(out string reason))
            {
                MessageBox.Show(reason);
                return null;
            }           
            // Creates a new sentence with no words and with the new id
            ISentence new_sentence = new Sentence(null, new_id, sentences);
            // The shift by which all the word ids in the second sentence will be decreased
            int fWID = AllWords.IndexOf(firstWord);
            // Removes all the deps attributes, where the words does not remain in one sentence.
            for (int i = 0; i < fWID; i++)
            {
                RemoveDeps(AllWords[i], (x) => { return x >= fWID; });
            }
            for (int i = fWID; i < CountWords; i++)
            {
                RemoveDeps(AllWords[i], (x) => { return x < fWID; });
            }
            // For all the words which will rest in the first sentence.
            for (int i = 0; i < fWID; i++)
            {
                IWord word = GetWord(i);
                if (word is ITreeWord && !(word as ITreeWord).IsRoot)
                {
                    // If the parent of the word will be moved to the second sentence, the new parent will be the root
                    if (GetIndexOf((word as ITreeWord).Parent) >= fWID)
                        Root.AddChild((word as ITreeWord), true);
                }
            }
            // For the second part of the sentence
            for(int i = fWID; i < CountWords;i++)
            {
                IWord wordToNext = GetWord(i);
                if (wordToNext is ITreeWord && !(wordToNext as ITreeWord).IsRoot)
                {
                    // If the parent is in the first half of the sentence, the new parent will be Root
                    if (AllWords.IndexOf((wordToNext as ITreeWord).Parent) < fWID)
                    {
                        new_sentence.Root.AddChild((wordToNext as ITreeWord), true);
                    }
                }
                // Shifts the id so that the ids in the second sentence start from 1
                wordToNext.ShiftId(-fWID + 1);
                // Adds the word to the second sentence
                new_sentence.AddWord(wordToNext, new_sentence.CountWords);
            }
            // Removes the remaining words from the first sentence
            AllWords.RemoveRange(fWID, CountWords - fWID);            
            // Actualizes the text of the sentence
            MakeText();
            return new_sentence;
        }

        /// <summary>
        /// Removes the deps of <paramref name="word"/>, that meet the <paramref name="predicate"/>.
        /// </summary>
        /// <param name="word">The word whose deps are being removed.</param>
        /// <param name="predicate">The predicate according to which the dep is removed or not.</param>
        private void RemoveDeps(IWord word, Func<int, bool> predicate)
        {
            List<IWord> removed = new List<IWord>();
            // Prepares the collection of removed. It can not remove it while iterating over the collection.
            foreach (var dep in word.Info.Deps)
            {
                IWord par = GetWord(dep.Key);
                if (predicate(GetIndexOf(par)))
                {
                    removed.Add(par);
                }
            }
            // Removes the prepared deps. 
            foreach(var rem in removed)
            {
                // Removes the child from its parent and removes its parent from the deps attribute.
                rem.RemoveChildEnhhanced(word);
            }
        }
    }
}
