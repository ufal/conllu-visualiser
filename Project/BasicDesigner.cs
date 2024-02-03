using System.Drawing;
using System.Windows.Forms;
using ConlluVisualiser;

namespace GraphVisualiser
{
    namespace BasicVisualiser
    {
        /// <summary>
        /// The designer that visualises the basic representation.
        /// </summary>
        public class BasicDesigner : IDesigner
        {

            /// <summary>
            /// Used graphics schema - colors, font, size of point.
            /// Same for all components which participate in graph visualisation.
            /// </summary>
            public GraphicsSchema Schema { get; protected set; } = new GraphicsSchema();

            /// <summary>
            /// Counts the coordinates according to the basic representation of the graph.
            /// </summary>
            BasicPointCounter PointCounter { get; set; }

            /// <summary>
            /// Draws the word with the lines that connect the word with its parent.
            /// </summary>
            protected IVisitor WordDrawer { get; set; }
            /// <summary>
            /// Draws the word point.
            /// </summary>
            protected IVisitor PointDrawer { get; set; }

            /// <summary>
            /// Returns the context menu of the clicked word.
            /// </summary>
            protected IGetMenuVisitor ContextMenuGetter { get; set; }
            /// <summary>
            /// Creates the designer and all the classes according to the color schema.
            /// </summary>
            public BasicDesigner()
            {
                WordDrawer = new DrawWordBasic(Schema);
                PointDrawer = new DrawPointBasic(Schema);
                ContextMenuGetter = new GetMenuBasic();
            }

            /// <summary>
            /// Visualises the whole sentence. Uses concrete designer to visualise the concrete representation.
            /// </summary>
            /// <param name="sentence">The sentence which is going to be visualsised.</param>
            public virtual void Visualise(ISentence sentence, Graphics g)
            {
                // Each sentence must have a root
                if (sentence == null || sentence.Root == null)
                    return;
                // Root point x-coordinate is in the middle of the width of the tree panel.
                // The middle of the width of the tree anel is the half of the start of the extra word.
                // Root point y-coordinate is the same as the minimal distance of two neighbouring graph floors.
                sentence.Root.GetWordPoint().SetPoint(PointCounter.GetRootPoint(Schema));
                // Draws the root point according to representation
                DrawWord(sentence.Root, g, sentence);
                // For all words except from the root, finishes making of the point and draws the word.
                for (int i = 1; i < sentence.CountWords; i++)
                {
                    // Gets the word according to position in the sentence.
                    IWord word = sentence.GetWord(i);
                    // Visits the concrete word and according to its type and representation draws the word or does not do anything.
                    DrawWord(word, g, sentence);
                }
                // Writes all words under their points
                WriteStrings(sentence, g);
            }

            /// <summary>
            /// Counts the new point coordinates for all words in the sentence.
            /// </summary>
            /// <param name="sentence">The sentence whose word points are counted.</param>
            /// <param name="width">The optimal width of the space where the sentence is drawn.</param>
            /// <param name="height">The optimal height of the space where the sentence is drawn.</param>
            /// <param name="g">The graphics that is used to visualise the sentence.</param>
            /// <returns>The final size of the sentence graph.</returns>
            public virtual SizeF NewPoints(ISentence sentence, int width, int height, Graphics g)
            {
                PointCounter = new BasicPointCounter(Schema);
                float graphWidth = PointCounter.CountAndCreatePoints(sentence, width, height, g, Schema);
                float graphHeight = (PointCounter.MaximumHeight - 1) * (int)PointCounter.LevelDifference;
                return new SizeF(graphWidth, graphHeight);
            }
            /// <summary>
            /// Writes the Form word attribute under each word point in the sentence.
            /// </summary>
            /// <param name="sentence">The visualised sentence.</param>
            protected void WriteStrings(ISentence sentence, Graphics g)
            {
                // For all words in the sentence
                for (int i = 0; i < sentence.CountWords; i++)
                {
                    // Gets word according to its position in the sentence
                    IWord word = sentence.GetWord(i);
                    // If the word point is visualised (not empty) writes the Form attribute under the point.
                    if (!word.GetWordPoint().GetPoint().IsEmpty)
                    {
                        int x = (int)word.GetWordPoint().X;
                        int y = (int)word.GetWordPoint().Y + Schema.SizeOfPoint.Height;
                        g.DrawString(word.Info.Form, Schema.BoldFont, Schema.StringBrush, x, y);
                    }
                }
            }

            /// <summary>
            /// Draws the word according to its type with the lines that connects the word with its parent(s).
            /// </summary>
            /// <param name="word">The word that is being drawn.</param>
            /// <param name="g">The graphics where the word is drawn to.</param>
            /// <param name="sentence">Not used parameter.</param>
            /// <returns>True if the word is visible in the representation; otherwise, false.</returns>
            public virtual bool DrawWord(IWord word, Graphics g, ISentence sentence)
            {
                return (bool)word.Accept(WordDrawer, g);
            }

            /// <summary>
            /// Draws the<paramref name="word"/> point on the graphics <paramref name="g"/>.
            /// </summary>
            /// <param name="word">The word whose point is being drawn.</param>
            /// <param name="g">The graphics where the word point is drawn to.</param>
            /// <returns>True if the word is visible in the representation; otherwise, false.</returns>
            public virtual bool DrawPoint(IWord word, Graphics g)
            {
                return (bool)word.Accept(PointDrawer, g);
            }

            /// <summary>
            /// Shifts the active word in the graph.
            /// </summary>
            /// <param name="w">The original active word.</param>
            /// <param name="key">The pressed key according to which the active word is changed.</param>
            /// <returns>The new active word.</returns>
            public virtual IWord ShiftActive(IWord w, ISentence sentence, Keys key)
            {
                if(!(w is ITreeWord))
                {
                    return w;
                }
                ITreeWord word = w as ITreeWord;
                switch (key)
                {
                    case Keys.Up:
                        if (word.Parent != null)
                            return word.Parent;
                        return word;
                    case Keys.Down:
                        if (word.Children.Count > 0)
                            return word.Children.Values[0];
                        return word;
                    case Keys.Left:
                        return GetSiblingOfChild(word.Parent, word, -1);
                    case Keys.Right:
                        return GetSiblingOfChild(word.Parent, word, 1);
                }
                return word;
            }

            /// <summary>
            /// Returns its child determined by the param <paramref name="child"/> distanced by <paramref name="direction"/>.
            /// If there is no suitable word, returns the <paramref name="child"/> from parameter.
            /// </summary>
            /// <param name="parent">The parent of the <paramref name="child"/>.</param>
            /// <param name="child">The child whose sibling is searched for</param>
            /// <param name="direction">The number which determines direction and distance from the original child</param>
            /// <returns>Suitable sibling, if it was found; the original child otherwise</returns>
            public IWord GetSiblingOfChild(ITreeWord parent, ITreeWord child, int direction)
            {
                // If the word has no parent, it has no siblings in the tree.
                if (parent == null)
                    return child;
                // Tries to find the sibling word in children.
                int idx = parent.Children.IndexOfValue(child);
                if (idx + direction >= 0 && idx + direction < parent.Children.Count)
                    return parent.Children.Values[idx + direction];
                // If the sibling was not found, return the original word.
                return child;
            }

            /// <summary>
            /// Returns the context menu with actions according to the type of the word.
            /// </summary>
            /// <param name="word">The word whose menu is wanted.</param>
            /// <param name="sentences">The list of sentences where the <paramref name="word"/> lies.</param>
            /// <param name="form">The commander of the main form to change active word or sentence.</param>
            /// <returns>The context menu that should be shown.</returns>
            public ContextMenuStrip GetContextMenu(IWord word, ListOfSentences sentences, CurrentState form)
            {
                return word.GetContextMenu(ContextMenuGetter, sentences, form);
            }
        }
    }
}