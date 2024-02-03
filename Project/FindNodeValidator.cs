using System.Windows.Forms;
using ConlluVisualiser;

namespace Finder
{
    /// <summary>
    /// Validates the input attribute values in the <seealso cref="FindSentenceBox"/> table of attributes.
    /// Allows user to input all values.
    /// </summary>
    class FindNodeValidator : IValidator
    {
        /// <summary>
        /// Does not do anything, because the duplicities are allowed in the <seealso cref="FindSentenceBox"/>.
        /// </summary>
        /// <param name="attribute">The attribute name.</param>
        /// <param name="property">The property name.</param>
        public void AddUsed(string attribute, string property) { }

        /// <summary>
        /// Makes readonly the cells in <paramref name="table"/> that should not be changed. 
        /// In this case does not make readonly any cell because all attribute values can be modified.
        /// </summary>
        /// <param name="table">The table where the cells are made readonly.</param>
        public void MakeReadonly(DataGridView table) { }

        /// <summary>
        /// Validates the cell input. All values are allowed.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        /// <param name="table">The table whose cell was changed.</param>
        public void ValidateCell(DataGridViewCellValidatingEventArgs e, DataGridView table) { }
        
    }
}
