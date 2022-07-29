using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// Audio Editor by MattNL

namespace PckStudio
{
	public partial class addCategory : MetroFramework.Forms.MetroForm
	{
		private string _category = string.Empty;
		public string Category => _category;
		public addCategory(string[] avalibleCategories)
		{
			InitializeComponent();
			comboBox1.Items.AddRange(avalibleCategories);
		}

		private void button1_Click(object sender, EventArgs e)
		{
			_category = comboBox1.Text;
			DialogResult = DialogResult.OK;
			if(comboBox1.SelectedIndex > -1) Close();
		}

	}
}
