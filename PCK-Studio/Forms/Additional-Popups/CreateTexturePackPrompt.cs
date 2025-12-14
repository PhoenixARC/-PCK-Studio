using System;
using System.Windows.Forms;
using PckStudio.Controls;
using PckStudio.Core.DLC;

namespace PckStudio
{
    public partial class CreateTexturePackPrompt : ImmersiveForm
	{
		/// <summary>
		/// Text entered <c>only access when DialogResult == DialogResult.OK</c>
		/// </summary>
		public string PackName => InputTextBox.Text;
		public string PackRes => resolutionComboBox.Text;

		public CreateTexturePackPrompt()
		{
			InitializeComponent();
            resolutionComboBox.Items.AddRange(Enum.GetNames(typeof(DLCTexturePackage.TextureResolution)));
		}

        private void OKBtn_Click(object sender, EventArgs e)
        {
			if (resolutionComboBox.SelectedIndex < 0)
				return;
			DialogResult = DialogResult.OK;
        }
    }
}
