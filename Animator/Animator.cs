//
//  THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
//  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
//  PURPOSE.
//
//  License: GNU Lesser General Public License (LGPLv3)
//
//  Email: pavel_torgashov@mail.ru.
//
//  Copyright (C) Pavel Torgashov, 2013. 


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace AnimatorNS
{
    /// <summary>
    /// Animation manager
    /// </summary>
    public class Animator : Component
    {
        IContainer components = null;
        protected List<QueueItem> queue = new List<QueueItem>();
        private Timer tm = new Timer();

        /// <summary>
        /// Occurs when animation of the control is completed
        /// </summary>
        public event EventHandler<AnimationCompletedEventArg> AnimationCompleted;
        /// <summary>
        /// Ocuurs when all animations are completed
        /// </summary>
        public event EventHandler AllAnimationsCompleted;
        /// <summary>
        /// Occurs when needed transform matrix
        /// </summary>
        public event EventHandler<TransfromNeededEventArg> TransfromNeeded;
        /// <summary>
        /// Occurs when needed non-linear transformation
        /// </summary>
        public event EventHandler<NonLinearTransfromNeededEventArg> NonLinearTransfromNeeded;
        /// <summary>
        /// Occurs when user click on the animated control
        /// </summary>
        public event EventHandler<MouseEventArgs> MouseDown;
        /// <summary>
        /// Occurs when frame of animation is painting
        /// </summary>
        public event EventHandler<PaintEventArgs> FramePaint;

        /// <summary>
        /// Default animation
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Animation DefaultAnimation { get; set; }

        /// <summary>
        /// Cursor of animated control
        /// </summary>
        [DefaultValue(typeof(Cursor), "Default")]
        public Cursor Cursor { get; set; }

        /// <summary>
        /// Are all animations completed?
        /// </summary>
        public bool IsCompleted
        { 
            get { lock(queue) return queue.Count == 0; }
        }

        /// <summary>
        /// Interval between frames (ms)
        /// </summary>
        [DefaultValue(10)]
        public int Interval 
        { 
            get { return tm.Interval; }
            set { tm.Interval = value; }
        }

        AnimationType animationType;
        /// <summary>
        /// Type of built-in animation
        /// </summary>
        public AnimationType AnimationType 
        {
            get { return animationType; }
            set { animationType = value; InitDefaultAnimation(animationType); }
        }

        public Animator()
        {
            Init();
        }

        public Animator(IContainer container)
        {
            container.Add(this);
            Init();
        }

        protected virtual void Init()
        {
            DefaultAnimation = new Animation();

            TimeStep = 0.02f;
            tm.Interval = 10;
            tm.Tick += new EventHandler(tm_Tick);
            tm.Start();

            Disposed += new EventHandler(Animator_Disposed);
        }

        void Animator_Disposed(object sender, EventArgs e)
        {
            ClearQueue();

            tm.Stop();
            tm.Dispose();
        }

        void tm_Tick(object sender, EventArgs e)
        {
            tm.Stop();

            try
            {
                var count = 0;
                var completed = new List<QueueItem>();
                var actived = new List<QueueItem>();

                //find completed
                lock (queue)
                {
                    count = queue.Count;
                    var wasActive = false;

                    foreach (var item in queue)
                    {
                        if (item.isActive) wasActive = true;

                        if (item.bmp != null && item.bmp.IsCompleted)
                        {
                            //dispose DoubleBitmap
                            item.bmp.Dispose();
                            completed.Add(item);
                        }
                        else
                        {
                            if (item.isActive)
                                //build next frame of DoubleBitmap
                                actived.Add(item);
                        }
                    }
                    //start next animation
                    if(!wasActive)
                    foreach(var item in queue)
                        if(!item.isActive)
                        {
                            actived.Add(item);
                            item.isActive = true;
                            break;
                        }
                }

                //build next frame of DoubleBitmap
                foreach (var item in actived)
                {
                    try
                    {
                        if (item.bmp == null)
                            item.bmp = CreateDoubleBitmap(item.control, item.mode, item.animation, item.clipRectangle);
                        item.bmp.BuildNextFrame();
                    }catch
                    {
                        completed.Add(item);
                    }
                }

                //call events
                foreach (var item in completed)
                {
                    OnAnimationCompleted(new AnimationCompletedEventArg
                                             {Animation = item.animation, Mode = item.mode, Control = item.control});
                }
                
                var allCompleted = false;
                lock(queue)
                    allCompleted = queue.Count == 0 && completed.Count > 0;

                if(allCompleted)
                    OnAllAnimationsCompleted();

                //remove completed from queue
                lock(queue)
                foreach (var item in completed)
                    queue.Remove(item);
            }
            catch
            {}

            tm.Start();
        }

        private void InitDefaultAnimation(AnimatorNS.AnimationType animationType)
        {
            switch (animationType)
            {
                case AnimationType.Custom: break;
                case AnimationType.Rotate: DefaultAnimation = Animation.Rotate; break;
                case AnimationType.HorizSlide: DefaultAnimation = Animation.HorizSlide; break;
                case AnimationType.VertSlide: DefaultAnimation = Animation.VertSlide; break;
                case AnimationType.Scale: DefaultAnimation = Animation.Scale; break;
                case AnimationType.ScaleAndRotate: DefaultAnimation = Animation.ScaleAndRotate; break;
                case AnimationType.HorizSlideAndRotate: DefaultAnimation = Animation.HorizSlideAndRotate; break;
                case AnimationType.ScaleAndHorizSlide: DefaultAnimation = Animation.ScaleAndHorizSlide; break;
                case AnimationType.Transparent: DefaultAnimation = Animation.Transparent; break;
                case AnimationType.Leaf: DefaultAnimation = Animation.Leaf; break;
                case AnimationType.Mosaic: DefaultAnimation = Animation.Mosaic; break;
                case AnimationType.Particles: DefaultAnimation = Animation.Particles; break;
                case AnimationType.VertBlind: DefaultAnimation = Animation.VertBlind; break;
                case AnimationType.HorizBlind: DefaultAnimation = Animation.HorizBlind; break;
            }
        }

        /// <summary>
        /// Time step
        /// </summary>
        [DefaultValue(0.02f)]
        public float TimeStep { get; set; }

        /// <summary>
        /// Shows the control. As result the control will be shown with animation.
        /// </summary>
        /// <param name="control">Target control</param>
        /// <param name="parallel">Allows to animate it same time as other animations</param>
        /// <param name="animation">Personal animation</param>
        public void Show(Control control, bool parallel = false, Animation animation = null)
        {
            AddToQueue(control, AnimateMode.Show, parallel, animation);
        }

        /// <summary>
        /// Shows the control and waits while animation will be completed. As result the control will be shown with animation.
        /// </summary>
        /// <param name="control">Target control</param>
        /// <param name="parallel">Allows to animate it same time as other animations</param>
        /// <param name="animation">Personal animation</param>
        public void ShowSync(Control control, bool parallel = false, Animation animation = null)
        {
            Show(control, parallel, animation);
            WaitAnimation(control);
        }

        /// <summary>
        /// Hides the control. As result the control will be hidden with animation.
        /// </summary>
        /// <param name="control">Target control</param>
        /// <param name="parallel">Allows to animate it same time as other animations</param>
        /// <param name="animation">Personal animation</param>
        public void Hide(Control control, bool parallel = false, Animation animation = null)
        {
            AddToQueue(control, AnimateMode.Hide, parallel, animation);
        }

        /// <summary>
        /// Hides the control and waits while animation will be completed. As result the control will be hidden with animation.
        /// </summary>
        /// <param name="control">Target control</param>
        /// <param name="parallel">Allows to animate it same time as other animations</param>
        /// <param name="animation">Personal animation</param>
        public void HideSync(Control control, bool parallel = false, Animation animation = null)
        {
            Hide(control, parallel, animation);
            WaitAnimation(control);
        }

        /// <summary>
        /// It makes snapshot of the control before updating. It requires EndUpdate calling.
        /// </summary>
        /// <param name="control">Target control</param>
        /// <param name="parallel">Allows to animate it same time as other animations</param>
        /// <param name="animation">Personal animation</param>
        /// <param name="clipRectangle">Clip rectangle for animation</param>
        public void BeginUpdateSync(Control control, bool parallel = false, Animation animation = null, Rectangle clipRectangle = default(Rectangle))
        {
            AddToQueue(control, AnimateMode.BeginUpdate, parallel, animation, clipRectangle);

            bool wait = false;
            do
            {
                wait = false;
                lock (queue)
                    foreach (var item in queue)
                        if (item.control == control && item.mode == AnimateMode.BeginUpdate)
                            if (item.bmp == null)
                                wait = true;

                if (wait)
                    Application.DoEvents();

            } while (wait);
        }

        /// <summary>
        /// Upadates control view with animation. It requires to call BeginUpdate before.
        /// </summary>
        /// <param name="control">Target control</param>
        public void EndUpdate(Control control)
        {
            lock (queue)
            {
                foreach (var item in queue)
                    if (item.control == control && item.mode == AnimateMode.BeginUpdate)
                    {
                        item.bmp.EndUpdate();
                        item.mode = AnimateMode.Update;
                    }
            }
        }

        /// <summary>
        /// Upadates control view with animation and waits while animation will be completed. It requires to call BeginUpdate before.
        /// </summary>
        /// <param name="control">Target control</param>
        public void EndUpdateSync(Control control)
        {
            EndUpdate(control);
            WaitAnimation(control);
        }

        /// <summary>
        /// Waits while all animations will completed.
        /// </summary>
        public void WaitAllAnimations()
        {
            while (!IsCompleted)
                Application.DoEvents();
        }

        /// <summary>
        /// Waits while animation of the control will completed.
        /// </summary>
        /// <param name="animatedControl"></param>
        public void WaitAnimation(Control animatedControl)
        {
            while (true)
            {
                bool flag = false;
                lock(queue)
                foreach(var item in queue)
                    if (item.control == animatedControl)
                    {
                        flag = true;
                        break;
                    }

                if(!flag)
                    return;

                Application.DoEvents();
            }
        }

        /// <summary>
        /// Adds the contol to animation queue.
        /// </summary>
        /// <param name="control">Target control</param>
        /// <param name="mode">Animation mode</param>
        /// <param name="parallel">Allows to animate it same time as other animations</param>
        /// <param name="animation">Personal animation</param> 
        public void AddToQueue(Control control, AnimateMode mode, bool parallel = true, Animation animation = null, Rectangle clipRectangle = default(Rectangle))
        {
            if(animation == null)
                animation = DefaultAnimation;

            //check visible state
            switch (mode)
            {
                case AnimateMode.Show:
                    if (control.Visible)//already showed
                    {
                        OnAnimationCompleted(new AnimationCompletedEventArg{Animation = animation, Control = control, Mode = mode});
                        return;
                    }
                    break;
                case AnimateMode.Hide:
                    if (!control.Visible)//already hiden
                    {
                        OnAnimationCompleted(new AnimationCompletedEventArg { Animation = animation, Control = control, Mode = mode });
                        return;
                    }
                    break;
            }

            //add to queue
            lock (queue)
                queue.Add(new QueueItem() { animation = animation, control = control, isActive = parallel, mode = mode, clipRectangle = clipRectangle });
        }

        private DoubleBitmap CreateDoubleBitmap(Control control, AnimateMode mode, Animation animation, Rectangle clipRect)
        {
            var bmp = new DoubleBitmap(control, mode, animation, TimeStep, clipRect);
            bmp.TransfromNeeded += OnTransformNeeded;
            bmp.NonLinearTransfromNeeded += OnNonLinearTransfromNeeded;
            bmp.MouseDown += OnMouseDown;
            bmp.Cursor = Cursor;
            return bmp;
        }

        protected virtual void OnMouseDown(object sender, MouseEventArgs e)
        {
            //transform point to animated control's coordinates 
            var db = (DoubleBitmap) sender;
            var l = e.Location;
            l.Offset(db.Left - db.AnimatedControl.Left, db.Top - db.AnimatedControl.Top);
            //
            if (MouseDown != null)
                MouseDown(sender, new MouseEventArgs(e.Button, e.Clicks, l.X, l.Y, e.Delta));
        }

        protected virtual void OnNonLinearTransfromNeeded(object sender, NonLinearTransfromNeededEventArg e)
        {
            if (NonLinearTransfromNeeded != null)
                NonLinearTransfromNeeded(this, e);
            else
                e.UseDefaultTransform = true;
        }

        protected virtual void OnTransformNeeded(object sender, TransfromNeededEventArg e)
        {
            if (TransfromNeeded != null)
                TransfromNeeded(this, e);
            else
                e.UseDefaultMatrix = true;
        }

        /// <summary>
        /// Clears queue.
        /// </summary>
        public void ClearQueue()
        {
            List<QueueItem> items = null;
            lock (queue)
            {
                items = new List<QueueItem>(queue);
                queue.Clear();
            }

            foreach (var item in items)
            {
                if (item.bmp != null)
                    item.bmp.Dispose();
                OnAnimationCompleted(new AnimationCompletedEventArg { Animation = item.animation, Control = item.control, Mode = item.mode });
            }

            if (items.Count > 0)
                OnAllAnimationsCompleted();
        }

        protected virtual void OnAnimationCompleted(AnimationCompletedEventArg e)
        {
            if (AnimationCompleted != null)
                AnimationCompleted(this, e);
        }

        protected virtual void OnAllAnimationsCompleted()
        {
            if (AllAnimationsCompleted != null)
                AllAnimationsCompleted(this, EventArgs.Empty);
        }

        #region Nested type: QueueItem

        protected class QueueItem
        {
            public Animation animation;
            public DoubleBitmap bmp;
            public Control control;
            public bool isActive;
            public AnimateMode mode;
            public Rectangle clipRectangle;
        }

        #endregion
    }


    public class AnimationCompletedEventArg : EventArgs
    {
        public Animation Animation { get; set; }
        public Control Control { get; internal set; }
        public AnimateMode Mode { get; internal set; }
    }

    public class TransfromNeededEventArg : EventArgs
    {
        public TransfromNeededEventArg()
        {
            Matrix = new Matrix(1, 0, 0, 1, 0, 0);
        }

        public Matrix Matrix { get; set; }
        public float CurrentTime { get; internal set; }
        public Rectangle ClientRectangle { get; internal set; }
        public Animation Animation { get; set; }
        public Control Control { get; internal set; }
        public AnimateMode Mode { get; internal set; }
        public bool UseDefaultMatrix { get; set; }
    }

    public class NonLinearTransfromNeededEventArg : EventArgs
    {
        public float CurrentTime { get; internal set; }
        public Rectangle ClientRectangle { get; internal set; }
        public byte[] Pixels { get; internal set; }
        public int Stride { get; internal set; }
        public Animation Animation { get; set; }
        public Control Control { get; internal set; }
        public AnimateMode Mode { get; internal set; }
        public bool UseDefaultTransform { get; set; }
    }


    public enum AnimateMode
    {
        Show,
        Hide,
        Update,
        BeginUpdate
    }
}