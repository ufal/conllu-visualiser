using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ConlluVisualiser;

namespace Finder
{
    /// <summary>
    /// Searches the sentence in the file according to the parameters from the user.
    /// </summary>
    class SentenceFinder : IFinder
    {
        /// <summary>
        /// The sentence that is being explored to see if the parameters fit.
        /// </summary>
        private ISentence Sentence { get; set; }
        /// <summary>
        /// The mean to change the active sentence and active word in the main form.
        /// </summary>
        private CurrentState Form { get; }
        /// <summary>
        /// The list of sentences in the file that is searched.
        /// </summary>
        private ListOfSentences Sentences { get; }
        /// <summary>
        /// The last sentence that matches the parameters.
        /// </summary>
        private string LastFoundSentence { get; set; }

        /// <summary>
        /// Creates the finder which search in <paramref name="sents"/>.
        /// It can change active sentence in the <paramref name="form"/>.
        /// </summary>
        /// <param name="form">The mean to command the main form to change active sentence.</param>
        /// <param name="sents">The list of the sentences in which the sentence is looked for.</param>
        public SentenceFinder(CurrentState form, ListOfSentences sents)
        {
            Form = form;
            Sentences = sents;
        }

        /// <summary>
        /// The method to start searching in the <seealso cref="Sentences"/>
        /// </summary>
        /// <param name="first">The sentence where the searching starts.</param>
        public void Find(ISentence first)
        {
            // Gets the previous sentence in the list of the sentenes.
            Sentence = Sentences.GetSentence(first.PlaceInFile - 1);
            // It makes the last sentence the previous sentence.
            LastFoundSentence = first.Sent_id;
            // Opens the dialog to insert the parameters and search for the sentence.
            FindSentenceBox dialog = new FindSentenceBox(FindNextSentence);
            dialog.ShowDialog();
        }

        /// <summary>
        /// Finds the first sentence from the last found sentence according to the pattern.
        /// </summary>
        /// <param name="pattern">The pattern to which the sentence is tested.</param>
        /// <returns>The first found sentence, null if there is no sentence that fits the parameters.</returns>
        private ISentence FindFirstSentenceFromIndex(string pattern)
        {
            // Gets the sentence where the searching starts. Gets the following sentence from the last found. 
            // If the the last sentence is null, starts searching from beginning.
            int actId = Sentence != null ? Sentence.PlaceInFile + 1 : 0;
            Sentence = Sentences.GetSentence(actId);
            // Creates the regular expression from the pattern.
            Regex r = new Regex(pattern);
            // While the sentence does not fit the parameters, search in the next sentence.
            // Or until the sentence list is over.
            while (Sentence != null && !r.IsMatch(Sentence.Text_attribute))
            {
                actId++;
                // Next sentence
                Sentence = Sentences.GetSentence(actId);
                // If the list is over.
                if (Sentence == null)
                    return null;
            }
            return Sentence;
        }

        /// <summary>
        /// Gets the first sentence from the last found sentence.
        /// </summary>
        /// <param name="findForm">The form where the parameters are written.</param>
        /// <param name="validRegex">False if the user tried to find the sentence with using of nonvalid pattern; otherwise, true. </param>
        /// <returns>The first found sentence.</returns>
        private ISentence GetFirstSentenceAccordingToParams(FindSentenceBox findForm, out bool validRegex)
        {
            ISentence result = null;
            // The user searches according to the sent_id.
            if (findForm.SentId != "")
            {
                result = Sentences.GetSentence(findForm.SentId);
            }
            // The user searches according to the regular pattern.
            else
            {
                // If the pattern is not empty
                if (!string.IsNullOrEmpty(findForm.SentRegex))
                {
                    // If the pattern is not valid, the ArgumentException is thrown.
                    try
                    {
                        result = FindFirstSentenceFromIndex(findForm.SentRegex);
                    }
                    catch (ArgumentException)
                    {
                        // Not valid pattern
                        Sentence = Sentences.GetSentence(LastFoundSentence);
                        MessageBox.Show("The expression must be a valid regular expression.");
                        validRegex = false;
                        return null;
                    }
                }
            }
            // Returns the found sentence.
            validRegex = true;
            return result;
        }

        /// <summary>
        /// The handler that handle the clock on the find button on the find form.
        /// </summary>
        /// <param name="sender">The clicked button.</param>
        /// <param name="e"></param>
        private void FindNextSentence(object sender, EventArgs e)
        {
            // Gets the original form.
            FindSentenceBox findForm = (FindSentenceBox)((Button)sender).FindForm();
            // Gets the first matching sentence.
            ISentence result = GetFirstSentenceAccordingToParams(findForm, out bool validRegex);
            // If there exist the right sentence.
            if (result != null)
            {
                // Changes the active sentence in the main form
                Form.ChangeActiveSentence(result);
                // Saves the found sentence as the last found.
                LastFoundSentence = result.Sent_id;
            }
            // If the patern was the valid regular expression, but there is no matching sentence.
            else if (validRegex)
            {
                DialogResult change;
                // The searching did not start from the first sentence - there are some sentences in the beginning of the file, that were not tested.
                if (LastFoundSentence != null)
                    change = MessageBox.Show("No other sentence matches the parameters. Do you want to search from beginning?", 
                        "Change your parameters", MessageBoxButtons.YesNo);
                // The whole file was tested.
                else
                    change = MessageBox.Show("No sentence matches the parameters. Do you want to search from beginning?", 
                        "Change your parameters", MessageBoxButtons.YesNo);
                // The user does not want to search from the beginning.
                if (change == DialogResult.No)
                {
                    // Does not change the last found sentence.
                    Sentence = Sentences.GetSentence(LastFoundSentence);
                    // No word is active.
                    Form.ChangeActiveWord(null);
                }
                // The user does wants to search from the beginning.
                else
                {
                    // The searching will start from the first sentence of the file.
                    Sentence = null;
                    // Continue in searching.
                    FindNextSentence(sender, e);
                }
            }
        }
    }
}
