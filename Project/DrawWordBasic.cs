using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ConlluVisualiser;

namespace GraphVisualiser
{
    namespace BasicVisualiser
    {
        /// <summary>
        ///  The class that provides the right visualisation of the whole word according to the type of word in the basic representation.
        /// </summary>
        class DrawWordBasic : DrawPointBasic, IVisitor
        {
            /// <summary>
            /// Creates the word drawer which respects the <paramref name="schema"/>.
            /// </summary>
            /// <param name="schema">The graphics schema used to visualise the graph.</param>
            public DrawWordBasic(GraphicsSchema schema) : base(schema)
            {
            }

            /// <summary>
            /// Draws the word point with line to its parent.
            /// </summary>
            /// <param name="word">The word that is being drawn.</param>
            /// <param name="parameters"><see cref="Graphics"/> graphics: the graphics where the word is drawn to.</param>
            /// <returns>Allways returns true.</returns>
            public override object Visit(BasicWord word, Graphics g)
            {
                // Draws the line that connects the word point with the point of its parent.
                DrawLine(word, g);
                // Draws the word point.
                DrawPoint(word, g);
                return true;
            }

            /// <summary>
            /// Draws the circle around the words that are parts of the multiword token.
            /// </summary>
            /// <param name="multiWord">The multiword token that is going to be visualised.</param>
            /// <param name="g">The graphics where the word is drawn to.</param>
            /// <returns>Allways returns true.</returns>
            public override object Visit(MultiWord multiWord, Graphics g)
            {
                // The most left word.
                IWord first = multiWord.GetSubWord(multiWord.From);
                // The most right word.
                IWord last = multiWord.GetSubWord(multiWord.To);
                // The words that are at the highest level in graph.
                List<IWord> tops = new List<IWord>();
                // The words that are at the lowest level in graph.
                List<IWord> downs = new List<IWord>();
                int start = int.Parse(multiWord.From);
                // Goes through all words in the multiword token.
                // Chooses the words that define the circle - the words on the top border and the words on the down border of multiword.
                for (int i = start; i <= int.Parse(multiWord.To); i++)
                {
                    IWord sub = multiWord.GetSubWord(i.ToString());
                    if (sub.GetWordPoint().Level == multiWord.MinLevel)
                        tops.Add(sub);
                    if (sub.GetWordPoint().Level == multiWord.GetMaxLevel())
                        downs.Add(sub);
                }
                // The chosen words that define the border of the multiword.
                PointF[] points = FindPointsToJoin(first, last, tops, downs);
                // Joins the border points.
                JoinAllPoints(points, g);
                return true;
            }

            /// <summary>
            /// Does not do anything, the empty node is not visualised in the basic representation.
            /// </summary>
            /// <param name="emptyWord">The empty node that tries to draw itself.</param>
            /// <param name="g">The graphics where the word is drawn to.</param>
            /// <returns>Allways returns false.</returns>
            public override object Visit(EmptyNodeWord emptyWord, Graphics g) => false;

            /// <summary>
            /// Draws the line from the <paramref name="word"/> to its parent.
            /// </summary>
            /// <param name="word">The word whose line is being drawn.</param>
            /// <param name="g">The graphics where the line is drawn to.</param>
            protected virtual void DrawLine(BasicWord word, Graphics g)
            {
                // If the parent exists, joins it by line.
                if (word.Parent != null)
                {
                    DrawOneLine(word.GetWordPoint().GetPoint(), word.Parent.GetWordPoint().GetPoint(), g);
                }
            }

            /// <summary>
            /// Draws the line that joins the points <paramref name="child"/> and <paramref name="parent"/> from parameters.
            /// </summary>
            /// <param name="child">First point that is going to be joined.</param>
            /// <param name="parent">Second point that is going to be joined.</param>
            /// <param name="g">The graphics where the line is drawn to.</param>
            protected virtual void DrawOneLine(PointF child, PointF parent, Graphics g)
            {
                // Draws the line from the middle of the child node to the middle of down part of the parent node.
                g.DrawLine(Schema.LinePen, child.X + Schema.SizeOfPoint.Width / 2, child.Y + Schema.SizeOfPoint.Height / 2,
                        parent.X + Schema.SizeOfPoint.Width / 2, parent.Y + Schema.SizeOfPoint.Height);
            }

            /// <summary>
            /// Finds and returns the points that are circumscribed by the shape that determines the multiword.
            /// </summary>
            /// <param name="left">The most left word</param>
            /// <param name="right">The most right word</param>
            /// <param name="top">The words in the highest level.</param>
            /// <param name="down">The words in the lowest level.</param>
            /// <returns>The array with points that should be circumscribed.</returns>
            private PointF[] FindPointsToJoin(IWord left, IWord right, List<IWord> top, List<IWord> down)
            {
                // Creates the array with points that goes arouund the multiword space.
                List<PointF> result = new List<PointF>
            {
                // Top left corner of the left word.
                left.GetWordPoint().GetPoint(),
                // Lower left corner of the left word.
                new PointF(left.GetWordPoint().X, left.GetWordPoint().Y + Schema.SizeOfPoint.Height),
                // Lower left corner of the down left word.
                new PointF(down[0].GetWordPoint().X, down[0].GetWordPoint().Y + Schema.SizeOfPoint.Height),
                // Lower right corner of the down right word.
                new PointF(down[down.Count - 1].GetWordPoint().X + Schema.SizeOfPoint.Width, down[down.Count - 1].GetWordPoint().Y + Schema.SizeOfPoint.Height),
                // Lower right corner of the right word.
                new PointF(right.GetWordPoint().X + Schema.SizeOfPoint.Width, right.GetWordPoint().Y + Schema.SizeOfPoint.Height),
                // Top right corner of the right word.
                new PointF(right.GetWordPoint().X + Schema.SizeOfPoint.Width, right.GetWordPoint().Y),
                // Top right corner of the top right word.
                new PointF(top[top.Count - 1].GetWordPoint().X + Schema.SizeOfPoint.Width, top[top.Count - 1].GetWordPoint().Y),
                // Top left corner of the left word.
                top[0].GetWordPoint().GetPoint()
            };
                // The array with result border points with removed duplicities.
                PointF[] resultArr = result.Distinct().ToArray();
                return resultArr;
            }

            /// <summary>
            /// Describes the shape around the points from the array.
            /// </summary>
            /// <param name="points">The points that are going to be circumscribed.</param>
            /// <param name="graphics">The graphics where the line is drawn to.</param>
            private void JoinAllPoints(PointF[] points, Graphics graphics)
            {
                // The pen that is used to join the points.
                Pen p = new Pen(Color.DarkRed, 2)
                {
                    DashStyle = System.Drawing.Drawing2D.DashStyle.Dash
                };
                // Describes the points.
                graphics.DrawClosedCurve(p, points, 0.3f, System.Drawing.Drawing2D.FillMode.Alternate);
            }
        }
    }
}