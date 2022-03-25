using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PckStudio.Classes
{
    static class COL
    {
        public class COLFile
        {
            byte[] data;
            List<byte> extradata = new List<byte>();
            public List<object[]> entries = new List<object[]>();
            public List<object[]> waterEntries = new List<object[]>();
            public void Open(byte[] filePath)
            {
                data = filePath;
                COL.Open(this, entries, waterEntries, data, extradata);
                foreach (object[] obj in entries)
                {
                    Console.WriteLine(obj[0].ToString() + " - #" + obj[1]);
                }
            }
            public byte[] Save()
            {
                List<byte> SaveData = new List<byte>();
                SaveData.AddRange(new byte[] { 0x00, 0x00, 0x00, 0x01 });
                Console.WriteLine(entries.Count);
                List<byte> ItemAmmount = new List<byte>();
                List<byte> WaterItemAmmount = new List<byte>();
                ItemAmmount.AddRange(BitConverter.GetBytes(entries.Count));
                WaterItemAmmount.AddRange(BitConverter.GetBytes(waterEntries.Count / 3));
                ItemAmmount.Reverse();
                WaterItemAmmount.Reverse();
                byte[] ItemNum = ItemAmmount.ToArray();
                byte[] WaterItemNum = WaterItemAmmount.ToArray();
                ItemNum.Reverse();
                WaterItemNum.Reverse();
                Console.WriteLine(BitConverter.ToString(ItemNum));
                Console.WriteLine(BitConverter.ToString(WaterItemNum));
                SaveData.AddRange(ItemNum);
                foreach (object[] obj in entries)
                {
                    //Console.WriteLine(obj[0] + " - #" + obj[1]);
                    byte[] NameLength = (BitConverter.GetBytes(obj[0].ToString().Length));
                    SaveData.Add(NameLength[1]);
                    SaveData.Add(NameLength[0]);
                    SaveData.AddRange(Encoding.ASCII.GetBytes(obj[0].ToString()));
                    SaveData.Add(data[SaveData.Count]);
                    SaveData.AddRange(StringToByteArrayFastest(obj[1].ToString()));
                }
                SaveData.AddRange(WaterItemNum);
                foreach (object[] obj in waterEntries)
                {
                    //Console.WriteLine(obj[0] + " - #" + obj[1]);
                    string name = obj[0].ToString();
                    if (!name.EndsWith("(Underwater)") && !name.EndsWith("(Fog)"))
					{
                        byte[] NameLength = (BitConverter.GetBytes(obj[0].ToString().Length));
                        SaveData.Add(NameLength[1]);
                        SaveData.Add(NameLength[0]);
                        SaveData.AddRange(Encoding.ASCII.GetBytes(obj[0].ToString()));
                    }
                    SaveData.AddRange(StringToByteArrayFastest(obj[1].ToString()));
                }
                return SaveData.ToArray();
                //File.WriteAllBytes(Path.GetDirectoryName(filePath) + "\\coloursSaved.col", SaveData.ToArray());
            }
        }
        public static void Open(COLFile This, List<object[]> entries, List<object[]> waterEntries, byte[] data, List<byte> extradata)
        {
            List<byte> MaxEntArrTemp = new List<byte>();
            MaxEntArrTemp.AddRange(data.Skip(4).Take(4));
            MaxEntArrTemp.Reverse();
            int MaxEntries = BitConverter.ToInt32(MaxEntArrTemp.ToArray(), 0);
            int i = 1;
            int LeftAt = 8;
            while (i <= MaxEntries)
            {
                List<byte> ItemNameLength = new List<byte>();
                ItemNameLength.AddRange(data.Skip(LeftAt).Take(2));
                ItemNameLength.Reverse();
                int EntryLength = BitConverter.ToInt16(ItemNameLength.ToArray(), 0);
                //Console.WriteLine(EntryLength);

                byte[] ItemName = (data.Skip(LeftAt + 2).Take(EntryLength).ToArray());
                byte[] ItemHex = (data.Skip(LeftAt + 3 + EntryLength).Take(3).ToArray());

                object[] outentry = { System.Text.Encoding.Default.GetString(ItemName), BitConverter.ToString(ItemHex).Replace("-", "") };
                entries.Add(outentry);
                LeftAt = LeftAt + 6 + EntryLength;

                i++;
            }
            Console.WriteLine(LeftAt);
            Console.WriteLine(data.Length);
            Console.WriteLine(data.Length - LeftAt);
            extradata.AddRange(data.Skip(LeftAt).Take((data.Length) - LeftAt).ToArray());
            try
			{
                List<byte> MaxEntArrTempB = new List<byte>();
                MaxEntArrTempB.AddRange(extradata.Skip(-1).Take(4));
                MaxEntArrTempB.Reverse();
                int MaxEntriesB = BitConverter.ToInt32(MaxEntArrTempB.ToArray(), 0);
                int LeftAtB = 4;
                Console.WriteLine("MaxEntries (Extra Data) - " + MaxEntriesB);
                for (int j = 0; j < MaxEntriesB; j++)
                {
                    List<byte> ItemNameLength = new List<byte>();
                    ItemNameLength.AddRange(extradata.Skip(LeftAtB).Take(2));
                    ItemNameLength.Reverse();
                    int EntryLength = BitConverter.ToInt16(ItemNameLength.ToArray(), 0);
                    //Console.WriteLine(EntryLength);

                    byte[] ItemName = (extradata.Skip(LeftAtB + 2).Take(EntryLength).ToArray());
                    byte[] ItemHex = (extradata.Skip(LeftAtB + 2 + EntryLength).Take(4).ToArray());
                    byte[] ItemHexB = (extradata.Skip(LeftAtB + 6 + EntryLength).Take(4).ToArray());
                    byte[] ItemHexC = (extradata.Skip(LeftAtB + 10 + EntryLength).Take(4).ToArray());

                    object[] outentry = { System.Text.Encoding.Default.GetString(ItemName), BitConverter.ToString(ItemHex).Replace("-", "") };
                    object[] outentryB = { System.Text.Encoding.Default.GetString(ItemName) + " (Underwater)", BitConverter.ToString(ItemHexB).Replace("-", "") };
                    object[] outentryC = { System.Text.Encoding.Default.GetString(ItemName) + " (Fog)", BitConverter.ToString(ItemHexC).Replace("-", "") };
                    waterEntries.Add(outentry);
                    waterEntries.Add(outentryB);
                    waterEntries.Add(outentryC);
                    LeftAtB = LeftAtB + 14 + EntryLength;
                }
            }
            catch(Exception e)
			{
                Console.WriteLine(e.Message);
			}
        }


        static byte[] StringToByteArrayFastest(string hex)
        {
            if (hex.Length % 2 == 1)
                throw new Exception("The binary key cannot have an odd number of digits");

            byte[] arr = new byte[hex.Length >> 1];

            for (int i = 0; i < hex.Length >> 1; ++i)
            {
                arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));
            }

            return arr;
        }

        static int GetHexVal(char hex)
        {
            int val = (int)hex;
            //For uppercase A-F letters:
            //return val - (val < 58 ? 48 : 55);
            //For lowercase a-f letters:
            //return val - (val < 58 ? 48 : 87);
            //Or the two combined, but a bit slower:
            return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
        }
    }
}
