namespace AgentDungeonGenerator
{
    partial class Display
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
            this.roomChanceLabel = new System.Windows.Forms.Label();
            this.directionChanceLabel = new System.Windows.Forms.Label();
            this.mainCanvas = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.mainCanvas)).BeginInit();
            this.SuspendLayout();
            // 
            // roomChanceLabel
            // 
            this.roomChanceLabel.AutoSize = true;
            this.roomChanceLabel.Location = new System.Drawing.Point(12, 35);
            this.roomChanceLabel.Name = "roomChanceLabel";
            this.roomChanceLabel.Size = new System.Drawing.Size(46, 13);
            this.roomChanceLabel.TabIndex = 0;
            this.roomChanceLabel.Text = "Room %";
            // 
            // directionChanceLabel
            // 
            this.directionChanceLabel.AutoSize = true;
            this.directionChanceLabel.Location = new System.Drawing.Point(12, 12);
            this.directionChanceLabel.Name = "directionChanceLabel";
            this.directionChanceLabel.Size = new System.Drawing.Size(60, 13);
            this.directionChanceLabel.TabIndex = 1;
            this.directionChanceLabel.Text = "Direction %";
            // 
            // mainCanvas
            // 
            this.mainCanvas.Location = new System.Drawing.Point(12, 12);
            this.mainCanvas.Name = "mainCanvas";
            this.mainCanvas.Size = new System.Drawing.Size(621, 577);
            this.mainCanvas.TabIndex = 2;
            this.mainCanvas.TabStop = false;
            this.mainCanvas.Paint += new System.Windows.Forms.PaintEventHandler(this.mainCanvas_Paint);
            // 
            // Display
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(645, 601);
            this.Controls.Add(this.directionChanceLabel);
            this.Controls.Add(this.roomChanceLabel);
            this.Controls.Add(this.mainCanvas);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Display";
            this.ShowIcon = false;
            this.Text = "Dungeon Generator";
            this.Shown += new System.EventHandler(this.Display_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.mainCanvas)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label roomChanceLabel;
        private System.Windows.Forms.Label directionChanceLabel;
        private System.Windows.Forms.PictureBox mainCanvas;
    }
}

