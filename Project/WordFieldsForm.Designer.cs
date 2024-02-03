namespace ConlluVisualiser
{
    partial class WordFieldsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        protected void InitializeComponent()
        {
            this.table = new System.Windows.Forms.DataGridView();
            this.headers = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.values = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.button = new System.Windows.Forms.Button();
            this.shortcutPanel = new System.Windows.Forms.Panel();
            this.shortcutTextBox = new System.Windows.Forms.TextBox();
            this.labelShortcut = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.table)).BeginInit();
            this.shortcutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // table
            // 
            this.table.AllowUserToAddRows = false;
            this.table.AllowUserToDeleteRows = false;
            this.table.AllowUserToResizeColumns = false;
            this.table.AllowUserToResizeRows = false;
            this.table.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.table.ColumnHeadersVisible = false;
            this.table.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.headers,
            this.values});
            this.table.Dock = System.Windows.Forms.DockStyle.Fill;
            this.table.Location = new System.Drawing.Point(0, 0);
            this.table.MaximumSize = new System.Drawing.Size(0, 280);
            this.table.Name = "table";
            this.table.RowHeadersVisible = false;
            this.table.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.table.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.table.Size = new System.Drawing.Size(184, 193);
            this.table.TabIndex = 0;
            this.table.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.ValidateCell);
            this.table.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.ChangedValue);
            // 
            // headers
            // 
            this.headers.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.headers.HeaderText = "Property";
            this.headers.Name = "headers";
            this.headers.ReadOnly = true;
            this.headers.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.headers.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.headers.Width = 5;
            // 
            // values
            // 
            this.values.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.values.HeaderText = "";
            this.values.Name = "values";
            this.values.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.values.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // button
            // 
            this.button.AutoSize = true;
            this.button.BackColor = System.Drawing.Color.White;
            this.button.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.button.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.button.Location = new System.Drawing.Point(0, 218);
            this.button.Name = "button";
            this.button.Size = new System.Drawing.Size(184, 23);
            this.button.TabIndex = 1;
            this.button.UseVisualStyleBackColor = false;
            // 
            // shortcutPanel
            // 
            this.shortcutPanel.Controls.Add(this.shortcutTextBox);
            this.shortcutPanel.Controls.Add(this.labelShortcut);
            this.shortcutPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.shortcutPanel.Location = new System.Drawing.Point(0, 193);
            this.shortcutPanel.Name = "shortcutPanel";
            this.shortcutPanel.Size = new System.Drawing.Size(184, 25);
            this.shortcutPanel.TabIndex = 2;
            this.shortcutPanel.Visible = false;
            // 
            // textBox1
            // 
            this.shortcutTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.shortcutTextBox.Location = new System.Drawing.Point(105, 2);
            this.shortcutTextBox.Name = "textBox1";
            this.shortcutTextBox.Size = new System.Drawing.Size(79, 20);
            this.shortcutTextBox.TabIndex = 1;
            // 
            // labelShortcut
            // 
            this.labelShortcut.AutoSize = true;
            this.labelShortcut.Location = new System.Drawing.Point(5, 6);
            this.labelShortcut.Name = "labelShortcut";
            this.labelShortcut.Size = new System.Drawing.Size(102, 13);
            this.labelShortcut.TabIndex = 0;
            this.labelShortcut.Text = "Press shortcut keys:";
            // 
            // WordFieldsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(184, 241);
            this.Controls.Add(this.table);
            this.Controls.Add(this.shortcutPanel);
            this.Controls.Add(this.button);
            this.MaximumSize = new System.Drawing.Size(300, 280);
            this.MinimumSize = new System.Drawing.Size(200, 39);
            this.Name = "WordFieldsForm";
            this.Text = "WordFieldsForm";
            ((System.ComponentModel.ISupportInitialize)(this.table)).EndInit();
            this.shortcutPanel.ResumeLayout(false);
            this.shortcutPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected System.Windows.Forms.DataGridView table;
        private System.Windows.Forms.Button button;
        private System.Windows.Forms.DataGridViewTextBoxColumn headers;
        private System.Windows.Forms.DataGridViewTextBoxColumn values;
        protected System.Windows.Forms.Panel shortcutPanel;
        protected System.Windows.Forms.TextBox shortcutTextBox;
        private System.Windows.Forms.Label labelShortcut;
    }
}