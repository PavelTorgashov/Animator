using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using AnimatorNS;

namespace AnimatorNS
{
    public partial class TabControlEx : System.Windows.Forms.TabControl
    {
        Animator animator;

        public TabControlEx()
        {
            InitializeComponent();
            animator = new Animator();
            animator.AnimationType = AnimationType.VertSlide;
            animator.DefaultAnimation.TimeCoeff = 1f;
            animator.DefaultAnimation.AnimateOnlyDifferences = false;
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Animation Animation 
        { 
            get { return animator.DefaultAnimation; }
            set { animator.DefaultAnimation = value; }
        }

        protected override void OnSelecting(TabControlCancelEventArgs e)
        {
            base.OnSelecting(e);
            animator.BeginUpdateSync(this, false, null, new Rectangle(0, ItemSize.Height + 3, Width, Height - ItemSize.Height - 3));
            BeginInvoke(new MethodInvoker(()=>animator.EndUpdate(this)));
        }
    }
}
