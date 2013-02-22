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
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace AnimatorNS
{
    /// <summary>
    /// Animation manager
    /// </summary>
    [ProvideProperty("Decoration", typeof(Control))] 
    public class Animator : Component, IExtenderProvider
    {
        IContainer components = null;
        protected List<QueueItem> queue = new List<QueueItem>();
        private Thread thread;

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
        public event EventHandler<PaintEventArgs> FramePainted;

        /// <summary>
        /// Max time of animation (ms)
        /// </summary>
        [DefaultValue(1500)]
        public int MaxAnimationTime { get; set; }

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
            get;
            set;
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
            MaxAnimationTime = 1500;
            TimeStep = 0.02f;
            Interval = 10;

            Disposed += new EventHandler(Animator_Disposed);

            //main working thread
            thread = new Thread(Work);
            thread.IsBackground = true;
            thread.Start();
        }

        void Animator_Disposed(object sender, EventArgs e)
        {
            ClearQueue();
            thread.Abort();
        }

        void Work()
        {
            while(true)
            {
                Thread.Sleep(Interval);
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
                            if (item.IsActive) wasActive = true;

                            if (item.controller != null && item.controller.IsCompleted)
                                completed.Add(item);
                            else
                            {
                                if (item.IsActive)
                                {
                                    if ((DateTime.Now - item.ActivateTime).TotalMilliseconds > MaxAnimationTime)
                                        completed.Add(item);
                                    else
                                        actived.Add(item);
                                }
                            }
                        }
                        //start next animation
                        if (!wasActive)
                            foreach (var item in queue)
                                if (!item.IsActive)
                                {
                                    actived.Add(item);
                                    item.IsActive = true;
                                    break;
                                }
                    }

                    //completed
                    foreach (var item in completed)
                        OnCompleted(item);

                    //build next frame of DoubleBitmap
                    foreach (var item in actived)
                        try
                        {
                            //build next frame of DoubleBitmap
                            item.control.BeginInvoke(new MethodInvoker(() => DoAnimation(item)));
                        }
                        catch
                        {
                            //we can not start animation, remove from queue
                            OnCompleted(item);
                        }

                    if (count == 0)
                    {
                        if (completed.Count > 0)
                            OnAllAnimationsCompleted();
                        CheckRequests();
                    }
                }
                catch
                {
                    //form was closed
                }
            }
        }

        /// <summary>
        /// Check result state of controls
        /// </summary>
        private void CheckRequests()
        {
            var toRemove = new List<QueueItem>();

            lock (requests)
            {
                var dict = new Dictionary<Control, QueueItem>();
                foreach (var item in requests)
                if(item.control != null)
                {
                    if (dict.ContainsKey(item.control))
                        toRemove.Add(dict[item.control]);
                    dict[item.control] = item;
                }else
                    toRemove.Add(item);

                foreach(var item in dict.Values)
                {
                    if (item.control != null && !IsStateOK(item.control, item.mode))
                        RepairState(item.control, item.mode);
                    else
                        toRemove.Add(item);
                }

                foreach (var item in toRemove)
                    requests.Remove(item);
            }
        }

        bool IsStateOK(Control control, AnimateMode mode)
        {
            switch (mode)
            {
                case AnimateMode.Hide: return !control.Visible;
                case AnimateMode.Show: return control.Visible;
            }

            return true;
        }

        void RepairState(Control control, AnimateMode mode)
        {
            control.BeginInvoke(new MethodInvoker(() =>
            {
                switch (mode)
                {
                    case AnimateMode.Hide: control.Visible = false; break;
                    case AnimateMode.Show: control.Visible = true; break;
                }
            }));
        }

        private void DoAnimation(QueueItem item)
        {
            if(Monitor.TryEnter(item))
            try
            {
                if (item.controller == null)
                {
                    item.controller = CreateDoubleBitmap(item.control, item.mode, item.animation,
                                                         item.clipRectangle);
                }
                if (item.controller.IsCompleted)
                    return;
                item.controller.BuildNextFrame();
            }
            catch
            {
                OnCompleted(item);
            }
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
                            if (item.controller == null)
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
                        item.controller.EndUpdate();
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

        List<QueueItem> requests = new List<QueueItem>();

        void OnCompleted(QueueItem item)
        {
            if (item.controller != null)
            {
                item.controller.Dispose();
            }
            lock (queue)
                queue.Remove(item);

            OnAnimationCompleted(new AnimationCompletedEventArg { Animation = item.animation, Control = item.control, Mode = item.mode });
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

            if (control is IFakeControl)
            {
                control.Visible = false;
                return;
            }

            var item = new QueueItem() { animation = animation, control = control, IsActive = parallel, mode = mode, clipRectangle = clipRectangle };

            //check visible state
            switch (mode)
            {
                case AnimateMode.Show:
                    if (control.Visible)//already showed
                    {
                        OnCompleted(new QueueItem {control = control, mode = mode});
                        return;
                    }
                    break;
                case AnimateMode.Hide:
                    if (!control.Visible)//already hidden
                    {
                        OnCompleted(new QueueItem { control = control, mode = mode });
                        return;
                    }
                    break;
            }

            //add to queue
            lock (queue)
                queue.Add(item);
            lock (requests)
                requests.Add(item);
        }

        private Controller CreateDoubleBitmap(Control control, AnimateMode mode, Animation animation, Rectangle clipRect)
        {
            var controller = new Controller(control, mode, animation, TimeStep, clipRect);
            controller.TransfromNeeded += OnTransformNeeded;
            controller.NonLinearTransfromNeeded += OnNonLinearTransfromNeeded;
            controller.MouseDown += OnMouseDown;
            controller.DoubleBitmap.Cursor = Cursor;
            controller.FramePainted += OnFramePainted;
            return controller;
        }

        void OnFramePainted(object sender, PaintEventArgs e)
        {
            if (FramePainted != null)
                FramePainted(sender, e);
        }

        protected virtual void OnMouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                //transform point to animated control's coordinates 
                var db = (Controller) sender;
                var l = e.Location;
                l.Offset(db.DoubleBitmap.Left - db.AnimatedControl.Left, db.DoubleBitmap.Top - db.AnimatedControl.Top);
                //
                if (MouseDown != null)
                    MouseDown(sender, new MouseEventArgs(e.Button, e.Clicks, l.X, l.Y, e.Delta));
            }catch
            {
            }
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
                if (item.control != null)
                    item.control.BeginInvoke(new MethodInvoker(() =>
                    {
                        switch (item.mode)
                        {
                            case AnimateMode.Hide: item.control.Visible = false; break;
                            case AnimateMode.Show: item.control.Visible = true; break;
                        }
                    }));
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
            public Controller controller;
            public Control control;
            public DateTime ActivateTime { get; private set;}
            public AnimateMode mode;
            public Rectangle clipRectangle;

            public bool isActive;
            public bool IsActive
            {
                get { return isActive; }
                set
                {
                    if (isActive == value) return;
                    isActive = value;
                    if (value)
                        ActivateTime = DateTime.Now;
                }
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                if (control != null)
                    sb.Append(control.GetType().Name + " ");
                sb.Append(mode);
                return sb.ToString();
            }
        }

        #endregion

        #region IExtenderProvider

        public DecorationType GetDecoration(Control control)
        {
            if (DecorationByControls.ContainsKey(control))
                return DecorationByControls[control].DecorationType;
            else
                return DecorationType.None;
        }
        
        public void SetDecoration(Control control, DecorationType decoration)
        {
            var wrapper = DecorationByControls.ContainsKey(control) ? DecorationByControls[control] : null;
            if (decoration == DecorationType.None)
            {
                if (wrapper!=null)
                    wrapper.Dispose();
                DecorationByControls.Remove(control);
            }
            else
            {
                if (wrapper == null)
                    wrapper = new DecorationControl(decoration, control);
                wrapper.DecorationType = decoration;
                DecorationByControls[control] = wrapper;
            }
        }

        private readonly Dictionary<Control, DecorationControl> DecorationByControls = new Dictionary<Control, DecorationControl>();

        public bool CanExtend(object extendee)
        {
            return extendee is Control;
        }

        #endregion
    }

    public enum DecorationType
    {
        None,
        BottomMirror,
        Custom
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
        public Rectangle ClipRectangle { get; internal set; }
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

        public Rectangle SourceClientRectangle { get; internal set; }
        public byte[] SourcePixels { get; internal set; }
        public int SourceStride { get; set; }

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