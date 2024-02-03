using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ConlluVisualiser
{
    /// <summary>
    /// The form that allows the user to insert a new sentence to the file.
    /// </summary>
    public partial class InsertNewSentenceBox : Form
    {
        /// <summary>
        /// The current list of sentences in the file. Used only to find duplicities in user filled sent_id.
        /// </summary>
        private Dictionary<string, ISentence> Sentences { get; }

        /// <summary>
        /// Returns the sent_id that the user filled into the proper box.
        /// </summary>
        /// <returns>Filled sent_id value.</returns>
        public string GetIdBox()
        {
            return idBox.Text;
        }

        /// <summary>
        /// Returns the whole textbox with the text of the sentence.
        /// </summary>
        /// <returns>The textbox with the text of the sentence.</returns>
        public TextBox GetSentence()
        {
            return sentBox;
        }

        /// <summary>
        /// Opens the box where the user can fill the values to.
        /// The user must fill the sent_id value, the text value is optional.
        /// </summary>
        /// <param name="sentences"></param>
        public InsertNewSentenceBox(Dictionary<string, ISentence> sentences)
        {
            InitializeComponent();
            Sentences = sentences;
        }

        /// <summary>
        /// Handles the click on the OK button. Checks if the field sent_id has right format and is unique in the file.
        /// </summary>
        /// <param name="sender">The <seealso cref="OKButton"/></param>
        /// <param name="e">Event arguments.</param>
        private void Submit(object sender, EventArgs e)
        {
            // The sent_id can not be empty.
            if(GetIdBox() == "")
            {
                MessageBox.Show("The id can not be empty.");
                return;
            }
            // The sent id must be unique.
            if (Sentences.ContainsKey(GetIdBox()))
            {
                MessageBox.Show("This id is occupied! You must use another one.");
                return;
            }
            // The insertion can be done.
            DialogResult = DialogResult.OK;
        }        
    }
}

