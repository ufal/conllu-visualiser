using System.Linq;
using System.Windows.Forms;
using ConlluVisualiser;

namespace Finder
{
    /// <summary>
    /// Class to find a node in the list of sentences. Implements the interface IFinder.
    /// </summary>
    class NodeFinder :IFinder
    {
        /// <summary>
        /// Specifies the index of the last found word in the sentence which is currently searched.
        /// </summary>
        private int LastWordIndex { get; set; } = -1;
        /// <summary>
        /// The sentence which is currently searched.
        /// </summary>
        private ISentence Sentence { get; set; }
        /// <summary>
        /// The last sentence which contained the searched node.
        /// </summary>
        private string LastFoundSentence { get; set; }
        /// <summary>
        /// The defined parameters of the word, which the user searches for.
        /// </summary>
        private WordInfo SelectedParams { get; set; }
        /// <summary>
        /// The mean by which it is possible to change the active word or sentence in the main form.
        /// </summary>
        private CurrentState Form { get; }
        /// <summary>
        /// The list of sentences which is searched.
        /// </summary>
        private ListOfSentences Sentences { get; }
        /// <summary>
        /// Creates a finder that search in the list of sentences <paramref name="sents"/> for the specific node
        /// </summary>
        /// <param name="f">The commander which can change the active word or sentence in the main form.</param>
        /// <param name="sents">The list of sentences which is searched</param>
        public NodeFinder(CurrentState f, ListOfSentences sents)
        {
            Form = f;
            Sentences = sents;
        }
        /// <summary>
        /// The method that starts searching in the list of sentences. 
        /// User defines the parameters of the wanted word and the sentences are searched until the user closes the window.
        /// </summary>
        /// <param name="first">The sentence where the searching starts</param>
        public void Find(ISentence first)
        {
            SelectedParams = new WordInfo();
            Sentence = first;
            LastFoundSentence = first.Sent_id;
            // Creation of the dialog to allow user to define the attributes.
            WordFieldsForm dialog = new WordFieldsForm(SelectedParams, new FindNodeValidator());
            // Initialize the button on the dialog with the text and actions.
            // After click on the button, the wanted attributes are saved (or modified) and the next node is searched for.
            dialog.InitButton("Find next word", (e, sender) => { SelectedParams.SaveChanges(dialog.GetState()); FindNextNode(); });
            dialog.ShowDialog();
        }
        
        /// <summary>
        /// Method to find the next node according to the <seealso cref="SelectedParams"/>
        /// </summary>
        private void FindNextNode()
        {
            // Returns the first node which corresponds with parameters after the last found node. Returns null if there is no such a word.
            var result = FindFirstNodeFromIndex(SelectedParams);
            // If the word exists, changes it in the main form
            if (result != null)
            {
                Form.ChangeActiveSentence(Sentence);
                Form.ChangeActiveWord(result);
            }
            else
            {
                // The word does not exist. Asks the user if he wants to search from the beginning.
                DialogResult change = MessageBox.Show("No word matches the parameters. Do you want to search from beginning?", 
                                                      "Change your parameters", MessageBoxButtons.YesNo);
                // If the user does not want to search from the beginning, nothing is searched for. He can change the parameters or close the finder.
                if (change == DialogResult.No)
                {
                    Sentence = Sentences.GetSentence(LastFoundSentence);
                    Form.ChangeActiveWord(null);
                }
                // If the user wants to search from the beginning, the searching starts again.
                else
                {
                    // Sets the starting index befor the first word
                    LastWordIndex = -1;
                    // Gets the first sentence of the list
                    Sentence = Sentences.GetSentence(0);
                }
            }
        }

        /// <summary>
        /// Starts searching from the last found. Finds the first node, which corresponds with the user defined parameters.
        /// Returns null if there is no such a node.
        /// </summary>
        /// <param name="findBox">The user defined parameters of the searched word</param>
        /// <returns>First found node which corresponds with the parameters. Null, if there is not such a node.</returns>
        private IWord FindFirstNodeFromIndex( WordInfo findBox)
        {
            IWord word;
            // Gets the index of the last found sentence.
            int actId = Sentence.PlaceInFile;
            // Searches until the sentence does not contain the right word.
            while ((word = FindFirstInSentence(findBox)) == null)
            {
                actId++;
                // Searching from the beginning of the sentence.
                LastWordIndex = -1;
                Sentence = Sentences.GetSentence(actId);
                // If there in no other sentence, stops the searching and returns null
                if (Sentence == null)
                    return null;
            }
            // Returns the found word.
            return word;
        }

        /// <summary>
        /// Returns the first node from the sentence starting by the index <seealso cref="LastWordIndex"/> + 1.
        /// Returns null if there is not such a node.
        /// </summary>
        /// <param name="findBox">>The user defined parameters of the searched word</param>
        /// <returns>The next node in the sentence starting from the last found. Returns null if there is not such a node.</returns>
        private IWord FindFirstInSentence(WordInfo findBox)
        {
            IWord word;
            // Shifts from the last found to start on the next node.
            int i = LastWordIndex + 1;
            // Searches to the end of the sentence.
            while ((word = Sentence.GetWord(i)) != null)
            {
                // Tests if the node corresponds with the user defined parameters.
                if (CanBeApplied(word.Info, findBox))
                {
                    // If the good word is found, returns it and sets it's index as the last found
                    LastWordIndex = i;
                    return word;
                }
                i++;
            }
            // The word was not found in the sentence, returns null.
            return null;
        }

        /// <summary>
        /// Checks one property, if it corresponds with the user's definition.
        /// If the user did not fill the attribute, it allways corresponds. If he did, the values must be the same (case insensitive)
        /// </summary>
        /// <param name="thisProperty">The property of cheched word</param>
        /// <param name="findProperty">The parameter of the user</param>
        /// <returns>False if the searching attribute is filled and different from the node's attribute. Otherwise, true.</returns>
        private bool PropertyIsOk(string thisProperty, string findProperty)
        {
            // If the searched parameter is not filled, aevery word meets it.
            if (findProperty == "" || findProperty == null)
                return true;
            // Returns true if they are same
            else return thisProperty.ToLower() == findProperty.ToLower();
        }

        /// <summary>
        /// Specifies whether the set of user defined parameters corresponds with the word info attributes.
        /// </summary>
        /// <param name="word">The info of the currently cheched word</param>
        /// <param name="findBox">The user defined parameters</param>
        /// <returns>true if the word is suitable; otherwise, false</returns>
        private bool CanBeApplied(WordInfo word, WordInfo findBox)
        {
            bool EverythingCanBeApplied = true;
            // To correspond, all checks of properties must return true
            EverythingCanBeApplied &= PropertyIsOk(word.Id, findBox.Id);
            EverythingCanBeApplied &= PropertyIsOk(word.Form, findBox.Form);
            EverythingCanBeApplied &= PropertyIsOk(word.Lemma, findBox.Lemma);
            EverythingCanBeApplied &= PropertyIsOk(word.Upos, findBox.Upos);
            EverythingCanBeApplied &= PropertyIsOk(word.Xpos, findBox.Xpos);
            // In the dictionary values, the word info must contain completelly same key/value pair as in the user's parameters..
            foreach (var f in findBox.Feats)
            {
                EverythingCanBeApplied &= word.Feats.Contains(f);
            }
            EverythingCanBeApplied &= PropertyIsOk(word.Head, findBox.Head);
            EverythingCanBeApplied &= PropertyIsOk(word.Deprel, findBox.Deprel);
            foreach (var d in findBox.Deps)
            {
                EverythingCanBeApplied &= word.Deps.Contains(d);
            }
            foreach (var m in findBox.Misc)
            {
                EverythingCanBeApplied &= word.Misc.Contains(m);
            }
            // True only if everything corresponds.
            return EverythingCanBeApplied;
        }
    }
}
