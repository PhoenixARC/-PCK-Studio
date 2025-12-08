using OMI;
using OMI.Workers;
using PckStudio.Core.FileFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PckStudio.Core.IO.PckAudio
{
    public class InvalidAudioPckException(string message) : Exception(message) { }

    public class PckAudioFileReader : IDataFormatReader<PckAudioFile>, IDataFormatReader
    {
        private PckAudioFile _file;
        private ByteOrder _endianness;
        private List<string> LUT = new List<string>();
        private List<PckAudioFile.Category> _OriginalAudioTypeOrder = new List<PckAudioFile.Category>();

        public PckAudioFileReader(ByteOrder endianness)
        {
            _endianness = endianness;
        }

        public PckAudioFile FromFile(string filename)
        {
            if(File.Exists(filename))
            {
                PckAudioFile file;
                using(FileStream fs = File.OpenRead(filename))
                {
                    file = FromStream(fs);
                }
                return file;
            }
            throw new FileNotFoundException(filename);
        }

        public PckAudioFile FromStream(Stream stream)
        {
            using (var reader = new EndiannessAwareBinaryReader(stream,
                _endianness == ByteOrder.BigEndian
                ? Encoding.BigEndianUnicode
                : Encoding.Unicode,
                leaveOpen: true, _endianness))
            {
                int pck_type = reader.ReadInt32();
                if (pck_type > 0xf00000) // 03 00 00 00 == true
                    throw new OverflowException(nameof(pck_type));
                if (pck_type > 1)
                    throw new InvalidAudioPckException(nameof(pck_type));
                _file = new PckAudioFile();
                ReadLookUpTable(reader);
                ReadCategories(reader);
                ReadCategorySongs(reader);
            }
            return _file;
        }

        private void ReadLookUpTable(EndiannessAwareBinaryReader reader)
        {
            int count = reader.ReadInt32();
            LUT = new List<string>(count);
            for (int i = 0; i < count; i++)
            {
                int index = reader.ReadInt32();
                string value = ReadString(reader);
                LUT.Insert(index, value);
            }
        }

        private void ReadCategories(EndiannessAwareBinaryReader reader)
        {
            int categoryEntryCount = reader.ReadInt32();
            for (; 0 < categoryEntryCount; categoryEntryCount--)
            {
                var parameterType = (PckAudioFile.EAudioParameterType)reader.ReadInt32();
                var audioType = (PckAudioFile.Category)reader.ReadInt32();
                string name = ReadString(reader);
                // AddCategory puts the file's categories out of order and causes some songs to be put in the wrong categories
                // This is my simple fix for the issue.
                _OriginalAudioTypeOrder.Add(audioType);
                _file.AddCategory(name, audioType, parameterType);
            }
        }

        private void ReadCategorySongs(EndiannessAwareBinaryReader reader)
        {
            List<string> credits = new List<string>();
            List<string> creditIds = new List<string>();
            foreach (PckAudioFile.Category c in _OriginalAudioTypeOrder)
            {
                int audioCount = reader.ReadInt32();
                for (; 0 < audioCount; audioCount--)
                {
                    string key = LUT[reader.ReadInt32()];
                    string value = ReadString(reader);
                    switch (key)
                    {
                        case "CUENAME":
                            _file.GetCategory(c).SongNames.Add(value);
                            break;
                        case "CREDIT":
                            credits.Add(value);
                            break;
                        case "CREDITID":
                            creditIds.Add(value);
                            _file.AddCreditId(value);
                            break;
                        default:
                            throw new InvalidDataException(nameof(key));
                    }
                }
            }
            foreach ((string str, string id) credit in credits.Zip(creditIds, (str, id) => (str, id)))
            {
                _file.SetCredit(credit.id, credit.str);
            }
        }

        private string ReadString(EndiannessAwareBinaryReader reader)
        {
            int len = reader.ReadInt32();
            string s = reader.ReadString(len);
            reader.ReadInt32(); // padding
            return s;
        }

        object IDataFormatReader.FromStream(Stream stream) => FromStream(stream);

        object IDataFormatReader.FromFile(string filename) => FromFile(filename);
    }
}
