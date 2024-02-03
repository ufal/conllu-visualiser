using System.Drawing;
using ConlluVisualiser;

namespace GraphVisualiser
{
    /// <summary>
    /// Class to determine the form size of the word in the graph according to its type in the basic representation.
    /// </summary>
    class FormSizeBasic : IVisitor
    {
        /// <summary>
        /// The graphics schema of the graph.
        /// </summary>
        protected readonly GraphicsSchema Schema;

        /// <summary>
        /// Creates the form size getter that search for the form size according to the <paramref name="schema"/>.
        /// </summary>
        /// <param name="schema">The graphics schema of the graph.</param>
        public FormSizeBasic(GraphicsSchema schema)
        {
            Schema = schema;
        }

        /// <summary>
        /// Returns the form size of the word. 
        /// </summary>
        /// <param name="word">The word whose form size is searched for.</param>
        /// <param name="g">The graphics where the form size is counted.</param>
        /// <returns><seealso cref="SizeF"/>: size of the written form.</returns>
        public virtual object Visit(BasicWord word, Graphics g)
        {
            return GetFormSize(word.Info.Form, g, Schema.BoldFont);
           
        }

        /// <summary>
        /// Returns the size of the <paramref name="form"/> parameter. The minimum width is the width of the word point.
        /// </summary>
        /// <param name="form">The text of the form whose size is counted.</param>
        /// <param name="g">The graphics where the form size is counted.</param>
        /// <param name="font">The font which is used to write the word form.</param>
        /// <returns></returns>
        public SizeF GetFormSize(string form, Graphics g, Font font)
        {
            SizeF size = g.MeasureString(form, font);
            // If the word is too short, the minimum width will be used.
            if (size.Width < Schema.SizeOfPoint.Width)
                size = new SizeF(Schema.SizeOfPoint.Width, size.Height);
            return size;
        }

        /// <summary>
        /// Returns the empty size because the multiword is not written in the graph.
        /// </summary>
        /// <param name="multiWord">The word whose form size is searched for.</param>
        /// <param name="g">The graphics where the form size is counted.</param>
        /// <returns><seealso cref="SizeF"/>: empty size.</returns>
        public virtual object Visit(MultiWord multiWord, Graphics g)
        {
            return new SizeF(0,0);
        }

        /// <summary>
        /// Returns the empty size because the empty node word is not written in the basic representation of the graph.
        /// </summary>
        /// <param name="multiWord">The word whose form size is searched for.</param>
        /// <param name="g">The graphics where the form size is counted.</param>
        /// <returns><seealso cref="SizeF"/>: empty size.</returns>
        public virtual object Visit(EmptyNodeWord emptyWord, Graphics g)
        {
            return new SizeF(0,0);
        }

    }

    /// <summary>
    /// Class to determine the form size of the word in the graph according to its type in the enhanced representation.
    /// </summary>
    class FormSizeEnhanced : FormSizeBasic, IVisitor
    {
        /// <summary>
        /// Creates the formsize getter according to the <paramref name="schema"/>.
        /// </summary>
        /// <param name="schema"></param>
        public FormSizeEnhanced(GraphicsSchema schema) : base(schema)
        {
        }

        /// <summary>
        /// Returns the form size of the word. 
        /// </summary>
        /// <param name="word">The word whose form size is searched for.</param>
        /// <param name="g">The graphics where the form size is counted.</param>
        /// <returns><seealso cref="SizeF"/>: size of the written form.</returns>
        public override object Visit(BasicWord word, Graphics g)
        {
            return base.Visit(word, g);
        }

        /// <summary>
        /// Returns the form size of the word form. 
        /// </summary>
        /// <param name="word">The word whose form size is searched for.</param>
        /// <param name="g">The graphics where the form size is counted.</param>
        /// <returns><seealso cref="SizeF"/>: size of the written form.</returns>
        public override object Visit(EmptyNodeWord emptyWord, Graphics g)
        {
            return GetFormSize(emptyWord.Info.Form, g, Schema.BoldFont);
        }

    }
}