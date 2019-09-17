namespace view
{
    partial class worldView
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
            this.NameTextbox = new System.Windows.Forms.TextBox();
            this.AddressTextbox = new System.Windows.Forms.TextBox();
            this.nameLabel = new System.Windows.Forms.Label();
            this.DrawingPanel = new System.Windows.Forms.Label();
            this.connectButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // NameTextbox
            // 
            this.NameTextbox.Location = new System.Drawing.Point(93, 8);
            this.NameTextbox.Margin = new System.Windows.Forms.Padding(4);
            this.NameTextbox.Name = "NameTextbox";
            this.NameTextbox.Size = new System.Drawing.Size(152, 31);
            this.NameTextbox.TabIndex = 1;
            // 
            // AddressTextbox
            // 
            this.AddressTextbox.Location = new System.Drawing.Point(359, 8);
            this.AddressTextbox.Margin = new System.Windows.Forms.Padding(4);
            this.AddressTextbox.Name = "AddressTextbox";
            this.AddressTextbox.Size = new System.Drawing.Size(171, 31);
            this.AddressTextbox.TabIndex = 2;
            this.AddressTextbox.Text = "localhost";
            // 
            // nameLabel
            // 
            this.nameLabel.AutoSize = true;
            this.nameLabel.Location = new System.Drawing.Point(20, 15);
            this.nameLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(74, 25);
            this.nameLabel.TabIndex = 3;
            this.nameLabel.Text = "Name:";
            // 
            // DrawingPanel
            // 
            this.DrawingPanel.AutoSize = true;
            this.DrawingPanel.Location = new System.Drawing.Point(263, 15);
            this.DrawingPanel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.DrawingPanel.Name = "DrawingPanel";
            this.DrawingPanel.Size = new System.Drawing.Size(97, 25);
            this.DrawingPanel.TabIndex = 4;
            this.DrawingPanel.Text = "Address:";
            // 
            // connectButton
            // 
            this.connectButton.Location = new System.Drawing.Point(556, 6);
            this.connectButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(172, 36);
            this.connectButton.TabIndex = 5;
            this.connectButton.Text = "CONNECT";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            // 
            // worldView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1233, 818);
            this.Controls.Add(this.connectButton);
            this.Controls.Add(this.DrawingPanel);
            this.Controls.Add(this.nameLabel);
            this.Controls.Add(this.AddressTextbox);
            this.Controls.Add(this.NameTextbox);
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "worldView";
            this.Padding = new System.Windows.Forms.Padding(4);
            this.Text = "worldView";
            this.Load += new System.EventHandler(this.worldView_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.worldView_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.worldView_KeyUp);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox NameTextbox;
        private System.Windows.Forms.TextBox AddressTextbox;
        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.Label DrawingPanel;
        private System.Windows.Forms.Button connectButton;
    }
}