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

namespace PckStudio
{
    public partial class presetMeta : MetroFramework.Forms.MetroForm
    {
        PCK currentPCK;
        PCK.MineFile file;
        string appData = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/PCK Studio/";

        public presetMeta(PCK.MineFile fileIn, PCK currentPCKIn)
        {
            InitializeComponent();
            file = fileIn;
            currentPCK = currentPCKIn;
            FormBorderStyle = FormBorderStyle.None;
        }

        private void presetMeta_Load(object sender, EventArgs e)
        {
            listView1.View = View.Details;
            listView1.Columns.Add("Meta Presets", 135);

            if (Directory.Exists(Application.StartupPath + @"\plugins\presets\"))
            {
                Directory.Move(Application.StartupPath + @"\plugins\presets\", appData + "presets");
            }

            if (!Directory.Exists(appData + "presets"))
            {
                Directory.CreateDirectory(appData + "presets");
            }

            string filepath = appData + "presets";
            DirectoryInfo d = new DirectoryInfo(filepath);

            try
            {
                foreach (var file in d.GetFiles("*.txt"))
                {
                    ListViewItem preset = new ListViewItem();
                    preset.Text = file.Name.Remove(file.Name.Length - 4, 4);
                    preset.Tag = File.ReadAllText(file.FullName);
                    listView1.Items.Add(preset);
                }
            }
            catch (Exception)
            {

            }
            if (listView1.Items.Count == 0)
            {
                labelSearch.Visible = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count != 0)
            {
                try
                {

                    string entryName = "";
                    string entryValue = "";
                    bool entryStart = true;
                    int i = 0;
                    foreach (char entry in listView1.SelectedItems[0].Tag.ToString().ToList())
                    {
                        if (entry.ToString() != ":" && entry.ToString() != "\n" && entryStart == true)
                        {
                            entryName += entry.ToString();
                        }
                        else if (entry.ToString() != ":" && entry.ToString() != "\n" && entryStart == false)
                        {
                            entryValue += entry.ToString();
                        }
                        else if (entry.ToString() == ":" && entryStart == true)
                        {
                            entryStart = false;
                        }
                        else
                        {
                            object[] ENTRY = { entryName, entryValue };
                            file.entries.Add(ENTRY);
                            entryName = "";
                            entryValue = "";
                            entryStart = true;
                        }
                        this.Close();
                    }
                }catch (Exception)
                {

                }
            }

        }
    }
}
