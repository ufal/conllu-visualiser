namespace ConlluVisualiser
{
    partial class AppForm
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
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AppForm));
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.conLLUSentenceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.simpleTextSentenceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findSentenceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findNodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.insertToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.insertNewSentenceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.representationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.basicRepresentationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.enhancedRepresentationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.shortToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.defineShortupKeysToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importShortupKeysToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportShortupKeysToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.leftPanel = new System.Windows.Forms.Panel();
            this.pagePanel = new System.Windows.Forms.Panel();
            this.filePanel = new System.Windows.Forms.Panel();
            this.basicWordInfo = new System.Windows.Forms.Panel();
            this.Lemma = new System.Windows.Forms.Label();
            this.Cpostag = new System.Windows.Forms.Label();
            this.Deprel = new System.Windows.Forms.Label();
            this.TreePanel = new System.Windows.Forms.Panel();
            this.menuStrip1.SuspendLayout();
            this.leftPanel.SuspendLayout();
            this.basicWordInfo.SuspendLayout();
            this.TreePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(10, 29);
            this.label1.Margin = new System.Windows.Forms.Padding(10, 0, 3, 0);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.label1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label1.Size = new System.Drawing.Size(838, 38);
            this.label1.TabIndex = 3;
            this.label1.Text = "Sentence:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.CausesValidation = false;
            this.textBox1.Location = new System.Drawing.Point(93, 32);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(755, 32);
            this.textBox1.TabIndex = 5;
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.menuStrip1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.menuStrip1.GripMargin = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this.menuStrip1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.findToolStripMenuItem,
            this.insertToolStripMenuItem,
            this.representationToolStripMenuItem,
            this.shortToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(10, 5);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(838, 24);
            this.menuStrip1.TabIndex = 6;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.newFileToolStripMenuItem,
            this.toolStripSeparator,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripSeparator1,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Padding = new System.Windows.Forms.Padding(0, 0, 4, 0);
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(34, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.conLLUSentenceToolStripMenuItem,
            this.simpleTextSentenceToolStripMenuItem});
            this.openToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("openToolStripMenuItem.Image")));
            this.openToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.openToolStripMenuItem.Text = "&Open";
            // 
            // conLLUSentenceToolStripMenuItem
            // 
            this.conLLUSentenceToolStripMenuItem.Name = "conLLUSentenceToolStripMenuItem";
            this.conLLUSentenceToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.conLLUSentenceToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.conLLUSentenceToolStripMenuItem.Text = "CoNLL-U format";
            this.conLLUSentenceToolStripMenuItem.Click += new System.EventHandler(this.ConLLUSentenceToolStripMenuItem_Click);
            // 
            // simpleTextSentenceToolStripMenuItem
            // 
            this.simpleTextSentenceToolStripMenuItem.Name = "simpleTextSentenceToolStripMenuItem";
            this.simpleTextSentenceToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.simpleTextSentenceToolStripMenuItem.Text = "Simple text format";
            this.simpleTextSentenceToolStripMenuItem.Click += new System.EventHandler(this.LoadSimpleSentencesToolStripMenuItem_Click);
            // 
            // newFileToolStripMenuItem
            // 
            this.newFileToolStripMenuItem.Name = "newFileToolStripMenuItem";
            this.newFileToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.newFileToolStripMenuItem.Text = "New File";
            this.newFileToolStripMenuItem.Click += new System.EventHandler(this.NewFileToolStripMenuItem_Click);
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(140, 6);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveToolStripMenuItem.Image")));
            this.saveToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.SaveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.saveAsToolStripMenuItem.Text = "Save &As";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.SaveAsToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(140, 6);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(140, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
            // 
            // findToolStripMenuItem
            // 
            this.findToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.findSentenceToolStripMenuItem,
            this.findNodeToolStripMenuItem});
            this.findToolStripMenuItem.Name = "findToolStripMenuItem";
            this.findToolStripMenuItem.Size = new System.Drawing.Size(42, 20);
            this.findToolStripMenuItem.Text = "Find";
            // 
            // findSentenceToolStripMenuItem
            // 
            this.findSentenceToolStripMenuItem.Name = "findSentenceToolStripMenuItem";
            this.findSentenceToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.findSentenceToolStripMenuItem.Text = "Find sentence";
            this.findSentenceToolStripMenuItem.Click += new System.EventHandler(this.FindSentenceToolStripMenuItem_Click);
            // 
            // findNodeToolStripMenuItem
            // 
            this.findNodeToolStripMenuItem.Name = "findNodeToolStripMenuItem";
            this.findNodeToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.findNodeToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.findNodeToolStripMenuItem.Text = "&Find node";
            this.findNodeToolStripMenuItem.Click += new System.EventHandler(this.FindNodeToolStripMenuItem_Click);
            // 
            // insertToolStripMenuItem
            // 
            this.insertToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.insertNewSentenceToolStripMenuItem});
            this.insertToolStripMenuItem.Name = "insertToolStripMenuItem";
            this.insertToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.insertToolStripMenuItem.Text = "Insert";
            // 
            // insertNewSentenceToolStripMenuItem
            // 
            this.insertNewSentenceToolStripMenuItem.Name = "insertNewSentenceToolStripMenuItem";
            this.insertNewSentenceToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.insertNewSentenceToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.insertNewSentenceToolStripMenuItem.Text = "Insert new sentence";
            this.insertNewSentenceToolStripMenuItem.Click += new System.EventHandler(this.InsertNewSentenceToolStripMenuItem_Click);
            // 
            // representationToolStripMenuItem
            // 
            this.representationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.basicRepresentationToolStripMenuItem,
            this.enhancedRepresentationToolStripMenuItem});
            this.representationToolStripMenuItem.Name = "representationToolStripMenuItem";
            this.representationToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.B)));
            this.representationToolStripMenuItem.Size = new System.Drawing.Size(105, 20);
            this.representationToolStripMenuItem.Text = "Representation";
            // 
            // basicRepresentationToolStripMenuItem
            // 
            this.basicRepresentationToolStripMenuItem.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.basicRepresentationToolStripMenuItem.Name = "basicRepresentationToolStripMenuItem";
            this.basicRepresentationToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.B)));
            this.basicRepresentationToolStripMenuItem.Size = new System.Drawing.Size(254, 22);
            this.basicRepresentationToolStripMenuItem.Text = "Basic representation";
            this.basicRepresentationToolStripMenuItem.Click += new System.EventHandler(this.BasicRepresentationToolStripMenuItem_Click);
            // 
            // enhancedRepresentationToolStripMenuItem
            // 
            this.enhancedRepresentationToolStripMenuItem.Name = "enhancedRepresentationToolStripMenuItem";
            this.enhancedRepresentationToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
            this.enhancedRepresentationToolStripMenuItem.Size = new System.Drawing.Size(254, 22);
            this.enhancedRepresentationToolStripMenuItem.Text = "Enhanced representation";
            this.enhancedRepresentationToolStripMenuItem.Click += new System.EventHandler(this.EnhancedRepresentationToolStripMenuItem_Click);
            // 
            // shortToolStripMenuItem
            // 
            this.shortToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.defineShortupKeysToolStripMenuItem,
            this.importShortupKeysToolStripMenuItem,
            this.exportShortupKeysToolStripMenuItem});
            this.shortToolStripMenuItem.Name = "shortToolStripMenuItem";
            this.shortToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
            this.shortToolStripMenuItem.Text = "Tools";
            // 
            // defineShortupKeysToolStripMenuItem
            // 
            this.defineShortupKeysToolStripMenuItem.Name = "defineShortupKeysToolStripMenuItem";
            this.defineShortupKeysToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.defineShortupKeysToolStripMenuItem.Text = "Define shortcut keys";
            this.defineShortupKeysToolStripMenuItem.Click += new System.EventHandler(this.DefineShortupKeysToolStripMenuItem_Click);
            // 
            // importShortupKeysToolStripMenuItem
            // 
            this.importShortupKeysToolStripMenuItem.Name = "importShortupKeysToolStripMenuItem";
            this.importShortupKeysToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.importShortupKeysToolStripMenuItem.Text = "Import shortcut keys";
            this.importShortupKeysToolStripMenuItem.Click += new System.EventHandler(this.ImportShortupKeysToolStripMenuItem_Click);
            // 
            // exportShortupKeysToolStripMenuItem
            // 
            this.exportShortupKeysToolStripMenuItem.Name = "exportShortupKeysToolStripMenuItem";
            this.exportShortupKeysToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.exportShortupKeysToolStripMenuItem.Text = "Export shortcut keys";
            this.exportShortupKeysToolStripMenuItem.Click += new System.EventHandler(this.ExportShortupKeysToolStripMenuItem_Click);
            // 
            // leftPanel
            // 
            this.leftPanel.AutoSize = true;
            this.leftPanel.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.leftPanel.Controls.Add(this.pagePanel);
            this.leftPanel.Controls.Add(this.filePanel);
            this.leftPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.leftPanel.Location = new System.Drawing.Point(10, 67);
            this.leftPanel.Name = "leftPanel";
            this.leftPanel.Size = new System.Drawing.Size(143, 326);
            this.leftPanel.TabIndex = 11;
            // 
            // pagePanel
            // 
            this.pagePanel.AutoScroll = true;
            this.pagePanel.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.pagePanel.Location = new System.Drawing.Point(0, 0);
            this.pagePanel.Name = "pagePanel";
            this.pagePanel.Size = new System.Drawing.Size(140, 35);
            this.pagePanel.TabIndex = 12;
            this.pagePanel.TabStop = true;
            // 
            // filePanel
            // 
            this.filePanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.filePanel.AutoScroll = true;
            this.filePanel.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.filePanel.Location = new System.Drawing.Point(0, 35);
            this.filePanel.Name = "filePanel";
            this.filePanel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.filePanel.Size = new System.Drawing.Size(140, 291);
            this.filePanel.TabIndex = 11;
            this.filePanel.TabStop = true;
            // 
            // basicWordInfo
            // 
            this.basicWordInfo.Controls.Add(this.Lemma);
            this.basicWordInfo.Controls.Add(this.Cpostag);
            this.basicWordInfo.Controls.Add(this.Deprel);
            this.basicWordInfo.Location = new System.Drawing.Point(113, 141);
            this.basicWordInfo.Name = "basicWordInfo";
            this.basicWordInfo.Size = new System.Drawing.Size(113, 65);
            this.basicWordInfo.TabIndex = 9;
            this.basicWordInfo.Visible = false;
            // 
            // Lemma
            // 
            this.Lemma.AutoSize = true;
            this.Lemma.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.Lemma.Location = new System.Drawing.Point(4, 6);
            this.Lemma.Name = "Lemma";
            this.Lemma.Size = new System.Drawing.Size(0, 16);
            this.Lemma.TabIndex = 3;
            // 
            // Cpostag
            // 
            this.Cpostag.AutoSize = true;
            this.Cpostag.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.Cpostag.Location = new System.Drawing.Point(3, 44);
            this.Cpostag.Name = "Cpostag";
            this.Cpostag.Size = new System.Drawing.Size(0, 16);
            this.Cpostag.TabIndex = 2;
            // 
            // Deprel
            // 
            this.Deprel.AutoSize = true;
            this.Deprel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.Deprel.Location = new System.Drawing.Point(3, 25);
            this.Deprel.Name = "Deprel";
            this.Deprel.Size = new System.Drawing.Size(0, 16);
            this.Deprel.TabIndex = 1;
            // 
            // TreePanel
            // 
            this.TreePanel.AutoScroll = true;
            this.TreePanel.AutoScrollMinSize = new System.Drawing.Size(200, 100);
            this.TreePanel.AutoSize = true;
            this.TreePanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.TreePanel.BackColor = System.Drawing.Color.White;
            this.TreePanel.Controls.Add(this.basicWordInfo);
            this.TreePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TreePanel.Location = new System.Drawing.Point(153, 67);
            this.TreePanel.Name = "TreePanel";
            this.TreePanel.Size = new System.Drawing.Size(695, 326);
            this.TreePanel.TabIndex = 11;
            this.TreePanel.TabStop = true;
            this.TreePanel.Paint += new System.Windows.Forms.PaintEventHandler(this.TreePanel_Paint);
            this.TreePanel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TreePanel_MouseClick);
            this.TreePanel.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.TreePanel_MouseDoubleClick);
            this.TreePanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TreePanel_MouseDown);
            this.TreePanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.TreePanel_MouseMove);
            this.TreePanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.TreePanel_MouseUp);
            // 
            // AppForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.ClientSize = new System.Drawing.Size(858, 403);
            this.Controls.Add(this.TreePanel);
            this.Controls.Add(this.leftPanel);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.menuStrip1);
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(250, 200);
            this.Name = "AppForm";
            this.Padding = new System.Windows.Forms.Padding(10, 5, 10, 10);
            this.Text = "Sentence editor";
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.leftPanel.ResumeLayout(false);
            this.basicWordInfo.ResumeLayout(false);
            this.basicWordInfo.PerformLayout();
            this.TreePanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem conLLUSentenceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem simpleTextSentenceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findSentenceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findNodeToolStripMenuItem;
        private System.Windows.Forms.Panel leftPanel;
        private System.Windows.Forms.Panel filePanel;
        private System.Windows.Forms.Panel pagePanel;
        private System.Windows.Forms.Panel basicWordInfo;
        private System.Windows.Forms.Label Cpostag;
        private System.Windows.Forms.Label Deprel;
        private System.Windows.Forms.Panel TreePanel;
        private System.Windows.Forms.ToolStripMenuItem shortToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem defineShortupKeysToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importShortupKeysToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportShortupKeysToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem representationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem basicRepresentationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem enhancedRepresentationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem insertToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem insertNewSentenceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newFileToolStripMenuItem;
        private System.Windows.Forms.Label Lemma;
    }
}

