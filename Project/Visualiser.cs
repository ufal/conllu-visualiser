using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ConlluVisualiser;

namespace GraphVisualiser
{    
    /// <summary>
    /// Class to visualise the graph of sentence with use of the concrete designer and according to the size of the form.
    /// Implements IVisualiser interface.
    /// </summary>
    public class Visualiser : IVisualiser
    {
        /// <summary>
        /// The graphics schema used by the current designer - colors, font, size of point.
        /// </summary>
        public GraphicsSchema Schema => designer.Schema;

        /// <summary>
        /// Current sroll position of TreePanel, where the sentence is visualised.
        /// </summary>
        /// <seealso cref="TreePanel"/>
        public PointF ScrollPosition { get; protected set; }       
        /// <summary>
        /// Created graphics of the TreePanel.
        /// </summary>
        /// <seealso cref="TreePanel"/>
        private Graphics Graphics { get; set; }
        /// <summary>
        /// The constant that determines the margin of TreePanel, which must be left empty.
        /// </summary>
        private const int margin = 50;
        /// <summary>
        /// The panel in the main form, where the graph of the active sentence is drawn.
        /// </summary>
        private Panel TreePanel;
        /// <summary>
        /// The set of IVisitors that allow different word rendering based on the word type and current representation.
        /// </summary>
        private IDesigner designer;
        /// <summary>
        /// Creates a new visualiser which will use <paramref name="designer"/> to represent sentence on the <paramref name="treePanel"/>
        /// </summary>
        /// <param name="treePanel"> </param>
        /// <param name="designer">The designer which will be used to draw the sentence graph according to representation.</param>
        public Visualiser(Panel treePanel, IDesigner designer)
        {
            this.designer = designer;
            TreePanel = treePanel;
            ScrollPosition = treePanel.AutoScrollPosition;
            Graphics = treePanel.CreateGraphics();
        }

        /// <summary>
        /// Prepares the graphics of the <seealso cref="TreePanel"/>.
        /// </summary>
        /// <param name="graphSize">The size of the whole sentence graph. 
        /// If the width is 0, the size is not changed.</param>
        private void PrepareNewGraphics(SizeF graphSize)
        {
            // Graph width must be greather than 0.
            if (graphSize.IsEmpty)
            {
                graphSize.Width = TreePanel.AutoScrollMinSize.Width;
                graphSize.Height = TreePanel.AutoScrollMinSize.Height;
            }
            else
            {
                // Height is the sum of all the spaces between the levels and the margin.
                graphSize.Height = (int)graphSize.Height + margin;
            }
            // It is necessary to edit the minimum scroll size in the TreePanel
            TreePanel.AutoScrollMinSize = graphSize.ToSize();
            // We need to create the new graphics
            Graphics gNew = TreePanel.CreateGraphics();
            gNew.TranslateTransform(TreePanel.AutoScrollPosition.X, TreePanel.AutoScrollPosition.Y);
            gNew.Clear(Color.White);
            Graphics = gNew;
        }
        
        
        /// <summary>
        /// Method used to count new coordinates of the words and to visualise the sentence.
        /// </summary>
        /// <param name="sentence">The sentence which is going to be visualised.</param>
        public void NewGraphics(ISentence sentence)
        {
            int height = TreePanel.Height - margin;
            SizeF graphSize = designer.NewPoints(sentence, TreePanel.Width, height - margin, Graphics);
            // Prepares the empty graphics and translate it according to the AutoScroll position
            PrepareNewGraphics(graphSize);
            // Visualises the whole graph
            designer.Visualise(sentence, Graphics);
            ScrollPosition = TreePanel.AutoScrollPosition;
        }

        /// <summary>
        /// Method used mainly while scrolling the existing graph. The point positions are not counted again.
        /// </summary>
        /// <param name="sentence">The sentence whose graph is going to be scrolled.</param>
        public void ScrollNewGraphics(ISentence sentence )
        {
            // Prepares the empty graphics and translate it according to the AutoScroll position
            // If the graphSize.Width argument is 0, the size of the drawing space will not change
            PrepareNewGraphics(new SizeF(0,0));
            // Visualises the whole graph
            designer.Visualise(sentence, Graphics);
            ScrollPosition = TreePanel.AutoScrollPosition;
        }
       
        /// <summary>
        /// Method used to draw only one word point without revisualising all the graph.
        /// </summary>
        /// <param name="word">The word whose pint will be drawn.</param>
        public void DrawOneWord(IWord word)
        {
            // The deleted word id is set to null, will not be visualised.
            if (word.Id == null)
                return;
            // Draws the point of the word according to the representation and the type of the word.
            designer.DrawPoint(word, Graphics);
        }

        /// <summary>
        /// Modifies the <paramref name="basicWordInfo"/> so thai it contains the basic information about the word or sentence.
        /// </summary>
        /// <param name="basicWordInfo">
        /// The panel, on which the basic information are written. Should have at least 3 controls.
        /// </param>
        /// <param name="word">The word whose information are shown</param>
        /// <param name="sentence">In the case the word is root, the sent_id is shown instead of the word info.</param>
        /// <returns>true if the info was made visible; otherwise, false</returns>
        public bool ShowBasicInfo(Panel basicWordInfo, IWord word, ISentence sentence)
        {
            // Sent to the Basic info creator the borders which the panel with info can not intersect
            return BasicInfo(word, word.GetWordBasicInfo(sentence.Sent_id), basicWordInfo, Graphics, 
                TreePanel.Height - margin, TreePanel.Width - margin, TreePanel.AutoScrollPosition);
        }

        /// <summary>
        /// The method modifies <paramref name="basicWordInfo"/> by the information from <paramref name="values"/>.
        /// It counts the good position of the panel.
        /// </summary>
        /// <param name="word">The word on whose point the panel will be positioned</param>
        /// <param name="values">The array of the values which should be visualised in the panel</param>
        /// <param name="basicWordInfo">The panel which is modified and set to be visible</param>
        /// <param name="g">The graphics where we measure the width of the values in the array</param>
        /// <param name="heightForm">The height of the drawing space, the panel must end above</param>
        /// <param name="widthForm">The width of the drawing space, the panel can not intersect it</param>
        /// <param name="scroll">The actual scroll position of the drawing space</param>
        /// <returns>true if the info was made visible; otherwise, false</returns>
        private bool BasicInfo(IWord word, string[] values, Panel basicWordInfo, Graphics g, float heightForm, float widthForm, PointF scroll)
        {
            // There must be at least one value to show
            if (values.Length == 0) return false;
            // We must find out the width and the height of the panel
            float maxWidth = values.Max(x => g.MeasureString(x, Schema.BoldFont).Width);
            float maxHeight = g.MeasureString(values[0], Schema.BoldFont).Height;
            // We try to set the starting coordinates to the right down corner of the word point
            float startX = word.GetWordPoint().X + Schema.SizeOfPoint.Width + scroll.X;
            float startY = word.GetWordPoint().Y + Schema.SizeOfPoint.Height + scroll.Y;
            // We shift the starting coordinates so that the panel can fit into the view
            if (startY + Schema.SizeOfPoint.Height + (values.Length * maxHeight) > heightForm)
            {
                startY = word.GetWordPoint().Y + scroll.Y - (maxHeight * values.Length);
            }
            if (startX + maxWidth > widthForm)
            {
                startX = Math.Max(0, word.GetWordPoint().X + scroll.X - maxWidth);
            }
            basicWordInfo.Location = new Point((int)startX, (int)startY);
            // All values in the array are visualised (if there are enough controls in the panel)
            int i;
            for (i = 0; i < values.Length; i++)
            {
                if (basicWordInfo.Controls.Count <= i)
                    break;
                basicWordInfo.Controls[i].Text = values[i];
                basicWordInfo.Controls[i].Visible = true;
            }
            // All remaining controls are hidden
            for (int j = i; i < basicWordInfo.Controls.Count; ++i)
            {
                basicWordInfo.Controls[i].Visible = false;
            }
            basicWordInfo.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            basicWordInfo.AutoSize = true;
            basicWordInfo.Visible = true;
            return true;
        }

        /// <summary>
        /// Gets the position inside the drawing space.
        /// Returns the word whose drawn point contains the received position (or null) 
        /// </summary>
        /// <param name="sentence">Sentence in which the word is searched</param>
        /// <param name="origPoint">The point inside the drawing space</param>
        /// <returns>The word, whose drawn point contains the received position, null if no word point contains it.</returns>
        public IWord FindRightPoint(ISentence sentence, PointF origPoint)
        {
            // Changes the point according to current scroll position
            PointF point = new PointF(origPoint.X - TreePanel.AutoScrollPosition.X, origPoint.Y - TreePanel.AutoScrollPosition.Y);
            for (int i = 0; i < sentence.CountWords; i++)
            {
                IWord word = sentence.GetWord(i);
                // Returns exactly the information that we need - true if the point is inside word point
                if (word.GetWordPoint().MatchPoint(point, Schema.SizeOfPoint))
                    return word;
            }
            return null;
        }

        /// <summary>
        /// Gets the active word and tries to shift from it in the given direction (<paramref name="key"/>) in the tree.
        /// If the shift fails, returns the current active word.
        /// </summary>
        /// <param name="key">The arrow key, specifies the direction of shift</param>
        /// <param name="active">Current active word</param>
        /// <returns>
        /// The word to where the activity moved. 
        /// If the shift failed, returns the original active word.
        /// If the original ward is deleted, returns null.
        /// </returns>
        public IWord ShiftActive(Keys key, ISentence sentence, IWord active)
        {
            // The deleted word id is set to null, could not shift anwhere. Returns null
            if (active.Id == null)
                return null;
            // Get the shifted word according to representation and the type of the word
            return designer.ShiftActive(active,sentence,key);
        }

        /// <summary>
        /// Returns the context menu according to the representation and the type of the word.
        /// </summary>
        /// <param name="word">The word whose context menu should be returned</param>
        /// <param name="sentences">The set of sentences in whole file - to handle actions from the context menu</param>
        /// <param name="form">Commander with methods on the main form - to handle actions from the context menu</param>
        /// <returns></returns>
        public ContextMenuStrip GetContextMenu(IWord word, ListOfSentences sentences, CurrentState form)
        {
            // Gets the context menu strip according to representation and the type of the word
            return designer.GetContextMenu(word, sentences, form);
        }

        public void Visualise(ISentence sentence)
        {
            designer.Visualise(sentence, Graphics);
        }
    }
}