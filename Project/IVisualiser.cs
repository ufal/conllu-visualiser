using System.Drawing;
using System.Windows.Forms;
using ConlluVisualiser;

namespace GraphVisualiser
{
    /// <summary>
    /// Provides an interface to visualise the sentence on the drawing panel.
    /// </summary>
    public interface IVisualiser
    {
        /// <summary>
        /// The current scroll position in the panel, where the sentence is visualised.
        /// </summary>
        PointF ScrollPosition { get; }

        /// <summary>
        /// Visualises the graph without counting of the new coordinates or changing of the scroll positions.
        /// </summary>
        /// <param name="sentence"></param>
        void Visualise(ISentence sentence);

        /// <summary>
        /// Creates a new graphics, recounts the coordinates of the <paramref name="sentence"/> points.
        /// </summary>
        /// <param name="sentence">The sentence that is visualised.</param>
        void NewGraphics(ISentence sentence);

        /// <summary>
        ///  Creates a new graphics, but does not recount the coordinates of the <paramref name="sentence"/> points.
        ///  Used mainly while scrolling - the graph does not change, only the scroll position.
        /// </summary>
        /// <param name="sentence">The sentence that is visualised.</param>
        void ScrollNewGraphics(ISentence sentence);

        /// <summary>
        /// Draws only one word point. It is used when the active word changes, 
        /// then the only changes are the colors of the old and the new active words.
        /// </summary>
        /// <param name="w">The word whose point is drawn.</param>
        void DrawOneWord(IWord w);

        /// <summary>
        /// Shows the basic info about the word. It is used while hovering the word point.
        /// </summary>
        /// <param name="basicWordInfo">The panel where the infomation are written to. It is relocated next to the hovered point.</param>
        /// <param name="word">The hovered word whose info is shown.</param>
        /// <param name="sentence">The sentence that is visualised in case the point is the root of the sentence.</param>
        /// <returns>True if the info is not empty and is shown; otherwise, false.</returns>
        bool ShowBasicInfo(Panel basicWordInfo, IWord word, ISentence sentence);

        /// <summary>
        /// Finds the word whose point lies on the <paramref name="origPoint"/> position on the drawing panel.
        /// </summary>
        /// <param name="sentence">The sentence that is currently visualised.</param>
        /// <param name="origPoint">The point on the drawing panel 
        /// whose word is searched for in the words of the sentence.</param>
        /// <returns>The word whose point was searched for or null, if there was no word on the point.</returns>
        IWord FindRightPoint(ISentence sentence, PointF origPoint);

        /// <summary>
        /// Shifts the active word according to pressed key. If the active word can not be changed, returns the original active word.
        /// </summary>
        /// <param name="key">The pressed key - should be the arrow.</param>
        /// <param name="sentence">The sentence that is currently visualised.</param>
        /// <param name="active">The original active word.</param>
        /// <returns>The new active word.</returns>
        IWord ShiftActive(Keys key, ISentence sentence, IWord active);

        /// <summary>
        /// Returns the context menu of the specific type of word in the current representation.
        /// </summary>
        /// <param name="word">The right clicked word.</param>
        /// <param name="sentences">The list of sentences in the currently open file.</param>
        /// <param name="form">The mean to change the active word or the active sentence in the main form.</param>
        /// <returns>The proper context menu with defined actions.</returns>
        ContextMenuStrip GetContextMenu(IWord word, ListOfSentences sentences, CurrentState form);
    }
}
