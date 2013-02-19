using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Tester.Controls
{
    public partial class PredefinedList2 : UserControl
    {
        public event EventHandler Changed;
        public string SelectedText { get; private set; }

        public PredefinedList2()
        {
            InitializeComponent();
        }

        private void rbHorizBlind_CheckedChanged(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();

            foreach(Control ctrl in Controls)
            if(ctrl is CheckBox)
            {
                if ((ctrl as CheckBox).Checked)
                    sb.Append(ctrl.Text + ";");
            }

            SelectedText = sb.ToString().Trim(';');

            if (Changed != null)
                Changed(this, EventArgs.Empty);
        }
    }
}
