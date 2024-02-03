using System.Windows.Forms;

namespace ConlluVisualiser
{
    /// <summary>
    /// The mean to validate the cell value in the table with word attributes.
    /// </summary>
    public interface IValidator
    {
        /// <summary>
        /// Tells the validator about the used property not to allow duplicities.
        /// </summary>
        /// <param name="attribute">The name of the attribute.</param>
        /// <param name="property">The property of one pair in the attribute.</param>
        void AddUsed(string attribute, string property);
        /// <summary>
        /// Validates the cell in the table.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        /// <param name="table">The table where the change has been done.</param>
        void ValidateCell( DataGridViewCellValidatingEventArgs e, DataGridView table);
        /// <summary>
        /// Makes some cells in the table readonly.
        /// </summary>
        /// <param name="table">The table which is changing.</param>
        void MakeReadonly(DataGridView table);
    }

    /// <summary>
    /// Tha validator that is used to validate the values during the definition of the shortcut key.
    /// </summary>
    class ShortcutValidator : ConlluValidator, IValidator
    {
        /// <summary>
        ///  Creates the shortcut key validator with no sentence.
        /// </summary>
        public ShortcutValidator() : base(null) { }
        /// <summary>
        /// Validates the format of the changed cell in the table.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        /// <param name="table">The table where the change has been done.</param>
        public override void ValidateCell(DataGridViewCellValidatingEventArgs e, DataGridView table)
        {
            // Does not validates the format if the cell is readonly.
            // Does not validate the format if the third column was changed (there is only a checkbox).
            if (table[e.ColumnIndex, e.RowIndex].ReadOnly ||e.ColumnIndex >= 2)
                return;
            // Validates a format of the changed cell.
            ValidateFormat(e, table);
        }

        /// <summary>
        /// Does not make any cells readonly.
        /// </summary>
        /// <param name="table">The table with the attributes.</param>
        public override void MakeReadonly(DataGridView table)
        {
            return;
        }
    }
}
