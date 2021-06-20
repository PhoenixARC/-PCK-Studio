using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace MinecraftUSkinEditor
{
    public class PCK
    {

        public class MineFile
        {
            public int filesize;
            public int type;
            public string name;
            public byte[] data;
            public List<object[]> entries = new List<object[]>();
        }
        public bool IsLittleEndian;
        public int pckType = 0;

        public Dictionary<int, string> types = new Dictionary<int, string>();
        public Dictionary<string, int> typeCodes = new Dictionary<string, int>();
        public List<MineFile> mineFiles = new List<MineFile>();

        public PCK()
        {

        }

        public PCK(string filename)
        {
            try
            {
                Read(File.ReadAllBytes(filename));
                IsLittleEndian = false;
            }
            catch
            {
                ReadVita(File.ReadAllBytes(filename));
                IsLittleEndian = true;
            }
        }

        private static byte[] endianReverseUnicode(byte[] str)
        {
            byte[] newStr = new byte[str.Length];
            for (int i = 0; i < str.Length; i += 2)
            {
                newStr[i] = str[i + 1];
                newStr[i + 1] = str[i];
            }
            return newStr;
        }

        public static string readMineString(FileData f)
        {
            int length = f.readInt() * 2;
            Console.WriteLine(length.ToString());
            return Encoding.Unicode.GetString(endianReverseUnicode(f.readBytes(length)));

        }
        public static string readMineStringVita(FileData f)
        {
            int length = f.readIntVita() * 2;
            Console.WriteLine(length.ToString());
            return Encoding.Unicode.GetString((f.readBytes(length)));

        }
        public void Read(byte[] data)
        {
            pckType = 0;
            types = new Dictionary<int, string>();
            typeCodes = new Dictionary<string, int>();
            mineFiles = new List<MineFile>();

            FileData fileData = new FileData(data);
            fileData.Endian = Endianness.Big;
            fileData.readInt();
            int entryTypeCount = fileData.readInt();
            //int a = 0;
            for (int i = 0; i < entryTypeCount; i++)
            {
                int unk = fileData.readInt();
                string text = "";
                try
                {
                    text = readMineString(fileData);
                    //File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\exp\\PCKDump" + a + ".bin", text);
                    //a++;
                }
                catch
                {
                    text = "Hello!";
                }
                types.Add(unk, text);
                typeCodes.Add(text, unk);
                fileData.skip(4);
            }

            int itemCount = fileData.readInt();

            Console.WriteLine(itemCount);
            // no metadata
            if (entryTypeCount == 0)
            {
                Console.WriteLine("PckType0");
            }
            // type 1 or 2
            else if (itemCount < 3)
            {
                pckType = itemCount;
                if (pckType == 1)
                {
                    Console.WriteLine("PckType1");
                    itemCount = fileData.readInt();
                }
                if (pckType == 2)
                    Console.WriteLine("PckType2");
            }
            // regular pck
            else
            {
                Console.WriteLine("NormalPCK");
            }


            for (int j = 0; j < itemCount; j++)
            {
                MineFile mineFile = new MineFile();
                mineFile.filesize = fileData.readInt();
                mineFile.type = fileData.readInt();
                int length = fileData.readInt() * 2;
                mineFile.name = Encoding.Unicode.GetString(endianReverseUnicode(fileData.readBytes(length)));
                fileData.skip(4);
                mineFiles.Add(mineFile);
            }

            foreach (MineFile mineFile2 in mineFiles)
            {
                int num4 = fileData.readInt();
                for (int k = 0; k < num4; k++)
                {
                    object[] array = new object[2];
                    int key = fileData.readInt();
                    array[0] = types[key];
                    array[1] = readMineString(fileData);
                    fileData.skip(4);
                    mineFile2.entries.Add(array);
                }
                mineFile2.data = fileData.readBytes(mineFile2.filesize);
            }
        }

        public void ReadVita(byte[] data)
        {
            pckType = 0;
            types = new Dictionary<int, string>();
            typeCodes = new Dictionary<string, int>();
            mineFiles = new List<MineFile>();

            FileData fileData = new FileData(data);
            fileData.Endian = Endianness.Big;
            fileData.readIntVita();
            int entryTypeCount = fileData.readIntVita();
            //int a = 0;
            for (int i = 0; i < entryTypeCount; i++)
            {
                int unk = fileData.readIntVita();
                string text = "";
                try
                {
                    text = readMineStringVita(fileData);
                    //File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\exp\\PCKDump" + a + ".bin", text);
                    //a++;
                }
                catch
                {
                    text = "Hello!";
                }
                types.Add(unk, text);
                typeCodes.Add(text, unk);
                fileData.skip(4);
            }

            int itemCount = fileData.readIntVita();

            Console.WriteLine(itemCount);
            // no metadata
            if (entryTypeCount == 0)
            {
                Console.WriteLine("PckType0");
            }
            // type 1 or 2
            else if (itemCount < 3)
            {
                pckType = itemCount;
                if (pckType == 1)
                {
                    Console.WriteLine("PckType1");
                    itemCount = fileData.readIntVita();
                }
                if (pckType == 2)
                    Console.WriteLine("PckType2");
            }
            // regular pck
            else
            {
                Console.WriteLine("NormalPCK");
            }


            for (int j = 0; j < itemCount; j++)
            {
                MineFile mineFile = new MineFile();
                mineFile.filesize = fileData.readIntVita();
                mineFile.type = fileData.readIntVita();
                int length = fileData.readIntVita() * 2;
                mineFile.name = Encoding.Unicode.GetString((fileData.readBytes(length)));
                fileData.skip(4);
                mineFiles.Add(mineFile);
            }

            foreach (MineFile mineFile2 in mineFiles)
            {
                int num4 = fileData.readIntVita();
                for (int k = 0; k < num4; k++)
                {
                    object[] array = new object[2];
                    int key = fileData.readIntVita();
                    array[0] = types[key];
                    array[1] = readMineStringVita(fileData);
                    fileData.skip(4);
                    mineFile2.entries.Add(array);
                }
                mineFile2.data = fileData.readBytes(mineFile2.filesize);
            }
        }

        private static void writeMinecraftString(FileOutput f, string str)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(str);
            f.writeInt(bytes.Length / 2);
            f.writeBytes(PCK.endianReverseUnicode(bytes));
            f.writeInt(0);
        }

        private static void writeMinecraftStringVita(FileOutput f, string str)
        {
            Console.WriteLine("WriteVita -- " + str);
            byte[] bytes = Encoding.Unicode.GetBytes(str);
            f.writeIntVita(bytes.Length / 2);
            f.writeBytes((bytes));
            f.writeIntVita(0);
        }

        public byte[] Rebuild()
        {
            FileOutput fileOutput = new FileOutput();
            fileOutput.Endian = Endianness.Big;
            fileOutput.writeInt(3);
            fileOutput.writeInt(this.types.Count);
            foreach (int num in this.types.Keys)
            {
                fileOutput.writeInt(num);
                PCK.writeMinecraftString(fileOutput, this.types[num]);
            }
            fileOutput.writeInt(this.mineFiles.Count);
            foreach (PCK.MineFile mineFile in this.mineFiles)
            {
                fileOutput.writeInt(mineFile.data.Length);
                fileOutput.writeInt(mineFile.type);
                PCK.writeMinecraftString(fileOutput, mineFile.name);
            }
            foreach (PCK.MineFile mineFile2 in this.mineFiles)
            {
                string str = "";
                try
                {
                    fileOutput.writeInt(mineFile2.entries.Count);
                    foreach (object[] array in mineFile2.entries)
                    {
                        str = array[0].ToString();
                        fileOutput.writeInt(this.typeCodes[(string)array[0]]);
                        PCK.writeMinecraftString(fileOutput, (string)array[1]);
                    }
                    fileOutput.writeBytes(mineFile2.data);
                }
                catch (Exception)
                {
                    MessageBox.Show(str + " is not in the main metadatabase");
                    break;
                }
            }
            return fileOutput.getBytes();
        }

        public byte[] RebuildVita()
        {
            FileOutput fileOutput = new FileOutput();
            fileOutput.Endian = Endianness.Big;
            fileOutput.writeIntVita(3);
            fileOutput.writeIntVita(this.types.Count);
            foreach (int num in this.types.Keys)
            {
                fileOutput.writeIntVita(num);
                PCK.writeMinecraftStringVita(fileOutput, this.types[num]);
            }
            fileOutput.writeIntVita(this.mineFiles.Count);
            foreach (PCK.MineFile mineFile in this.mineFiles)
            {
                fileOutput.writeIntVita(mineFile.data.Length);
                fileOutput.writeIntVita(mineFile.type);
                PCK.writeMinecraftStringVita(fileOutput, mineFile.name);
            }
            foreach (PCK.MineFile mineFile2 in this.mineFiles)
            {
                string str = "";
                try
                {
                    fileOutput.writeIntVita(mineFile2.entries.Count);
                    foreach (object[] array in mineFile2.entries)
                    {
                        str = array[0].ToString();
                        fileOutput.writeIntVita(this.typeCodes[(string)array[0]]);
                        PCK.writeMinecraftStringVita(fileOutput, (string)array[1]);
                    }
                    fileOutput.writeBytes(mineFile2.data);
                }
                catch (Exception)
                {
                    MessageBox.Show(str + " is not in the main metadatabase");
                    break;
                }
            }
            return fileOutput.getBytes();
        }
    }
}