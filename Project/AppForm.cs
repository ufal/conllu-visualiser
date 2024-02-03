using System;
using System.Drawing;
using System.Windows.Forms;
using Finder;
using GraphVisualiser;
using GraphVisualiser.EnhancedVisualiser;
using GraphVisualiser.BasicVisualiser;

namespace ConlluVisualiser
{
    /// <summary>
    /// Main form where the application is running.
    /// </summary>
    public partial class AppForm : Form
    {
        /// <summary>
        /// A set of sentences in the opened file.
        /// </summary>
        private ListOfSentences Sentences { get; set; }

        /// <summary>
        /// A structure that visualises actual sentence graph to user.
        /// </summary>
        public IVisualiser SentenceVisualiser { get; set; }

        /// <summary>
        /// The current state with the Active Word. 
        /// </summary>
        CurrentState State { get; } 
        /// <summary>
        /// A word, whose point is currently moving by mouse.
        /// </summary>
        private ITreeWord MovedPoint { get; set; }
        /// <summary>
        /// A set of keyboard shortcuts with actions that are applied to the active word (empty by default).
        /// </summary>
        private ShortcutKeys ShorcutKeys { get; } = new ShortcutKeys();

        /// <summary>
        /// A name of a file where the project is going to be saved (it is empty at the beginning).
        /// </summary>
        private string NameOfSaved { get; set; }
        public AppForm()
        {
            InitializeComponent();
            // At the beginning, the basic designer is chosen to draw the sentence structure.
            SentenceVisualiser = new Visualiser(TreePanel, new BasicDesigner());
            State = new CurrentState(this);
            Sentences = new ListOfSentences(filePanel, pagePanel, State);
        }

        /// <summary>
        /// A method to visualise the new graph of the active sentence, used if the graph is changed.
        /// </summary>
        /// <remarks>
        /// Change can be in the sentence (different sentence, change in structure,..) or in the size of the form.
        /// If there is no active sentence(it actually means that there is no sentence), the graph space is empty.
        /// </remarks>
        public void VisualiseNewGraph()
        {
            // The panel with basic information about hovered word is hidden.
            basicWordInfo.Visible = false;
            textBox1.Clear();
            if (Sentences.ActiveSentence != null)
            {
                // The text of the sentence is actualized and all possible changes are visible.
                textBox1.Text = Sentences.ActiveSentence.MakeText();
                // The new graphics is created, coordinates of points are newly counted.
                SentenceVisualiser.NewGraphics(Sentences.ActiveSentence);
            }
            else
            {
                // If there is no sentence, empty white space is visualised.
                Graphics g = TreePanel.CreateGraphics();
                g.Clear(Color.White);
            }
        }

         /// <summary>
         /// Creates a new list of sentences after loading a new file.
         /// If some file is loaded, user is alerted to save the actual file.
         /// </summary>
        private void MakeNewListOfSentences()
        {
            if (Sentences.ActiveSentence != null && MessageBox.Show("Do you want to save actual file?", "save file", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                SaveToolStripMenuItem_Click();
            }
            Sentences = new ListOfSentences(filePanel, pagePanel, new CurrentState(this));
            // No sentence is active before loading the new file
            ChangeActiveSentence(null);
        }

        /// <summary>
        /// Loads all file and saves all sentences to a new list of sentences,
        /// the file must be in the right ConLL-U format.
        /// </summary>
        /// <remarks>
        /// If the file is in a bad format, the user get a notice and no sentences are loaded.
        /// </remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConLLUSentenceToolStripMenuItem_Click(object sender, EventArgs e) => LoadFile(new ConlluFileLoader(TreePanel));

        /// <summary>
        /// Loads all simple text file and saves all sentences to a new list of sentences.
        /// It splits the file line by line, makes the sentence from each line.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadSimpleSentencesToolStripMenuItem_Click(object sender, EventArgs e) => LoadFile(new SimpleFileLoader(TreePanel));

        /// <summary>
        /// Asks the user if he is sure to leave the application. If yes, exits the application without saving.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Exit application?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        /// <summary>
        /// Creates the new list of sentences. Tries to load a file in a format that user has chosen.
        /// </summary>
        /// <remarks>
        /// If the operation failed (bad format, user did not chose anything,... ), the list of sentences is emptied. 
        /// </remarks>
        /// <param name="loader">Loader that loads a file in the concrete format</param>
        private void LoadFile(IFileLoader loader)
        {
            MakeNewListOfSentences();
            // Returns false if no file is properly loaded
            if (loader.LoadFile(Sentences))
                // Active sentence is changed to the first sentence from the file or null, if there is no sentence.
                ChangeActiveSentence(Sentences.GetSentence(0));
            else MakeNewListOfSentences();
        }

         /// <summary>
         /// If the changes has been saved before, it uses the same file, otherwise it asks for a name of the file.
         /// Then it writes all sentences to the file in the right CoNLL-U format
         /// </summary>
         /// <param name="sender"></param>
         /// <param name="e"></param>
        private void SaveToolStripMenuItem_Click(object sender = null, EventArgs e = null)
        {
            // NameOfSaved is the name of the file where the current file has been saved
            if (NameOfSaved == null)
                // It has not been saved, we need to find out the right file
                SaveAsToolStripMenuItem_Click(sender, e);
            else
                Sentences.SaveAll(NameOfSaved);
        }

        /// <summary>
        /// Asks the user for the name of the file, where the list of sentences should be saved.
        /// If possible, writes the sentences in the right CoNLL-U format and remembers the file for the next saving.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Opens the save file dialog.
            SaveFileDialog dialog = new SaveFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                // Writes all sentences into the file.
                Sentences.SaveAll(dialog.FileName);
                NameOfSaved = dialog.FileName;
            }
        }

        /// <summary>
        /// Creates the new empty list of sentences. If some file is loaded, asks user to save it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MakeNewListOfSentences();
        }

        /// <summary>
        /// Creates NodeFinder to search the right node in all sentences across the file.
        /// The node is searched based on fields of CoNLL-U format.
        /// </summary>
        /// <remarks>
        /// The Finder has access to change the active sentence or word and the changes are visualised in this form.
        /// </remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FindNodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Search(new NodeFinder(new CurrentState(this), Sentences));
        }

        /// <summary>
        /// Creates SentenceFinder to search the right sentence in all sentences across the file.
        /// The sentence is searched based on id or regular expression.
        /// </summary>
        /// <remarks>
        /// The Finder has access to change the active sentence or word and the changes are visualised in this form.
        /// </remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FindSentenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Search(new SentenceFinder(new CurrentState(this), Sentences));
        }

        /// <summary>
        /// Uses any finder to find all right parts of the file.
        /// </summary>
        /// <remarks>
        /// If there are no sentences, the searching does not start and the user is asked to load or insert some sentences.
        /// </remarks>
        /// <param name="finder"></param>
        private void Search(IFinder finder)
        {
            if (Sentences.ActiveSentence == null)
            {
                MessageBox.Show("You must add some sentence.");
                return;
            }
            // Starts searching according to the used finder.
            finder.Find(Sentences.ActiveSentence);
        }

        /// <summary>
        /// Allows user to define and add the new keyboard shortcut.
        /// </summary>
        /// <remarks>
        /// If the user choses the existing shortcut key, the original action is overriden by the new.
        /// </remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DefineShortupKeysToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShorcutKeys.AddShortcutKey();
        }

        /// <summary>
        /// Writes all defined shortcut keys into the file.
        /// </summary>
        /// <remarks>
        /// The details of the format can be found in the documentation.
        /// </remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportShortupKeysToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShorcutKeys.Save();
        }


        /// <summary>
        /// Loads the file and saves the shortcut keys. The file must have the right format.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImportShortupKeysToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShorcutKeys.Load();
        }

        /// <summary>
        /// Changes the representation of the sentence to the basic representation.
        /// </summary>
        /// <remarks>
        /// Changes the color of the menuitems, so the 'Basic Representation' menu item is colored as active.
        /// </remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BasicRepresentationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Makes the new visualiser with the basic designer
            SentenceVisualiser = new Visualiser(TreePanel, new BasicDesigner());
            if (Sentences.ActiveSentence != null)
                VisualiseNewGraph();
            // Changes the colors of menu items
            basicRepresentationToolStripMenuItem.BackColor = SystemColors.ActiveCaption;
            enhancedRepresentationToolStripMenuItem.BackColor = SystemColors.Control;
        }

        /// <summary>
        /// Changes the representation of the sentence to the representation of enhanced graph.
        /// </summary>
        /// <remarks>
        /// Changes the color of the menuitems, so the 'Enhanced Representation' menu item is colored as active.
        /// </remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnhancedRepresentationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Makes the new visualiser with the enhanced designer
            SentenceVisualiser = new Visualiser(TreePanel, new EnhancedDesigner());
            if (Sentences.ActiveSentence != null)
                VisualiseNewGraph();
            // Changes the colors of menu items
            basicRepresentationToolStripMenuItem.BackColor = SystemColors.Control;
            enhancedRepresentationToolStripMenuItem.BackColor = SystemColors.ActiveCaption;
        }

        /// <summary>
        /// Creates the new sentence and inserts it to the beginning of the file.
        /// </summary>
        /// <remarks>
        /// To make a sentence, the user must fill the sentence id (unique) and can write the text of the sentence.
        /// </remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InsertNewSentenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Creates the new sentence. If the operation is canceled, returns null.
            ISentence s = Sentences.MakeNewSentence();
            if (s != null)
            {
                // Inserts the sentence as first to the file
                Sentences.InsertNew(s, 0);
                // Makes the new sentence active
                ChangeActiveSentence(s);
            }
        }

        /// <summary>
        /// After resizing of the main form, it recounts the coordinates of the active sentence and shows the modified graph.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Resize(object sender, EventArgs e)
        {
            if (Sentences.ActiveSentence != null)
            {
                SentenceVisualiser.NewGraphics(Sentences.ActiveSentence);
            }
        }

        /// <summary>
        /// An event which starts operation of moving the point of a word to its new parent.
        /// If the clicked point is the point of some word, this word is saved as MovedPoint.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreePanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (Sentences.ActiveSentence != null)
            {
                // Returns the word, whose point is clicked. If no word is clicked, returns null.
                var point = SentenceVisualiser.FindRightPoint(Sentences.ActiveSentence, e.Location);
                if(point is ITreeWord)
                {
                    MovedPoint = point as ITreeWord;
                }
            }
        }

        /// <summary>
        /// An event which occurs after MouseDown event, where the MovedPoint is chosen.
        /// If possible, the word, on whose point the MouseUp event occured, is the new parent of the moved word.
        /// </summary>
        /// <remarks>
        /// Any of words can not be null and the parent word must be able to be the parent of the new word.
        /// (The parent is th descendant of the child, the parent is a special word, which can not have any children...)
        /// </remarks>
        /// <seealso cref="TreePanel_MouseDown(object, MouseEventArgs)"/>
        /// <see cref="MovedPoint"/>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreePanel_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                return;
            if (Sentences.ActiveSentence != null)
            {
                // If the mouse is not over any word point, it returns null.
                IWord parent = SentenceVisualiser.FindRightPoint(Sentences.ActiveSentence, e.Location);
                if (MovedPoint != null && parent != null)
                {
                    // Returns false if the word is special and can not have children 
                    // or if the parent is descendant of the child
                    if (!(parent is ITreeWord) || !(parent as ITreeWord).CanBeParentOf(MovedPoint))
                        return;
                    // Adds the new child to parent and removes the the child from the original parent
                    (parent as ITreeWord).AddChild(MovedPoint, true);
                    // Shows the modified graph
                    VisualiseNewGraph();
                }
            }
        }
        
         /// <summary>
         /// An event which occurs after double click to the sentence graph space.
         /// If the clicked point is the point of some word, the info about this word is shown.
         /// </summary>
         /// <param name="sender"></param>
         /// <param name="e"></param>
        private void TreePanel_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (Sentences.ActiveSentence != null)
            {
                // Returns the word whose point is on the clicked place. If there is no word, returns null.
                IWord clickedWord = SentenceVisualiser.FindRightPoint(Sentences.ActiveSentence, e.Location);
                if (clickedWord != null)
                {
                    // Shows all information about the word, which are the parts of CoNLL-U format.
                    ShowInfoAboutWord(clickedWord);
                }
            }
        }

        /// <summary>
        /// An event which occurs after click to the sentence graph space.
        /// If some word point is clicked, it changes the active word to clicked word.
        /// If the user pushes the right mouse button, the proper context menu strip is shown.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreePanel_MouseClick(object sender, MouseEventArgs e)
        {
            if (Sentences.ActiveSentence == null)
                return;
            // Returns the word whose point is on the clicked place. If there is no word, returns null.
            IWord clickedWord = SentenceVisualiser.FindRightPoint(Sentences.ActiveSentence, e.Location);
            if (clickedWord != null)
            {
                // Changes the active word and draw the change into the visualiser
                State.ChangeActiveWord(clickedWord);
                if (e.Button == MouseButtons.Right)
                {
                    // Returns the proper context menu according to the current designer and the clicked word
                    ContextMenuStrip menu = SentenceVisualiser.GetContextMenu(clickedWord, Sentences, new CurrentState(this));
                    if (menu != null)
                        menu.Show(TreePanel, e.Location);
                }
            }
        }

        /// <summary>
        /// Occurs after all mouse movements in the sentence graph space.
        /// If some word point is hovered, the basic information(Lemma, Cpostag, Deprel) are shown (if not empty).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreePanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (Sentences.ActiveSentence != null)
            {
                // Finds the hovered word or returns null
                IWord chosenWord = SentenceVisualiser.FindRightPoint(Sentences.ActiveSentence, e.Location);
                // If some word is hovered and the panel with info is invisible
                if (chosenWord != null && !basicWordInfo.Visible)
                {
                    // Shows the basic information of the word
                    SentenceVisualiser.ShowBasicInfo(basicWordInfo, chosenWord, Sentences.ActiveSentence);
                }
                // If no word is hovered and the info panel is visible
                else if (chosenWord == null && basicWordInfo.Visible)
                {
                    // Hides the info panel
                    basicWordInfo.Visible = false;
                    // It is necessary to revisualise the graph, because the place under the info panel would rest empty.
                    // The coordinates rest the same, we only draw the same graph over the original.
                    SentenceVisualiser.Visualise(Sentences.ActiveSentence);
                }
            }
        }

        /// <summary>
        /// Shows all the information about word which are the parts of CoNLL-U format.
        /// </summary>
        /// <param name="word">The word whose information are going to be shown.</param>
        private void ShowInfoAboutWord(IWord word)
        {
            basicWordInfo.Visible = false;
            word.ShowInfo(Sentences.ActiveSentence);
            VisualiseNewGraph();
        }        

        
        /// <summary>
        /// Makes the new sentence active and visualises its graph. 
        /// Changes the active word to the root of the new sentence.
        /// </summary>
        /// <seealso cref="ChangeActiveWord(IWord, bool)"/>
        /// <param name="new_sentence">The new active sentence</param>
        public void ChangeActiveSentence(ISentence new_sentence)
        {
            // Changes active sentence in a list of sentences
            Sentences.ChangeActive(new_sentence);
            // Returns the scroll position to the start position
            TreePanel.AutoScrollPosition = new Point(0, 0);

            if (new_sentence != null)
            {
                // The new active word is the root of the new sentence, if it is not a null.
                State.ChangeActiveWord(new_sentence.Root, false);
            }
            // New graph is counted and visualised
            VisualiseNewGraph();

        }       
        
        /// <summary>
        /// An event thar occurs after painting on TreePanel. 
        /// The method ensures that the correct graph is drawn according to scroll position.
        /// </summary>
        private void TreePanel_Paint(object sender, PaintEventArgs e)
        {
            // If the scroll position has been changed from the last visualisation
            if (SentenceVisualiser.ScrollPosition != TreePanel.AutoScrollPosition && Sentences.ActiveSentence != null)
            {
                // Shifts the coordinates according to the new scroll position
                SentenceVisualiser.ScrollNewGraphics(Sentences.ActiveSentence);
            }
            else if (Sentences.ActiveSentence != null)
            {
                // Visualises the original graph of the sentence
                SentenceVisualiser.Visualise(Sentences.ActiveSentence);
            }
        }        

        /// <summary>
        /// Process a command key. If the key is found in shortcut keys or if it does some action, process the special action.
        /// If not, process the base.ProcessCmdKey method
        /// </summary>
        /// <param name="msg">A message, passed by reference, that represents the Win32 message to process</param>
        /// <param name="keyData">One of the key values, that represents the key to process</param>
        /// <returns>base.ProcessCmdKey(<paramref name="msg"/>, <paramref name="keyData"/>) return value.</returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // If there is no active word or sentence, the default action is done.
            if (Sentences == null || Sentences.ActiveSentence == null || State.ActiveWord == null)
                return base.ProcessCmdKey(ref msg, keyData);
            else if (keyData == Keys.Enter)
            {
                // Shows the complete information about the active word
                ShowInfoAboutWord(State.ActiveWord);
            }
            else if (keyData == Keys.Delete)
            {
                // Deletes the active word from the active sentence
                State.ActiveWord.Delete(Sentences.ActiveSentence);
                // Visualises the modified graph
                VisualiseNewGraph();
            }
            // If the key si an arrow, the visualiser tries to shift the active word, if it is possible
            else if(keyData == Keys.Left || keyData == Keys.Right || keyData == Keys.Up || keyData == Keys.Down)
            {
                // If it is not possible to shift the word, visualiser returns the original word
                IWord new_active = SentenceVisualiser.ShiftActive(keyData, Sentences.ActiveSentence, State.ActiveWord);
                if (State.ActiveWord != new_active)
                {
                    // If the word is different from the original, The change is visualised in the graph.
                    State.ChangeActiveWord(new_active);
                }
            }
            // If there is the same shortcut key in user defined shortcut keys
            else if (ShorcutKeys.Contains(keyData))
            {
                // Processes the user defined action and makes changes on the active word
                ShorcutKeys.ProcessKey(keyData, State.ActiveWord);
                // It is necessary to visualise the new graph in some cases - the word could have been changed visibly.
                VisualiseNewGraph();
            }
            // If no action was processed, the default action is chosen.
            else return base.ProcessCmdKey(ref msg, keyData);
            return true;
        }    
    }
}