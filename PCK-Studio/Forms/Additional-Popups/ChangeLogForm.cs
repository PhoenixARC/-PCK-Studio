using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using PckStudio.Properties;

namespace PckStudio.Forms.Additional_Popups
{
    public partial class ChangeLogForm : Form
    {
        public ChangeLogForm()
        {
            InitializeComponent();
            ChangelogRichTextBox.Text = Resources.CHANGELOG;
        }

        private void ChangeLogForm_Load(object sender, EventArgs e)
        {

        }
    }
}
