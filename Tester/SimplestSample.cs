using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using AnimatorNS;

namespace Tester
{
    public partial class SimplestSample : Form
    {
        public SimplestSample()
        {
            InitializeComponent();
        }

        private void btHide_Click(object sender, EventArgs e)
        {
            animator.Hide(pb1);
        }

        private void btShow_Click(object sender, EventArgs e)
        {
            animator.Show(pb1);
        }
    }
}
