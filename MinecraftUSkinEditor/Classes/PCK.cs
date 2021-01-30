using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;

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

        public int pckType = 0;

        public Dictionary<int, string> types = new Dictionary<int, string>();
        public Dictionary<string, int> typeCodes = new Dictionary<string, int>();
        public List<MineFile> mineFiles = new List<MineFile>();

        public PCK()
        {

        }

        public PCK(string filename)
        {
            Read(File.ReadAllBytes(filename));
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
                int length = f.readInt() / 20000000;
                Console.WriteLine(length.ToString() + " - caught");
                return Encoding.Unicode.GetString(endianReverseUnicode(f.readBytes(length)));
            
        }

        public static string readMineStringVita2(FileData f)
        {
                int length = (f.readInt() / 20000000) * 2;
                Console.WriteLine(length.ToString() + " - caught");
                return Encoding.Unicode.GetString(endianReverseUnicode(f.readBytes(length)));
            
        }

        public void Read(byte[] data)
        {
            FileData fileData = new FileData(data);
            fileData.Endian = Endianness.Big;
            fileData.readInt();
            int entryTypeCount = fileData.readInt();
            for (int i = 0; i < entryTypeCount; i++)
            {
                int unk = fileData.readInt();
                string text = "";
                try
                {
                    try
                    {
                        text = readMineString(fileData);
                    }
                    catch
                    {
                        try
                        {
                            text = readMineStringVita(fileData);
                        }
                        catch
                        {
                            text = readMineStringVita2(fileData);
                        }
                    }
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

            // no metadata
            if (entryTypeCount == 0)
            {
                Console.WriteLine("PckType0");
            }
            // type 1 or 2
            else if (itemCount < 3)
            {
                pckType = itemCount;
                itemCount = fileData.readInt();
                if (pckType == 1)
                    Console.WriteLine("PckType1");
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

        private static void writeMinecraftString(FileOutput f, string str)
        {
            byte[] d = Encoding.Unicode.GetBytes(str);
            f.writeInt(d.Length / 2);
            f.writeBytes(endianReverseUnicode(d));
            f.writeInt(0);
        }

        public byte[] Rebuild()
        {
            FileOutput f = new FileOutput();
            f.Endian = Endianness.Big;

            f.writeInt(3);
            f.writeInt(types.Count);
            foreach (int type in types.Keys)
            {
                f.writeInt(type);
                writeMinecraftString(f, types[type]);
            }

            f.writeInt(mineFiles.Count);
            foreach (MineFile mf in mineFiles)
            {
                f.writeInt(mf.data.Length);
                f.writeInt(mf.type);
                writeMinecraftString(f, mf.name);
            }

            foreach (MineFile mf in mineFiles)
            {
                string missing = "";
                try
                {
                    f.writeInt(mf.entries.Count);
                    foreach (object[] entry in mf.entries)
                    {
                        missing = entry[0].ToString();
                        f.writeInt(typeCodes[(string)entry[0]]);
                        writeMinecraftString(f, (string)entry[1]);
                    }

                    f.writeBytes(mf.data);
                }
                catch (Exception)
                {
                    MessageBox.Show(missing + " is not in the main metadatabase");
                    break;
                }
            }


            return f.getBytes();
        }
    }
}
