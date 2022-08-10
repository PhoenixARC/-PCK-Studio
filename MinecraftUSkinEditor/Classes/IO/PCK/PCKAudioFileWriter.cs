using PckStudio.Classes.FileTypes;
using PckStudio.Classes.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PckStudio.Classes.IO.PCK
{
    internal class PCKAudioFileWriter : StreamDataWriter
    {

        private PCKAudioFile _file;
        private static readonly List<string> LUT = new List<string>
        {
            "CUENAME",
            "CREDIT",
            "CREDITID"
        };

        public static void Write(Stream stream, PCKAudioFile file, bool isLittleEndian)
        {
            new PCKAudioFileWriter(file, isLittleEndian).WriteToStream(stream);
        }

        private PCKAudioFileWriter(PCKAudioFile file, bool isLittleEndian) : base(isLittleEndian)
        {
            _file = file;
        }

        private void WriteToStream(Stream stream)
        {
            WriteInt(stream, _file.type);
            WriteLookUpTable(stream);
            WriteCategories(stream);
            WriteCategorySongs(stream);
        }

        private void WriteString(Stream stream, string s)
        {
            WriteInt(stream, s.Length);
            WriteString(stream, s, IsUsingLittleEndian ? Encoding.Unicode : Encoding.BigEndianUnicode);
            WriteInt(stream, 0); // padding
        }

        private void WriteLookUpTable(Stream stream)
        {
            WriteInt(stream, 3);
            for (int i = 0; i < 3; i++)
            {
                WriteInt(stream, i);
                WriteString(stream, LUT[i]);
            }
        }

        private void WriteCategories(Stream stream)
        {
            WriteInt(stream, _file.Categories.Length);
            foreach (var category in _file.Categories)
            {
                WriteInt(stream, (int)category.parameterType);
                WriteInt(stream, (int)category.audioType);
                WriteString(stream, category.Name);
            }
        }

        private void WriteCategorySongs(Stream stream)
        {
            bool addCredit = true;
            foreach (var category in _file.Categories)
            {
                WriteInt(stream, category.SongNames.Count + (addCredit ? _file.Credits.Count * 2 : 0));
                foreach (var name in category.SongNames)
                {
                    WriteInt(stream, LUT.IndexOf("CUENAME"));
                    WriteString(stream, name);
                }
                if (addCredit)
                {
                    foreach (var credit in _file.Credits)
                    { 
                        WriteInt(stream, LUT.IndexOf("CREDIT"));
                        WriteString(stream, credit.Value);
                        WriteInt(stream, LUT.IndexOf("CREDITID"));
                        WriteString(stream, credit.Key);
                    }
                }
                addCredit = false;
            }
        }
    }
}
