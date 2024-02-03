using System.Drawing;

namespace ConlluVisualiser
{
    public class WordPoint
    {
        /// <summary>
        /// The X-coordinate of the word point.
        /// </summary>
        public float X { get => point.X; }

        /// <summary>
        /// The Y-coordinate of the word point.
        /// </summary>
        public float Y { get => point.Y; }

        /// <summary>
        /// The point where the word is visualised in the graph.
        /// </summary>
        private PointF point;
        /// <summary>
        /// Returns the point on the drawing space where the word is visualised.
        /// </summary>
        public PointF GetPoint()
        {
            return point;
        }
        
        /// <summary>
        /// Sets the point on the drawing space where the word will be visualised.
        /// </summary>
        public void SetPoint(PointF value)
        {
            point = value;
        }

        /// <summary>
        /// The floor in the graph where this word is situated.
        /// </summary>
        public virtual int Level { get; set; }

        /// <summary>
        /// Specifies if the given <paramref name="searchedPoint"/> is situated inside the word point.
        /// </summary>
        /// <param name="searchedPoint">The point which is searched inside the word point space.</param>
        /// <param name="sizeOfPoint">The size of the space where the searched point can lie.</param>
        /// <returns>true if the <paramref name="searchedPoint"/>lies in the word point space; otherwise, false.</returns>
        public bool MatchPoint(PointF searchedPoint, Size sizeOfPoint)
        {
            // The word is not visualised in the current representation - no point can lie inside it.
            if (GetPoint() == PointF.Empty)
                return false;
            // Tests if the point lies in the range.
            if (searchedPoint.X > GetPoint().X && searchedPoint.X < GetPoint().X + sizeOfPoint.Width &&
                searchedPoint.Y > GetPoint().Y && searchedPoint.Y < GetPoint().Y + sizeOfPoint.Height)
            {
                return true;
            }
            return false;
        }
    }
}
