﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace PckStudio
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

        public Dictionary<int, string> types = new Dictionary<int, string>();
        public Dictionary<string, int> typeCodes = new Dictionary<string, int>();
        public List<MineFile> mineFiles = new List<MineFile>();
        public bool IsLittleEndian = false;
        public int pckType = 0;

        public PCK()
        {

        }

        public PCK(string filename)
        {
            try
            {
                IsLittleEndian = !(Read(File.ReadAllBytes(filename)));
                if (IsLittleEndian)
                    ReadVita(File.ReadAllBytes(filename));
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
        }

        #region NormalPCK

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

        static string readMineString(FileData f)
        {
            int length = f.readInt() * 2;
            return Encoding.Unicode.GetString(endianReverseUnicode(f.readBytes(length)));
        }

        public bool Read(byte[] data)
        {
            try
            {
                pckType = 0;
                types = new Dictionary<int, string>();
                typeCodes = new Dictionary<string, int>();
                mineFiles = new List<MineFile>();

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
                        text = readMineString(fileData);
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

                for (int i = 0; i < itemCount; i++)
                {
                    MineFile mf = new MineFile();
                    mf.filesize = fileData.readInt();
                    mf.type = fileData.readInt();
                    int length = fileData.readInt() * 2;
                    mf.name = Encoding.Unicode.GetString(endianReverseUnicode(fileData.readBytes(length)));
                    fileData.skip(4);
                    mineFiles.Add(mf);
                }

                foreach (MineFile mf in mineFiles)
                {
                    int entryCount = fileData.readInt();
                    for (int i = 0; i < entryCount; i++)
                    {
                        object[] temp = new object[2];
                        int entryTypeInt = fileData.readInt();
                        temp[0] = types[entryTypeInt]; //Entry type
                        temp[1] = readMineString(fileData); //Entry data

                        fileData.skip(4);
                        mf.entries.Add(temp);
                    }
                    mf.data = fileData.readBytes(mf.filesize);
                }
                return true;
            }
            catch
            {
                return false;
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

        #endregion

        #region Vita/PS4 PCK

        static string readMineStringVita(FileData f)
        {
            int length = f.readIntVita() * 2;
            Console.WriteLine(length.ToString());
            return Encoding.Unicode.GetString((f.readBytes(length)));
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

        private static void writeMinecraftStringVita(FileOutput f, string str)
        {
            Console.WriteLine("WriteVita -- " + str);
            byte[] bytes = Encoding.Unicode.GetBytes(str);
            f.writeIntVita(bytes.Length / 2);
            f.writeBytes((bytes));
            f.writeIntVita(0);
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

        #endregion
    }
}
