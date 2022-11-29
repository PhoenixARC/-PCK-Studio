using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using PckStudio.Classes.Utils;

namespace PckStudio.Forms.Utilities.Skins
{
	public partial class BoxEditor : MetroFramework.Forms.MetroForm
	{
		public bool saved = false;

		public string out_box;

		class BOX
		{
			public BOX(string inBOX)
			{
				string[] arguments = inBOX.Split(' ');

				try
				{
					parent = arguments[0];
					posX = float.Parse(arguments[1]);
					posY = float.Parse(arguments[2]);
					posZ = float.Parse(arguments[3]);
					sizeX = float.Parse(arguments[4]);
					sizeY = float.Parse(arguments[5]);
					sizeZ = float.Parse(arguments[6]);
					uvX = float.Parse(arguments[7]);
					uvY = float.Parse(arguments[8]);
					hideWithArmor = arguments[9] == "1";
					mirror = arguments[10] == "1";
					inflation = float.Parse(arguments[11]);
				}
				catch (IndexOutOfRangeException ex2)
				{
					// This is normal as some box values can have less parameters but no more than 12
					return;
				}
				catch (Exception ex)
				{
					parent = "invalid";
				}
			}

			public string parent;
			public float posX, posY, posZ;
			public float sizeX, sizeY, sizeZ;
			public float uvX, uvY;
			public bool hideWithArmor;
			public bool mirror;
			public float inflation;
		}

		public BoxEditor(string inBOX, bool hasInflation)
		{
			InitializeComponent();

			inflationUpDown.Enabled = hasInflation;

			BOX box = new BOX(inBOX);

			if (box.parent == "invalid" || !parentComboBox.Items.Contains(box.parent))
			{
				throw new Exception("Failed to parse BOX value");
			}

			parentComboBox.SelectedItem = parentComboBox.Items[parentComboBox.Items.IndexOf(box.parent)];
			PosXUpDown.Value = (decimal)box.posX;
			PosYUpDown.Value = (decimal)box.posY;
			PosZUpDown.Value = (decimal)box.posZ;
			SizeXUpDown.Value = (decimal)box.sizeX;
			SizeYUpDown.Value = (decimal)box.sizeY;
			SizeZUpDown.Value = (decimal)box.sizeZ;
			uvXUpDown.Value = (decimal)box.uvX;
			uvYUpDown.Value = (decimal)box.uvY;
			armorCheckBox.Checked = box.hideWithArmor;
			mirrorCheckBox.Checked = box.mirror;
			inflationUpDown.Value = (decimal)box.inflation;
		}

		private void closeButton_Click(object sender, EventArgs e)
		{
			out_box =
				parentComboBox.SelectedItem.ToString() + " " +
				PosXUpDown.Value.ToString() + " " +
				PosYUpDown.Value.ToString() + " " +
				PosZUpDown.Value.ToString() + " " +
				SizeXUpDown.Value.ToString() + " " +
				SizeYUpDown.Value.ToString() + " " +
				SizeZUpDown.Value.ToString() + " " +
				uvXUpDown.Value.ToString() + " " +
				uvYUpDown.Value.ToString() + " " +
				(armorCheckBox.Checked ? "1 " : "0 ") +
				(mirrorCheckBox.Checked ? "1 " : "0 ") +
				inflationUpDown.Value.ToString();
			saved = true;

			DialogResult = DialogResult.OK;

			Close();
		}
	}
}
