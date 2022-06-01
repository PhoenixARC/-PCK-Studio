using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework.Forms;
using PckStudio.Classes.FileTypes;

namespace PckStudio.Forms
{
    public partial class AddPCKPassword : MetroForm
    {
        PCKFile currentPCK;
        PCKFile.FileData file;
        public AddPCKPassword(PCKFile.FileData fileIn, PCKFile currentPCKIn)
        {
            InitializeComponent();
            file = fileIn;
            currentPCK = currentPCKIn;
        }

        private void buttonUnlocked_Click(object sender, EventArgs e)
        {
            file.properties.Add("LOCK", MD5(textBoxPass.Text));
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
