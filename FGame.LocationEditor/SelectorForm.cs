using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FGame.LocationEditor
{
    public partial class SelectorForm : Form
    {
        public string[] values;
        public int selected;

        public SelectorForm()
        {
            InitializeComponent();
        }

        private void SelectorForm_Load(object sender, EventArgs e)
        {
            selectListBox.Items.Clear();
            selectListBox.Items.AddRange(values);
            selectListBox.SelectedIndex = 0;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (selectListBox.SelectedIndex != -1)
            {
                selected = selectListBox.SelectedIndex;
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show("Select one item!");
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void selectListBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = selectListBox.IndexFromPoint(e.Location);
            if (index != ListBox.NoMatches)
            {
                selectListBox.SelectedIndex = index;
                okButton_Click(sender, null);
            }
        }

        private void selectListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Space)
            {
                okButton_Click(sender, null);
            }
            if (e.KeyCode == Keys.W)
            {
                selectListBox.SelectedIndex = (selectListBox.SelectedIndex - 1) % selectListBox.Items.Count;
            }
            if (e.KeyCode == Keys.S)
            {
                selectListBox.SelectedIndex = (selectListBox.SelectedIndex + 1)% selectListBox.Items.Count;
            }
        }
    }
}
