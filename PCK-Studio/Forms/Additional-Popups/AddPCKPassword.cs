using System;
using System.Text;
using System.Windows.Forms;
using MetroFramework.Forms;

namespace PckStudio.Forms
{
    public partial class AddPCKPassword : Form
    {
        public string Password { get; private set; }
        public AddPCKPassword()
        {
            InitializeComponent();
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

        private void AddPCKPassword_Load(object sender, EventArgs e)
        {

        }

        private void LockPCKButton_Click(object sender, EventArgs e)
        {
            Password = MD5(textBoxPass.Text);
            Close();
        }
    }
}
