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
		public string Category { get; private set; }
		public addCategory(Forms.Utilities.AudioEditor audioIn)
		{
			InitializeComponent();
			if(!audioIn.cats.Contains(0)) comboBox1.Items.Add("Overworld");
			if(!audioIn.cats.Contains(1)) comboBox1.Items.Add("Nether");
			if(!audioIn.cats.Contains(2)) comboBox1.Items.Add("End");
			if(!audioIn.cats.Contains(3)) comboBox1.Items.Add("Creative");
			if(!audioIn.cats.Contains(4)) comboBox1.Items.Add("Menu");
			if(!audioIn.cats.Contains(5)) comboBox1.Items.Add("Battle");
			if(!audioIn.cats.Contains(6)) comboBox1.Items.Add("Tumble");
			if(!audioIn.cats.Contains(7)) comboBox1.Items.Add("Glide");
			FormBorderStyle = FormBorderStyle.None;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			Category = comboBox1.Text;
			DialogResult = DialogResult.OK;
			if(comboBox1.SelectedIndex > -1) Close();
		}

	}
}
