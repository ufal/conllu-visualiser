using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
namespace ConlluVisualiser
{
    /// <summary>
    /// The windows form object, which visualises the table with the word attributes.
    /// Optionally validates the inserted values.
    /// </summary>
    public partial class WordFieldsForm : Form
    {
        /// <summary>
        /// The validator that validates the cells according to their attribute types.
        /// </summary>
        private IValidator Validator { get; }
       
        /// <summary>
        /// Creates the Form with attribute table of the <paramref name="info"/>.
        /// The changes are validated according to the <paramref name="validator"/>.
        /// </summary>
        /// <param name="info">The word info that is visualised in the table.</param>
        /// <param name="validator">The validator that checks the changes.</param>
        public WordFieldsForm(WordInfo info, IValidator validator)
        {
            InitializeComponent();
            Validator = validator;
            // Adds the rows to the table according to the info values.
            AddRows(info);
        }

        public WordFieldsForm()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Initializes the button text and eventhandlers.
        /// </summary>
        /// <param name="text">The text that is written on the submit button.</param>
        /// <param name="handlers">The set of handlers that handle the click on the submit button.</param>
        public void InitButton(string text, params EventHandler[] handlers)
        {
            button.Text = text;
            foreach (EventHandler handler in handlers)
            {
                button.Click += handler;
            }
        }

        /// <summary>
        /// Adds the rows into the table according to the contain of <paramref name="Info"/>.
        /// </summary>
        /// <param name="Info">The dataset of the attributes.</param>
        protected void AddRows(WordInfo Info)
        {
            table.Rows.Clear();
            // Goes over all properties in word info.
            foreach(KeyValuePair<string, string> property in Info.Properties())
            {
                // Adds row for each property/value pair in the dictionary records
                if (property.Key == nameof(WordInfo.Feats) || property.Key == nameof(WordInfo.Misc))
                {
                    // Creates the rows with pairs.
                     AddDictRows(property, WordInfo.delimFeatsMisc);
                }
                // Adds the row for each Deps record.
                else if (property.Key == nameof(WordInfo.Deps))
                {
                    AddDictRows(property, WordInfo.delimDeps);
                }
                else
                {
                    // Adds the row for all other attributes.
                    string[] row = { property.Key, property.Value };
                    table.Rows.Add(row);
                }
            }
            // According to the validator makes some cells readonly.
            Validator.MakeReadonly(table);
            table.Visible = true;
        }

        /// <summary>
        /// Validates the cell value according to the <seealso cref="Validator"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">The data from the event.</param>
        public virtual void ValidateCell(object sender, DataGridViewCellValidatingEventArgs e)
        {
            // Sends the event to the validator which handles the change.
            Validator.ValidateCell(e, table);
        }

        /// <summary>
        /// Adds the rows from the dictionary to the table. Adds the row for each pair in dictionary to the table.
        /// </summary>
        /// <param name="value">The attribute with value.</param>
        /// <param name="delim">The delimiter that separates the property and value in pairs.</param>
        private void AddDictRows(KeyValuePair<string, string> value, char delim)
        {
            string[] arr = value.Value.Split(WordInfo.delimPairs);
            // Separates the property and value in each pair and saves it as a row.
            foreach (var pair in arr)
            {
                // Empty properties are not added.
                if (pair == "")
                    continue;
                // Splits the pair by the delimiter.
                string[] oneProp = pair.Split(new[] { delim }, 2);
                // Adds the property name to the validator to check the duplicities.
                Validator.AddUsed(value.Key, oneProp[0]);
                // Adds the row to the table.
                table.Rows.Add(new string[] { value.Key, pair });
            }
            // Adds the empty row under the noempty rows to allow user to insert another value.
            table.Rows.Add(new string[]{ value.Key, ""});
        }
        
        /// <summary>
        /// Handles the adding or removing the dictionary value in the table. 
        /// Ensures that after the rows with dictionary pairs there is allways right one empty line.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">The event data.</param>
        private void ChangedValue(object sender, DataGridViewCellEventArgs e)
        {
            // Does not do anything in the case the index is greather than one.
            // In the case of modification of the third column of shorcut key table, the next row is not added.
            if (e.RowIndex < 0 || e.ColumnIndex < 0 || e.ColumnIndex > 1) return;
            // Cares only about the dictionary rows.
            if ((string)table[0, e.RowIndex].Value == nameof(WordInfo.Feats) || 
                (string)table[0, e.RowIndex].Value == nameof(WordInfo.Misc) ||
                (string)table[0, e.RowIndex].Value == nameof(WordInfo.Deps))
            {
                // If the row is empty and it is not the last empty row, it is removed.
                if ((string)table[e.ColumnIndex, e.RowIndex].Value == "" || table[e.ColumnIndex, e.RowIndex].Value == null)
                {
                    // If the title of the next row is the same as the title of the actual row,
                    // the actual row is not the last and will be removed.
                    if (e.RowIndex + 1 < table.RowCount &&
                        (string)table[0, e.RowIndex + 1].Value == (string)table[0, e.RowIndex].Value)
                    {
                        table.Rows.RemoveAt(e.RowIndex);
                    }
                }
                // If the row is the last in the table or if it is the last of the dictionary rows,
                // the next row will be added.
                else if (e.RowIndex + 1 == table.RowCount ||
                    (string)table[0, e.RowIndex + 1].Value != (string)table[0, e.RowIndex].Value)
                {
                    // Creates the new row with the same title.
                    string[] row = { (string)table[0, e.RowIndex].Value, "" };
                    // Inserts it under the changed line (as the last of the dictionary rows)
                    table.Rows.Insert(e.RowIndex + 1, row);
                }
            }
        }
        
        /// <summary>
        /// Returns the state of the table after all changes validated by <seealso cref="Validator"/>.
        /// </summary>
        /// <returns>The dictionary with attributes and their values.</returns>
        public Dictionary<string, string> GetState()
        {
            Dictionary<string, string> list = new Dictionary<string, string>();
            // Adds the record for each attribute. If there are more lines for one attribute (dictionary attributes),
            // makes the string form from it (joins the pairs by the delimiter)
            foreach(var row in table.Rows.Cast<DataGridViewRow>())
            {
                if (!list.ContainsKey((string)row.Cells[0].Value))
                    list[(string)row.Cells[0].Value] = (string)row.Cells[1].Value;
                else
                    list[(string)row.Cells[0].Value] += (WordInfo.delimPairs + (string)row.Cells[1].Value);
            }
            return list;
        }
    }
}