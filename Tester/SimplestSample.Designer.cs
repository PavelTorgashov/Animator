namespace Tester
{
    partial class SimplestSample
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
            this.components = new System.ComponentModel.Container();
            AnimatorNS.Animation animation1 = new AnimatorNS.Animation();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SimplestSample));
            this.pb1 = new System.Windows.Forms.PictureBox();
            this.btHide = new System.Windows.Forms.Button();
            this.btShow = new System.Windows.Forms.Button();
            this.animator = new AnimatorNS.Animator(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pb1)).BeginInit();
            this.SuspendLayout();
            // 
            // pb1
            // 
            this.pb1.Image = global::Tester.Properties.Resources.cat_and_flowers;
            this.pb1.Location = new System.Drawing.Point(46, 32);
            this.pb1.Name = "pb1";
            this.pb1.Size = new System.Drawing.Size(200, 150);
            this.pb1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pb1.TabIndex = 0;
            this.pb1.TabStop = false;
            // 
            // btHide
            // 
            this.btHide.Location = new System.Drawing.Point(49, 217);
            this.btHide.Name = "btHide";
            this.btHide.Size = new System.Drawing.Size(75, 23);
            this.btHide.TabIndex = 1;
            this.btHide.Text = "Hide";
            this.btHide.UseVisualStyleBackColor = true;
            this.btHide.Click += new System.EventHandler(this.btHide_Click);
            // 
            // btShow
            // 
            this.btShow.Location = new System.Drawing.Point(160, 217);
            this.btShow.Name = "btShow";
            this.btShow.Size = new System.Drawing.Size(75, 23);
            this.btShow.TabIndex = 2;
            this.btShow.Text = "Show";
            this.btShow.UseVisualStyleBackColor = true;
            this.btShow.Click += new System.EventHandler(this.btShow_Click);
            // 
            // animator
            // 
            this.animator.AnimationType = AnimatorNS.AnimationType.Mosaic;
            this.animator.Cursor = null;
            animation1.AnimateOnlyDifferences = true;
            animation1.BlindCoeff = ((System.Drawing.PointF)(resources.GetObject("animation1.BlindCoeff")));
            animation1.LeafCoeff = 0F;
            animation1.MaxTime = 1F;
            animation1.MinTime = 0F;
            animation1.MosaicCoeff = ((System.Drawing.PointF)(resources.GetObject("animation1.MosaicCoeff")));
            animation1.MosaicShift = ((System.Drawing.PointF)(resources.GetObject("animation1.MosaicShift")));
            animation1.MosaicSize = 20;
            animation1.Padding = new System.Windows.Forms.Padding(30);
            animation1.RotateCoeff = 0F;
            animation1.RotateLimit = 0F;
            animation1.ScaleCoeff = ((System.Drawing.PointF)(resources.GetObject("animation1.ScaleCoeff")));
            animation1.SlideCoeff = ((System.Drawing.PointF)(resources.GetObject("animation1.SlideCoeff")));
            animation1.TimeCoeff = 0F;
            animation1.TransparencyCoeff = 0F;
            this.animator.DefaultAnimation = animation1;
            // 
            // SimplestSample
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(301, 275);
            this.Controls.Add(this.btShow);
            this.Controls.Add(this.btHide);
            this.Controls.Add(this.pb1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "SimplestSample";
            this.Text = "SimplestSample";
            ((System.ComponentModel.ISupportInitialize)(this.pb1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pb1;
        private System.Windows.Forms.Button btHide;
        private System.Windows.Forms.Button btShow;
        private AnimatorNS.Animator animator;
    }
}