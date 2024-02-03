using System.Drawing;

namespace GraphVisualiser
{
    /// <summary>
    /// Class to represent common colors and other drawing properties which are used by various components. 
    /// </summary>
    public class GraphicsSchema
    {
        /// <summary>
        /// The main color of the graph parts (points, arrows in the enhanced representation).
        /// </summary>
        public Brush ColorOfGraph { get; } = Brushes.Salmon;

        /// <summary>
        /// The color of the active node point.
        /// </summary>
        public Brush ColorOfActiveNode { get; } = Brushes.DarkRed;

        /// <summary>
        /// The brush that is used to write texts in the graph.
        /// </summary>
        public Brush StringBrush { get; } = Brushes.Black;

        /// <summary>
        /// The color that is used to draw the shape around the words that are joined to a multiword token.
        /// </summary>
        public Pen JoinedWordCircumference { get; } = Pens.DarkRed;

        /// <summary>
        /// The color that is used to draw the circumference around the point of the empty node word.
        /// </summary>
        public Pen EmptyWordCircumference { get; } = Pens.DarkBlue;

        /// <summary>
        /// The font that is used to write the form of the words under the points.
        /// </summary>
        public Font BoldFont { get; } = new Font("Arial", 10, FontStyle.Bold);

        /// <summary>
        /// The font that is used to write the text of the dependency on the arrows in the enhanced graph representation.
        /// </summary>
        public Font DepFont { get; } = new Font("Arial", 9);

        /// <summary>
        /// The size of point in the graph.
        /// </summary>
        public Size SizeOfPoint { get; } = new Size(15, 15);

        /// <summary>
        /// The pen by which the lines in the graph are drawn.
        /// </summary>
        public Pen LinePen { get; } = new Pen(Color.Salmon, 2);
    }

}
