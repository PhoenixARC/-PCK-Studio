using OMI;
using OMI.Workers;
using PckStudio.Core.FileFormats;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PckStudio.Core.IO.PckAudio
{
    public class PckAudioFileWriter : IDataFormatWriter
    {

        private PckAudioFile _file;
        private ByteOrder _endianness;
        private static readonly List<string> LUT = new List<string>
        {
            "CUENAME",
            "CREDIT",
            "CREDITID"
        };

        public PckAudioFileWriter(PckAudioFile file, ByteOrder endianness)
        {
            _file = file;
            _endianness = endianness;
        }

        public void WriteToFile(string filename)
        {
            using (FileStream fs = File.OpenWrite(filename))
            {
                WriteToStream(fs);
            }
        }

        public void WriteToStream(Stream stream)
        {
            using (var writer = new EndiannessAwareBinaryWriter(stream,
                _endianness == ByteOrder.BigEndian
                ? Encoding.BigEndianUnicode
                : Encoding.Unicode,
                leaveOpen: true, _endianness))
            {
                writer.Write(_file.Type);
                WriteLookUpTable(writer);
                WriteCategories(writer);
                WriteCategorySongs(writer);
            }
        }

        private void WriteString(EndiannessAwareBinaryWriter writer, string s)
        {
            writer.Write(s.Length);
            writer.WriteString(s);
            writer.Write(0); // padding
        }

        private void WriteLookUpTable(EndiannessAwareBinaryWriter writer)
        {
            writer.Write(3);
            for (int i = 0; i < 3; i++)
            {
                writer.Write(i);
                WriteString(writer, LUT[i]);
            }
        }

        private void WriteCategories(EndiannessAwareBinaryWriter writer)
        {
            writer.Write(_file.Categories.Length);
            foreach (PckAudioFile.AudioCategory category in _file.Categories)
            {
                writer.Write((int)category.ParameterType);
                writer.Write((int)category.AudioType);
                WriteString(writer, category.Name);
            }
        }

        private void WriteCategorySongs(EndiannessAwareBinaryWriter writer)
        {
            bool addCredit = true;
            foreach (PckAudioFile.AudioCategory category in _file.Categories)
            {
                writer.Write(category.SongNames.Count + (addCredit ? _file.Credits.Count * 2 : 0));
                foreach (var name in category.SongNames)
                {
                    writer.Write(LUT.IndexOf("CUENAME"));
                    WriteString(writer, name);
                }
                if (addCredit)
                {
                    foreach (KeyValuePair<string, string> credit in _file.Credits)
                    { 
                        writer.Write(LUT.IndexOf("CREDIT"));
                        WriteString(writer, credit.Value);
                        writer.Write(LUT.IndexOf("CREDITID"));
                        WriteString(writer, credit.Key);
                    }
                }
                addCredit = false;
            }
        }
    }
}
