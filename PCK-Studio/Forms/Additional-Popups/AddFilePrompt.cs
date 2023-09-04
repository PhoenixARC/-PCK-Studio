using System;
using System.Windows.Forms;
using PckStudio.ToolboxItems;
using MetroFramework.Forms;
using OMI.Formats.Pck;

namespace PckStudio.Popups
{
    public partial class AddFilePrompt : ThemeForm
	{
        /// <summary>
        /// Text entered only valid when <see cref="DialogResult"/> == <see cref="DialogResult.OK"/>,
        /// otherwise <see cref="string.Empty"/>
        /// </summary>
        public string Filepath => DialogResult == DialogResult.OK ? InputTextBox.Text : string.Empty;
		public PckFileType Filetype => (PckFileType)FileTypeComboBox.SelectedIndex;

		public AddFilePrompt(string initialText) : this(initialText, -1)
		{ }

		public AddFilePrompt(string initialText, int maxPathLength)
		{
			InitializeComponent();
			InputTextBox.Text = initialText;
			InputTextBox.MaxLength = maxPathLength < 0 ? short.MaxValue : maxPathLength;
		}

        private void OKBtn_Click(object sender, EventArgs e)
        {
			if(FileTypeComboBox.SelectedIndex > -1)
				DialogResult = DialogResult.OK;
        }

        private void InputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
			if (e.KeyCode == Keys.Enter)
				OKBtn_Click(sender, e);
        }
    }
}
