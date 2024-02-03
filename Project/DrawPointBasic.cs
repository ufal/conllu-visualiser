using System.Drawing;
using ConlluVisualiser;

namespace GraphVisualiser
{
    namespace BasicVisualiser
    {
        /// <summary>
        ///  The class that provides the right visualisation of the word point according to the type of word in the basic representation.
        /// </summary>
        public class DrawPointBasic : IVisitor
        {
            /// <summary>
            /// The schema with information about the visualisation.
            /// </summary>
            protected readonly GraphicsSchema Schema;
            /// <summary>
            /// Creates the point drawer which respects the <paramref name="schema"/>.
            /// </summary>
            /// <param name="schema">The graphics schema used to visualise the graph.</param>
            public DrawPointBasic(GraphicsSchema schema)
            {
                Schema = schema;
            }

            /// <summary>
            /// Draws the word point in the basic representation.
            /// </summary>
            /// <param name="word">The word whose point is being drawn.</param>
            /// <param name="g">The graphics where the word is drawn to.</param>
            /// <returns>Allways returns true, the point is allways drawn.</returns>
            public virtual object Visit(BasicWord word, Graphics g)
            {
                DrawPoint(word, g);
                return true;
            }

            /// <summary>
            /// Does not do anything, the multiword is not visualised in the basic representation. 
            /// </summary>
            /// <param name="multiWord">The multiword that tries to draw its point.</param>
            /// <param name="g">The graphics where the word is drawn to.</param>
            /// <returns>Allways returns false.</returns>
            public virtual object Visit(MultiWord multiWord, Graphics g) { return false; }

            /// <summary>
            /// Does not do anything, the empty node is not visualised in the basic representation. 
            /// </summary>
            /// <param name="multiWord">The empty node that tries to draw its point.</param>
            /// <param name="g">The graphics where the word is drawn to.</param>
            /// <returns>Allways returns false.</returns>
            public virtual object Visit(EmptyNodeWord emptyWord, Graphics g) { return false; }

            /// <summary>
            /// Draws the point by the color of the graph. If the word is active, draws it by the color of active node.
            /// </summary>
            /// <param name="word">The word whose point is going to be drawn.</param>
            /// <param name="g">The graphics where the point is drawn to. </param>
            protected virtual void DrawPoint(IWord word, Graphics g)
            {
                if (word.IsActive)
                {
                    // Draws the point with use of the color of active node.
                    g.FillEllipse(Schema.ColorOfActiveNode, word.GetWordPoint().X, word.GetWordPoint().Y, Schema.SizeOfPoint.Width, Schema.SizeOfPoint.Height);
                }
                else
                {
                    // Draws the point with use of the color of the graph.
                    g.FillEllipse(Schema.ColorOfGraph, word.GetWordPoint().X, word.GetWordPoint().Y, Schema.SizeOfPoint.Width, Schema.SizeOfPoint.Height);
                }
            }
        }
    }
}