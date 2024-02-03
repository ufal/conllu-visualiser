using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ConlluVisualiser
{
    /// <summary>
    /// The list of all sentences in the file.
    /// </summary>
    public class ListOfSentences
    {
        /// <summary>
        /// The sentences stored in the dictionary according to their sent_id.
        /// </summary>
        private Dictionary<string, ISentence> Sentences { get; } = new Dictionary<string, ISentence>();
        /// <summary>
        /// The panel with links to the sentences.
        /// </summary>
        private Panel FilePanel { get; }
        /// <summary>
        /// The panel with the links to switch pages.
        /// </summary>
        private Panel PagePanel { get; }
        /// <summary>
        /// The mean to control the main form and change the active sentence in it.
        /// </summary>
        private CurrentState MainForm { get; }
        /// <summary>
        /// The number of the page where the active sentence is.
        /// If no sentence is active and no page is shown, the value is -1.
        /// </summary>
        private int ShownPage { get; set; } = -1;
        /// <summary>
        /// The symbol which signalizes whether the list of the links should be actualized.
        /// </summary>
        private bool ChangedSentences { get; set; } = false;
        /// <summary>
        /// The list of controls on the <seealso cref="FilePanel"/>.
        /// </summary>
        private List<Control> Ids_on_panel { get; } = new List<Control>();
        /// <summary>
        /// The count of sentence links on one page.
        /// </summary>
        private const int OnOnePage = 70;
        /// <summary>
        /// Color of visited link.
        /// </summary>
        private readonly Color VisitedColor = Color.DarkSlateBlue;
        /// <summary>
        /// Color of active link.
        /// </summary>
        private readonly Color ActiveColor = Color.Salmon;
        /// <summary>
        /// The currently visualised active sentence.
        /// </summary>
        public ISentence ActiveSentence { get; private set;}
        /// <summary>
        /// Creates a new list of sentences in file.
        /// </summary>
        /// <param name="filePanel"></param>
        /// <param name="pagePanel"></param>
        /// <param name="form"></param>
        public ListOfSentences(Panel filePanel, Panel pagePanel, CurrentState form)
        {
            FilePanel = filePanel;
            filePanel.Controls.Clear();
            PagePanel = pagePanel;
            // Clear the panel with pages to add new pages.
            pagePanel.Controls.Clear();
            MainForm = form;
        }

        /// <summary>
        /// Adds the sentence to the list. Does not actualize the page count.
        /// </summary>
        /// <param name="sentence">The sentence which is going to be added.</param>
        /// <param name="index">The sequence number of the sentence in the file.</param>
        public LinkLabel AddNew(ISentence sentence, int index)
        {
            // Saves the sequence number of the sentence.
            sentence.PlaceInFile = index;
            // Adds the sentence to the dictionary
            Sentences.Add(sentence.Sent_id, sentence);
            // Adds the sentence link to the list of links.
            LinkLabel label = MakeSentenceLinkButton(sentence.Sent_id);
            Ids_on_panel.Insert(index, label);
            return label;
        }

        /// <summary>
        /// Inserts the sentence into the list of sentences and shifts all following sentences indexes.
        /// Adds new page if it is necessary.
        /// </summary>
        /// <param name="sentence">The sentence that is going to be added.</param>
        /// <param name="index">The place in the file where the sentence will lie.</param>
        public void InsertNew(ISentence sentence, int index)
        {
            // Shifts all following positions in file.
            for (int i = index; i < Ids_on_panel.Count; i++)
            {
                Sentences[Ids_on_panel[i].Text].PlaceInFile++;
            }
            // The labels on the panel must change.
            ChangedSentences = true;
            // Adds the new label into the list of labels.
            LinkLabel label = AddNew(sentence, index);
            // Adds the page link into the panel with pages, if it is necessary.
            if (Ids_on_panel.Count % OnOnePage == 1)
            {
                // Creates the page button
                var page = MakePageButton((Ids_on_panel.Count / OnOnePage) + 1);
                // Adds the new page button to the end of the page links.
                PagePanel.Controls.SetChildIndex(page, 0);
            }
        }

        /// <summary>
        /// Creates the page link.
        /// </summary>
        /// <param name="num">The page number.</param>
        /// <returns>The added page LinkLabel.</returns>
        private LinkLabel MakePageButton(int num)
        {
            // The page link label whose text is the number from parameter.
            LinkLabel label = new LinkLabel
            {
                Text = num.ToString(),
                AutoSize = true,
                Padding = new Padding(1),
                TabStop = true,
                Dock = DockStyle.Left
            };
            label.LinkClicked += (o,sender) => { Page_LinkClicked(label); };
            // Shows the label as the next page.
            PagePanel.Controls.Add(label);
            return label;
        }

        /// <summary>
        /// Creates the sentence link button.
        /// </summary>
        /// <param name="text">The text that is on the link.</param>
        /// <returns>The created link label.</returns>
        private LinkLabel MakeSentenceLinkButton(string text)
        {
            LinkLabel label = new LinkLabel
            {
                Text = text,
                Dock = DockStyle.Top,
                Padding = new Padding(1),
                AutoSize = true
            };
            label.LinkClicked += (o, sender) => { Sentence_LinkClicked((LinkLabel)o, sender); };            
            return label;
        }

        /// <summary>
        /// Creates all page buttons and shows them in the <seealso cref="PagePanel"/>.
        /// </summary>
        public void ShowAll()
        {
            FilePanel.Controls.Clear();
            PagePanel.Controls.Clear();
            // Counts the number of pages.
            int countOfPages =( Ids_on_panel.Count - 1) / OnOnePage + 1;
            // Creates and adds all page buttons.
            for (int i = countOfPages; i > 0; i--)
            {
                MakePageButton(i);
            }            
        }

        /// <summary>
        /// The event handler that handles the click on the page link. Changes the page and shows the links on the current page.
        /// </summary>
        /// <param name="clickedPage">The button of the page.</param>
        private void Page_LinkClicked(LinkLabel clickedPage)
        {
            // Gets the number of the actual page.
            int actPage = int.Parse(clickedPage.Text);
            // If the clicked page is currently shown and no changes on the shown links were done, nothing happends.
            if (ShownPage == actPage && !ChangedSentences) return;
            ChangedSentences = false;
            // If there is some page that were active before the click, marks it as visited.
            if (ShownPage != -1 && ShownPage != actPage)
            {
                ((LinkLabel)PagePanel.Controls[PagePanel.Controls.Count - ShownPage]).LinkColor = VisitedColor;
            }
            // Marks the clicked link as active.
            clickedPage.LinkColor = ActiveColor;
            // Counts the range of the sentences that will be visualised on the file panel.
            int start = (actPage - 1) * OnOnePage;
            int end = actPage * OnOnePage;
            // The case on the last page
            if (end > Ids_on_panel.Count)
            {
                end = Ids_on_panel.Count;
            }
            FilePanel.Controls.Clear();
            // The width of the longest link.
            int maxSize = 0;
            // Shows all links. It is adding them to the top of the panel from the last to the first.
            for (int i = end-1;i >= start; i--)
            {
                // Adds the link to the panel.
                FilePanel.Controls.Add(Ids_on_panel[i]);
                // Actualizes the maxSize if it is necessary.
                if (Ids_on_panel[i].Width > maxSize)
                    maxSize = Ids_on_panel[i].Width;
            }
            // Actualizes the scroll min size if it is necessary.
            FilePanel.AutoScrollMinSize = new Size(maxSize, FilePanel.AutoScrollMinSize.Height);
            ShownPage = actPage;
        }

        /// <summary>
        /// Handles the click on the sentence link button.
        /// </summary>
        /// <param name="link">The button with link.</param>
        /// <param name="e">The event arguments.</param>
        private void Sentence_LinkClicked(LinkLabel link, LinkLabelLinkClickedEventArgs e)
        {
            // On the left button click changes the active sentence in the main form.
            if (e.Button == MouseButtons.Left)
            {
                // The text of the link is the id of the sentence.
                string sent_id = link.Text;
                MainForm.ChangeActiveSentence(Sentences[sent_id]);
            }
            // On the right button click opens the context menu.
            else
            {
                // Initializes the context menu and opens it.
                var delete = new ToolStripMenuItem("Delete sentence");
                delete.Click += (s, eh) => { RemoveSentence(link); };                
                var join = new ToolStripMenuItem("Join sentence with the next");
                join.Click += (s, eh) => { JoinSentences(link); };
                var insert = new ToolStripMenuItem("Insert new sentence after this");
                insert.Click += (s, eh) => { InsertEmptySentence(link); };
                ContextMenuStrip menu = new ContextMenuStrip();
                menu.Items.AddRange(new ToolStripItem[]{ delete, join, insert});
                menu.Show(link, 0, 0);
            }
        }

        /// <summary>
        /// Creates the new sentence and inserts it next to the sentence (with the link <paramref name="sourceControl"/>) where the action started.
        /// </summary>
        /// <param name="sourceControl">The link of the sentence that lies before the new sentence.</param>
        private void InsertEmptySentence(Control sourceControl)
        {
            // The preceding sentence is the sentence with the id from the link.
            ISentence preceding = Sentences[sourceControl.Text];
            // Creates the new sentence.
            ISentence new_sent = MakeNewSentence();
            // If the new sentence was not created, does not add anything.
            if (new_sent == null) return;
            // Inserts the sentence into the list of sentences and makes it active in the main form.
            InsertNew(new_sent, preceding.PlaceInFile + 1);
            MainForm.ChangeActiveSentence(new_sent);
        }

        /// <summary>
        /// Completely deletes the sentence.
        /// </summary>
        /// <param name="linkOfSentence">The link of the sentence that will be deleted.</param>
        private void RemoveSentence(Control linkOfSentence)
        {
            // Removes the link from the set of links.
            Ids_on_panel.Remove(linkOfSentence);
            FilePanel.Controls.Remove(linkOfSentence);
            // Shifts all positions in file of following sentences.
            int place = Sentences[linkOfSentence.Text].PlaceInFile;
            for (int i = place; i < Ids_on_panel.Count; i++)
            {
                Sentences[Ids_on_panel[i].Text].PlaceInFile--;
            }
            // If the removed sentence is active, finds the new active.
            // The new active sentence will be the preceding of the removed or the following in case that the removed sentence was first.
            if (Sentences[linkOfSentence.Text] == ActiveSentence)
            {
                // Gets the sentence before the removed or the new first in case that the removed sentence was first.
                int new_active = Math.Max(place - 1, 0);
                if (Ids_on_panel.Count > new_active)
                {
                    MainForm.ChangeActiveSentence(Sentences[Ids_on_panel[new_active].Text]);
                }
                // If there does not rest any other sentence, tha active sentence changes to null.
                else
                {
                    MainForm.ChangeActiveSentence(null);
                }
            }
            // If there does not rest anything on the last page, removes the last page.
            if (Ids_on_panel.Count % OnOnePage == 0)
            {
                PagePanel.Controls.RemoveAt(0);
            }
            // Removes the sentence from the list of all sentences.
            Sentences.Remove(linkOfSentence.Text);
        }

        /// <summary>
        /// Joins the sentence which has the link <paramref name="sourceControl"/> with the following sentence, if there is any.
        /// Hangs the second sentence on the root of the first.
        /// </summary>
        /// <param name="sourceControl">The link of the sentence that will be joined with the following.</param>
        private void JoinSentences(Control sourceControl)
        {
            ISentence clicked = Sentences[sourceControl.Text];
            int index = clicked.PlaceInFile;
            // If the sentence is not the last in the file.
            if (Ids_on_panel.Count > index + 1)
            {
                // Gets the following sentence.
                ISentence next = Sentences[Ids_on_panel[index + 1].Text];
                // The number how all ids in the second sentence shift.
                int shiftId = clicked.CountWords - 1;
                // Shifts all ids in the second sentence and changes the root of the second sentence to the root of the first sentence.
                for (int i = 1; i < next.CountWords; i++)
                {
                    // Gets the word on the right position
                    IWord w = next.GetWord(i);
                    // If the word is the direct descendant of the root, changes its parent to the root of the first sentence.
                    if (w is ITreeWord && (!(w as ITreeWord).IsRoot && (w as ITreeWord).Parent.Parent == null))
                    {
                        // Adds the new child to the root and removes it from the previous parent
                        clicked.Root.AddChild(w as ITreeWord, true);
                    }
                    // Shifts the id and add the word to the first sentence.
                    w.ShiftId(shiftId);
                    clicked.AddWord(w, clicked.CountWords);
                }
                // Removes the second sentence and makes the first sentence to be active.
                RemoveSentence(Ids_on_panel[index + 1]);
                MainForm.ChangeActiveSentence(clicked);
            }
        }

        /// <summary>
        /// Splits the <paramref name="sentence"/> to two new sentences. The second sentence starts by <paramref name="word"/>.
        /// </summary>
        /// <param name="sentence">The sentence that is going to be splitted.</param>
        /// <param name="word">The word that will start the second sentence.</param>
        public void Split(ISentence sentence, IWord word)
        {
            // Creates the id of the second sentence by adding the sign '_' and the number 2.
            string new_id = sentence.Sent_id + "_2";
            int copyNum = 2;
            // If the new id is not unique in the file, continues by increasing the added number until the id is unique.
            while (GetSentence(new_id) != null)
            {
                copyNum++;
                new_id = sentence.Sent_id + "_" + copyNum;
            }
            if(word is ITreeWord && (word as ITreeWord).IsJoined)
            {
                word = (word as ITreeWord).JoinedWord;
            }
            // Splits the sentence and gets the second part.
            ISentence new_sent = sentence.Split(word, new_id, this);
            // If there was not possible to split the sentence on the current word, nothing happens.
            if (new_sent == null)
                return;
            // Inserts the new sentence into the list and changes the active sentence to the second part of the splitted sentence.
            InsertNew(new_sent, sentence.PlaceInFile + 1);
            MainForm.ChangeActiveSentence(new_sent);
        }

        /// <summary>
        /// Returns the sentence by position in the file.
        /// </summary>
        /// <param name="index">The position of returned sentence in the file.</param>
        /// <returns>The sentence on the <paramref name="index"/>, if it exists. Otherwise, null.</returns>
        public ISentence GetSentence(int index)
        {
            if (Ids_on_panel.Count > index && index >= 0)
            {
                return Sentences[Ids_on_panel[index].Text];
            }
            else return null;
        }

        /// <summary>
        /// Returns the sentence by the sent_id.
        /// </summary>
        /// <param name="id">The sent_id attribute of the returned sentence.</param>
        /// <returns>The sentence with the right id or null, if there is no such a sentence.</returns>
        public ISentence GetSentence(string id)
        {
            if (Sentences.ContainsKey(id))
            {
                return Sentences[id];
            }
            else return null;
        }

        /// <summary>
        /// Changes the active sentence link.
        /// </summary>
        /// <param name="sentence">The new active sentence</param>
        public void ChangeActive(ISentence sentence)
        {
            // If there is the sentence which was active till now, marks it at visited.
            if (ActiveSentence != null){
                // If the previous active sentence still exists
                if (Ids_on_panel.Count > ActiveSentence.PlaceInFile)
                {
                    var old_link = Ids_on_panel[ActiveSentence.PlaceInFile];
                    ((LinkLabel)old_link).LinkColor = VisitedColor;
                }
            }
            //If the new active sentence is not null, marks it as active.
            if(sentence != null)
            {                
                var link = Ids_on_panel[sentence.PlaceInFile];
                ((LinkLabel)link).LinkColor = ActiveColor;
                // Counts the page where the sentene lies.
                int page = sentence.PlaceInFile / OnOnePage;
                // Simulates the click on the right page.
                Page_LinkClicked((LinkLabel)PagePanel.Controls[PagePanel.Controls.Count - page - 1]);
            }
            ActiveSentence = sentence;

        }

        /// <summary>
        /// Saves all sentences into the file with the name from the parameter <paramref name="nameOfSaved"/>.
        /// </summary>
        /// <param name="nameOfSaved">The name of the file where the sentences should be written to.</param>
        public void SaveAll(string nameOfSaved)
        {
            using (StreamWriter newFile = new StreamWriter(nameOfSaved))
            {
                // Writes the sentences in the original order.
                foreach (var control in Ids_on_panel)
                {
                    // Writes one sentence to the file.
                    ISentence sentence = Sentences[control.Text];
                    sentence.SaveTo(newFile);
                    // Adds the new line under the sentence.
                    newFile.WriteLine();
                }
            }
        }

        /// <summary>
        /// Sets the new id to existing sentence.
        /// </summary>
        /// <param name="sentence">The sentence whose id has changed.</param>
        /// <param name="new_id">The new id of the changed sentence.</param>
        public void SetId(ISentence sentence, string new_id)
        {
            // Changes the dictionary record.
            Sentences.Remove(sentence.Sent_id);
            Sentences.Add(new_id, sentence);
            // Changes the text of the link button.
            Ids_on_panel[sentence.PlaceInFile] = MakeSentenceLinkButton(new_id);
            FilePanel.Controls[FilePanel.Controls.Count - sentence.PlaceInFile].Text = new_id;
        }

        /// <summary>
        /// Creates the new sentence. Opens the box where the user can insert the parameters.
        /// </summary>
        /// <returns>The new sentence or null, if the operation failed.</returns>
        public ISentence MakeNewSentence()
        {
            // Opens the form where user puts the parameters to.
            InsertNewSentenceBox inputBox = new InsertNewSentenceBox(Sentences);
            if (inputBox.ShowDialog() == DialogResult.OK)
            {
                // Gets the id of the new sentence.
                string sent_id = inputBox.GetIdBox();
                // Gets the textbox with the text of the new sentence and creates the new sentence.
                using (IReader r = new Reader(inputBox.GetSentence()))
                {
                    string[] words = null;
                    string line = r.ReadLine();
                    if(line != null)
                    {
                        words = line.Split();
                    }
                    ISentence sentence = new Sentence(words, sent_id, this);
                    return sentence;
                }
            }
            // The operation failed.
            return null;
        }

        /// <summary>
        /// Adds all sentences that are made by a specific factory.
        /// </summary>
        /// <param name="factory">The concrete factory with loaded file with sentences.</param>
        public void AddSentences(ISentenceFactory factory)
        {
            int index = 0;
            ISentence sentence;
            // While there is another sentence, adds it to the list on the right index.
            while((sentence = factory.GetSentence(this)) != null)
            {
                AddNew(sentence, index);
                index++;
            } 
            // Shows all page buttons.
            ShowAll();
        }
    }
}