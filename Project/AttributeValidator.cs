using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ConlluVisualiser
{
    /// <summary>
    /// The validator that validates input into the table according to the CoNLL-U format.
    /// </summary>
    class ConlluValidator : IValidator
    {
        /// <summary>
        /// The list of the feats attributes that are used in the current info.
        /// </summary>
        private List<string> UsedFeats { get; } = new List<string>();
        /// <summary>
        /// The list of the misc attributes that are used in the current info.
        /// </summary>
        private List<string> UsedMisc { get; } = new List<string>();
        /// <summary>
        /// The list of the deps attributes that are used in the current info.
        /// </summary>
        private List<string> UsedDeps { get; } = new List<string>();
        /// <summary>
        /// The sentence that contains the word whose information are shown in the table.
        /// </summary>
        private ISentence Sentence { get; }
        /// <summary>
        /// Creates the validator to validate CoNLL-U format of attributes.
        /// </summary>
        /// <param name="sentence">The sentence that contains the word whose attributes are validated.</param>
        public ConlluValidator(ISentence sentence)
        {
            Sentence = sentence;
        }

        /// <summary>
        /// Saves the dictionary value as used not to duplicate it in the same attribute.
        /// Used during the initialization of the <seealso cref="WordFieldsForm"/> table.
        /// </summary>
        /// <param name="attribute">
        /// The attribute name. 
        /// Must be the name of <see cref="o
        /// o.Feats"/>, <see cref="WordInfo.Deps"/> or <see cref="WordInfo.Misc"/>.
        /// </param>
        /// <param name="property">The first part in pair before the delimiter.</param>
        public void AddUsed(string attribute, string property)
        {
            // Adds the property into the right list.
            switch (attribute)
            {
                case nameof(WordInfo.Feats):
                    UsedFeats.Add(property);
                    return;
                case nameof(WordInfo.Misc):
                    UsedMisc.Add(property);
                    return;
                case nameof(WordInfo.Deps):
                    UsedDeps.Add(property);
                    return;
            }
        }

        /// <summary>
        /// Makes readonly the id value. 
        /// If the word is the <seealso cref="MultiWord"/> or the <seealso cref="EmptyNodeWord"/>, 
        /// makes readonly the rows that have to stay empty.
        /// </summary>
        /// <param name="table">The table where the info is shown.</param>
        public virtual void MakeReadonly(DataGridView table)
        {
            // The cell with Id value is allways readonly.
            table[1, 0].ReadOnly = true;
            string id = (string)table[1, 0].Value;
            // If it is the id of the multiword token, all cells must rest empty except for the id, form, and misc.
            // The multiword token id is in the form of x-y.
            if (id.Split('-').Count() > 1)
            {
                foreach (DataGridViewRow row in table.Rows)
                {
                    if ((string)row.Cells[0].Value != nameof(WordInfo.Form) && 
                        (string)row.Cells[0].Value != nameof(WordInfo.Misc))
                        row.Cells[1].ReadOnly = true;
                }
            }
            // If the id is in the form of the empty node word id, the head and deprel must rest empty.
            // The empty node word id is in the form x.y.
            if (id.Split('.').Count() > 1)
            {
                foreach (DataGridViewRow row in table.Rows)
                {
                    if ((string)row.Cells[0].Value == nameof(WordInfo.Head) || 
                        (string)row.Cells[0].Value == nameof(WordInfo.Deprel))
                        row.Cells[1].ReadOnly = true;
                }
            }
        }

        /// <summary>
        /// Validates only the format of the fields. Does not checks the relations in the sentence.
        /// </summary>
        /// <param name="e">The event arguments</param>
        /// <param name="table">The table where the event originated.</param>
        /// <returns>True if the format is good; otherwise, false.</returns>
        protected bool ValidateFormat(DataGridViewCellValidatingEventArgs e, DataGridView table)
        {
            // The name of the attribute that is checked.
            string RowTitle = (string)table[0, e.RowIndex].Value;
            // Validates the feats form
            if (RowTitle == nameof(WordInfo.Feats))
            {
                return ValidateDictValue(e, UsedFeats, WordInfo.delimFeatsMisc, table);
            }
            // Validates the misc form
            else if (RowTitle == nameof(WordInfo.Misc))
            {
                return ValidateDictValue(e, UsedMisc, WordInfo.delimFeatsMisc, table);
            }
            // Another attributes do not have the specific form.
            return true;
        }

        /// <summary>
        /// Validates the relations in the sentence. Checks the values that modify the graph structure,
        /// so the structure rests valid.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        /// <param name="table">The table where the info is shown.</param>
        /// <returns>True if the changes are valid; otherwise, false.</returns>
        protected bool ValidateSentenceRelations(DataGridViewCellValidatingEventArgs e, DataGridView table)
        {
            string RowTitle = (string)table[0, e.RowIndex].Value;
            string id = (string)table[1, 0].Value;
            // Validates the head according to the id
            // Checks if the word with id from head could be a new parent of the word.
            if (RowTitle == nameof(WordInfo.Head))
            {
                return ValidateHead(e, id, table);
            }
            // Validates the value in Deps so the parents in Enhanced graph are valid.
            if (RowTitle == nameof(WordInfo.Deps))
            {
                return ValidateDeps(e, table, id);
            }
            // Another attributes do not change the structure.
            return true;
        }

        /// <summary>
        /// Validates the cell so it is valid and possible to save it to the CoNLL-U format.
        /// </summary>
        /// <param name="e">The event arguments</param>
        /// <param name="table">The table where the change were done.</param>
        public virtual void ValidateCell(DataGridViewCellValidatingEventArgs e, DataGridView table)
        {
            // Does not validate the ReadOnly values.
            if (table[e.ColumnIndex, e.RowIndex].ReadOnly)
            {
                return;
            }
            // If the format is not good, does not continue in vaidation.
            if(!ValidateFormat(e, table))
            {
                return;
            }
            // Validates the value according to the sentence relations, if the value can change the sentence structure.
            ValidateSentenceRelations(e, table);
        }

        /// <summary>
        /// Validates the Head attribute value. Ensures that the new parent can be really the parent of the word.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        /// <param name="id">The id of the current word.</param>
        /// <param name="table">The table of the current word.</param>
        /// <returns>True if the value is good; otherwise, false.</returns>
        private bool ValidateHead(DataGridViewCellValidatingEventArgs e, string id, DataGridView table)
        {
            string value = e.FormattedValue.ToString();
            // Gets the word and parent from the sentence.
            IWord parent = Sentence.GetWord(value);
            IWord word = Sentence.GetWord(id);
            // Checks if the new parent exists and if the word is not the parent of itself.
            // Checks if the new parent can be parent of this word according to the graph structure.
            if (parent != null && id == value && (parent is ITreeWord) && (word is ITreeWord) && (parent as ITreeWord).CanBeParentOf(word as ITreeWord))
            {
                return true;
            }
            // If something is wrong, cancels the validation.
            CancelValidation(e, table, message: "The value must be a valid ID of another word." +
                "The new head can not be a descendant of this word.");
            return false;
        }

        /// <summary>
        /// Validates the cell with dictionary pair value.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        /// <param name="used">The list of used properties of the current attribute.</param>
        /// <param name="delim">The delimiter that separates the pair property and value.</param>
        /// <param name="table">The table where the change has been done.</param>
        /// <returns>True if the value is in the good format; otherwise, false.</returns>
        private bool ValidateDictValue(DataGridViewCellValidatingEventArgs e, List<string> used, char delim,
            DataGridView table)
        {
            // If the new value is empty, removes the original value from the dictionary.
            if ((string)e.FormattedValue == "")
            {
                TryRemoveUsed(e, delim, used, table);
                return true;
            }
            // Checks if the value is in the format x=y.
            string[] splitted = ((string)e.FormattedValue).Split(delim);
            if (splitted.Length != 2)
            {
                // If the value has different format, cancels the change.
                CancelValidation(e, table, message: "The value must be in the format 'x" + delim + "value'.");
                return false;
            }
            // Tries to add new property and checks if it is not a duplicate.
            return TryAddUsed(e, splitted[0], delim, used,table);
        }

        /// <summary>
        /// Validates the format of the attribute Deps.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        /// <param name="table">The table where the change has been done.</param>
        /// <param name="id">The id of the current word.</param>
        /// <returns>True if the value is in the good format; otherwise, false.</returns>
        private bool ValidateDeps(DataGridViewCellValidatingEventArgs e, DataGridView table, string id)
        {
            // If the value is empty, removes the original property.
            if ((string)e.FormattedValue == "")
            {
                TryRemoveUsed(e, WordInfo.delimDeps, UsedDeps, table);
                return true;
            }
            // Finds the word and the parent in the sentence.
            string[] splitted = ((string)e.FormattedValue).Split(new[] { WordInfo.delimDeps }, 2);
            IWord parent = Sentence.GetWord(splitted[0]);
            IWord word = Sentence.GetWord(id);
            // If the form is not x:y, cancels the change.
            if (splitted.Length < 2)
            {
                CancelValidation(e, table, message: "The value must be in the form 'head" + WordInfo.delimDeps + "deprel'.");
                return false;
            }
            // If the parent can not be the parent of the word, cancels the change.
            else if (parent == null || parent == word)
            {
                CancelValidation(e, table, message: "The first part must be a valid ID of another word.");
                return false;
            }
            else
            {
                // Tries to add the property to used.
                return TryAddUsed(e, splitted[0], WordInfo.delimDeps, UsedDeps, table);
            }
        }

        /// <summary>
        /// Tries to add the property to the list of used properties of the attribute.
        /// Checks if there are not duplicities.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        /// <param name="newVal">The new property.</param>
        /// <param name="delim">The delimiter that separates the property and value of the attribute.</param>
        /// <param name="used">The list of used properties of the attribute.</param>
        /// <param name="table">The table where the change has been done.</param>
        /// <returns>True if the value is in the good format; otherwise, false.</returns>
        private bool TryAddUsed(DataGridViewCellValidatingEventArgs e, string newVal, char delim, List<string> used, 
            DataGridView table)
        {
            string[] splittedOrig = ((string)table[1, e.RowIndex].Value).Split(delim);
            if (used.Contains(newVal) && splittedOrig[0] != newVal)
            {
                CancelValidation(e, table, message: "This value is already set.");
                return false;
            }
            else used.Add(newVal);
            return true;
        }

        /// <summary>
        /// Removes the original property from the list with used properties of the attribute.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        /// <param name="delim">The delimiter that separates the property and value of the attribute.</param>
        /// <param name="used">The list of used properties of the attribute.</param>
        /// <param name="table">The table where the change has been done.</param>
        private void TryRemoveUsed(DataGridViewCellValidatingEventArgs e, char delim, List<string> used,
            DataGridView table)
        {
            string[] splittedO = ((string)table[1, e.RowIndex].Value).Split(delim);
            // If the original value was in the good format and noempty, removes it.
            if (splittedO.Length >= 2)
                used.Remove(splittedO[0]);
        }

        /// <summary>
        /// Cancels the editation of the cell. Informs the user by the message box with <paramref name="message"/>.
        /// </summary>
        /// <param name="e">The event arguments</param>
        /// <param name="table">The table where the change has been done.</param>
        /// <param name="message">The text of the message that is shown to the user.</param>
        private void CancelValidation(DataGridViewCellValidatingEventArgs e, DataGridView table, string message)
        {           
            e.Cancel = true;
            table.CancelEdit();
            MessageBox.Show(message);
        }
    }
}