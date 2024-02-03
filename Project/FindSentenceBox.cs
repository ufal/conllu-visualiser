using System;
using System.Windows.Forms;

namespace Finder
{
    public partial class FindSentenceBox : Form
    {
        /// <summary>
        /// Returns the value that the user filled into the sent_id textbox.
        /// </summary>
        public string SentId => idBox.Text;

        /// <summary>
        /// Returns the value that the user filled into the textbox with the regular expression.
        /// </summary>
        public string SentRegex => sentBox.Text;

        /// <summary>
        /// Creates the form to search for the sentence.
        /// </summary>
        /// <param name="nextSentence">The event handler that handles the click on the <seealso cref="OKButton"/></param>
        public FindSentenceBox(EventHandler nextSentence)
        {
            InitializeComponent();
            // Sets the handler.
            OKHandler = nextSentence;
        }

        /// <summary>
        /// Handles a button click event. Starts searching for the next sentence.
        /// </summary>
        private EventHandler OKHandler;

        /// <summary>
        /// The evet handler that handles the click on the OK button.
        /// </summary>
        /// <param name="sender">The clicked button</param>
        /// <param name="e"></param>
        private void OKButton_Click(object sender, EventArgs e)
        {
            OKHandler.Invoke(sender, e);
        }
    }
}
