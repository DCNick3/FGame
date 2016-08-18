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
    public partial class IntegerDialogForm : Form
    {
        public int value;

        public IntegerDialogForm()
        {
            InitializeComponent();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (int.TryParse(inputTextBox.Text, out value))
            {
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show("Invalid value!");
            }
        }

        private void IntegerDialogForm_Load(object sender, EventArgs e)
        {
            inputTextBox.Text = value.ToString();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void inputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                okButton_Click(sender, null);
            }
        }
    }
}
