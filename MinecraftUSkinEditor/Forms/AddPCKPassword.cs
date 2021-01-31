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

namespace minekampf.Forms
{
    public partial class AddPCKPassword : MetroForm
    {
        MinecraftUSkinEditor.PCK currentPCK;
        MinecraftUSkinEditor.PCK.MineFile file;
        public AddPCKPassword(MinecraftUSkinEditor.PCK.MineFile fileIn, MinecraftUSkinEditor.PCK currentPCKIn)
        {

            InitializeComponent();
            file = fileIn;
            currentPCK = currentPCKIn;
        }

        private void buttonUnlocked_Click(object sender, EventArgs e)
        {
            object[] obj = { "LOCK", MD5(textBoxPass.Text) };
            file.entries.Add(obj);
            this.Close();
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
