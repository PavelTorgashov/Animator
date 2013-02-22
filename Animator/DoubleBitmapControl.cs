using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace AnimatorNS
{
    public partial class DoubleBitmapControl : Control, IFakeControl
    {
        Bitmap bgBmp;
        Bitmap frame;

        Bitmap IFakeControl.BgBmp { get { return this.bgBmp; } set { this.bgBmp = value; } }
        Bitmap IFakeControl.Frame { get { return this.frame; } set { this.frame = value; } }
        public event EventHandler<TransfromNeededEventArg> TransfromNeeded;
        public event EventHandler<PaintEventArgs> FramePainted;
        public event EventHandler<PaintEventArgs> FramePainting;

        public DoubleBitmapControl()
        {
            InitializeComponent();

            Visible = false;
            SetStyle(ControlStyles.Selectable, false);
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var gr = e.Graphics;

            OnFramePainting(e);

            try
            {
                gr.DrawImage(bgBmp, 0, 0);
                if (frame != null)
                {
                    var ea = new TransfromNeededEventArg(){ ClientRectangle = new Rectangle(0, 0, this.Width, this.Height) };
                    OnTransfromNeeded(ea);
                    gr.SetClip(ea.ClipRectangle);
                    gr.Transform = ea.Matrix;
                    gr.DrawImage(frame, 0, 0);
                }
            }
            catch { }

            OnFramePainted(e);
        }

        private void OnTransfromNeeded(TransfromNeededEventArg ea)
        {
            if (TransfromNeeded != null)
                TransfromNeeded(this, ea);
        }

        protected virtual void OnFramePainting(PaintEventArgs e)
        {
            if (FramePainting != null)
                FramePainting(this, e);
        }

        protected virtual void OnFramePainted(PaintEventArgs e)
        {
            if (FramePainted != null)
                FramePainted(this, e);
        }


        public void InitParent(Control control, Padding padding)
        {
            Parent = control.Parent;
            var i = control.Parent.Controls.GetChildIndex(control);
            control.Parent.Controls.SetChildIndex(this, i);
            Bounds = new Rectangle(
                control.Left - padding.Left,
                control.Top - padding.Top,
                control.Size.Width + padding.Left + padding.Right,
                control.Size.Height + padding.Top + padding.Bottom);
        }
    }

    public interface IFakeControl
    {
        Bitmap BgBmp { get; set; }
        Bitmap Frame { get; set; }
        event EventHandler<TransfromNeededEventArg> TransfromNeeded;
        event EventHandler<PaintEventArgs> FramePainting;
        event EventHandler<PaintEventArgs> FramePainted;
        void InitParent(Control animatedControl, Padding padding);
    }
}
