using System;
using System.IO;
using System.Windows.Forms;
using PckStudio.Classes.ToolboxItems;

namespace PckStudio.Forms
{
    public partial class PCK_Manager : ThemeForm
    {
        public PCK_Manager()
        {
            InitializeComponent();
        }

        private void metroButton5_Click(object sender, EventArgs e)
        {
            string nom = metroTextBox2.Text;
            string pckurl = metroTextBox3.Text;
            string pckimg = metroTextBox4.Text;
            string DLUrl = metroTextBox5.Text;
            string auth = metroTextBox6.Text;
            string desc = metroTextBox7.Text.Replace("\n","\\n");

            dataGridView1.Rows.Add(nom, DLUrl, auth, desc);
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                using (DataGridViewRow dr = dataGridView1.SelectedRows[0])
                {
                    if (dr.Cells[0].Value != null && dr.Cells[1].Value != null)
                    {
                        dataGridView1.Rows.Remove(dr);
                        string filenom = (dr.Cells[0].Value.ToString()).Replace(" ", "");
                        File.Delete(metroTextBox1.Text + "\\mod\\pcks\\" + filenom + ".pck");
                        File.Delete(metroTextBox1.Text + "\\mod\\pcks\\" + filenom + ".png");
                        File.Delete(metroTextBox1.Text + "\\mod\\pcks\\" + filenom + ".desc");
                        File.WriteAllText(metroTextBox1.Text + "\\" + metroComboBox1.SelectedItem.ToString() + ".txt", File.ReadAllText(metroTextBox1.Text + "\\" + metroComboBox1.SelectedItem.ToString() + ".txt").Replace("\n" + filenom, ""));
                        File.WriteAllText(metroTextBox1.Text + "\\" + metroComboBox1.SelectedItem.ToString() + ".txt", File.ReadAllText(metroTextBox1.Text + "\\" + metroComboBox1.SelectedItem.ToString() + ".txt").Replace(filenom, ""));
                    }
                }
            }
            catch
            {

            }
        }

        private void metroButton2_Click(object sender, EventArgs e)
        {
            try
            {
                if (!File.Exists(metroTextBox1.Text + "\\" + metroComboBox1.Text + ".txt"))
                {
                    File.Create(metroTextBox1.Text + "\\" + metroComboBox1.Text + ".txt");
                    Directory.CreateDirectory(metroTextBox1.Text + "\\mod\\pcks");
                }

                Console.WriteLine(metroTextBox1.Text + "\\" + metroComboBox1.SelectedItem.ToString() + ".txt");
                Console.WriteLine(metroTextBox1.Text + "\\" + metroComboBox1.Text + ".txt");
               string data = File.ReadAllText(metroTextBox1.Text + "\\" + metroComboBox1.SelectedItem.ToString() + ".txt");
                foreach(string pack in data.Split(new[] { "\n", "\r\n"}, StringSplitOptions.None))
                {
                    if (!string.IsNullOrWhiteSpace(pack))
                    {
                        string loaded = File.ReadAllText(metroTextBox1.Text + "\\mod\\pcks\\" + pack + ".desc");
                        string[] loadedx = loaded.Split(new[] { "\n", "\r\n" }, StringSplitOptions.None);

                        string nom = loadedx[0];
                        string auth = loadedx[1];
                        string desc = loadedx[2];
                        string dlurl = loadedx[3];
                        dataGridView1.Rows.Add(nom, dlurl, auth, desc);
                    }
                }
                    metroPanel1.Enabled = true;
            }
            catch
            {

            }
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if(fbd.ShowDialog() == DialogResult.OK)
            {
                dataGridView1.Rows.Clear();
                metroTextBox1.Text = fbd.SelectedPath;
            }
        }

        private void metroButton3_Click(object sender, EventArgs e)
        {
                OpenFileDialog opd = new OpenFileDialog();
                opd.Filter = "PCK Files | *.pck";
                if (opd.ShowDialog() == DialogResult.OK)
                {
                    metroTextBox5.Text = File.ReadAllText(Environment.CurrentDirectory + "\\settings.ini").Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)[1] + "mod/pcks/" + metroTextBox2.Text.Replace(" ", "") + ".pck";
                    metroTextBox3.Text = opd.FileName;
                    File.Copy(opd.FileName, metroTextBox1.Text + "\\mod\\pcks\\" + metroTextBox2.Text.Replace(" ", "") + ".pck", true);
                }
        }

        private void metroComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
        }

        private void metroButton6_Click(object sender, EventArgs e)
        {
            string listdata = "";
            foreach (DataGridViewRow dr in dataGridView1.Rows)
            {
                string descdat = "";
                try
                {
                    if (dr.Cells[0] != null)
                        //listdata += dr.Cells[0] + "\n";

                    if (dr.Cells[0].Value != null && dr.Cells[1].Value != null)
                    {
                        string contentValue1 = dr.Cells[0].Value.ToString();
                        string contentValue2 = dr.Cells[1].Value.ToString();
                        string contentValue3 = dr.Cells[2].Value.ToString();
                        string contentValue4 = dr.Cells[3].Value.ToString();
                            listdata += contentValue1.Replace(" ","");
                            descdat = contentValue1 + "\n" + contentValue3 + "\n" + contentValue4 + "\n" + contentValue2 + "\nadline";
                            File.WriteAllText((metroTextBox1.Text + "\\mod\\pcks\\" + contentValue1.Replace(" ", "") + ".desc"), descdat);
                    }
                }
                catch
                {

                }
            }
            File.WriteAllText((metroTextBox1.Text + "\\" + metroComboBox1.SelectedItem.ToString() + ".txt"), listdata);
        }

        private void metroButton4_Click(object sender, EventArgs e)
        {

            OpenFileDialog opd = new OpenFileDialog();
            opd.Filter = "PNG Files | *.png";
            if (opd.ShowDialog() == DialogResult.OK)
            {
                metroTextBox4.Text = opd.FileName;
                File.Copy(opd.FileName, metroTextBox1.Text + "\\mod\\pcks\\" + metroTextBox2.Text.Replace(" ", "") + ".png", true);
            }
        }
    }
}
