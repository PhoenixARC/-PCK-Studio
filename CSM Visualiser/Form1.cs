using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MoreLinq.Extensions;
using MoreLinq;
using System.Linq;
using System.IO;

namespace CSM_Visualiser
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

            string intro = "{\n	\"format_version\": \"1.12.0\",\n	\"minecraft:geometry\": [\n		{\n			\"description\": {\n				\"identifier\": \"geometry.steve\",\n				\"texture_width\": 64,\n				\"texture_height\": 64,\n				\"visible_bounds_width\": 3,\n				\"visible_bounds_height\": 3,\n				\"visible_bounds_offset\": [0, 0, 0]\n			},\n			\"bones\": [";
            string outro = "\n			]\n		}\n	]\n}";
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Custom Skin Model File | *.CSM";
            openFileDialog.Title = "Select Custom Skin Model File";
            if(openFileDialog.ShowDialog() == DialogResult.OK)
            {
                System.IO.File.WriteAllText(openFileDialog.FileName + ".json", intro);
                string data = System.IO.File.ReadAllText(openFileDialog.FileName);
                int splitnum = 11;
                var dat0 = stringfunctions.SplitBy(data, '\n', splitnum);

                foreach (string str in dat0)
                {
                   // MessageBox.Show(str.ToString());
                    string[] data1 = str.ToString().Split('\n');
                    string name = data1[0];
                    string parent = data1[1];
                    string name2 = data1[2];
                    int PosX = int.Parse(data1[3])*3;
                    int PosY = int.Parse(data1[4]) + 32;
                    int PosZ = int.Parse(data1[5])*3;
                    int SizeX = int.Parse(data1[6]);
                    int SizeY = int.Parse(data1[7]);
                    int SizeZ = int.Parse(data1[8]);
                    int UVx = int.Parse(data1[9]);
                    int UVy = int.Parse(data1[10]);
                    int UVx2 = UVx + 32;
                    string entry = "\n\t\t\t\t{\n\t\t\t\t\t\"name\": \"" + name + "\"\x2C\n\t\t\t\t\t\"pivot\": [0, 24, 0],\n\t\t\t\t\t\"cubes\": [\n\t\t\t\t\t\t{\"origin\": [" + PosX + ", " + PosY + ", " + PosZ + "], \"size\": [" + SizeX + ", " + SizeY + ", " + SizeZ + "], \"uv\": [" + UVx + ", " + UVy + "]},\n\t\t\t\t\t\t{\"origin\": [" + PosX + ", " + PosY + ", " + PosZ + "], \"size\": [" + SizeX + ", " + SizeY + ", " + SizeZ + "], \"inflate\": 0.5, \"uv\": [" + UVx2 + ", " + UVy + "]}\n\t\t\t\t\t]\n\t\t\t\t},";
                    System.IO.File.AppendAllText(openFileDialog.FileName + ".json", entry.Replace("\n\"\x2C", "\"\x2C"));
                }
                System.IO.FileStream fs = new System.IO.FileStream(openFileDialog.FileName + ".json", System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite);
                fs.SetLength(fs.Length - 1);
                fs.Close();
                System.IO.File.AppendAllText(openFileDialog.FileName + ".json", outro);
                MessageBox.Show("\x4D\x65\x73\x73\x61\x67\x65\x42\x6F\x78\x2E\x53\x68\x6F\x77\x28\x29\x3B");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Custom Skin Model File | *.txt";
                openFileDialog.Title = "Select Custom Skin Model File";
                openFileDialog.Multiselect = true;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    foreach (string url in openFileDialog.FileNames)
                    {
                        StreamWriter sw = new StreamWriter(url + ".CSM");
                        sw.Write("");
                        sw.Close();
                        string data = System.IO.File.ReadAllText(url);
                        int splitnum = 11;
                        string[] data0 = data.Split('\n');

                        foreach (string str in data0)
                        {
                            if (str.StartsWith("BOX"))
                            {
                                string[] data1 = str.ToString().Split(' ');
                                string name = data1[0];
                                string PosX = (data1[1]);
                                string PosY = (data1[2]);
                                string PosZ = (data1[3]);
                                string SizeX = (data1[4]);
                                string SizeY = (data1[5]);
                                string SizeZ = (data1[6]);
                                string UVx = (data1[7]);
                                string UVy = (data1[8]);
                                string entry = name + "\n" + name + "\n" + name + "\n" + PosX + "\n" + PosY + "\n" + PosZ + "\n" + SizeX + "\n" + SizeY + "\n" + SizeZ + "\n" + UVx + "\n" + UVy + "\n";
                                System.IO.File.AppendAllText(url + ".CSM", entry.Replace("BOX:", ""));
                            }
                        }
                    }
                }
        }
    }
    static class stringfunctions
        {
        public static IEnumerable<string> SplitBy(string input, char separator, int n)
        {
            int lastindex = 0;
            int curr = 0;

            while (curr < input.Length)
            {
                int count = 0;
                while (curr < input.Length && count < n)
                {
                    if (input[curr++] == separator) count++;
                }
                yield return input.Substring(lastindex, curr - lastindex - (curr < input.Length ? 1 : 0));
                lastindex = curr;
            }
        }
    }
    
}
