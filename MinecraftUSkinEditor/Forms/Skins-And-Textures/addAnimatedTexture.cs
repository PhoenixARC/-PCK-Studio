using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Linq.Expressions;

namespace MinecraftUSkinEditor
{
    public partial class addAnimatedTexture : MetroFramework.Forms.MetroForm
    {
        PCK currentPCK;
        TreeView treeView1;
        TreeNode texture = new TreeNode();
        PCK.MineFile mf = new PCK.MineFile();
        PCK.MineFile mfc = new PCK.MineFile();
        string ofd;
        bool useCape = false;
        int loop = 0;
        int i = 0;
        string data;
        int speed;

        public addAnimatedTexture(PCK currentPCKIn, TreeView treeView1In, string ofdIn, string name)
        {
            InitializeComponent();

            textBox1.Text = name;

            currentPCK = currentPCKIn;
            treeView1 = treeView1In;
            ofd = ofdIn;
            
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.InterpolationMode = InterpolationMode.NearestNeighbor;
            pictureBox1.Image = Image.FromFile(ofd);

            mf.data = File.ReadAllBytes(ofd);

        }

        public class displayId
        {
            public string id;
            public string defaultName;
        }
        
        private void button1_Click_1(object sender, EventArgs e)
        {
             try{
                int frames = int.Parse(textBox2.Text);

                speed = int.Parse(textBox3.Text);

                data = "0*" + speed + ",";

                loop = frames - 1;

                generateANIM();

                object[] ANIM = {"ANIM", data};
                mf.entries.Add(ANIM);

                string path = "";

                if (radioButton1.Checked == true)
                {
                    path = "res/textures/blocks/" + textBox1.Text + ".png";
                }
                else
                {
                    path = "res/textures/items/" + textBox1.Text + ".png";
                }

                mf.filesize = mf.data.Length;
                mf.name = path;
                mf.type = 0;

                currentPCK.mineFiles.Add(mf);
                texture.Text = path;
                texture.Tag = mf;
                treeView1.Nodes.Insert(17, texture);

                this.Close();
            }catch (Exception)
             {
                 MessageBox.Show("Invalid values were entered");
             }
        }

        private void generateANIM()
        {
            do
            {
                    i += 1;
                    data += i + "*" + speed + ",";
                loop -= 1;
            } while (loop != 0);
        }

        private void addAnimatedTexture_Load(object sender, EventArgs e)
        {

        }
    }
}


