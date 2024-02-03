using System.Drawing;
using System.Windows.Forms;
using ConlluVisualiser;

namespace GraphVisualiser
{
    /// <summary>
    /// The interface for the parts of the <seealso cref="IDesigner"/>. 
    /// Provides one action that is specific for the representation and distinguish between the types of words.
    /// Based on the Visitor design pattern.
    /// </summary>
    public interface IVisitor
    {
        /// <summary>
        /// The action specific for the word.
        /// </summary>
        /// <param name="word">The word on which the action is made.</param>
        /// <param name="g">The graphics where the word is drawn to.</param>
        /// <returns>The result of the action, different for each action.</returns>
        object Visit(BasicWord word, Graphics g);

        /// <summary>
        /// The action specific for the multiword token.
        /// </summary>
        /// <param name="multiWord">The multiword token on which the action is made.</param>
        /// <param name="g">The graphics where the word is drawn to.</param>
        /// <returns>The result of the action, different for each action.</returns>
        object Visit(MultiWord multiWord, Graphics g);

        /// <summary>
        /// The action specific for the empty node word.
        /// </summary>
        /// <param name="emptyWord">The empty node word on which the action is made.</param>
        /// <param name="g">The graphics where the word is drawn to.</param>
        /// <returns>The result of the action, different for each action.</returns>
        object Visit(EmptyNodeWord emptyWord, Graphics g);
    }

    /// <summary>
    /// Gets the Context menu according to the representation and the type of the word.
    /// </summary>
    public interface IGetMenuVisitor
    {
        /// <summary>
        /// Returns the context menu with defined actions on the word according the curret representation.
        /// </summary>
        /// <param name="word">The word whose menu is shown.</param>
        /// <param name="sentences">The list of sentences where the defined actions can be processed.</param>
        /// <param name="form">The mean to command the main form to change the active word or sentence.</param>
        /// <returns>The created context menu.</returns>
        ContextMenuStrip GetMenu(BasicWord word, ListOfSentences sentences, CurrentState form);

        /// <summary>
        /// Returns the context menu with defined actions on the multiword according to the curret representation.
        /// </summary>
        /// <param name="multiword">The word whose menu is shown.</param>
        /// <param name="sentences">The list of sentences where the defined actions can be processed.</param>
        /// <param name="form">The mean to command the main form to change the active word or sentence.</param>
        /// <returns>The created context menu.</returns>
        ContextMenuStrip GetMenu(MultiWord multiword, ListOfSentences sentences, CurrentState form);

        /// <summary>
        /// Returns the context menu with defined actions on the empty node word according to the curret representation.
        /// </summary>
        /// <param name="emptyword">The word whose menu is shown.</param>
        /// <param name="sentences">The list of sentences where the defined actions can be processed.</param>
        /// <param name="form">The mean to command the main form to change the active word or sentence.</param>
        /// <returns>The created context menu.</returns>
        ContextMenuStrip GetMenu(EmptyNodeWord emptyword, ListOfSentences sentences, CurrentState form);
    }
}
