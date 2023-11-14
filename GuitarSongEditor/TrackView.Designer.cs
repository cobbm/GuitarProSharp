namespace GuitarSongEditor
{
    partial class TrackView
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.instrumentLabel = new System.Windows.Forms.Label();
            this.trackIndexLabel = new System.Windows.Forms.Label();
            this.trackIconPictureBox = new System.Windows.Forms.PictureBox();
            this.trackColorPanel = new System.Windows.Forms.PictureBox();
            this.hideToggle = new System.Windows.Forms.CheckBox();
            this.muteToggle = new System.Windows.Forms.CheckBox();
            this.soloToggle = new System.Windows.Forms.CheckBox();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.trackBar2 = new System.Windows.Forms.TrackBar();
            ((System.ComponentModel.ISupportInitialize)(this.trackIconPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackColorPanel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar2)).BeginInit();
            this.SuspendLayout();
            // 
            // instrumentLabel
            // 
            this.instrumentLabel.Dock = System.Windows.Forms.DockStyle.Left;
            this.instrumentLabel.Location = new System.Drawing.Point(57, 0);
            this.instrumentLabel.Name = "instrumentLabel";
            this.instrumentLabel.Size = new System.Drawing.Size(125, 24);
            this.instrumentLabel.TabIndex = 7;
            this.instrumentLabel.Text = "Instrument";
            this.instrumentLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // trackIndexLabel
            // 
            this.trackIndexLabel.Dock = System.Windows.Forms.DockStyle.Left;
            this.trackIndexLabel.ForeColor = System.Drawing.Color.DimGray;
            this.trackIndexLabel.Location = new System.Drawing.Point(33, 0);
            this.trackIndexLabel.Name = "trackIndexLabel";
            this.trackIndexLabel.Size = new System.Drawing.Size(24, 24);
            this.trackIndexLabel.TabIndex = 6;
            this.trackIndexLabel.Text = "0";
            this.trackIndexLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // trackIconPictureBox
            // 
            this.trackIconPictureBox.Dock = System.Windows.Forms.DockStyle.Left;
            this.trackIconPictureBox.Location = new System.Drawing.Point(5, 0);
            this.trackIconPictureBox.Name = "trackIconPictureBox";
            this.trackIconPictureBox.Size = new System.Drawing.Size(28, 24);
            this.trackIconPictureBox.TabIndex = 5;
            this.trackIconPictureBox.TabStop = false;
            // 
            // trackColorPanel
            // 
            this.trackColorPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.trackColorPanel.Location = new System.Drawing.Point(0, 0);
            this.trackColorPanel.Name = "trackColorPanel";
            this.trackColorPanel.Size = new System.Drawing.Size(5, 24);
            this.trackColorPanel.TabIndex = 4;
            this.trackColorPanel.TabStop = false;
            // 
            // hideToggle
            // 
            this.hideToggle.Appearance = System.Windows.Forms.Appearance.Button;
            this.hideToggle.Dock = System.Windows.Forms.DockStyle.Left;
            this.hideToggle.Location = new System.Drawing.Point(182, 0);
            this.hideToggle.Name = "hideToggle";
            this.hideToggle.Size = new System.Drawing.Size(44, 24);
            this.hideToggle.TabIndex = 8;
            this.hideToggle.Text = "Hide";
            this.hideToggle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.hideToggle.UseVisualStyleBackColor = true;
            // 
            // muteToggle
            // 
            this.muteToggle.Appearance = System.Windows.Forms.Appearance.Button;
            this.muteToggle.Dock = System.Windows.Forms.DockStyle.Left;
            this.muteToggle.Location = new System.Drawing.Point(226, 0);
            this.muteToggle.Name = "muteToggle";
            this.muteToggle.Size = new System.Drawing.Size(44, 24);
            this.muteToggle.TabIndex = 9;
            this.muteToggle.Text = "Mute";
            this.muteToggle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.muteToggle.UseVisualStyleBackColor = true;
            // 
            // soloToggle
            // 
            this.soloToggle.Appearance = System.Windows.Forms.Appearance.Button;
            this.soloToggle.Dock = System.Windows.Forms.DockStyle.Left;
            this.soloToggle.Location = new System.Drawing.Point(270, 0);
            this.soloToggle.Name = "soloToggle";
            this.soloToggle.Size = new System.Drawing.Size(44, 24);
            this.soloToggle.TabIndex = 10;
            this.soloToggle.Text = "Solo";
            this.soloToggle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.soloToggle.UseVisualStyleBackColor = true;
            // 
            // trackBar1
            // 
            this.trackBar1.Dock = System.Windows.Forms.DockStyle.Right;
            this.trackBar1.LargeChange = 20;
            this.trackBar1.Location = new System.Drawing.Point(639, 0);
            this.trackBar1.Maximum = 100;
            this.trackBar1.Minimum = -100;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(50, 24);
            this.trackBar1.SmallChange = 10;
            this.trackBar1.TabIndex = 11;
            this.trackBar1.TickFrequency = 25;
            // 
            // trackBar2
            // 
            this.trackBar2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trackBar2.LargeChange = 50;
            this.trackBar2.Location = new System.Drawing.Point(314, 0);
            this.trackBar2.Maximum = 100;
            this.trackBar2.Name = "trackBar2";
            this.trackBar2.Size = new System.Drawing.Size(325, 24);
            this.trackBar2.SmallChange = 10;
            this.trackBar2.TabIndex = 12;
            this.trackBar2.TickFrequency = 10;
            // 
            // TrackView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.trackBar2);
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.soloToggle);
            this.Controls.Add(this.muteToggle);
            this.Controls.Add(this.hideToggle);
            this.Controls.Add(this.instrumentLabel);
            this.Controls.Add(this.trackIndexLabel);
            this.Controls.Add(this.trackIconPictureBox);
            this.Controls.Add(this.trackColorPanel);
            this.Name = "TrackView";
            this.Size = new System.Drawing.Size(689, 24);
            ((System.ComponentModel.ISupportInitialize)(this.trackIconPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackColorPanel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label instrumentLabel;
        private Label trackIndexLabel;
        private PictureBox trackIconPictureBox;
        private PictureBox trackColorPanel;
        private CheckBox hideToggle;
        private CheckBox muteToggle;
        private CheckBox soloToggle;
        private TrackBar trackBar1;
        private TrackBar trackBar2;
    }
}
