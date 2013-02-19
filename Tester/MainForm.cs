using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using AnimatorNS;

namespace Tester
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            rbPredefined.Checked = true;
            pg.SelectedObject = new Animation();
            dataGridView1.Rows.Add("some string", null, 2, 3, 4, 5);
            dataGridView1.Rows.Add("some string", null, 2, 3, 4, 5);
            dataGridView1.Rows.Add("some string", null, 2, 3, 4, 5);
            dataGridView1.Rows.Add("some string", null, 2, 3, 4, 5);
            dataGridView1.Rows.Add("some string", null, 2, 3, 4, 5);
            dataGridView1.Rows.Add("some string", null, 2, 3, 4, 5);

            dataGridView2.Rows.Add();
            dataGridView2.Rows.Add();
            dataGridView2.Rows.Add();
            dataGridView2.Rows.Add();
            dataGridView2.Rows.Add();
            dataGridView2.Rows.Add();

            DoAnimation();
        }

        private void tcMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            DoAnimation();
        }

        void CreateAnimation()
        {
            if (rbPredefined.Checked)
                animator.DefaultAnimation = GetPredefinedAnimation(predefinedList1.SelectedText);
            else
            if (rbCombination.Checked)
                animator.DefaultAnimation = GetCombinationAnimation(predefinedList2.SelectedText);
            else
            if (rbAdjusted.Checked)
                animator.DefaultAnimation = GetAdjustedAnimation();
            else
                animator.DefaultAnimation = Animation.Transparent;
        }

        private Animation GetCombinationAnimation(string name)
        {
            var result = new Animation();
            if (name != null)
            {
                var parts = name.Split(';');
                foreach (var part in parts)
                {
                    var a = GetPredefinedAnimation(part);
                    result.Add(a);
                }
            }

            return result;
        }

        private Animation GetAdjustedAnimation()
        {
            return (Animation)pg.SelectedObject;
        }

        private Animation GetPredefinedAnimation(string name)
        {
            switch (name)
            {
                case "Rotate": return Animation.Rotate;
                case "HorizSlide": return Animation.HorizSlide;
                case "VertSlide": return Animation.VertSlide;
                case "Scale": return Animation.Scale;
                case "ScaleAndRotate": return Animation.ScaleAndRotate;
                case "HorizSlideAndRotate": return Animation.HorizSlideAndRotate;
                case "ScaleAndHorizSlide": return Animation.ScaleAndHorizSlide;
                case "Transparent": return Animation.Transparent;
                case "Leaf": return Animation.Leaf;
                case "Mosaic": return Animation.Mosaic;
                case "Particles": return Animation.Particles;
                case "VertBlind": return Animation.VertBlind;
                case "HorizBlind": return Animation.HorizBlind;
            }
            return Animation.Scale;
        }

        private void DoAnimation()
        {
            //wait while all animations will be completed
            animator.WaitAllAnimations();

            HideControls();
            CreateAnimation();
            ShowControls();
        }

        //show control(s)
        private void ShowControls()
        {
            switch (tcMain.SelectedIndex)
            {
                case 0:
                    animator.Show(pictureBox1);
                    break;

                case 1:
                    animator.Show(textBox1);
                    animator.Show(lb);
                    animator.Show(bt);
                    animator.Show(checkBox1);
                    animator.Show(comboBox1);
                    animator.Show(progressBar1);
                    break;

                case 2:
                    animator.Show(dataGridView1);
                    break;

                case 3:
                    animator.Show(menuStrip1);
                    animator.Show(groupBox1);
                    animator.Show(toolStrip1);
                    animator.Show(statusStrip1);
                    animator.Show(treeView1);
                    animator.Show(listBox1);
                    break;

                case 4:
                    animator.Show(lbUpdate, true);
                    animator.Show(dataGridView2, true);
                    animator.Show(flowLayoutPanel1, true);
                    break;
            }

            //wait while all animations will be completed
            animator.WaitAllAnimations();
        }

        //hide visible control
        private void HideControls()
        {
            //hide all visible controls
            foreach (Control ctrl in pnMain.Controls)
                if (ctrl.Visible)
                    animator.Hide(ctrl);

            //wait while all animations will be completed
            animator.WaitAllAnimations();
        }

        private void predefinedList2_Changed(object sender, EventArgs e)
        {
            DoAnimation();
        }

        private void rbPredefined_CheckedChanged(object sender, EventArgs e)
        {
            var rb = (RadioButton)sender;
            if (rb.Checked)
                BuildLeftPanel();
        }

        private void BuildLeftPanel()
        {
            Control panel = null;
            if (rbPredefined.Checked)
                panel = predefinedList1;
            if (rbCombination.Checked)
                panel = predefinedList2;
            if (rbAdjusted.Checked)
                panel = pg;
            if (rbCustom.Checked)
                panel = tbCode;

            if (panel != null)
            {
                panel.BringToFront();
                animator.ShowSync(panel, true, Animation.VertSlide);
            }

            foreach (Control ctrl in pnLeft.Controls)
                if (ctrl.Visible && ctrl != panel)
                    if (ctrl == predefinedList1 || ctrl == predefinedList2 || ctrl == pg || ctrl == tbCode)
                        ctrl.Hide();

            if (rbCustom.Checked)
            {
                CreateAnimation();
                DoAnimation();
            }
        }

        private void pg_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            DoAnimation();
        }

        private void btAnimate_Click(object sender, EventArgs e)
        {
            DoAnimation();
        }

        private void btShow_Click(object sender, EventArgs e)
        {
            ShowControls();
        }

        private void btHide_Click(object sender, EventArgs e)
        {
            HideControls();
        }

        private void btExample1_Click(object sender, EventArgs e)
        {
            new SimplestSample().Show();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();

            var rnd = new Random();
            if (dataGridView2.Visible)
            {
                var row = rnd.Next(dataGridView2.Rows.Count);
                var col = rnd.Next(dataGridView2.Columns.Count);
                var n = rnd.Next(100000);

                animator.BeginUpdateSync(dataGridView2, true);

                if (col < dataGridView2.Columns.Count && row < dataGridView2.Rows.Count)//may be grid was changed, so check it ...
                    dataGridView2[col, row].Value = n;

                animator.EndUpdate(dataGridView2);
            }

            if (flowLayoutPanel1.Visible)
            {
                var i = rnd.Next(flowLayoutPanel1.Controls.Count);
                var n = rnd.Next(100000);

                animator.BeginUpdateSync(flowLayoutPanel1, true);

                if (n % 2 == 0 && flowLayoutPanel1.Controls.Count > 3)
                {
                    if (i < flowLayoutPanel1.Controls.Count) //may be control was changed, so check it ...
                        flowLayoutPanel1.Controls[i].Parent = null; //remove random item
                }
                else
                {
                    new Button { Text = n.ToString(), Parent = flowLayoutPanel1 };//add item
                }

                animator.EndUpdate(flowLayoutPanel1);
            }

            animator.WaitAllAnimations();

            timer1.Start();
        }

        /// <summary>
        /// Handles TransfromNeeded event
        /// </summary>
        private void animator_TransfromNeeded(object sender, TransfromNeededEventArg e)
        {
            if (rbCustom.Checked)
            {
                var cy = e.ClientRectangle.Height / 2;

                var sy = 1 - 2  * e.CurrentTime;
                if (sy < 0.01f && sy > -0.01f)
                    sy = 0.01f;

                e.Matrix.Translate(0, cy);
                e.Matrix.Scale(1, sy);
                e.Matrix.Translate(0, -cy);
            }else
                e.UseDefaultMatrix = true;
        }

        private void btAnimatedTabs_Click(object sender, EventArgs e)
        {
            new AnimatedTabsSample().Show();
        }
    }
}
