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
    public partial class MoveToForm : Form
    {
        public float X;
        public float Y;



        public MoveToForm()
        {
            InitializeComponent();
        }

        private void MoveToForm_Load(object sender, EventArgs e)
        {
            xTextBox.Text = X.ToString();
            yTextBox.Text = Y.ToString();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (!(float.TryParse(xTextBox.Text, out X) && float.TryParse(yTextBox.Text, out Y)))
            {
                MessageBox.Show("Invalid input!");
                return;
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                okButton_Click(sender, null);
            }
        }
    }
}
