using System.Windows.Forms;
using ConlluVisualiser;

namespace GraphVisualiser
{
    namespace BasicVisualiser
    {
        /// <summary>
        /// Returns the context menu with defined actions in the basic representation according to the type of the word.
        /// </summary>
        class GetMenuBasic : IGetMenuVisitor
        {
            /// <summary>
            /// Returns the context menu with defined actions on the word in the basic representation.
            /// </summary>
            /// <param name="word">The word whose menu is shown.</param>
            /// <param name="sentences">The list of sentences where the defined actions can be processed.</param>
            /// <param name="form">The mean to command the main form to change the active word or sentence.</param>
            /// <returns>The created context menu.</returns>
            public virtual ContextMenuStrip GetMenu(BasicWord word, ListOfSentences sentences, CurrentState form)
            {
                // If the word is the root, opens the root menu.
                if (word.IsRoot)
                {
                    return RootMenu(word, sentences, form);
                }
                // Else creates the basic word menu with following actions:
                ContextMenuStrip menu = new ContextMenuStrip();
                // Shows the word attribute info.
                menu.Items.Add(ShowInfoAboutWord(word, sentences.ActiveSentence, form));
                // Adds the child to the word.
                menu.Items.Add(AddChild(word, sentences.ActiveSentence, form));
                // Splits the node.
                menu.Items.Add(SplitNode(word, sentences.ActiveSentence, form));
                // Deletes the node.
                menu.Items.Add(DeleteNode(word, sentences.ActiveSentence, form));
                // If the word is not a part of the multiword token, adds some other actions:
                if (!word.IsJoined)
                {
                    // Swaps the word with the left sibling.
                    menu.Items.Add(ShiftLeft(word, sentences.ActiveSentence, form));
                    // Swaps the word with the right sibling.
                    menu.Items.Add(ShiftRight(word, sentences.ActiveSentence, form));
                }
                // If the word is not a part of the multiword token or if the word is the first word in that token:
                if (!word.IsJoined || (word.IsJoined && word.JoinedWord.From == word.Id))
                {
                    // Allows to split the sentence here.
                    menu.Items.Add(SplitSentence(word, sentences, form));
                }
                return menu;
            }

            /// <summary>
            /// Returns the context menu of the root.
            /// </summary>
            /// <param name="word">The root word.</param>
            /// <param name="sentences">The list of sentences.</param>
            /// <param name="form">The mean to command the main form to change the active word or sentence.</param>
            /// <returns>The created context menu for the root.</returns>
            protected ContextMenuStrip RootMenu(BasicWord word, ListOfSentences sentences, CurrentState form)
            {
                ContextMenuStrip menu = new ContextMenuStrip();
                // Shows the info about the word.
                menu.Items.Add(ShowInfoAboutWord(word, sentences.ActiveSentence, form));
                // Adds a new child to the word.
                menu.Items.Add(AddChild(word, sentences.ActiveSentence, form));
                return menu;
            }

            /// <summary>
            /// Returns null because the multiword is not visualised in the basic representation.
            /// </summary>
            /// <param name="multiword">The word whose menu tries to be shown.</param>
            /// <param name="sentences">The list of sentences where the defined actions can be processed.</param>
            /// <param name="form">The mean to command the main form to change the active word or sentence.</param>
            /// <returns>Allways returns null.</returns>
            public virtual ContextMenuStrip GetMenu(MultiWord multiword, ListOfSentences sentences, CurrentState form) { return null; }

            /// <summary>
            /// Returns null because the empty node word is not visualised in the basic representation.
            /// </summary>
            /// <param name="emtyword">The word whose menu tries to be shown.</param>
            /// <param name="sentences">The list of sentences where the defined actions can be processed.</param>
            /// <param name="form">The mean to command the main form to change the active word or sentence.</param>
            /// <returns>Allways returns null.</returns>
            public virtual ContextMenuStrip GetMenu(EmptyNodeWord emptyword, ListOfSentences sentences, CurrentState form) { return null; }

            /// <summary>
            /// The menu item that provides the action of opening the word info.
            /// </summary>
            /// <param name="word">The word whose context menu strip is shown.</param>
            /// <param name="ActiveSentence">The sentence in which the word lies.</param>
            /// <param name="form">The mean to command the main form to change the active word or sentence.</param>
            /// <returns>The menu item with the right text and action.</returns>
            protected ToolStripMenuItem ShowInfoAboutWord(IWord word, ISentence ActiveSentence, CurrentState form)
            {
                ToolStripMenuItem WordInfoItem = new ToolStripMenuItem("Show info about word");
                WordInfoItem.Click += (sender, e) =>
                {
                    word.ShowInfo(ActiveSentence);
                    form.VisualiseNewGraph();
                };
                return WordInfoItem;
            }

            /// <summary>
            /// The menu item that provides the action of deleting the word.
            /// </summary>
            /// <param name="word">The word whose context menu strip is shown.</param>
            /// <param name="ActiveSentence">The sentence in which the word lies.</param>
            /// <param name="form">The mean to command the main form to change the active word or sentence.</param>
            /// <returns>The menu item with the right text and action.</returns>
            protected ToolStripMenuItem DeleteNode(IWord word, ISentence ActiveSentence, CurrentState form)
            {
                ToolStripMenuItem DeleteItem = new ToolStripMenuItem("Delete node");
                DeleteItem.Click += (sender, e) =>
                {
                    word.Delete(ActiveSentence);
                // Changes the active word to null - no word will be active.
                form.ChangeActiveWord(null);
                    form.VisualiseNewGraph();
                };
                return DeleteItem;
            }

            /// <summary>
            /// The menu item that provides the action of swapping the <paramref name="word"/> with the left sibling.
            /// </summary>
            /// <param name="word">The word whose context menu strip is shown.</param>
            /// <param name="ActiveSentence">The sentence in which the word lies.</param>
            /// <param name="form">The mean to command the main form to change the active word or sentence.</param>
            /// <returns>The menu item with the right text and action.</returns>
            protected ToolStripMenuItem ShiftLeft(BasicWord word, ISentence ActiveSentence, CurrentState form)
            {
                ToolStripMenuItem ShiftLeftItem = new ToolStripMenuItem("Shift word to left");
                ShiftLeftItem.Click += (sender, e) =>
                {
                // Swaps the word with the sibling whose index is lower by 1.
                word.Swap(-1, ActiveSentence);
                    form.VisualiseNewGraph();
                };
                return ShiftLeftItem;
            }

            /// <summary>
            /// The menu item that provides the action of swapping the <paramref name="word"/> with the right sibling.
            /// </summary>
            /// <param name="word">The word whose context menu strip is shown.</param>
            /// <param name="ActiveSentence">The sentence in which the word lies.</param>
            /// <param name="form">The mean to command the main form to change the active word or sentence.</param>
            /// <returns>The menu item with the right text and action.</returns>
            protected ToolStripMenuItem ShiftRight(BasicWord word, ISentence ActiveSentence, CurrentState form)
            {
                ToolStripMenuItem ShiftLeftItem = new ToolStripMenuItem("Shift word to right");
                ShiftLeftItem.Click += (sender, e) =>
                {
                // Swaps the word with the sibling whose index is greater by 1.
                word.Swap(1, ActiveSentence);
                    form.VisualiseNewGraph();
                };
                return ShiftLeftItem;
            }

            /// <summary>
            /// The menu item that provides the action of splitting the word on two.
            /// </summary>
            /// <param name="word">The word whose context menu strip is shown.</param>
            /// <param name="ActiveSentence">The sentence in which the word lies.</param>
            /// <param name="form">The mean to command the main form to change the active word or sentence.</param>
            /// <returns>The menu item with the right text and action.</returns>
            protected ToolStripMenuItem SplitNode(BasicWord word, ISentence ActiveSentence, CurrentState form)
            {
                ToolStripMenuItem SplitNodeItem = new ToolStripMenuItem("Split node");
                SplitNodeItem.Click += (sender, e) =>
                {
                // The root can not be splitted.
                if (word.IsRoot) return;
                    ActiveSentence.InsertChild(word.Parent, word);
                    form.VisualiseNewGraph();
                };
                return SplitNodeItem;
            }

            /// <summary>
            /// The menu item that provides the action of splitting the sentence on two, so that the second sentence starts by this word.
            /// </summary>
            /// <param name="word">The word whose context menu strip is shown.</param>
            /// <param name="ActiveSentence">The sentence in which the word lies.</param>
            /// <param name="form">The mean to command the main form to change the active word or sentence.</param>
            /// <returns>The menu item with the right text and action.</returns>
            protected ToolStripMenuItem SplitSentence(IWord word, ListOfSentences sentences, CurrentState form)
            {
                ToolStripMenuItem SplitSentenceItem = new ToolStripMenuItem("Split sentence here");
                SplitSentenceItem.Click += (sender, e) =>
                {
                    sentences.Split(sentences.ActiveSentence, word);
                    form.VisualiseNewGraph();
                };
                return SplitSentenceItem;
            }

            /// <summary>
            /// The menu item that provides the action of adding the child to the <paramref name="word"/>.
            /// </summary>
            /// <param name="word">The word whose context menu strip is shown.</param>
            /// <param name="ActiveSentence">The sentence in which the word lies.</param>
            /// <param name="form">The mean to command the main form to change the active word or sentence.</param>
            /// <returns>The menu item with the right text and action.</returns>
            protected ToolStripMenuItem AddChild(BasicWord word, ISentence ActiveSentence, CurrentState form)
            {
                ToolStripMenuItem AddChildItem = new ToolStripMenuItem("Add child");
                AddChildItem.Click += (sender, e) =>
                {
                    ActiveSentence.InsertChild(word, word);
                    form.VisualiseNewGraph();
                };
                return AddChildItem;
            }
        }
    }
}