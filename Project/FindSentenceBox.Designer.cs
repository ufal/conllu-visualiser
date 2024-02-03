﻿namespace Finder
{ 
    partial class FindSentenceBox
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
            this.flowPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.legendId = new System.Windows.Forms.Label();
            this.idBox = new System.Windows.Forms.TextBox();
            this.LegendSent = new System.Windows.Forms.Label();
            this.sentBox = new System.Windows.Forms.TextBox();
            this.OKButton = new System.Windows.Forms.Button();
            this.flowPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowPanel
            // 
            this.flowPanel.AutoSize = true;
            this.flowPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowPanel.BackColor = System.Drawing.SystemColors.Control;
            this.flowPanel.Controls.Add(this.legendId);
            this.flowPanel.Controls.Add(this.idBox);
            this.flowPanel.Controls.Add(this.LegendSent);
            this.flowPanel.Controls.Add(this.sentBox);
            this.flowPanel.Controls.Add(this.OKButton);
            this.flowPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowPanel.Location = new System.Drawing.Point(12, 12);
            this.flowPanel.Name = "flowPanel";
            this.flowPanel.Size = new System.Drawing.Size(287, 119);
            this.flowPanel.TabIndex = 6;
            // 
            // legendId
            // 
            this.legendId.AutoSize = true;
            this.legendId.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.legendId.Location = new System.Drawing.Point(3, 3);
            this.legendId.Margin = new System.Windows.Forms.Padding(3);
            this.legendId.Name = "legendId";
            this.legendId.Size = new System.Drawing.Size(192, 13);
            this.legendId.TabIndex = 2;
            this.legendId.Text = "Search according to sentence id";
            // 
            // idBox
            // 
            this.idBox.Location = new System.Drawing.Point(3, 22);
            this.idBox.Name = "idBox";
            this.idBox.Size = new System.Drawing.Size(262, 20);
            this.idBox.TabIndex = 0;
            // 
            // LegendSent
            // 
            this.LegendSent.AutoSize = true;
            this.LegendSent.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.LegendSent.Location = new System.Drawing.Point(3, 48);
            this.LegendSent.Margin = new System.Windows.Forms.Padding(3);
            this.LegendSent.Name = "LegendSent";
            this.LegendSent.Size = new System.Drawing.Size(281, 13);
            this.LegendSent.TabIndex = 4;
            this.LegendSent.Text = "Enter the regular expression to find a sentence: ";
            // 
            // sentBox
            // 
            this.sentBox.Location = new System.Drawing.Point(3, 67);
            this.sentBox.Name = "sentBox";
            this.sentBox.Size = new System.Drawing.Size(262, 20);
            this.sentBox.TabIndex = 3;
            // 
            // OKButton
            // 
            this.OKButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OKButton.AutoSize = true;
            this.OKButton.Location = new System.Drawing.Point(200, 93);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(84, 23);
            this.OKButton.TabIndex = 6;
            this.OKButton.Text = "OK";
            this.OKButton.UseVisualStyleBackColor = true;
            this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
            // 
            // FindSentenceBox
            // 
            this.AcceptButton = this.OKButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(309, 139);
            this.Controls.Add(this.flowPanel);
            this.Name = "FindSentenceBox";
            this.Text = "FindSentenceBox";
            this.flowPanel.ResumeLayout(false);
            this.flowPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowPanel;
        private System.Windows.Forms.Label legendId;
        private System.Windows.Forms.TextBox idBox;
        private System.Windows.Forms.Label LegendSent;
        private System.Windows.Forms.TextBox sentBox;
        private System.Windows.Forms.Button OKButton;
    }
}