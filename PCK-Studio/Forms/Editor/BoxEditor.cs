using System;
using System.Numerics;
using System.Windows.Forms;
using PckStudio.Controls;
using PckStudio.Core.Skin;
using PckStudio.Properties;

namespace PckStudio.Forms.Editor
{
	public partial class BoxEditor : ImmersiveForm
	{
		private SkinBOX result;
		public SkinBOX Result => result;

        public BoxEditor(string formattedBoxString, bool hasInflation)
			: this(SkinBOX.FromString(formattedBoxString), hasInflation)
		{
		}

		public BoxEditor(SkinBOX box, bool hasInflation)
		{
			InitializeComponent();
			boxVisibilityComboBox.Items.AddRange(Enum.GetNames(typeof(SkinBOX.BoxVisibility)));

            if (string.IsNullOrEmpty(box.Type) || !parentComboBox.Items.Contains(box.Type))
            {
                throw new Exception("Failed to parse BOX value");
            }

            closeButton.Visible = !Settings.Default.AutoSaveChanges;

            inflationUpDown.Enabled = hasInflation;

			parentComboBox.SelectedIndex = parentComboBox.Items.IndexOf(box.Type);
			PosXUpDown.Value = (decimal)box.Position.X;
			PosYUpDown.Value = (decimal)box.Position.Y;
			PosZUpDown.Value = (decimal)box.Position.Z;
			SizeXUpDown.Value = (decimal)box.Size.X;
			SizeYUpDown.Value = (decimal)box.Size.Y;
			SizeZUpDown.Value = (decimal)box.Size.Z;
			uvXUpDown.Value = (decimal)box.Uv.X;
			uvYUpDown.Value = (decimal)box.Uv.Y;
			boxVisibilityComboBox.SelectedItem = Enum.GetName(typeof(SkinBOX.BoxVisibility), box.Visibility);
			mirrorCheckBox.Checked = box.Mirror;
			inflationUpDown.Value = (decimal)box.Inflate;
		}

		private void saveButton_Click(object sender, EventArgs e)
		{
            SkinBOX.BoxVisibility visibility = Enum.TryParse(boxVisibilityComboBox.SelectedItem?.ToString(), out SkinBOX.BoxVisibility v) ? v : default;
			Vector3 pos = new Vector3((float)PosXUpDown.Value, (float)PosYUpDown.Value, (float)PosZUpDown.Value);
			Vector3 size = new Vector3((float)SizeXUpDown.Value, (float)SizeYUpDown.Value, (float)SizeZUpDown.Value);
			Vector2 uv = new Vector2((int)uvXUpDown.Value, (int)uvYUpDown.Value);
            result = new SkinBOX(parentComboBox.SelectedItem.ToString(), pos, size, uv, visibility, mirrorCheckBox.Checked, (float)inflationUpDown.Value);
			DialogResult = DialogResult.OK;
		}

        private void BoxEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
			if (Settings.Default.AutoSaveChanges)
			{
				saveButton_Click(sender, EventArgs.Empty);
			}
        }
    }
}
