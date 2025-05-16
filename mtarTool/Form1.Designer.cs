namespace mtarTool
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Button btnBrowseMtar;
        private System.Windows.Forms.TextBox txtFilePath;
        private System.Windows.Forms.ListBox listBoxEntries;
        private System.Windows.Forms.Button btnBrowseHex;
        private System.Windows.Forms.TextBox txtStatus;
        private System.Windows.Forms.Label lblFilePath;
        private System.Windows.Forms.Label lblEntries;
        private System.Windows.Forms.Label lblStatus;

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
            this.btnBrowseMtar = new System.Windows.Forms.Button();
            this.txtFilePath = new System.Windows.Forms.TextBox();
            this.listBoxEntries = new System.Windows.Forms.ListBox();
            this.btnBrowseHex = new System.Windows.Forms.Button();
            this.txtStatus = new System.Windows.Forms.TextBox();
            this.lblFilePath = new System.Windows.Forms.Label();
            this.lblEntries = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnBrowseMtar
            // 
            this.btnBrowseMtar.Location = new System.Drawing.Point(400, 30);
            this.btnBrowseMtar.Name = "btnBrowseMtar";
            this.btnBrowseMtar.Size = new System.Drawing.Size(100, 25);
            this.btnBrowseMtar.TabIndex = 0;
            this.btnBrowseMtar.Text = "Browse .mtar";
            this.btnBrowseMtar.UseVisualStyleBackColor = true;
            this.btnBrowseMtar.Click += new System.EventHandler(this.btnBrowseMtar_Click);
            // 
            // txtFilePath
            // 
            this.txtFilePath.Location = new System.Drawing.Point(120, 32);
            this.txtFilePath.Name = "txtFilePath";
            this.txtFilePath.Size = new System.Drawing.Size(270, 20);
            this.txtFilePath.TabIndex = 1;
            // 
            // listBoxEntries
            // 
            this.listBoxEntries.FormattingEnabled = true;
            this.listBoxEntries.Location = new System.Drawing.Point(15, 90);
            this.listBoxEntries.Name = "listBoxEntries";
            this.listBoxEntries.Size = new System.Drawing.Size(485, 160);
            this.listBoxEntries.TabIndex = 2;
            // 
            // btnBrowseHex
            // 
            this.btnBrowseHex.Location = new System.Drawing.Point(170, 270);
            this.btnBrowseHex.Name = "btnBrowseHex";
            this.btnBrowseHex.Size = new System.Drawing.Size(150, 30);
            this.btnBrowseHex.TabIndex = 3;
            this.btnBrowseHex.Text = "Import Anim";
            this.btnBrowseHex.UseVisualStyleBackColor = true;
            this.btnBrowseHex.Click += new System.EventHandler(this.btnBrowseHex_Click);
            // 
            // txtStatus
            // 
            this.txtStatus.Location = new System.Drawing.Point(15, 340);
            this.txtStatus.Multiline = true;
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtStatus.Size = new System.Drawing.Size(485, 150);
            this.txtStatus.TabIndex = 4;
            // 
            // lblFilePath
            // 
            this.lblFilePath.AutoSize = true;
            this.lblFilePath.Location = new System.Drawing.Point(12, 35);
            this.lblFilePath.Name = "lblFilePath";
            this.lblFilePath.Size = new System.Drawing.Size(102, 13);
            this.lblFilePath.TabIndex = 5;
            this.lblFilePath.Text = "Selected .mtar File:";
            // 
            // lblEntries
            // 
            this.lblEntries.AutoSize = true;
            this.lblEntries.Location = new System.Drawing.Point(12, 70);
            this.lblEntries.Name = "lblEntries";
            this.lblEntries.Size = new System.Drawing.Size(77, 13);
            this.lblEntries.TabIndex = 6;
            this.lblEntries.Text = "DataTable List:";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(12, 320);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(40, 13);
            this.lblStatus.TabIndex = 7;
            this.lblStatus.Text = "Status:";
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(524, 511);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.lblEntries);
            this.Controls.Add(this.lblFilePath);
            this.Controls.Add(this.txtStatus);
            this.Controls.Add(this.btnBrowseHex);
            this.Controls.Add(this.listBoxEntries);
            this.Controls.Add(this.txtFilePath);
            this.Controls.Add(this.btnBrowseMtar);
            this.Name = "Form1";
            this.Text = ".mtar Editor";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}