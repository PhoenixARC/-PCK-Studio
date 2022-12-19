using PckStudio.ToolboxItems;
using System;
using System.Text;
using System.Windows.Forms;

namespace PckStudio.Forms
{
    public partial class LockPrompt : ThemeForm
    {
        string pass;

        public LockPrompt(string pass)
        {
            this.pass = pass;
            InitializeComponent();
        }

        private void textBoxPass_Click(object sender, EventArgs e)
        {
        }

        private void textBoxPass_TextChanged(object sender, EventArgs e)
        {
        }

        private void textBoxPass_Enter(object sender, EventArgs e)
        {
        }

        public string MD5(string s)
        {
            using (var provider = System.Security.Cryptography.MD5.Create())
            {
                StringBuilder builder = new StringBuilder();

                foreach (byte b in provider.ComputeHash(Encoding.UTF8.GetBytes(s)))
                    builder.Append(b.ToString("x2").ToLower());

                return builder.ToString();
            }
        }

        private void UnlockPCKButton_Click(object sender, EventArgs e)
        {
            if (MD5(textBoxPass.Text) == pass || MD5(textBoxPass.Text) == MD5(pass))
            {
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show("Incorrect password!");
                DialogResult = DialogResult.Abort;
                textBoxPass.Text = "";
            }
            #if DEBUG
            DialogResult = DialogResult.OK;
            #endif
        }
    }
}
