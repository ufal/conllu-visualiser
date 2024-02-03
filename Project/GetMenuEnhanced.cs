using System.Windows.Forms;
using ConlluVisualiser;
using GraphVisualiser.BasicVisualiser;

namespace GraphVisualiser
{
    namespace EnhancedVisualiser
    {
    /// <summary>
    /// Returns the context menu with defined actions in the enhanced representation according to the type of the word.
    /// </summary>
    class GetMenuEnhanced : GetMenuBasic, IGetMenuVisitor
        {
            /// <summary>
            /// Returns the context menu with defined actions on the word in the enhanced representation.
            /// </summary>
            /// <param name="word">The word whose menu is shown.</param>
            /// <param name="sentences">The list of sentences where the defined actions can be processed.</param>
            /// <param name="form">The mean to command the main form to change the active word or sentence.</param>
            /// <returns>The created context menu.</returns>
            public override ContextMenuStrip GetMenu(BasicWord word, ListOfSentences sentences, CurrentState form)
            {
                // The words that are the parts of the multiword nodes, have the different context menu.
                if (word.IsJoined)
                {
                    return JoinedMenu(word, sentences, form);
                }
                // The root has the special context menu.
                if (word.IsRoot)
                {
                    ContextMenuStrip rootMenu = RootMenu(word, sentences, form);
                    // Inserts the empty node after this root.
                    rootMenu.Items.Add(InsertEmptyAfter(word, sentences.ActiveSentence, form));
                    return rootMenu;
                }
                // Else creates the context menu with actions:
                ContextMenuStrip menu = new ContextMenuStrip();
                // Shows the word attribute info.
                menu.Items.Add(ShowInfoAboutWord(word, sentences.ActiveSentence, form));
                // Adds the child to the word.
                menu.Items.Add(AddChild(word, sentences.ActiveSentence, form));
                // Inserts the empty node after this word.
                menu.Items.Add(InsertEmptyAfter(word, sentences.ActiveSentence, form));
                // Makes the multiword token with the next word.
                menu.Items.Add(MakeMultiword(word, sentences.ActiveSentence, form));
                // Swaps the word with the left sibling.
                menu.Items.Add(ShiftLeft(word, sentences.ActiveSentence, form));
                // Swaps the word with the right sibling.
                menu.Items.Add(ShiftRight(word, sentences.ActiveSentence, form));
                // Deletes the node.
                menu.Items.Add(DeleteNode(word, sentences.ActiveSentence, form));

                return menu;
            }

            /// <summary>
            /// Returns the context menu with defined actions in the enhanced representation 
            /// on the word that is the part of the multiword token.
            /// </summary>
            /// <param name="joined">The word whose menu is shown.</param>
            /// <param name="sentences">The list of sentences where the defined actions can be processed.</param>
            /// <param name="form">The mean to command the main form to change the active word or sentence.</param>
            /// <returns>The created context menu.</returns>
            private ContextMenuStrip JoinedMenu(BasicWord joined, ListOfSentences sentences, CurrentState form)
            {
                ContextMenuStrip menu = new ContextMenuStrip();
                // Gets the multiword token where the joined word lies in.
                MultiWord MW = joined.JoinedWord;
                // Shows the word attribute info.
                menu.Items.Add(ShowInfoAboutWord(joined, sentences.ActiveSentence, form));
                // Adds the child to the word.
                menu.Items.Add(AddChild(joined, sentences.ActiveSentence, form));
                // Deletes the node.
                menu.Items.Add(DeleteNode(joined, sentences.ActiveSentence, form));
                // Shows the multiword attribute info.
                menu.Items.Add(ShowInfoMultiword(MW, sentences.ActiveSentence, form));
                // Adds the next node to the multiword token.
                menu.Items.Add(AddToMultiword(MW, sentences.ActiveSentence, form));
                // Removes the multiword token.
                menu.Items.Add(RemoveMultiword(MW, sentences.ActiveSentence, form));
                // Inserts the empty node item behind the joined word.
                menu.Items.Add(InsertEmptyAfter(joined, sentences.ActiveSentence, form));

                return menu;
            }

            /// <summary>
            /// Returns the context menu with defined actions on the empty node word in the enhanced representation.
            /// </summary>
            /// <param name="emptyWord">The word whose menu is shown.</param>
            /// <param name="sentences">The list of sentences where the defined actions can be processed.</param>
            /// <param name="form">The mean to command the main form to change the active word or sentence.</param>
            /// <returns>The created context menu.</returns>
            public override ContextMenuStrip GetMenu(EmptyNodeWord emptyWord, ListOfSentences sentences, CurrentState form)
            {
                ContextMenuStrip menu = new ContextMenuStrip();

                // Shows the word attribute info.
                menu.Items.Add(ShowInfoAboutWord(emptyWord, sentences.ActiveSentence, form));
                // Deletes the node.
                menu.Items.Add(DeleteNode(emptyWord, sentences.ActiveSentence, form));
                // Swaps the empty node with the next empty node in the same group, if there is some.
                menu.Items.Add(SwapWithNextEmpty(emptyWord, sentences.ActiveSentence, form));
                return menu;
            }

            /// <summary>
            /// Gets the next word in the sentence that is not the empty node word.
            /// </summary>
            /// <param name="word">The original word.</param>
            /// <param name="sentence">The sentence where the next word is searched for.</param>
            /// <returns>The next word in the sentence that is not the empty node word or null, if there is no such a word.</returns>
            private IWord GetNextNotEmpty(IWord word, ISentence sentence)
            {
                int posFirst = sentence.GetIndexOf(word);
                IWord next = sentence.GetWord(posFirst + 1);
                int i = 2;
                while (next != null && next is EmptyNodeWord)
                {
                    next = sentence.GetWord(posFirst + i);
                    i++;
                }
                return next;
            }

            /// <summary>
            /// The menu item that provides the action of adding the new word to the existing multiword token.
            /// </summary>
            /// <param name="MW">The multiword where the new word will be added.</param>
            /// <param name="ActiveSentence">The sentence that contains the multiword.</param>
            /// <param name="form">The mean to command the main form to change the active word or sentence.</param>
            /// <returns>The menu item with the right text and action.</returns>
            private ToolStripMenuItem AddToMultiword(MultiWord MW, ISentence ActiveSentence, CurrentState form)
            {
                ToolStripMenuItem AddNextToMultiItem = new ToolStripMenuItem("Add next node to this multiword token");
                AddNextToMultiItem.Click += (sender, e) => {
                    // Gets the next word that is not the empty node word.
                    IWord next = GetNextNotEmpty(MW.GetSubWord(MW.To), ActiveSentence);
                    // If the word does not exists, does not do anything
                    if (next == null) return;
                    MW.AddSubWord(next, ActiveSentence);
                    form.VisualiseNewGraph();
                };
                return AddNextToMultiItem;
            }

            /// <summary>
            /// The menu item that provides the action of creating a new multiword.
            /// </summary>
            /// <param name="word">The word whose context menu is shown.</param>
            /// <param name="ActiveSentence">The sentence that contains the multiword.</param>
            /// <param name="form">The mean to command the main form to change the active word or sentence.</param>
            /// <returns>The menu item with the right text and action.</returns>
            private ToolStripMenuItem MakeMultiword(ITreeWord word, ISentence ActiveSentence, CurrentState form)
            {
                ToolStripMenuItem MakeMultiWordItem = new ToolStripMenuItem("Make multiword with the next node");
                MakeMultiWordItem.Click += (sender, e) => {
                    // Gets the next word that is not the empty node word.
                    IWord next = GetNextNotEmpty(word, ActiveSentence);
                    // If the word does not exists, does not do anything
                    if (next == null) return;
                    // Creates the new multiword and adds it to the sentence.
                    MultiWord MW = new MultiWord(word, next, ActiveSentence);
                    ActiveSentence.AddWord(MW, ActiveSentence.GetIndexOf(word));
                    form.VisualiseNewGraph();
                };
                return MakeMultiWordItem;
            }

            /// <summary>
            /// The menu item that provides the action of showing the info about the multiword token.
            /// </summary>
            /// <param name="MW">The multiword whose context menu is shown.</param>
            /// <param name="ActiveSentence">The sentence that contains the multiword.</param>
            /// <param name="form">The mean to command the main form to change the active word or sentence.</param>
            /// <returns>The menu item with the right text and action.</returns>
            private ToolStripMenuItem ShowInfoMultiword(MultiWord MW, ISentence ActiveSentence, CurrentState form)
            {
                ToolStripMenuItem WordInfoItem = new ToolStripMenuItem("Show info about the multiword token");
                WordInfoItem.Click += (sender, e) => {
                    MW.ShowInfo(ActiveSentence);
                    form.VisualiseNewGraph();
                };
                return WordInfoItem;
            }

            /// <summary>
            /// The menu item that provides the action of inserting a new empty word after <paramref name="word"/>.
            /// </summary>
            /// <param name="word">The word whose context menu strip is shown.</param>
            /// <param name="ActiveSentence">The sentence in which the word lies.</param>
            /// <param name="form">The mean to command the main form to change the active word or sentence.</param>
            /// <returns>The menu item with the right text and action.</returns>
            protected ToolStripMenuItem InsertEmptyAfter(BasicWord word, ISentence ActiveSentence, CurrentState form)
            {
                ToolStripMenuItem AddEmptyItem = new ToolStripMenuItem("Insert empty node after");
                AddEmptyItem.Click += (sender, e) =>
                {
                    word.AddEmptyNode(ActiveSentence);
                    form.VisualiseNewGraph();
                };
                return AddEmptyItem;
            }

            /// <summary>
            /// The menu item that provides the action of swapping the <paramref name="empty"/> word 
            /// with the right empty node sibling in the same group, if there is any.
            /// </summary>
            /// <param name="empty">The word whose context menu strip is shown.</param>
            /// <param name="ActiveSentence">The sentence in which the word lies.</param>
            /// <param name="form">The mean to command the main form to change the active word or sentence.</param>
            /// <returns>The menu item with the right text and action.</returns>
            private ToolStripMenuItem SwapWithNextEmpty(EmptyNodeWord empty, ISentence ActiveSentence, CurrentState form)
            {
                ToolStripMenuItem SwapEmptyItem = new ToolStripMenuItem("Swap node with the next empty node");
                SwapEmptyItem.Click += (sender, e) => {
                    // Swaps the word with the sibling whose index is greater by 1.
                    empty.Swap(+1, ActiveSentence);
                    form.VisualiseNewGraph();
                };
                return SwapEmptyItem;
            }

            /// <summary>
            /// The menu item that provides the action of removing the multiword token.
            /// </summary>
            /// <param name="MW">The multiword whose context menu is shown.</param>
            /// <param name="ActiveSentence">The sentence that contains the multiword.</param>
            /// <param name="form">The mean to command the main form to change the active word or sentence.</param>
            /// <returns>The menu item with the right text and action.</returns>
            private ToolStripMenuItem RemoveMultiword(MultiWord multiWord, ISentence ActiveSentence, CurrentState form)
            {
                ToolStripMenuItem RemoveMultiItem = new ToolStripMenuItem("Remove this multiword token");
                RemoveMultiItem.Click += (sender, e) => {
                    multiWord.Delete(ActiveSentence);
                    form.VisualiseNewGraph();
                };
                return RemoveMultiItem;
            }
        }
    }
}