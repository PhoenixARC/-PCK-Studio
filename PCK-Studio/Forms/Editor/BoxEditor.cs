using System;
using System.Windows.Forms;
using PckStudio.Classes.Models;

namespace PckStudio.Forms.Editor
{
	public partial class BoxEditor : MetroFramework.Forms.MetroForm
	{
		public string Result;

		public BoxEditor(string inBOX, bool hasInflation)
		{
			InitializeComponent();

			inflationUpDown.Enabled = hasInflation;

			SkinBox box = new SkinBox(inBOX);

			if (string.IsNullOrEmpty(box.Type) || !parentComboBox.Items.Contains(box.Type))
			{
				throw new Exception("Failed to parse BOX value");
			}

			parentComboBox.SelectedItem = parentComboBox.Items[parentComboBox.Items.IndexOf(box.Type)];
			PosXUpDown.Value = (decimal)box.Pos.X;
			PosYUpDown.Value = (decimal)box.Pos.Y;
			PosZUpDown.Value = (decimal)box.Pos.Z;
			SizeXUpDown.Value = (decimal)box.Size.X;
			SizeYUpDown.Value = (decimal)box.Size.Y;
			SizeZUpDown.Value = (decimal)box.Size.Z;
			uvXUpDown.Value = (decimal)box.U;
			uvYUpDown.Value = (decimal)box.V;
			armorCheckBox.Checked = box.HideWithArmor;
			mirrorCheckBox.Checked = box.Mirror;
			inflationUpDown.Value = (decimal)box.Scale;
		}

		private void saveButton_Click(object sender, EventArgs e)
		{
			Result =
				$"{parentComboBox.SelectedItem} " +
				$"{PosXUpDown.Value} {PosYUpDown.Value} {PosZUpDown.Value} " +
				$"{SizeXUpDown.Value} {SizeYUpDown.Value} {SizeZUpDown.Value} " +
				$"{uvXUpDown.Value} {uvYUpDown.Value} " +
				$"{Convert.ToInt32(armorCheckBox.Checked)} " +
				$"{Convert.ToInt32(mirrorCheckBox.Checked)} " +
				$"{inflationUpDown.Value}";
			DialogResult = DialogResult.OK;
			Close();
		}
	}
}
