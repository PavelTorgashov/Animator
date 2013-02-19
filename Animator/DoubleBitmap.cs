//#define debug

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace AnimatorNS
{
    /// <summary>
    /// DoubleBitmap displays animation
    /// </summary>
    public class DoubleBitmap: Control
    {
        protected Bitmap bgBmp;
        protected Bitmap ctrlBmp;
        public float CurrentTime { get; private set; }
        protected float TimeStep { get; private set; }

        public event EventHandler<TransfromNeededEventArg> TransfromNeeded;
        public event EventHandler<NonLinearTransfromNeededEventArg> NonLinearTransfromNeeded;
        public event EventHandler<PaintEventArgs> FramePaint;

        public Control AnimatedControl { get; set; }
        Point[] buffer;
        byte[] pixelsBuffer;
        public Bitmap frame;
        protected Rectangle clipRect;
        
        AnimateMode mode;
        Animation animation;

        public new void Dispose()
        {
            Hide();
            if (ctrlBmp != null)
                bgBmp.Dispose();
            if (ctrlBmp!=null)
                ctrlBmp.Dispose();
            if (frame != null)
                frame.Dispose();
            AnimatedControl = null;
            base.Dispose();
        }

        protected virtual Rectangle GetBounds()
        {
            return new Rectangle(
                AnimatedControl.Left - animation.Padding.Left,
                AnimatedControl.Top - animation.Padding.Top,
                AnimatedControl.Size.Width + animation.Padding.Left + animation.Padding.Right,
                AnimatedControl.Size.Height + animation.Padding.Top + animation.Padding.Bottom);
        }

        protected virtual Rectangle ControlRectToMyRect(Rectangle rect)
        {
            return new Rectangle(
                animation.Padding.Left + rect.Left,
                animation.Padding.Top + rect.Top,
                rect.Width + animation.Padding.Left + animation.Padding.Right,
                rect.Height + animation.Padding.Top + animation.Padding.Bottom);
        }

        public DoubleBitmap(Control control, AnimateMode mode, Animation animation, float timeStep, Rectangle controlClipRect)
        {
            this.animation = animation;
            this.AnimatedControl = control;
            this.mode = mode;
            if (controlClipRect == default(Rectangle))
                this.clipRect = new Rectangle(Point.Empty, GetBounds().Size);
            else
                this.clipRect = ControlRectToMyRect(controlClipRect);

            if (mode == AnimateMode.Show || mode == AnimateMode.BeginUpdate)
                timeStep = -timeStep;

            this.TimeStep = timeStep * (animation.TimeCoeff == 0f ? 1f : animation.TimeCoeff);
            if (this.TimeStep == 0f)
                timeStep = 0.01f;

            if (control.Parent == null)
                throw new Exception("Can not create DoubleBitmap because animated control has not parent.");

            Visible = false;
            SetStyle(ControlStyles.Selectable, false);
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);

            switch(mode)
            {
                case AnimateMode.Hide:
                    {
                        bgBmp = GetBackground(control);
                        this.Bounds = GetBounds();
                        this.Parent = control.Parent;
                        var i = control.Parent.Controls.GetChildIndex(control);
                        control.Parent.Controls.SetChildIndex(this, i);
                        ctrlBmp = GetForeground(control);
                        this.Visible = true;
                        control.Visible = false;
                    }
                    break;

                case AnimateMode.Show:
                    {
                        bgBmp = GetBackground(control);
                        var i = control.Parent.Controls.GetChildIndex(control);
                        this.Parent = control.Parent;
                        this.Bounds = GetBounds();
                        control.Parent.Controls.SetChildIndex(this, i);
                        this.Visible = true;
                        control.Visible = true;
                        ctrlBmp = GetForeground(control);
                    }
                    break;
                case AnimateMode.BeginUpdate:
                case AnimateMode.Update:
                    {
                        this.Bounds = GetBounds();
                        this.Parent = control.Parent;
                        var i = control.Parent.Controls.GetChildIndex(control);
                        control.Parent.Controls.SetChildIndex(this, i);
                        bgBmp = GetBackground(control, true);
                        this.Visible = true;
#if debug
                        bgBmp.Save("c:\\bgBmp.png");
#endif
                    }
                    break;
            }

            CurrentTime = timeStep > 0 ? animation.MinTime : animation.MaxTime;
        }

        protected virtual Bitmap GetBackground(Control ctrl, bool includeForeground = false, bool clip = false)
        {
            var bounds = GetBounds();
            var w = bounds.Width;
            var h = bounds.Height;
            if (w == 0) w = 1;
            if (h == 1) h = 1;
            Bitmap bmp = new Bitmap(w, h);

            var clientRect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            PaintEventArgs ea = new PaintEventArgs(Graphics.FromImage(bmp), clientRect);
            if (clip)
                ea.Graphics.SetClip(clipRect);

            //ea.Graphics.Clear(ctrl.Parent.BackColor);

            for (int i = ctrl.Parent.Controls.Count - 1; i >= 0; i--)
            {
                var c = ctrl.Parent.Controls[i];
                if (c == ctrl && !includeForeground) break;
                if (c.Visible && !c.IsDisposed)
                    if (c.Bounds.IntersectsWith(bounds))
                    {
                        using (Bitmap cb = new Bitmap(c.Width, c.Height))
                        {
                            c.DrawToBitmap(cb, new Rectangle(0, 0, c.Width, c.Height));
                            /*if (c == ctrl)
                                ea.Graphics.SetClip(clipRect);*/
                            ea.Graphics.DrawImage(cb, c.Left - bounds.Left, c.Top - bounds.Top, c.Width, c.Height);
                        }
                    }
                if (c == ctrl) break;
            }

            ea.Graphics.Dispose();

            return bmp;
        }

        protected virtual Bitmap GetForeground(Control ctrl)
        {
            Bitmap bmp = new Bitmap(this.Width, this.Height);

            if (!ctrl.IsDisposed)
                ctrl.DrawToBitmap(bmp, new Rectangle(ctrl.Left - Left, ctrl.Top - Top, ctrl.Width, ctrl.Height));
#if debug
            using (var gr = Graphics.FromImage(bmp))
                gr.DrawLine(Pens.Red, 0, 0, Width, Height);
#endif

            return bmp;
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            var gr = e.Graphics;

            try
            {
                gr.DrawImage(bgBmp, 0, 0);

                if (frame != null)
                {
                    var ea = new TransfromNeededEventArg()
                                 {
                                     CurrentTime = CurrentTime,
                                     ClientRectangle = new Rectangle(0, 0, this.Width, this.Height)
                                 };
                    OnTransfromNeeded(ea);
                    gr.SetClip(clipRect);
                    gr.Transform = ea.Matrix;
                    gr.DrawImage(frame, 0, 0);
                }

                OnFramePaint(e);
            }
            catch { }
        }

        protected virtual void OnFramePaint(PaintEventArgs e)
        {
            if (FramePaint != null)
                FramePaint(this, e);
        }

        protected virtual void OnTransfromNeeded(TransfromNeededEventArg e)
        {
            try
            {
                if (TransfromNeeded != null)
                    TransfromNeeded(this, e);
                else
                    e.UseDefaultMatrix = true;

                if (e.UseDefaultMatrix)
                {
                    TransfromHelper.DoScale(e, animation);
                    TransfromHelper.DoRotate(e, animation);
                    TransfromHelper.DoSlide(e, animation);
                }
            }
            catch
            {
            }
        }

        protected virtual Bitmap OnNonLinearTransfromNeeded()
        {
            Bitmap bmp = (Bitmap)ctrlBmp.Clone();

            const int bytesPerPixel = 4;
            PixelFormat pxf = PixelFormat.Format32bppArgb;
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, pxf);
            IntPtr ptr = bmpData.Scan0;
            int numBytes = bmp.Width * bmp.Height * bytesPerPixel;
            byte[] argbValues = new byte[numBytes];

            System.Runtime.InteropServices.Marshal.Copy(ptr, argbValues, 0, numBytes);

            try
            {
                var e = new NonLinearTransfromNeededEventArg(){CurrentTime = CurrentTime,ClientRectangle = ClientRectangle,Pixels = argbValues,Stride = bmpData.Stride};

                if (NonLinearTransfromNeeded != null)
                    NonLinearTransfromNeeded(this, e);
                else
                    e.UseDefaultTransform = true;

                if (e.UseDefaultTransform)
                {
                    TransfromHelper.DoBlind(e, animation);
                    TransfromHelper.DoMosaic(e, animation, ref buffer, ref pixelsBuffer);

                    TransfromHelper.DoTransparent(e, animation);
                    TransfromHelper.DoLeaf(e, animation);
                }
            }
            catch
            {
            }

            System.Runtime.InteropServices.Marshal.Copy(argbValues, 0, ptr, numBytes);
            bmp.UnlockBits(bmpData);

            return bmp;
        }

        public void EndUpdate()
        {
            var bmp = GetBackground(AnimatedControl, true, true);
#if debug
            bmp.Save("c:\\bmp.png");
#endif
            if(animation.AnimateOnlyDifferences)
                TransfromHelper.CalcDifference(bmp, bgBmp);

            ctrlBmp = bmp;
            mode = AnimateMode.Update;
#if debug
            ctrlBmp.Save("c:\\ctrlBmp.png");
#endif
        }

        public bool IsCompleted 
        {
            get { return (TimeStep >= 0f && CurrentTime >= animation.MaxTime) || (TimeStep <= 0f && CurrentTime <= animation.MinTime); }
        }

        internal void BuildNextFrame()
        {
            if (frame != null)
                frame.Dispose();

            if (mode == AnimateMode.BeginUpdate)
                return;

            frame = OnNonLinearTransfromNeeded();

            var time = CurrentTime + TimeStep;
            if (time > animation.MaxTime) time = animation.MaxTime;
            if (time < animation.MinTime) time = animation.MinTime;
            CurrentTime = time;
            Invalidate();
        }
    }
}