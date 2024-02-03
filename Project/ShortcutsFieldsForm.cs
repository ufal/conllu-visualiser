using System.Collections.Generic;
using System.Windows.Forms;

namespace ConlluVisualiser
{
    /// <summary>
    /// The form that is used to define and insert shortcut keys.
    /// </summary>
    public class ShortcutsFieldsForm : WordFieldsForm
    {
        /// <summary>
        /// Returns the defined shortcut key.
        /// </summary>
        /// <returns>The shortcut key that runs the action.</returns>
        public Keys GetShortcutKeys()
        {
            return ShortcutKeys;
        }

        /// <summary>
        /// The defined action.
        /// </summary>
        private List<ItemInWordInfo> Result { get; set; }

        /// <summary>
        /// The keys that must be pressed to run actions.
        /// </summary>
        private Keys ShortcutKeys { get; set; }

        /// <summary>
        /// Returns the list of actions that are connected with the key.
        /// </summary>
        /// <returns>The list of actions that will start after pressig of <seealso cref="ShortcutKeys"/>.</returns>
        public List<ItemInWordInfo> GetResult()
        {
            return Result;
        }

        /// <summary>
        /// Creates the form with empty word attributes to define a shortcut key.
        /// </summary>
        public ShortcutsFieldsForm() : base(new WordInfo(), new ShortcutValidator())
        {
            // Removes the row with the Deps attribute.
            table.Rows.RemoveAt(8);
            // Removes the row with the Head attribute.
            table.Rows.RemoveAt(6);
            // Removes the row with the ID attribute.
            table.Rows.RemoveAt(0);
            // Makes the textbox to insert the shortcut key visible.
            shortcutPanel.Visible = true;
            // Adds the extra column with check box cells to the table
            var new_col = new DataGridViewCheckBoxColumn
            {
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            };
            table.Columns.Add(new_col);
            // Adds the hint text to the third table.
            foreach (DataGridViewRow row in table.Rows)
            {
                row.Cells[2].ToolTipText = "Owerwrite original not empty value?";
            }
            // Initializes the submit button.
            InitButton(text: "Set shortcut keys", handlers: (sender, e) => { SaveState(); });
        }

        /// <summary>
        /// Processes the command key.
        /// </summary>
        /// <param name="msg">Message to process</param>
        /// <param name="keyData">Represents the key to process.</param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // If the textbox to insert the shortcut key is not the active control, do the default action.
            if (ActiveControl != shortcutTextBox)
            {
                return base.ProcessCmdKey(ref msg, keyData);
            }
            // The shortcut key must contain the Control, Shift or Alt key.
            if (keyData.HasFlag(Keys.Control) || keyData.HasFlag(Keys.Shift) || keyData.HasFlag(Keys.Alt))
            {
                // Fills the string form of keys into the textbox.
                shortcutTextBox.Text = keyData.ToString();
                ShortcutKeys = keyData;
            }
            else
            {
                MessageBox.Show(text: "Please, use CONTROL, SHIFT or ALT key");
            }
            return true;
        }

        /// <summary>
        /// Saves the state of the table.
        /// </summary>
        private void SaveState()
        {
            // The shortcut key can not be empty.
            if (shortcutTextBox.Text == "")
            {
                MessageBox.Show(text: "You must define the shortcut key.");
                return;
            }
            Result = new List<ItemInWordInfo>();
            foreach (DataGridViewRow row in table.Rows)
            {
                string name = (string)row.Cells[0].Value;
                string value = (string)row.Cells[1].Value;
                bool overwrite = (row.Cells[2].Value != null && (bool)row.Cells[2].Value);
                // Does not save the empty lines.
                if (value == "" && overwrite == false)
                    continue;
                // Creates the item that represents the line.
                ItemInWordInfo item = new ItemInWordInfo(name, value, overwrite);
                // Saves the line with action.
                Result.Add(item);
            }
            // Closes the window.
            DialogResult = DialogResult.OK;
            Close();
        }
    }

    /// <summary>
    /// The class that holds one shortcut key action.
    /// </summary>
    public class ItemInWordInfo
    {
        /// <summary>
        /// The name of the attribute.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// The value of the attribute.
        /// </summary>
        public string Value { get; }
        /// <summary>
        /// Specifies whether the <seealso cref="Value"/> should overwrite the original value, if there is some.
        /// </summary>
        public bool Overwrite { get; }
        /// <summary>
        /// Creates the action item.
        /// </summary>
        /// <param name="name">The name of the attribute that can be changed by this action.</param>
        /// <param name="value">The new value which can the attribute have after application of the action.</param>
        /// <param name="overwrite">The sign if the original value should be overwritten by the new or not.</param>
        public ItemInWordInfo(string name, string value, bool overwrite)
        {
            Name = name;
            Value = value;
            Overwrite = overwrite;
        }
    }
}
