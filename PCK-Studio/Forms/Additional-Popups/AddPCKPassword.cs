using System;
using System.Text;
using MetroFramework.Forms;

namespace PckStudio.Forms
{
    public partial class AddPCKPassword : MetroForm
    {
        public string Password { get; private set; }
        public AddPCKPassword()
        {
            InitializeComponent();
        }

        private void buttonUnlocked_Click(object sender, EventArgs e)
        {
            Password = MD5(textBoxPass.Text);
            Close();
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
    }
}
