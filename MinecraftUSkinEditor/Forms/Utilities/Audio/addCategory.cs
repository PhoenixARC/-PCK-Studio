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
		PckStudio.Forms.Utilities.AudioEditor audio;
		public addCategory(PckStudio.Forms.Utilities.AudioEditor audioIn)
		{
			InitializeComponent();
			audio = audioIn;
			if(!audio.cats.Contains(0)) comboBox1.Items.Add("Overworld");
			if(!audio.cats.Contains(1)) comboBox1.Items.Add("Nether");
			if(!audio.cats.Contains(2)) comboBox1.Items.Add("End");
			if(!audio.cats.Contains(3)) comboBox1.Items.Add("Creative");
			if(!audio.cats.Contains(4)) comboBox1.Items.Add("Menu");
			if(!audio.cats.Contains(5)) comboBox1.Items.Add("Battle");
			if(!audio.cats.Contains(6)) comboBox1.Items.Add("Tumble");
			if(!audio.cats.Contains(7)) comboBox1.Items.Add("Glide");
			FormBorderStyle = FormBorderStyle.None;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			audio.cat = comboBox1.Text;
			if(comboBox1.SelectedIndex > -1) this.Close();
		}

		private void addCategory_Load(object sender, EventArgs e)
		{

		}

	}
}
