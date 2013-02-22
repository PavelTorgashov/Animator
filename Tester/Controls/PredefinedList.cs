using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Tester.Controls
{
    public partial class PredefinedList : UserControl
    {
        public event EventHandler Changed;
        public string SelectedText { get; private set; }

        public PredefinedList()
        {
            InitializeComponent();
        }

        private void rbHorizBlind_CheckedChanged(object sender, EventArgs e)
        {
            var rb = (RadioButton)sender;
            if (rb.Checked)
            {
                SelectedText = rb.Text;
                if (Changed != null)
                    Changed(this, EventArgs.Empty);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            rbHorizSlide.Checked = true;
            //rbHorizBlind_CheckedChanged(rbHorizSlide, EventArgs.Empty);
        }
    }
}
