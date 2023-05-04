using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.IO.Packaging;
using PckStudio;
using System.IO.Compression;
using PckStudio.Classes.FileTypes;
using PckStudio.Classes.IO.PCK;
using OMI.Formats.Pck;
using OMI.Workers.Pck;
using PckStudio.Extensions;
using PckStudio.ToolboxItems;

namespace PckStudio.Forms
{
    public partial class PCKCenterOpen : ThemeForm
    {
        string name;
        string author;
        string desc;
        string direct;
        string ad;
        int mode = 0;
        string mod;
        MethodInvoker reloader;
        bool IsVita;
        string Pack;

        public class Item
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }

        public PCKCenterOpen(string name, string authorIn, string descIn, string directIn, string adIn, Bitmap display, int mode, string mod, MethodInvoker reloader, bool Vita, string PackName)
        {
            InitializeComponent();
            pictureBoxDisplay.Image = display;

            this.reloader = reloader;
            this.mode = mode;
            this.mod = mod;
            this.reloader = reloader;

            this.name = name;
            author = authorIn;
            desc = descIn;
            direct = directIn;
            ad = adIn;
            IsVita = Vita;
            Pack = PackName;
        }

        private void pckCenterOpen_Load(object sender, EventArgs e)
        {
            if (mode == 0) // Unowned Mode
            {
                buttonDirect.Visible = true;

                if (File.Exists(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/PCK Studio/PCK Center/myPcks/" + direct + ".pck"))
                {
                    buttonDirect.Enabled = false;
                    buttonDirect.Text = "Already in Collection";
                    buttonDirect.Font = new Font("Segoe UI", 8, FontStyle.Bold);
                    buttonDirect.ForeColor = Color.White;
                }
                buttonBedrock.Visible = buttonDelete.Visible = buttonExport.Visible = false;
                buttonInstallPs3.Visible = buttonInstallXbox.Visible = buttonInstallWiiU.Visible = false;
            }
            else if (mode == 1) // My Collection Mode
            {
                buttonBedrock.Visible = true;
                buttonInstallPs3.Visible = true;
                buttonInstallXbox.Visible = true;
                buttonInstallWiiU.Visible = true;
                buttonDelete.Visible = true;
                buttonExport.Visible = true;
                buttonDirect.Visible = false;
            }
            if (IsVita)
            {
                buttonBedrock.Visible = false;
                buttonDelete.Visible = false;
                buttonExport.Visible = false;
                buttonInstallPs3.Visible = false;
                buttonInstallXbox.Visible = false;
                buttonInstallWiiU.Visible = false;
            }

            labelName.Text = name;
            labelDesc.Text = desc;
            if(IsVita)
            labelDesc.Text += "\nPS4 / PSVita PCK";
            if(IsVita)
            labelDesc.Text += "\nPack: " + Pack;
            labelDesc.Text += Environment.NewLine + Environment.NewLine + "Creator: " + author;
        }

        private void buttonDirect_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(direct);
        }
        //converts and ports all skins in pck to mc bedrock format
        private void convertToBedrockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string packName = mod;//Determines skin packs name off of pck file name

                //Lets user choose were to put generated pack
                SaveFileDialog convert = new SaveFileDialog();
                convert.Filter = "PCK (Minecarft Bedrock DLC)|*.mcpack";
                convert.FileName = packName;

                if (convert.ShowDialog() == DialogResult.OK)
                {
                    //creates directory for conversion
                    string root = Path.GetDirectoryName(convert.FileName) + "\\" + packName;
                    string rootFinal = Path.GetDirectoryName(convert.FileName) + "\\";

                    //creates pack uuid off of the last skin id detected
                    string uuid = "99999999";//default

                    //creates list of skin display names
                    List<Item> skinDisplayNames = new List<Item>();

                    //MessageBox.Show(root);//debug thingy to make sure filepath is correct

                    //add all skins to a list
                    List<PckFile.FileData> skinsList = new List<PckFile.FileData>();
                    List<PckFile.FileData> capesList = new List<PckFile.FileData>();

                    PckFile pck = null;

                    var reader = new PckFileReader();
                    PckFile currentPCK = reader.FromFile(Program.AppData + "/PCK-Center/myPcks/" + mod + ".pck");
                    foreach (PckFile.FileData skin in currentPCK.Files)
                    {
                        if (skin.Filename.Count() == 19)
                        {
                            if (skin.Filename.Remove(7, skin.Filename.Count() - 7) == "dlcskin")
                            {
                                skinsList.Add(skin);
                                uuid = skin.Filename.Remove(12, 7);
                                uuid = uuid.Remove(0, 7);
                                uuid = "abcdefa" + uuid;
                            }
                            if (skin.Filename.Remove(7, skin.Filename.Count() - 7) == "dlccape")
                            {
                                capesList.Add(skin);
                            }
                        }
                    }

                    if (skinsList.Count() == 0)
                    {
                        MessageBox.Show("No skins were found");
                        return;
                    }

                    Directory.CreateDirectory(root);//Creates directory for skin pack
                    Directory.CreateDirectory(root + "/texts");//create directory for skin pack text files

                    //create skins json file
                    using (StreamWriter writeSkins = new StreamWriter(root + "/skins.json"))
                    {
                        writeSkins.WriteLine("{");
                        writeSkins.WriteLine("  \"skins\": [");

                        int skinAmount = 0;
                        foreach (PckFile.FileData newSkin in skinsList)
                        {
                            skinAmount += 1;
                            string skinName = "skinName";
                            string capePath = "";
                            bool hasCape = false;

                            foreach (var entry in newSkin.Properties)
                            {
                                if (entry.Item1 == "DISPLAYNAME")
                                {
                                    skinName = entry.Item2;
                                    skinDisplayNames.Add(new Item() { Id = newSkin.Filename.Remove(15, 4), Name = entry.Item2 });
                                }
                                if (entry.Item1 == "CAPEPATH")
                                {
                                    hasCape = true;
                                    capePath = entry.Item2;
                                }
                            }

                            writeSkins.WriteLine("    {");
                            writeSkins.WriteLine("      \"localization_name\": " + "\"" + newSkin.Filename.Remove(15, 4) + "\",");

                            MemoryStream png = new MemoryStream(newSkin.Data); //Gets image data from minefile data
                            Image skinPicture = Image.FromStream(png); //Constructs image data into image
                            if (skinPicture.Height == skinPicture.Width)
                            {
                                writeSkins.WriteLine("      \"geometry\": \"geometry." + packName + "." + newSkin.Filename.Remove(15, 4) + "\",");
                            }
                            writeSkins.WriteLine("      \"texture\": " + "\"" + newSkin.Filename + "\",");
                            if (hasCape == true)
                            {
                                writeSkins.WriteLine("      \"cape\":" + "\"" + capePath + "\",");
                            }
                            writeSkins.WriteLine("      \"type\": \"free\"");
                            if (skinAmount != skinsList.Count)
                            {
                                writeSkins.WriteLine("    },");
                            }
                            else
                            {
                                writeSkins.WriteLine("    }");
                            }
                        }

                        writeSkins.WriteLine("  ],");
                        writeSkins.WriteLine("  \"serialize_name\": \"" + packName + "\",");
                        writeSkins.WriteLine("  \"localization_name\": \"" + packName + "\"");
                        writeSkins.WriteLine("}");
                    }

                    //Create geometry file
                    using (StreamWriter writeSkins = new StreamWriter(root + "/geometry.json"))
                    {
                        writeSkins.WriteLine("{");
                        int newSkinCount = 0;
                        foreach (PckFile.FileData newSkin in skinsList)
                        {

                            newSkinCount += 1;
                            string skinType = "steve";
                            MemoryStream png = new MemoryStream(newSkin.Data); //Gets image data from minefile data
                            Image skinPicture = Image.FromStream(png); //Constructs image data into image

                            if (skinPicture.Height == skinPicture.Width / 2)
                            {
                                skinType = "64x32";
                                continue;
                            }

                            double offsetHead = 0;
                            double offsetBody = 0;
                            double offsetArms = 0;
                            double offsetLegs = 0;

                            //creates list of skin model data
                            List<Item> modelDataHead = new List<Item>();
                            List<Item> modelDataBody = new List<Item>();
                            List<Item> modelDataLeftArm = new List<Item>();
                            List<Item> modelDataRightArm = new List<Item>();
                            List<Item> modelDataLeftLeg = new List<Item>();
                            List<Item> modelDataRightLeg = new List<Item>();
                            List<Item> modelData = new List<Item>();


                            if (skinPicture.Height == skinPicture.Width)
                            {
                                //determines skin type based on image dimensions, existence of BOX tags, and the ANIM value
                                foreach (var entry in newSkin.Properties)
                                {
                                    if (entry.Item1.ToString() == "BOX")
                                    {
                                        string mClass = "";
                                        string mData = "";
                                        foreach (char dCheck in entry.Item2)
                                        {
                                            if (dCheck.ToString() != " ")
                                            {
                                                mClass += dCheck.ToString();
                                            }
                                            else
                                            {
                                                mData = entry.Item2.Remove(0, mClass.Count() + 1);
                                                break;
                                            }
                                        }

                                        if (mClass == "HEAD")
                                        {
                                            mClass = "head";
                                            modelDataHead.Add(new Item() { Id = mClass, Name = mData });
                                        }
                                        else if (mClass == "BODY")
                                        {
                                            mClass = "body";
                                            modelDataBody.Add(new Item() { Id = mClass, Name = mData });
                                        }
                                        else if (mClass == "ARM0")
                                        {
                                            mClass = "rightArm";
                                            modelDataRightArm.Add(new Item() { Id = mClass, Name = mData });
                                        }
                                        else if (mClass == "ARM1")
                                        {
                                            mClass = "leftArm";
                                            modelDataLeftArm.Add(new Item() { Id = mClass, Name = mData });
                                        }
                                        else if (mClass == "LEG0")
                                        {
                                            mClass = "leftLeg";
                                            modelDataLeftLeg.Add(new Item() { Id = mClass, Name = mData });
                                        }
                                        else if (mClass == "LEG1")
                                        {
                                            mClass = "rightLeg";
                                            modelDataRightLeg.Add(new Item() { Id = mClass, Name = mData });
                                        }
                                    }

                                    if (entry.Item1 == "OFFSET")
                                    {
                                        string oClass = "";
                                        string oData = "";
                                        foreach (char oCheck in entry.Item2)
                                        {
                                            oData = entry.Item2;
                                            if (oCheck.ToString() != " ")
                                            {
                                                oClass += oCheck.ToString();
                                            }
                                            else
                                            {
                                                break;
                                            }

                                            if (oClass == "HEAD")
                                            {
                                                offsetHead += Double.Parse(oData.Remove(0, 7)) * -1;
                                            }
                                            else if (oClass == "BODY")
                                            {
                                                offsetBody += Double.Parse(oData.Remove(0, 7)) * -1;
                                            }
                                            else if (oClass == "ARM0")
                                            {
                                                offsetArms += Double.Parse(oData.Remove(0, 7)) * -1;
                                            }
                                            else if (oClass == "LEG0")
                                            {
                                                offsetLegs += Double.Parse(oData.Remove(0, 7)) * -1;
                                            }
                                        }
                                    }

                                    if (entry.Item1 == "ANIM")
                                    {
                                        if (entry.Item2 == "0x40000")
                                        {

                                        }
                                        else if (entry.Item2 == "0x80000")
                                        {
                                            skinType = "alex";
                                        }
                                    }
                                }

                                if (modelDataHead.Count + modelDataBody.Count + modelDataLeftArm.Count + modelDataRightArm.Count + modelDataLeftLeg.Count + modelDataRightLeg.Count > 0)
                                {
                                    skinType = "custom";
                                }
                            }

                            writeSkins.WriteLine("  \"" + "geometry." + packName + "." + newSkin.Filename.Remove(15, 4) + "\": {");

                            //makes skin model depending on what skin type the skin is
                            if (skinType == "custom")
                            {
                                writeSkins.WriteLine("    \"bones\": [");

                                //Head Data
                                writeSkins.WriteLine("      {");
                                writeSkins.WriteLine("        \"pivot\": [ 0, 24, 0 ],");
                                writeSkins.WriteLine("         \"rotation\": [ 0, 0, 0 ],");
                                writeSkins.WriteLine("          \"cubes\": [ ");
                                //Creates bones for each head box
                                int modelAmount = 0;
                                foreach (Item model in modelDataHead)
                                {
                                    modelAmount += 1;

                                    string xo = "";
                                    string yo = "";
                                    string zo = "";
                                    string xs = "";
                                    string ys = "";
                                    string zs = "";
                                    string xv = "";
                                    string yv = "";

                                    int spaceCheck = 0;

                                    foreach (char value in model.Name.ToString())
                                    {
                                        //0X1Y2Z3X4Y5Z6X7Y
                                        if (value.ToString() != " " && spaceCheck == 0)
                                        {
                                            xo += value.ToString();
                                        }
                                        else if (value.ToString() != " " && spaceCheck == 1)
                                        {
                                            yo += value.ToString();
                                        }
                                        else if (value.ToString() != " " && spaceCheck == 2)
                                        {
                                            zo += value.ToString();
                                        }
                                        else if (value.ToString() != " " && spaceCheck == 3)
                                        {
                                            xs += value.ToString();
                                        }
                                        else if (value.ToString() != " " && spaceCheck == 4)
                                        {
                                            ys += value.ToString();
                                        }
                                        else if (value.ToString() != " " && spaceCheck == 5)
                                        {
                                            zs += value.ToString();
                                        }
                                        else if (value.ToString() != " " && spaceCheck == 6)
                                        {
                                            xv += value.ToString();
                                        }
                                        else if (value.ToString() != " " && spaceCheck == 7)
                                        {
                                            yv += value.ToString();
                                        }
                                        else if (value.ToString() == " ")
                                        {
                                            spaceCheck += 1;
                                        }
                                    }

                                    writeSkins.WriteLine("           {");
                                    try
                                    {
                                        writeSkins.WriteLine("            \"origin\": [ " + (Double.Parse(xo)) + ", " + ((Double.Parse(yo) + 0) * -1 + offsetHead + 24 - Double.Parse(ys)) + ", " + (Double.Parse(zo)) + " ],");
                                        writeSkins.WriteLine("            \"size\": [ " + Double.Parse(xs) + ", " + (Double.Parse(ys)) + ", " + Double.Parse(zs) + " ],");
                                        writeSkins.WriteLine("            \"uv\": [ " + Double.Parse(xv) + ", " + Double.Parse(yv) + " ],");
                                        writeSkins.WriteLine("            \"inflate\": 0,");
                                        writeSkins.WriteLine("            \"mirror\": false");
                                    }
                                    catch (Exception)
                                    {
                                        MessageBox.Show("A HEAD BOX tag in " + newSkin.Filename + " has an invalid value!");
                                    }
                                    if (modelAmount != modelDataHead.Count)
                                    {
                                        writeSkins.WriteLine("    },");
                                    }
                                    else
                                    {
                                        writeSkins.WriteLine("    }");
                                    }
                                }
                                writeSkins.WriteLine("        ],");
                                writeSkins.WriteLine("        \"META_BoneType\": \"" + "clothing" + "\",");
                                writeSkins.WriteLine("        \"name\": \"" + "head" + "\",");
                                writeSkins.WriteLine("        \"parent\":" + " null");
                                writeSkins.WriteLine("        },");


                                //Body Data
                                writeSkins.WriteLine("      {");
                                writeSkins.WriteLine("        \"pivot\": [ 0, 12, 0 ],");
                                writeSkins.WriteLine("         \"rotation\": [ 0, 0, 0 ],");
                                writeSkins.WriteLine("          \"cubes\": [ ");
                                //Creates bones for each body box
                                modelAmount = 0;
                                foreach (Item model in modelDataBody)
                                {
                                    modelAmount += 1;

                                    string xo = "";
                                    string yo = "";
                                    string zo = "";
                                    string xs = "";
                                    string ys = "";
                                    string zs = "";
                                    string xv = "";
                                    string yv = "";

                                    int spaceCheck = 0;

                                    foreach (char value in model.Name.ToString())
                                    {
                                        //0X1Y2Z3X4Y5Z6X7Y
                                        if (value.ToString() != " " && spaceCheck == 0)
                                        {
                                            xo += value.ToString();
                                        }
                                        else if (value.ToString() != " " && spaceCheck == 1)
                                        {
                                            yo += value.ToString();
                                        }
                                        else if (value.ToString() != " " && spaceCheck == 2)
                                        {
                                            zo += value.ToString();
                                        }
                                        else if (value.ToString() != " " && spaceCheck == 3)
                                        {
                                            xs += value.ToString();
                                        }
                                        else if (value.ToString() != " " && spaceCheck == 4)
                                        {
                                            ys += value.ToString();
                                        }
                                        else if (value.ToString() != " " && spaceCheck == 5)
                                        {
                                            zs += value.ToString();
                                        }
                                        else if (value.ToString() != " " && spaceCheck == 6)
                                        {
                                            xv += value.ToString();
                                        }
                                        else if (value.ToString() != " " && spaceCheck == 7)
                                        {
                                            yv += value.ToString();
                                        }
                                        else if (value.ToString() == " ")
                                        {
                                            spaceCheck += 1;
                                        }
                                    }
                                    writeSkins.WriteLine("           {");
                                    try
                                    {
                                        writeSkins.WriteLine("            \"origin\": [ " + (Double.Parse(xo)) + ", " + ((Double.Parse(yo) + 0) * -1 + offsetBody + 24 - Double.Parse(ys)) + ", " + (Double.Parse(zo)) + " ],");
                                        writeSkins.WriteLine("            \"size\": [ " + Double.Parse(xs) + ", " + Double.Parse(ys) + ", " + Double.Parse(zs) + " ],");
                                        writeSkins.WriteLine("            \"uv\": [ " + Double.Parse(xv) + ", " + Double.Parse(yv) + " ],");
                                        writeSkins.WriteLine("            \"inflate\": 0,");
                                        writeSkins.WriteLine("            \"mirror\": false");
                                    }
                                    catch (Exception)
                                    {
                                        MessageBox.Show("A BODY BOX tag in " + newSkin.Filename + " has an invalid value!");
                                    }
                                    if (modelAmount != modelDataBody.Count)
                                    {
                                        writeSkins.WriteLine("    },");
                                    }
                                    else
                                    {
                                        writeSkins.WriteLine("    }");
                                    }
                                }
                                writeSkins.WriteLine("        ],");
                                writeSkins.WriteLine("        \"META_BoneType\": \"" + "base" + "\",");
                                writeSkins.WriteLine("        \"name\": \"" + "body" + "\",");
                                writeSkins.WriteLine("        \"parent\":" + " null");
                                writeSkins.WriteLine("        },");


                                //LeftArm Data
                                writeSkins.WriteLine("      {");
                                writeSkins.WriteLine("        \"pivot\": [ 5, 22, 0 ],");
                                writeSkins.WriteLine("         \"rotation\": [ 0, 0, 0 ],");
                                writeSkins.WriteLine("          \"cubes\": [ ");
                                //Creates bones for each arm1 box
                                modelAmount = 0;
                                foreach (Item model in modelDataLeftArm)
                                {
                                    modelAmount += 1;

                                    string xo = "";
                                    string yo = "";
                                    string zo = "";
                                    string xs = "";
                                    string ys = "";
                                    string zs = "";
                                    string xv = "";
                                    string yv = "";

                                    int spaceCheck = 0;

                                    foreach (char value in model.Name.ToString())
                                    {
                                        //0X1Y2Z3X4Y5Z6X7Y
                                        if (value.ToString() != " " && spaceCheck == 0)
                                        {
                                            xo += value.ToString();
                                        }
                                        else if (value.ToString() != " " && spaceCheck == 1)
                                        {
                                            yo += value.ToString();
                                        }
                                        else if (value.ToString() != " " && spaceCheck == 2)
                                        {
                                            zo += value.ToString();
                                        }
                                        else if (value.ToString() != " " && spaceCheck == 3)
                                        {
                                            xs += value.ToString();
                                        }
                                        else if (value.ToString() != " " && spaceCheck == 4)
                                        {
                                            ys += value.ToString();
                                        }
                                        else if (value.ToString() != " " && spaceCheck == 5)
                                        {
                                            zs += value.ToString();
                                        }
                                        else if (value.ToString() != " " && spaceCheck == 6)
                                        {
                                            xv += value.ToString();
                                        }
                                        else if (value.ToString() != " " && spaceCheck == 7)
                                        {
                                            yv += value.ToString();
                                        }
                                        else if (value.ToString() == " ")
                                        {
                                            spaceCheck += 1;
                                        }
                                    }
                                    writeSkins.WriteLine("           {");
                                    try
                                    {
                                        writeSkins.WriteLine("            \"origin\": [ " + (Double.Parse(xo) + 5) + ", " + ((Double.Parse(yo)) * -1 + offsetArms + 22 - Double.Parse(ys)) + ", " + (Double.Parse(zo)) + " ],");
                                        writeSkins.WriteLine("            \"size\": [ " + Double.Parse(xs) + ", " + Double.Parse(ys) + ", " + Double.Parse(zs) + " ],");
                                        writeSkins.WriteLine("            \"uv\": [ " + Double.Parse(xv) + ", " + Double.Parse(yv) + " ],");
                                        writeSkins.WriteLine("            \"inflate\": 0,");
                                        writeSkins.WriteLine("            \"mirror\": false");
                                    }
                                    catch (Exception)
                                    {
                                        MessageBox.Show("A ARM0 BOX tag in " + newSkin.Filename + " has an invalid value!");
                                    }
                                    if (modelAmount != modelDataLeftArm.Count)
                                    {
                                        writeSkins.WriteLine("    },");
                                    }
                                    else
                                    {
                                        writeSkins.WriteLine("    }");
                                    }
                                }
                                writeSkins.WriteLine("        ],");
                                writeSkins.WriteLine("        \"META_BoneType\": \"" + "base" + "\",");
                                writeSkins.WriteLine("        \"name\": \"" + "leftArm" + "\",");
                                writeSkins.WriteLine("        \"parent\":" + " null");
                                writeSkins.WriteLine("        },");

                                //RightArm Data
                                writeSkins.WriteLine("      {");
                                writeSkins.WriteLine("        \"pivot\": [ -5, 22, 0 ],");
                                writeSkins.WriteLine("         \"rotation\": [ 0, 0, 0 ],");
                                writeSkins.WriteLine("          \"cubes\": [ ");
                                //Creates bones for each arm0 box
                                modelAmount = 0;
                                foreach (Item model in modelDataRightArm)
                                {
                                    modelAmount += 1;

                                    string xo = "";
                                    string yo = "";
                                    string zo = "";
                                    string xs = "";
                                    string ys = "";
                                    string zs = "";
                                    string xv = "";
                                    string yv = "";

                                    int spaceCheck = 0;

                                    foreach (char value in model.Name.ToString())
                                    {
                                        //0X1Y2Z3X4Y5Z6X7Y
                                        if (value.ToString() != " " && spaceCheck == 0)
                                        {
                                            xo += value.ToString();
                                        }
                                        else if (value.ToString() != " " && spaceCheck == 1)
                                        {
                                            yo += value.ToString();
                                        }
                                        else if (value.ToString() != " " && spaceCheck == 2)
                                        {
                                            zo += value.ToString();
                                        }
                                        else if (value.ToString() != " " && spaceCheck == 3)
                                        {
                                            xs += value.ToString();
                                        }
                                        else if (value.ToString() != " " && spaceCheck == 4)
                                        {
                                            ys += value.ToString();
                                        }
                                        else if (value.ToString() != " " && spaceCheck == 5)
                                        {
                                            zs += value.ToString();
                                        }
                                        else if (value.ToString() != " " && spaceCheck == 6)
                                        {
                                            xv += value.ToString();
                                        }
                                        else if (value.ToString() != " " && spaceCheck == 7)
                                        {
                                            yv += value.ToString();
                                        }
                                        else if (value.ToString() == " ")
                                        {
                                            spaceCheck += 1;
                                        }
                                    }
                                    writeSkins.WriteLine("           {");
                                    try
                                    {
                                        writeSkins.WriteLine("            \"origin\": [ " + (Double.Parse(xo) - 5) + ", " + ((Double.Parse(yo)) * -1 + offsetArms + 22 - Double.Parse(ys)) + ", " + (Double.Parse(zo)) + " ],");
                                        writeSkins.WriteLine("            \"size\": [ " + Double.Parse(xs) + ", " + Double.Parse(ys) + ", " + Double.Parse(zs) + " ],");
                                        writeSkins.WriteLine("            \"uv\": [ " + Double.Parse(xv) + ", " + Double.Parse(yv) + " ],");
                                        writeSkins.WriteLine("            \"inflate\": 0,");
                                        writeSkins.WriteLine("            \"mirror\": false");
                                    }
                                    catch (Exception)
                                    {
                                        MessageBox.Show("A ARM1 BOX tag in " + newSkin.Filename + " has an invalid value!");
                                    }
                                    if (modelAmount != modelDataRightArm.Count)
                                    {
                                        writeSkins.WriteLine("    },");
                                    }
                                    else
                                    {
                                        writeSkins.WriteLine("    }");
                                    }
                                }
                                writeSkins.WriteLine("        ],");
                                writeSkins.WriteLine("        \"META_BoneType\": \"" + "base" + "\",");
                                writeSkins.WriteLine("        \"name\": \"" + "rightArm" + "\",");
                                writeSkins.WriteLine("        \"parent\":" + " null");
                                writeSkins.WriteLine("        },");

                                //LeftLeg Data
                                writeSkins.WriteLine("      {");
                                writeSkins.WriteLine("        \"pivot\": [ 1.9, 12, 0 ],");
                                writeSkins.WriteLine("         \"rotation\": [ 0, 0, 0 ],");
                                writeSkins.WriteLine("          \"cubes\": [ ");
                                //Creates bones for each leg1 box
                                modelAmount = 0;
                                foreach (Item model in modelDataLeftLeg)
                                {
                                    modelAmount += 1;

                                    string xo = "";
                                    string yo = "";
                                    string zo = "";
                                    string xs = "";
                                    string ys = "";
                                    string zs = "";
                                    string xv = "";
                                    string yv = "";

                                    int spaceCheck = 0;

                                    foreach (char value in model.Name.ToString())
                                    {
                                        //0X1Y2Z3X4Y5Z6X7Y
                                        if (value.ToString() != " " && spaceCheck == 0)
                                        {
                                            xo += value.ToString();
                                        }
                                        else if (value.ToString() != " " && spaceCheck == 1)
                                        {
                                            yo += value.ToString();
                                        }
                                        else if (value.ToString() != " " && spaceCheck == 2)
                                        {
                                            zo += value.ToString();
                                        }
                                        else if (value.ToString() != " " && spaceCheck == 3)
                                        {
                                            xs += value.ToString();
                                        }
                                        else if (value.ToString() != " " && spaceCheck == 4)
                                        {
                                            ys += value.ToString();
                                        }
                                        else if (value.ToString() != " " && spaceCheck == 5)
                                        {
                                            zs += value.ToString();
                                        }
                                        else if (value.ToString() != " " && spaceCheck == 6)
                                        {
                                            xv += value.ToString();
                                        }
                                        else if (value.ToString() != " " && spaceCheck == 7)
                                        {
                                            yv += value.ToString();
                                        }
                                        else if (value.ToString() == " ")
                                        {
                                            spaceCheck += 1;
                                        }
                                    }
                                    writeSkins.WriteLine("           {");
                                    try
                                    {
                                        writeSkins.WriteLine("            \"origin\": [ " + (Double.Parse(xo) - 1.9) + ", " + ((Double.Parse(yo)) * -1 + offsetLegs + 12 - Double.Parse(ys)) + ", " + (Double.Parse(zo)) + " ],");
                                        writeSkins.WriteLine("            \"size\": [ " + Double.Parse(xs) + ", " + Double.Parse(ys) + ", " + Double.Parse(zs) + " ],");
                                        writeSkins.WriteLine("            \"uv\": [ " + Double.Parse(xv) + ", " + Double.Parse(yv) + " ],");
                                        writeSkins.WriteLine("            \"inflate\": 0,");
                                        writeSkins.WriteLine("            \"mirror\": false");
                                    }
                                    catch (Exception)
                                    {
                                        MessageBox.Show("A LEG1 BOX tag in " + newSkin.Filename + " has an invalid value!");
                                    }
                                    if (modelAmount != modelDataLeftLeg.Count)
                                    {
                                        writeSkins.WriteLine("    },");
                                    }
                                    else
                                    {
                                        writeSkins.WriteLine("    }");
                                    }
                                }
                                writeSkins.WriteLine("        ],");
                                writeSkins.WriteLine("        \"META_BoneType\": \"" + "base" + "\",");
                                writeSkins.WriteLine("        \"name\": \"" + "leftLeg" + "\",");
                                writeSkins.WriteLine("        \"parent\":" + " null");
                                writeSkins.WriteLine("        },");

                                //RightLeg Data
                                writeSkins.WriteLine("      {");
                                writeSkins.WriteLine("        \"pivot\": [ -1.9, 12, 0 ],");
                                writeSkins.WriteLine("         \"rotation\": [ 0, 0, 0 ],");
                                writeSkins.WriteLine("          \"cubes\": [ ");
                                //Creates bones for each leg0 box
                                modelAmount = 0;
                                foreach (Item model in modelDataRightLeg)
                                {
                                    modelAmount += 1;

                                    string xo = "";
                                    string yo = "";
                                    string zo = "";
                                    string xs = "";
                                    string ys = "";
                                    string zs = "";
                                    string xv = "";
                                    string yv = "";

                                    int spaceCheck = 0;

                                    foreach (char value in model.Name.ToString())
                                    {
                                        //0X1Y2Z3X4Y5Z6X7Y
                                        if (value.ToString() != " " && spaceCheck == 0)
                                        {
                                            xo += value.ToString();
                                        }
                                        else if (value.ToString() != " " && spaceCheck == 1)
                                        {
                                            yo += value.ToString();
                                        }
                                        else if (value.ToString() != " " && spaceCheck == 2)
                                        {
                                            zo += value.ToString();
                                        }
                                        else if (value.ToString() != " " && spaceCheck == 3)
                                        {
                                            xs += value.ToString();
                                        }
                                        else if (value.ToString() != " " && spaceCheck == 4)
                                        {
                                            ys += value.ToString();
                                        }
                                        else if (value.ToString() != " " && spaceCheck == 5)
                                        {
                                            zs += value.ToString();
                                        }
                                        else if (value.ToString() != " " && spaceCheck == 6)
                                        {
                                            xv += value.ToString();
                                        }
                                        else if (value.ToString() != " " && spaceCheck == 7)
                                        {
                                            yv += value.ToString();
                                        }
                                        else if (value.ToString() == " ")
                                        {
                                            spaceCheck += 1;
                                        }
                                    }
                                    writeSkins.WriteLine("           {");
                                    try
                                    {
                                        writeSkins.WriteLine("            \"origin\": [ " + (Double.Parse(xo) + 1.9) + ", " + ((Double.Parse(yo)) * -1 + offsetLegs + 12 - Double.Parse(ys)) + ", " + (Double.Parse(zo)) + " ],");
                                        writeSkins.WriteLine("            \"size\": [ " + Double.Parse(xs) + ", " + Double.Parse(ys) + ", " + Double.Parse(zs) + " ],");
                                        writeSkins.WriteLine("            \"uv\": [ " + Double.Parse(xv) + ", " + Double.Parse(yv) + " ],");
                                        writeSkins.WriteLine("            \"inflate\": 0,");
                                        writeSkins.WriteLine("            \"mirror\": false");
                                    }
                                    catch (Exception)
                                    {
                                        MessageBox.Show("A LEG0 BOX tag in " + newSkin.Filename + " has an invalid value!");
                                    }
                                    if (modelAmount != modelDataRightLeg.Count)
                                    {
                                        writeSkins.WriteLine("    },");
                                    }
                                    else
                                    {
                                        writeSkins.WriteLine("    }");
                                    }
                                }
                                writeSkins.WriteLine("        ],");
                                writeSkins.WriteLine("        \"META_BoneType\": \"" + "base" + "\",");
                                writeSkins.WriteLine("        \"name\": \"" + "rightLeg" + "\",");
                                writeSkins.WriteLine("        \"parent\":" + " null");
                                writeSkins.WriteLine("        }");
                                writeSkins.WriteLine("    ],");
                            }
                            else if (skinType == "64x32")
                            {
                                writeSkins.Write("    \"bones\": [ ],");
                            }
                            else if (skinType == "steve")
                            {
                                writeSkins.Write("    \"bones\": [ " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 0, 24, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ -4, 12, -2 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 8, 12, 4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 16, 16 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"base\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"body\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": null " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 0, 24, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"bodyArmor\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"body\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 0, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"belt\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"body\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 0, 24, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ -4, 24, -4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 8, 8, 8 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"base\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"head\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": null " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 0, 24, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ -4, 24, -4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 8, 8, 8 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 32, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0.5, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"clothing\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"hat\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"head\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 0, 24, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ 0, 24, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"helmet\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"head\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 5, 22, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ 4, 12, -2 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 4, 12, 4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 32, 48 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"base\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"leftArm\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": null " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ -5, 22, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ -8, 12, -2 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 4, 12, 4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 40, 16 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"base\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"rightArm\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": null " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 5, 22, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"leftArmArmor\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"leftArm\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ -5, 22, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"rightArmArmor\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"rightArm\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 5, 22, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ 4, 12, -2 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 4, 12, 4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 48, 48 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0.25, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"clothing\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"leftSleeve\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"leftArm\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ -5, 22, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ -8, 12, -2 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 4, 12, 4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 40, 32 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0.25, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"clothing\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"rightSleeve\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"rightArm\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 1.9, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ -0.1, 0, -2 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 4, 12, 4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 16, 48 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"base\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"leftLeg\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": null " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ -1.9, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ -3.9, 0, -2 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 4, 12, 4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 0, 16 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"base\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"rightLeg\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": null " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 1.9, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"leftLegging\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"leftLeg\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ -1.9, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"rightLegging\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"rightLeg\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 1.9, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ -0.1, 0, -2 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 4, 12, 4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 0, 48 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0.25, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"clothing\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"leftPants\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"leftLeg\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ -1.9, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ -3.9, 0, -2 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 4, 12, 4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 0, 32 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0.25, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"clothing\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"rightPants\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"rightLeg\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 0, 24, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ -4, 12, -2 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 8, 12, 4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 16, 32 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0.25, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"clothing\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"jacket\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"body\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 0, 24, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor_offset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"helmetArmorOffset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"head\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 0, 24, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor_offset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"bodyArmorOffset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"body\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ -5, 22, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor_offset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"rightArmArmorOffset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"rightArm\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 5, 22, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor_offset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"leftArmArmorOffset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"leftArm\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 0, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor_offset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"waist\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"body\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ -1.9, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor_offset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"rightLegArmorOffset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"rightLeg\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 1.9, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor_offset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"leftLegArmorOffset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"leftLeg\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ -1.9, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor_offset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"rightBootArmorOffset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"rightLeg\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 1.9, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor_offset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"leftBootArmorOffset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"leftLeg\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ -6, 15, 1 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"item\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"rightItem\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"rightArm\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 6, 15, 1 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"item\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"leftItem\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"leftArm\" " + Environment.NewLine + "  " + Environment.NewLine + "       } " + Environment.NewLine + "  " + Environment.NewLine + "     ],");
                            }
                            else if (skinType == "alex")
                            {
                                writeSkins.Write("    \"bones\": [ " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 0, 24, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ -4, 12, -2 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 8, 12, 4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 16, 16 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"base\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"body\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": null " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 0, 24, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"bodyArmor\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"body\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 0, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"belt\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"body\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 0, 24, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ -4, 24, -4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 8, 8, 8 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"base\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"head\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": null " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 0, 24, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ -4, 24, -4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 8, 8, 8 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 32, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0.5, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"clothing\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"hat\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"head\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 0, 24, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ 0, 24, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"helmet\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"head\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 5, 21.5, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ 4, 11.5, -2 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 3, 12, 4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 32, 48 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"base\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"leftArm\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": null " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ -5, 21.5, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ -7, 11.5, -2 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 3, 12, 4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 40, 16 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"base\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"rightArm\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": null " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 5, 21.5, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"leftArmArmor\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"leftArm\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ -5, 21.5, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"rightArmArmor\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"rightArm\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 5, 21.5, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ 4, 11.5, -2 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 3, 12, 4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 48, 48 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0.25, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"clothing\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"leftSleeve\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"leftArm\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ -5, 21.5, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ -7, 11.5, -2 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 3, 12, 4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 40, 32 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0.25, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"clothing\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"rightSleeve\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"rightArm\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 1.9, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ -0.1, 0, -2 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 4, 12, 4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 16, 48 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"base\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"leftLeg\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": null " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ -1.9, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ -3.9, 0, -2 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 4, 12, 4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 0, 16 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"base\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"rightLeg\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": null " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 1.9, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"leftLegArmor\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"leftLeg\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ -1.9, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"rightLegging\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"rightLeg\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 1.9, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ -0.1, 0, -2 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 4, 12, 4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 0, 48 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0.25, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"clothing\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"leftPants\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"leftLeg\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ -1.9, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ -3.9, 0, -2 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 4, 12, 4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 0, 32 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0.25, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"clothing\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"rightPants\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"rightLeg\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 0, 24, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ -4, 12, -2 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 8, 12, 4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 16, 32 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0.25, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"clothing\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"jacket\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"body\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 0, 24, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor_offset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"helmetArmorOffset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"head\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 0, 24, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor_offset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"bodyArmorOffset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"body\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ -5, 21.5, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor_offset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"rightArmArmorOffset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"rightArm\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 5, 21.5, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor_offset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"leftArmArmorOffset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"leftArm\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 0, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor_offset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"waist\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"body\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ -1.9, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor_offset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"rightLegArmorOffset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"rightLeg\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 1.9, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor_offset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"leftLegArmorOffset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"leftLeg\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ -1.9, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor_offset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"rightBootArmorOffset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"rightLeg\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 1.9, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor_offset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"leftBootArmorOffset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"leftLeg\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ -6, 14.5, 1 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"item\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"rightItem\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"rightArm\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 6, 14.5, 1 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"item\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"leftItem\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"leftArm\" " + Environment.NewLine + "  " + Environment.NewLine + "       } " + Environment.NewLine + "  " + Environment.NewLine + "     ],");
                            }


                            writeSkins.WriteLine("    \"texturewidth\": 64 , ");
                            writeSkins.WriteLine("    \"textureheight\": 64,");
                            writeSkins.WriteLine("    \"META_ModelVersion\": \"1.0.6\",");
                            writeSkins.WriteLine("    \"rigtype\": \"normal\",");
                            writeSkins.WriteLine("    \"animationArmsDown\": false,");
                            writeSkins.WriteLine("    \"animationArmsOutFront\": false,");
                            writeSkins.WriteLine("    \"animationStatueOfLibertyArms\": false,");
                            writeSkins.WriteLine("    \"animationSingleArmAnimation\": false,");
                            writeSkins.WriteLine("    \"animationStationaryLegs\": false,");
                            writeSkins.WriteLine("    \"animationSingleLegAnimation\": false,");
                            writeSkins.WriteLine("    \"animationNoHeadBob\": false,");
                            writeSkins.WriteLine("    \"animationDontShowArmor\": false,");
                            writeSkins.WriteLine("    \"animationUpsideDown\": false,");
                            writeSkins.WriteLine("    \"animationInvertedCrouch\": false");
                            if (newSkinCount != skinsList.Count)
                            {
                                writeSkins.WriteLine("  },");
                            }
                            else
                            {
                                writeSkins.WriteLine("  }");
                            }
                        }
                    }
                    Random rnd = new Random();
                    int month = rnd.Next(1, 13); // creates a number between 1 and 12
                    int dice = rnd.Next(1, 7);   // creates a number between 1 and 6
                    int card = rnd.Next(52);

                    string randomPlus = month.ToString() + dice.ToString() + card.ToString();
                    if (randomPlus.Count() > 12)
                    {
                        randomPlus.Remove(0, randomPlus.Count() - 12);
                    }
                    else if (randomPlus.Count() < 12)
                    {
                        int ii = 12 - randomPlus.Count();
                        for (int i = 0; i < ii; i++)
                        {
                            randomPlus += 0;
                        }
                    }
                    else if (randomPlus.Count() == 12)
                    {
                    }

                    //Create Manifest file
                    using (StreamWriter writeSkins = new StreamWriter(root + "/manifest.json"))
                    {
                        writeSkins.WriteLine("{");
                        writeSkins.WriteLine("  \"header\": {");
                        writeSkins.WriteLine("    \"version\": [");
                        writeSkins.WriteLine("      1,");
                        writeSkins.WriteLine("      0,");
                        writeSkins.WriteLine("      0");
                        writeSkins.WriteLine("    ],");
                        writeSkins.WriteLine("    \"description\": \"Template by Ultmate_Mario, Conversion by Nobledez\",");
                        writeSkins.WriteLine("    \"name\": \"" + packName + "\",");
                        writeSkins.WriteLine("    \"uuid\": \"" + uuid.Remove(0, 4) + "-" + uuid.Remove(0, 8) + "-" + uuid.Remove(1, 8) + "-" + uuid.Remove(2, 8) + "-" + randomPlus + "\""); //8-4-4-4-12
                        writeSkins.WriteLine("  },");
                        writeSkins.WriteLine("  \"modules\": [");
                        writeSkins.WriteLine("    {");
                        writeSkins.WriteLine("      \"version\": [");
                        writeSkins.WriteLine("        1,");
                        writeSkins.WriteLine("        0,");
                        writeSkins.WriteLine("        0");
                        writeSkins.WriteLine("      ],");
                        writeSkins.WriteLine("      \"type\": \"skin_pack\",");
                        writeSkins.WriteLine("      \"uuid\": \"8dfd1d65-b3ca-4726-b9e0-9b46a40b72a4\"");
                        writeSkins.WriteLine("    }");
                        writeSkins.WriteLine("  ],");
                        writeSkins.WriteLine("  \"format_version\": 1");
                        writeSkins.WriteLine("}");
                    }

                    //create lang file
                    using (StreamWriter writeSkins = new StreamWriter(root + "/texts/en_US.lang"))
                    {
                        writeSkins.WriteLine("skinpack." + packName + "=" + Path.GetFileNameWithoutExtension(convert.FileName));
                        foreach (Item displayName in skinDisplayNames)
                        {
                            writeSkins.WriteLine("skin." + packName + "." + displayName.Id + "=" + displayName.Name);
                        }
                    }

                    //adds skin textures
                    foreach (PckFile.FileData skinTexture in skinsList)
                    {
                        var ms = new MemoryStream(skinTexture.Data);
                        Bitmap saveSkin = new Bitmap(Image.FromStream(ms));
                        var config = new GraphicsConfig()
                        {
                             CompositingMode = CompositingMode.SourceCopy,
                             CompositingQuality = CompositingQuality.HighQuality,
                             InterpolationMode = InterpolationMode.NearestNeighbor,
                             SmoothingMode = SmoothingMode.HighQuality,
                             PixelOffsetMode = PixelOffsetMode.HighQuality,
                        };

                        if (saveSkin.Width == saveSkin.Height)
                        {
                            saveSkin.ResizeImage(64, 64, config);
                        }
                        else if (saveSkin.Height == saveSkin.Width / 2)
                        {
                            saveSkin.ResizeImage(64, 32, config);
                        }
                        else
                        {
                            saveSkin.ResizeImage(64, 64, config);
                        }
                        saveSkin.Save(root + "/" + skinTexture.Filename, ImageFormat.Png);
                    }

                    //adds cape textures
                    foreach (PckFile.FileData capeTexture in capesList)
                    {
                        File.WriteAllBytes(root + "/" + capeTexture.Filename, capeTexture.Data);
                    }

                    string startPath = root;
                    string zipPath = rootFinal + "content.zipe";

                    try
                    {
                        System.IO.Compression.ZipFile.CreateFromDirectory(startPath, zipPath);//Creates contents zipe
                    }
                    catch (Exception)
                    {
                        File.Delete(zipPath);
                        ZipFile.CreateFromDirectory(startPath, zipPath);//Creates contents zipe
                    }

                    rootFinal = root + "temp/";
                    Directory.CreateDirectory(rootFinal);
                    File.Move(zipPath, rootFinal + "content.zipe");
                    File.Copy(root + "/manifest.json", rootFinal + "/manifest.json");
                    ZipFile.CreateFromDirectory(rootFinal, convert.FileName);//Creates mcpack
                    Directory.Delete(root, true);
                    Directory.Delete(rootFinal, true);

                    MessageBox.Show("Conversion Complete");
                }
            }
            catch (Exception convertEr)
            {
                MessageBox.Show(convertEr.ToString());
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            try
            {
                File.Delete(Program.AppData + "/PCK-Center/myPcks/" + mod + ".pck");
                File.Delete(Program.AppData + "/PCK-Center/myPcks/" + mod + ".pck");
                File.Delete(Program.AppData + "/PCK-Center/myPcks/" + mod + ".png");
                File.Delete(Program.AppData + "/PCK-Center/myPcks/" + mod + ".desc");
                reloader();
            }
            catch (Exception)
            {
                MessageBox.Show("Error");
            }
            this.Close();
        }

        private void buttonExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog export = new SaveFileDialog();
            export.Title = "Get your PCK file";
            export.Filter = "PCK (Minecraft Wii U Package)|*.pck";

            if (export.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    File.Copy(Program.AppData + "/PCK-Center/myPcks/" + mod + ".pck", export.FileName);
                    MessageBox.Show("PCK Received from location!");
                }catch (Exception)
                {
                    MessageBox.Show("Error");
                }
            }
        }

        private void buttonInstallXbox_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://nobledez.com/pckStudio#install");
        }

        private void buttonInstallPs3_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://nobledez.com/pckStudio#install");
        }

        private void buttonInstallWiiU_Click(object sender, EventArgs e)
        {
            InstallWiiU install = new InstallWiiU(Program.AppData + "/PCK Center/myPcks/" + mod + ".pck");
            install.ShowDialog();
        }
    }
}
