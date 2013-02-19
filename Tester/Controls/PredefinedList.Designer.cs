namespace Tester.Controls
{
    partial class PredefinedList
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
            this.rbRotate = new System.Windows.Forms.RadioButton();
            this.rbHorizSlide = new System.Windows.Forms.RadioButton();
            this.rbVertSlide = new System.Windows.Forms.RadioButton();
            this.rbScale = new System.Windows.Forms.RadioButton();
            this.rbScaleAndRotate = new System.Windows.Forms.RadioButton();
            this.rbHorizSlideAndRotate = new System.Windows.Forms.RadioButton();
            this.rbScaleAndHorizSlide = new System.Windows.Forms.RadioButton();
            this.rbTransparent = new System.Windows.Forms.RadioButton();
            this.rbLeaf = new System.Windows.Forms.RadioButton();
            this.rbMosaic = new System.Windows.Forms.RadioButton();
            this.rbVertBlind = new System.Windows.Forms.RadioButton();
            this.rbHorizBlind = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.rbParticles = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // rbRotate
            // 
            this.rbRotate.AutoSize = true;
            this.rbRotate.Location = new System.Drawing.Point(15, 26);
            this.rbRotate.Name = "rbRotate";
            this.rbRotate.Size = new System.Drawing.Size(57, 17);
            this.rbRotate.TabIndex = 0;
            this.rbRotate.TabStop = true;
            this.rbRotate.Text = "Rotate";
            this.rbRotate.UseVisualStyleBackColor = true;
            this.rbRotate.CheckedChanged += new System.EventHandler(this.rbHorizBlind_CheckedChanged);
            // 
            // rbHorizSlide
            // 
            this.rbHorizSlide.AutoSize = true;
            this.rbHorizSlide.Location = new System.Drawing.Point(15, 49);
            this.rbHorizSlide.Name = "rbHorizSlide";
            this.rbHorizSlide.Size = new System.Drawing.Size(72, 17);
            this.rbHorizSlide.TabIndex = 1;
            this.rbHorizSlide.TabStop = true;
            this.rbHorizSlide.Text = "HorizSlide";
            this.rbHorizSlide.UseVisualStyleBackColor = true;
            this.rbHorizSlide.CheckedChanged += new System.EventHandler(this.rbHorizBlind_CheckedChanged);
            // 
            // rbVertSlide
            // 
            this.rbVertSlide.AutoSize = true;
            this.rbVertSlide.Location = new System.Drawing.Point(15, 72);
            this.rbVertSlide.Name = "rbVertSlide";
            this.rbVertSlide.Size = new System.Drawing.Size(67, 17);
            this.rbVertSlide.TabIndex = 2;
            this.rbVertSlide.TabStop = true;
            this.rbVertSlide.Text = "VertSlide";
            this.rbVertSlide.UseVisualStyleBackColor = true;
            this.rbVertSlide.CheckedChanged += new System.EventHandler(this.rbHorizBlind_CheckedChanged);
            // 
            // rbScale
            // 
            this.rbScale.AutoSize = true;
            this.rbScale.Location = new System.Drawing.Point(15, 95);
            this.rbScale.Name = "rbScale";
            this.rbScale.Size = new System.Drawing.Size(52, 17);
            this.rbScale.TabIndex = 3;
            this.rbScale.TabStop = true;
            this.rbScale.Text = "Scale";
            this.rbScale.UseVisualStyleBackColor = true;
            this.rbScale.CheckedChanged += new System.EventHandler(this.rbHorizBlind_CheckedChanged);
            // 
            // rbScaleAndRotate
            // 
            this.rbScaleAndRotate.AutoSize = true;
            this.rbScaleAndRotate.Location = new System.Drawing.Point(15, 118);
            this.rbScaleAndRotate.Name = "rbScaleAndRotate";
            this.rbScaleAndRotate.Size = new System.Drawing.Size(103, 17);
            this.rbScaleAndRotate.TabIndex = 4;
            this.rbScaleAndRotate.TabStop = true;
            this.rbScaleAndRotate.Text = "ScaleAndRotate";
            this.rbScaleAndRotate.UseVisualStyleBackColor = true;
            this.rbScaleAndRotate.CheckedChanged += new System.EventHandler(this.rbHorizBlind_CheckedChanged);
            // 
            // rbHorizSlideAndRotate
            // 
            this.rbHorizSlideAndRotate.AutoSize = true;
            this.rbHorizSlideAndRotate.Location = new System.Drawing.Point(15, 141);
            this.rbHorizSlideAndRotate.Name = "rbHorizSlideAndRotate";
            this.rbHorizSlideAndRotate.Size = new System.Drawing.Size(123, 17);
            this.rbHorizSlideAndRotate.TabIndex = 5;
            this.rbHorizSlideAndRotate.TabStop = true;
            this.rbHorizSlideAndRotate.Text = "HorizSlideAndRotate";
            this.rbHorizSlideAndRotate.UseVisualStyleBackColor = true;
            this.rbHorizSlideAndRotate.CheckedChanged += new System.EventHandler(this.rbHorizBlind_CheckedChanged);
            // 
            // rbScaleAndHorizSlide
            // 
            this.rbScaleAndHorizSlide.AutoSize = true;
            this.rbScaleAndHorizSlide.Location = new System.Drawing.Point(15, 164);
            this.rbScaleAndHorizSlide.Name = "rbScaleAndHorizSlide";
            this.rbScaleAndHorizSlide.Size = new System.Drawing.Size(118, 17);
            this.rbScaleAndHorizSlide.TabIndex = 6;
            this.rbScaleAndHorizSlide.TabStop = true;
            this.rbScaleAndHorizSlide.Text = "ScaleAndHorizSlide";
            this.rbScaleAndHorizSlide.UseVisualStyleBackColor = true;
            this.rbScaleAndHorizSlide.CheckedChanged += new System.EventHandler(this.rbHorizBlind_CheckedChanged);
            // 
            // rbTransparent
            // 
            this.rbTransparent.AutoSize = true;
            this.rbTransparent.Location = new System.Drawing.Point(15, 187);
            this.rbTransparent.Name = "rbTransparent";
            this.rbTransparent.Size = new System.Drawing.Size(82, 17);
            this.rbTransparent.TabIndex = 7;
            this.rbTransparent.TabStop = true;
            this.rbTransparent.Text = "Transparent";
            this.rbTransparent.UseVisualStyleBackColor = true;
            this.rbTransparent.CheckedChanged += new System.EventHandler(this.rbHorizBlind_CheckedChanged);
            // 
            // rbLeaf
            // 
            this.rbLeaf.AutoSize = true;
            this.rbLeaf.Location = new System.Drawing.Point(15, 210);
            this.rbLeaf.Name = "rbLeaf";
            this.rbLeaf.Size = new System.Drawing.Size(46, 17);
            this.rbLeaf.TabIndex = 8;
            this.rbLeaf.TabStop = true;
            this.rbLeaf.Text = "Leaf";
            this.rbLeaf.UseVisualStyleBackColor = true;
            this.rbLeaf.CheckedChanged += new System.EventHandler(this.rbHorizBlind_CheckedChanged);
            // 
            // rbMosaic
            // 
            this.rbMosaic.AutoSize = true;
            this.rbMosaic.Location = new System.Drawing.Point(15, 233);
            this.rbMosaic.Name = "rbMosaic";
            this.rbMosaic.Size = new System.Drawing.Size(59, 17);
            this.rbMosaic.TabIndex = 9;
            this.rbMosaic.TabStop = true;
            this.rbMosaic.Text = "Mosaic";
            this.rbMosaic.UseVisualStyleBackColor = true;
            this.rbMosaic.CheckedChanged += new System.EventHandler(this.rbHorizBlind_CheckedChanged);
            // 
            // rbVertBlind
            // 
            this.rbVertBlind.AutoSize = true;
            this.rbVertBlind.Location = new System.Drawing.Point(15, 302);
            this.rbVertBlind.Name = "rbVertBlind";
            this.rbVertBlind.Size = new System.Drawing.Size(67, 17);
            this.rbVertBlind.TabIndex = 10;
            this.rbVertBlind.TabStop = true;
            this.rbVertBlind.Text = "VertBlind";
            this.rbVertBlind.UseVisualStyleBackColor = true;
            this.rbVertBlind.CheckedChanged += new System.EventHandler(this.rbHorizBlind_CheckedChanged);
            // 
            // rbHorizBlind
            // 
            this.rbHorizBlind.AutoSize = true;
            this.rbHorizBlind.Location = new System.Drawing.Point(15, 279);
            this.rbHorizBlind.Name = "rbHorizBlind";
            this.rbHorizBlind.Size = new System.Drawing.Size(72, 17);
            this.rbHorizBlind.TabIndex = 11;
            this.rbHorizBlind.TabStop = true;
            this.rbHorizBlind.Text = "HorizBlind";
            this.rbHorizBlind.UseVisualStyleBackColor = true;
            this.rbHorizBlind.CheckedChanged += new System.EventHandler(this.rbHorizBlind_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(167, 20);
            this.label1.TabIndex = 13;
            this.label1.Text = "Predefined animations";
            // 
            // rbParticles
            // 
            this.rbParticles.AutoSize = true;
            this.rbParticles.Location = new System.Drawing.Point(15, 256);
            this.rbParticles.Name = "rbParticles";
            this.rbParticles.Size = new System.Drawing.Size(65, 17);
            this.rbParticles.TabIndex = 14;
            this.rbParticles.TabStop = true;
            this.rbParticles.Text = "Particles";
            this.rbParticles.UseVisualStyleBackColor = true;
            this.rbParticles.CheckedChanged += new System.EventHandler(this.rbHorizBlind_CheckedChanged);
            // 
            // PredefinedList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.rbParticles);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.rbHorizBlind);
            this.Controls.Add(this.rbVertBlind);
            this.Controls.Add(this.rbMosaic);
            this.Controls.Add(this.rbLeaf);
            this.Controls.Add(this.rbTransparent);
            this.Controls.Add(this.rbScaleAndHorizSlide);
            this.Controls.Add(this.rbHorizSlideAndRotate);
            this.Controls.Add(this.rbScaleAndRotate);
            this.Controls.Add(this.rbScale);
            this.Controls.Add(this.rbVertSlide);
            this.Controls.Add(this.rbHorizSlide);
            this.Controls.Add(this.rbRotate);
            this.Name = "PredefinedList";
            this.Size = new System.Drawing.Size(175, 341);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton rbRotate;
        private System.Windows.Forms.RadioButton rbHorizSlide;
        private System.Windows.Forms.RadioButton rbVertSlide;
        private System.Windows.Forms.RadioButton rbScale;
        private System.Windows.Forms.RadioButton rbScaleAndRotate;
        private System.Windows.Forms.RadioButton rbHorizSlideAndRotate;
        private System.Windows.Forms.RadioButton rbScaleAndHorizSlide;
        private System.Windows.Forms.RadioButton rbTransparent;
        private System.Windows.Forms.RadioButton rbLeaf;
        private System.Windows.Forms.RadioButton rbMosaic;
        private System.Windows.Forms.RadioButton rbVertBlind;
        private System.Windows.Forms.RadioButton rbHorizBlind;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton rbParticles;
    }
}
