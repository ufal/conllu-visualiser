using System.Drawing;
using ConlluVisualiser;
using GraphVisualiser.BasicVisualiser;

namespace GraphVisualiser
{
    namespace EnhancedVisualiser
    {
        /// <summary>
        ///  The class that provides the right visualisation of the word point according to the type of word in the enhanced representation.
        /// </summary>
        class DrawPointEnhanced : DrawWordBasic, IVisitor
        {
            /// <summary>
            /// Creates the point drawer which respect the <paramref name="schema"/>.
            /// </summary>
            /// <param name="schema">The graphics schema used to visualise the graph.</param>
            public DrawPointEnhanced(GraphicsSchema schema) : base(schema)
            {
            }

            /// <summary>
            /// Draws the word point in the enhanced representation. If the word is joined, it is not visualised.
            /// </summary>
            /// <param name="word">The word whose point is being drawn.</param>
            /// <param name="g"/>The graphics where the point is drawn to.</param>
            /// <returns>True, if the point is not a part of the multiword token; otherwise, false.</returns>
            public override object Visit(BasicWord word, Graphics g)
            {
                DrawPoint(word, g);
                return true;
            }

            /// <summary>
            ///Does not draw the multiword token besause it should not be visualised.
            /// </summary>
            /// <param name="word">The multiword whose point tries to be drawn.</param>        
            /// <param name="g"/>The graphics where the point is drawn to.</param>
            /// <returns>Allways returns false.</returns>
            public override object Visit(MultiWord multiWord, Graphics g)
            {
                return base.Visit(multiWord, g);
            }

            /// <summary>
            /// Draws the empty node word point in the enhanced representation.
            /// </summary>
            /// <param name="emptyWord">The empty node word whose point is being drawn.</param>
            /// <param name="g"/>The graphics where the point is drawn to.</param>
            /// <returns>Allways returns true.</returns>
            public override object Visit(EmptyNodeWord emptyWord, Graphics g)
            {
                // Draws the empry node word point. It has highlighted circumference.
                DrawPoint(emptyWord, g);
                return true;
            }

            /// <summary>
            /// Draws the empty node point with the highlighted circumference according to the <seealso cref="GraphicsSchema"/>
            /// </summary>
            /// <param name="word">The word whose point is being drawn.</param>
            /// <param name="g">The graphics where the point is drawn to.</param>
            private void DrawPoint(EmptyNodeWord word, Graphics g)
            {
                // Draws the basic point.
                base.DrawPoint(word, g);
                // Adds the circumference.
                g.DrawEllipse(Schema.EmptyWordCircumference, word.GetWordPoint().X, word.GetWordPoint().Y, Schema.SizeOfPoint.Width, Schema.SizeOfPoint.Height);
            }
        }
    }
}
