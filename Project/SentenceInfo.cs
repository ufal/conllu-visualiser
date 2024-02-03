using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ConlluVisualiser
{
    /// <summary>
    /// The class which contains the information attributes of the sentence.
    /// </summary>
    class SentenceInfo
    {
        /// <summary>
        /// The table which contains information.
        /// </summary>
        private DataGridView TableRoot { get; } = new DataGridView();
        /// <summary>
        /// The sentence whose info is saved here.
        /// </summary>
        private Sentence Sentence { get; set; }
        /// <summary>
        /// List of the information attributes.
        /// </summary>
        public List<string> Info { get; private set; }
        public string Sent_id { get; set; }
        /// <summary>
        /// Saves the information from parameter <paramref name="i"/> about the <paramref name="sent"/>
        /// </summary>
        /// <exception cref="ArgumentException">Throws Argument exception if the info does not contain the sent_id attribute.</exception>
        /// <param name="sent">The sentence whose info is saved here</param>
        /// <param name="i">The list with info attributes</param>
        public SentenceInfo(Sentence sent, List<string> info, ListOfSentences sentences)
        {
            Sentence = sent;
            Info = info;
            // Initialize the table
            InitTable(sentences);
            for (int i = 0; i < Info.Count; ++i)
            {
                var line = Info[i].Split('=');
                if (line[0].Trim() == "sent_id")
                {
                    Sent_id = line[1];
                    return;
                }
            }
            throw new ArgumentException();
        }

        /// <summary>
        /// Shows the form with the tabe with the sentence attributes
        /// </summary>
        public void ShowInfo()
        {
            // Deletes the rows from the table and add the actualized rows
            TableRoot.Rows.Clear();
            AddRows();
            // Creates the form with the save button to save changes and with the table.
            Form form = new Form();
            // Creation of the save button
            Button save = new Button
            {
                Dock = DockStyle.Bottom,
                Text = "Save changes",
                Font = new Font("Microsoft Sans Serif", 8.5f, FontStyle.Bold),
                BackColor = Color.White,
            };          
            save.Click += (sender, e) => { SaveChanges(); };
            // Add the created controls ito the form
            form.Controls.Add(TableRoot);
            form.Controls.Add(save);
            form.Height = TableRoot.Rows.GetRowsHeight(DataGridViewElementStates.Visible) + SystemInformation.HorizontalScrollBarHeight * 2
                        + save.Height + SystemInformation.CaptionHeight;
            form.BackColor = Control.DefaultBackColor;
            TableRoot.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            form.ShowDialog();
        }

        /// <summary>
        /// Initializes the table - Adds column, creates user's rights
        /// </summary>
        private void InitTable(ListOfSentences sentences)
        {
            // Add one column
            TableRoot.Columns.Add("Comments", "Comments");
            TableRoot.ColumnHeadersVisible = false;
            TableRoot.CellValidating += (sender, e) => { ValidateCell(e, sentences); };
            // Makes the table to fill all the form
            TableRoot.Dock = DockStyle.Fill;
            // Defines user's rights
            TableRoot.AllowUserToAddRows = true;
            TableRoot.AllowUserToDeleteRows = true;
            TableRoot.AllowUserToResizeRows = false;
            TableRoot.AllowUserToOrderColumns = false;
            TableRoot.AllowUserToResizeColumns = true;
        }

        /// <summary>
        /// Adds the rows with attributes from <seealso cref="Info"/> list to the <seealso cref="TableRoot"/>.
        /// </summary>
        private void AddRows()
        {
            foreach (var infoItem in Info)
            {
                TableRoot.Rows.Add(infoItem);
            }
        }

        /// <summary>
        /// Checks if the modified value is not the id of another sentence
        /// </summary>
        /// <param name="e">The event arguments</param>
        /// <param name="sentences">The list of sentences where the sentence lies.</param>
        private void ValidateCell(DataGridViewCellValidatingEventArgs e, ListOfSentences sentences)
        {
            string[] arr = e.FormattedValue.ToString().Split('=');
            if (arr[0].Trim() == "sent_id" && arr.Length > 1)
            {
                // If the sentence with the same id exists, delete it.
                if (sentences.GetSentence(arr[1].Trim())!= null)
                {
                    e.Cancel = true;
                    MessageBox.Show("The sentence id must be unique.");
                }
                if (TableRoot.Rows[e.RowIndex].Cells[e.ColumnIndex].FormattedValue.ToString().Split('=')[0].Trim() != "sent_id")
                {
                    e.Cancel = true;
                    MessageBox.Show("One sentence can have only one sent_id.");
                }
                // Changes the sentence id
                sentences.SetId(Sentence, arr[1]);
                Sentence.SetId(arr[1]);
            }

        }

        /// <summary>
        /// Saves the actual state of rows into the list <seealso cref="Info"/>
        /// </summary>
        private void SaveChanges()
        {
            List<string> comments = new List<string>();
            foreach (DataGridViewRow row in TableRoot.Rows)
            {
                // Adds all values
                if ((row.Cells[0]).Value != null)
                    comments.Add(row.Cells[0].Value.ToString());
            }
            Info = comments;
            TableRoot.FindForm().DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// Changes the text attribute
        /// </summary>
        /// <param name="text">The new contain of the text attribute</param>
        public void ChangeText(string text)
        {
            string new_text = "text = " + text;
            int i = 0;
            // Tries to find the text attribute in info. If it is there, replace the text by the new.
            for (i = 0; i < Info.Count; i++)
            {
                if (Info[i].Split('=')[0].Trim() == "text")
                {
                    Info[i] = new_text;
                    break;
                }
            }
            // If no text attribute is in info, creates the new,
            if (i == Info.Count)
            {
                Info.Add(new_text);
            }
        }

        /// <summary>
        /// Saves the information to the file as the attributes of the sentence
        /// </summary>
        /// <param name="stream">The stream where the information are written</param>
        public void SaveToFile(StreamWriter stream)
        {
            foreach (string comment in Info)
            {
                // Writes eaach comment on one line
                stream.WriteLine("# " + comment);
            }
        }
    }
}