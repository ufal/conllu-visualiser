using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ConlluVisualiser;
using GraphVisualiser.BasicVisualiser;

namespace GraphVisualiser
{
    namespace EnhancedVisualiser
    {
        /// <summary>
        /// The designer that visualises the enhanced representation.
        /// </summary>
        public class EnhancedDesigner : BasicDesigner, IDesigner
        {
            /// <summary>
            /// The distance between two levels of lines in the graph.
            /// </summary>
            private const int HeightOfLevel = 30;

            /// <summary>
            /// The class that counts the word points in the graph.
            /// </summary>
            private EnhancedPointCounter PointCounter { get; set; }

            /// <summary>
            /// Creates the designer of the enhanced representation of the graph.
            /// </summary>
            public EnhancedDesigner()
            {
                PointDrawer = new DrawPointEnhanced(Schema);
                ContextMenuGetter = new GetMenuEnhanced();
            }

            /// <summary>
            /// Shifts the active word according to the word order in the graph.
            /// If the shift is not possible, returns the original <paramref name="word"/>.
            /// </summary>
            /// <param name="word">The original active word.</param>
            /// <param name="sentence">The sentence where the active word lies.</param>
            /// <param name="key">The key that was pressed.</param>
            /// <returns>The new active word.</returns>
            public override IWord ShiftActive(IWord word, ISentence sentence, Keys key)
            {
                switch (key)
                {
                    // If the left or down key was pressed, the new active word is the preceding word
                    // in the word order in the sentence.
                    case Keys.Left:
                    case Keys.Down:
                        return GetSiblingOrItself(word, sentence, -1);
                    // If the right or up key was pressed, the new active word is the following word.
                    case Keys.Right:
                    case Keys.Up:
                        return GetSiblingOrItself(word, sentence, 1);
                }
                return word;
            }

            /// <summary>
            /// Returns the sibling of the word according to the direction. 
            /// If the sibling does not exists, returns the original word.
            /// </summary>
            /// <param name="word">The original word whose sibling is searched for.</param>
            /// <param name="sentence">The sentence which contains the words.</param>
            /// <param name="direction">The direction which defines the sibling.</param>
            /// <returns>The sibling or the original word, if the sibling was not found.</returns>
            private IWord GetSiblingOrItself(IWord word, ISentence sentence, int direction)
            {
                //  Finds the position of the word in the sentence.
                int index = sentence.GetIndexOf(word);
                // Tries to get the sibling. Returns null if the sibling does not exists.
                IWord sibling = sentence.GetWord(index + direction);
                // If the sibling is not null, returns it. Else returns the original word.
                return sibling ?? word;
            }

            /// <summary>
            /// Draws the word point in the graph. Does not change the structure of the graph.
            /// </summary>
            /// <param name="word">The word whose point is being drawn.</param>
            /// <param name="g">The graphics where the point is drawn to.</param>
            /// <returns>True if the point could have been drawn; otherwise, false.</returns>
            public override bool DrawPoint(IWord word, Graphics g)
            {
                // Draws the word according to itd type.
                return (bool)word.Accept(PointDrawer, g);
            }

            /// <summary>
            /// Visualises the whole sentence. Does not count the coordinates, uses the sounted points.
            /// </summary>
            /// <param name="sentence">The sentence that is being drawn.</param>
            /// <param name="g">The graphics where the sentence is drawn to.</param>
            public override void Visualise(ISentence sentence, Graphics g)
            {
                // For all words except from the root, draws their point.
                for (int i = 0; i < sentence.CountWords; i++)
                {
                    // Gets the word according to position in the sentence.
                    IWord word = sentence.GetWord(i);
                    // Visits the concrete word and according to its type and representation draws the word.
                    DrawPoint(word, g);
                }
                // Draws all lines that join the deps in the sentence.
                DrawLines(sentence, g);

                // Writes all words under their points.
                WriteStrings(sentence, g);
            }

            /// <summary>
            /// Draws the lines that join the words and their deps in the <paramref name="sentence"/>.
            /// </summary>
            /// <param name="sentence">The sentence that is being drawn.</param>
            /// <param name="g">The graphics where the sentence is drawn to.</param>
            private void DrawLines(ISentence sentence, Graphics g)
            {
                // Draws the lines from the highest level.
                // The lines on the lower levels will not overlap the texts on the higher levels.
                foreach (var level in PointCounter.SortedLevels.OrderByDescending(x => x.Key))
                {
                    // Draws all lines on one level. Adds them the text of the dependency.
                    foreach (var dep in level.Value)
                    {
                        // Gets the words that should be joined by the line.
                        IWord child = sentence.GetWord(dep.Key);
                        IWord parent = sentence.GetWord(dep.Value);
                        // Gets the direction of the arrow.
                        int direction = (sentence.GetIndexOf(child) < sentence.GetIndexOf(parent)) ? -1 : 1;
                        // Counts the middle point of the line.
                        PointF middle = DrawDepLine(child.GetWordPoint().GetPoint(),
                            parent.GetWordPoint().GetPoint(), g, PointCounter.GetLevelDep(child, parent));
                        // Adds the text and the arrow to the line.
                        AddDepText(middle, child.Info.Deps[parent.Id], direction, g);
                    }
                }
            }

            /// <summary>
            /// Counts the new point coordinates for all words in the sentence.
            /// </summary>
            /// <param name="sentence">The sentence whose word points are counted.</param>
            /// <param name="width">The optimal width of the space where the sentence is drawn.</param>
            /// <param name="height">The optimal height of the space where the sentence is drawn.</param>
            /// <param name="g">The graphics that is used to visualise the sentence.</param>
            /// <returns>The final size of the sentence graph.</returns>
            public override SizeF NewPoints(ISentence sentence, int width, int height, Graphics g)
            {
                // Actualizes the point counter.
                PointCounter = new EnhancedPointCounter(Schema);
                // Counts the points and returns the graph width.
                float graphWidth = PointCounter.CountAndCreatePoints(sentence, width, height, g, Schema);
                // Counts the graph height as the number of the highest level multiplied by the level space.
                float graphHeight = PointCounter.MaximumHeight * (int)PointCounter.LevelDifference;
                return new SizeF(graphWidth, graphHeight);
            }

            /// <summary>
            /// Draws the line between the <paramref name="child"/> and <paramref name="parent"/> point in right <paramref name="level"/>.
            /// </summary>
            /// <param name="child">The child point.</param>
            /// <param name="parent">The parent point.</param>
            /// <param name="g">The graphics where the line is drawn to.</param>
            /// <param name="level">The level in which the line lies.</param>
            /// <returns>The middle point of the line.</returns>
            private PointF DrawDepLine(PointF child, PointF parent, Graphics g, int level)
            {
                // Assumes that the child point is on the left side of the parent point. 
                // If not, do the same action in the swapped order.
                if (child.X > parent.X)
                {
                    return DrawDepLine(parent, child, g, level);
                }
                // Even level lines join the points from above, the odd level lines join the points from below.
                // The up value specifies if the line is above (+1) or below (-1) the points.
                int up = level % 2 == 0 ? 1 : -1;
                // Changes the levels to their halves, because the half of levels is above and the second half is under the graph.
                // The levels of lines below points goes from 2, because the word texts are below points and it should not cross.
                level = level / 2 + (level % 2) * 2;
                // Prepares the points that are joined by the line.
                PointF[] points =
                {
                // The X coordinate is in the middle of the child point. The Y is at the top or at the bottom of the point.
                // It depends on whether a given line is drawn under points or above points.
                new PointF(child.X + Schema.SizeOfPoint.Width/2, child.Y + (Math.Max(-up, 0)) * Schema.SizeOfPoint.Height),
                // The point in the right level above the child point. It is slightly at right.
                new PointF(child.X + Schema.SizeOfPoint.Width, child.Y - up*HeightOfLevel*level),
                // The point in the right level above the parent point. It is slightly at left.
                new PointF(parent.X, child.Y - up*HeightOfLevel*level),
                // The point of the parent word, symetrical with the child point.
                new PointF(parent.X + Schema.SizeOfPoint.Width/2, parent.Y + (Math.Max(-up, 0)) * Schema.SizeOfPoint.Height),
            };
                // Draws the line that connects all the points.
                g.DrawLines(Schema.LinePen, points);
                // Counts the point in the middle of the line and returns it.
                return new PointF((child.X + parent.X + Schema.SizeOfPoint.Width) / 2, child.Y - up * HeightOfLevel * level);
            }

            /// <summary>
            /// Adds the text that is defined by the Deps attribute on the line that connects the child and parent in enhanced.
            /// </summary>
            /// <param name="middleOfLine">The point in the middle of the line.</param>
            /// <param name="depText">The text that is written on the line.</param>
            /// <param name="direction">
            /// The direction of the arrow. -1, if the child is at left and the parent is at right; otherwise +1.
            /// </param>
            /// <param name="g"></param>
            private void AddDepText(PointF middleOfLine, string depText, int direction, Graphics g)
            {
                // Gets the size of the rectangle with the word.
                var size = g.MeasureString(depText, Schema.DepFont);
                // Creates the rectangle situated in the middle of the line.
                RectangleF rect = new RectangleF(new PointF(middleOfLine.X - size.Width / 2, middleOfLine.Y - size.Height / 2), size);
                // Fills the rectangle by the white color and adds the text in it.
                g.FillRectangle(Brushes.White, rect);
                g.DrawString(depText, Schema.DepFont, Schema.StringBrush, rect);
                PointF[] trianglePoints = new PointF[3];
                // Draws the arrow as the triangle according to the direction.
                if (direction > 0)
                {
                    // The arrow points to the left. It is situated before the rectangle with the word.
                    trianglePoints[0] = new PointF(rect.X, rect.Y);
                    trianglePoints[1] = new PointF(rect.X - Schema.SizeOfPoint.Width / 2, rect.Y + size.Height / 2);
                    trianglePoints[2] = new PointF(rect.X, rect.Y + size.Height);
                }
                else
                {
                    // The arrow points to the right. It is situated at the end of the rectangle with the word.
                    trianglePoints[0] = new PointF(rect.X + size.Width, rect.Y);
                    trianglePoints[1] = new PointF(rect.X + size.Width + Schema.SizeOfPoint.Width / 2, rect.Y + size.Height / 2);
                    trianglePoints[2] = new PointF(rect.X + size.Width, rect.Y + size.Height);
                }
                // Draws and fills the arrow.
                g.DrawPolygon(Schema.LinePen, trianglePoints);
                g.FillPolygon(Schema.ColorOfGraph, trianglePoints);
            }

        }
    }
}