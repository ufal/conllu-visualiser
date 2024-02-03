using System.Drawing;
using System.Windows.Forms;
using ConlluVisualiser;

namespace GraphVisualiser
{
    /// <summary>
    /// The interface that provides design of the specific representation.
    /// </summary>
    public interface IDesigner
    {
        /// <summary>
        /// Used graphics schema - colors, font, size of point.
        /// Same for all components which participate in graph visualisation.
        /// </summary>
        GraphicsSchema Schema { get; }

        /// <summary>
        /// Visualises the graph of the <paramref name="sentence"/>.
        /// </summary>
        /// <param name="sentence">The sentence that is being visualised.</param>
        /// <param name="g">The graphics where the sentence graph is drawn to.</param>
        void Visualise(ISentence sentence, Graphics g);

        /// <summary>
        /// Draws the<paramref name="word"/> point on the graphics <paramref name="g"/>, if it is visible in the designer's representation.
        /// </summary>
        /// <param name="word">The word whose point is being drawn.</param>
        /// <param name="g">The graphics where the word point is drawn to.</param>
        /// <returns>True if the word is visible in the representation; otherwise, false.</returns>
        bool DrawPoint(IWord word, Graphics g);

        /// <summary>
        /// Shifts the active word in the graph.
        /// </summary>
        /// <param name="word">The original active word.</param>
        /// <param name="key">The pressed key according to which the active word is changed.</param>
        /// <returns>The new active word.</returns>
        IWord ShiftActive(IWord word, ISentence sentence, Keys key);

        /// <summary>
        /// Returns the context menu with actions according to the type of the word.
        /// </summary>
        /// <param name="word">The word whose menu is wanted.</param>
        /// <param name="sentences">The list of sentences where the <paramref name="word"/> lies.</param>
        /// <param name="form">The commander of the main form to change active word or sentence.</param>
        /// <returns>The context menu that should be shown.</returns>
        ContextMenuStrip GetContextMenu(IWord word, ListOfSentences sentences, CurrentState form);

        /// <summary>
        /// Counts the new point coordinates for all words in the sentence.
        /// </summary>
        /// <param name="sentence">The sentence whose word points are counted.</param>
        /// <param name="width">The optimal width of the space where the sentence is drawn.</param>
        /// <param name="height">The optimal height of the space where the sentence is drawn.</param>
        /// <param name="g">The graphics that is used to visualise the sentence.</param>
        /// <returns>The final size of the sentence graph.</returns>
        SizeF NewPoints(ISentence sentence, int width, int height, Graphics g);
    }
}
