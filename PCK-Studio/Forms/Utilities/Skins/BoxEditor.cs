using PckStudio.ToolboxItems;
using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace PckStudio.Forms.Utilities.Skins
{
	public partial class BoxEditor : ThemeForm
	{
		public string Result;

		class BOX
		{
			public string Parent;
			public (float X, float Y, float Z) Pos;
			public (float X, float Y, float Z) Size;
			public float uvX, uvY;
			public bool HideWithArmor;
			public bool Mirror;
			public float Inflation;

			public BOX(string input)
			{
				string[] arguments = Regex.Split(input, @"\s+");

				try
				{
					Parent = arguments[0];
					Pos.X = float.Parse(arguments[1]);
					Pos.Y = float.Parse(arguments[2]);
					Pos.Z = float.Parse(arguments[3]);
					Size.X = float.Parse(arguments[4]);
					Size.Y = float.Parse(arguments[5]);
					Size.Z = float.Parse(arguments[6]);
					uvX = float.Parse(arguments[7]);
					uvY = float.Parse(arguments[8]);
					HideWithArmor = arguments[9] == "1";
					Mirror = arguments[10] == "1";
					Inflation = float.Parse(arguments[11]);
				}
				catch (IndexOutOfRangeException)
				{
					// This is normal as some box values can have less parameters but no more than 12
					return;
				}
				catch (Exception ex)
				{
					Parent = string.Empty;
				}
			}

		}

        public BoxEditor(string inBOX, bool hasInflation)
		{
			InitializeComponent();

			inflationUpDown.Enabled = hasInflation;

			BOX box = new BOX(inBOX);

			if (string.IsNullOrEmpty(box.Parent) || !parentComboBox.Items.Contains(box.Parent))
			{
				throw new Exception("Failed to parse BOX value");
			}

			parentComboBox.SelectedItem = parentComboBox.Items[parentComboBox.Items.IndexOf(box.Parent)];
			PosXUpDown.Value = (decimal)box.Pos.X;
			PosYUpDown.Value = (decimal)box.Pos.Y;
			PosZUpDown.Value = (decimal)box.Pos.Z;
			SizeXUpDown.Value = (decimal)box.Size.X;
			SizeYUpDown.Value = (decimal)box.Size.Y;
			SizeZUpDown.Value = (decimal)box.Size.Z;
			uvXUpDown.Value = (decimal)box.uvX;
			uvYUpDown.Value = (decimal)box.uvY;
			armorCheckBox.Checked = box.HideWithArmor;
			mirrorCheckBox.Checked = box.Mirror;
			inflationUpDown.Value = (decimal)box.Inflation;
		}

		private void saveButton_Click(object sender, EventArgs e)
		{
			Result =
				$"{parentComboBox.SelectedItem} " +
				$"{PosXUpDown.Value} {PosYUpDown.Value} {PosZUpDown.Value} " +
				$"{SizeXUpDown.Value} {SizeYUpDown.Value} {SizeZUpDown.Value} " +
				$"{uvXUpDown.Value} {uvYUpDown.Value} " +
				$"{(mirrorCheckBox.Checked ? "1 " : "0 ")} {(mirrorCheckBox.Checked ? "1 " : "0 ")} " +
				$"{inflationUpDown.Value}";
			DialogResult = DialogResult.OK;
			Close();
		}
	}
}
