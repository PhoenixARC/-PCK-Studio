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

    public class InvalidAudioPckException : Exception
    {
        public InvalidAudioPckException(string message) : base(message)
        { }
    }

    internal class PCKAudioFileReader : StreamDataReader
    {
        private PCKAudioFile _file;
        private List<string> LUT = new List<string>();


        public static PCKAudioFile Read(Stream stream, bool isLittleEndian)
        {
            return new PCKAudioFileReader(isLittleEndian).ReadFromStream(stream);
        }

        private PCKAudioFileReader(bool isLittleEndian) : base(isLittleEndian)
        {
        }

        private PCKAudioFile ReadFromStream(Stream stream)
        {
            int pck_type = ReadInt(stream);
            if (pck_type > 0xf00000) // 03 00 00 00 == true
                throw new OverflowException(nameof(pck_type));
            if (pck_type > 1)
                throw new InvalidAudioPckException(nameof(pck_type));
            _file = new PCKAudioFile();
            ReadLookUpTabel(stream);
            ReadCategories(stream);
            ReadCategorySongs(stream);
            return _file;
        }

        private void ReadLookUpTabel(Stream stream)
        {
            int count = ReadInt(stream);
            LUT = new List<string>(count);
            for (int i = 0; i < count; i++)
            {
                int index = ReadInt(stream);
                LUT.Insert(index, ReadString(stream));
            }
        }

        private List<PCKAudioFile.AudioCategory.EAudioType> original_audio_types = new List<PCKAudioFile.AudioCategory.EAudioType>();
        private void ReadCategories(Stream stream)
        {
            int categoryEntryCount = ReadInt(stream);
            for (; 0 < categoryEntryCount; categoryEntryCount--)
            {
                var parameterType = (PCKAudioFile.AudioCategory.EAudioParameterType)ReadInt(stream);
                var audioType = (PCKAudioFile.AudioCategory.EAudioType)ReadInt(stream);
                string name = ReadString(stream);
                // AddCategory puts the file's categories out of order and causes some songs to be put in the wrong categories
                // This is my simple fix for the issue.
                original_audio_types.Add(audioType);
                _file.AddCategory(parameterType, audioType, name);
            }
        }

        private void ReadCategorySongs(Stream stream)
        {
            List<string> credits = new List<string>();
            List<string> creditIds = new List<string>();
            foreach (var c in original_audio_types)
            {
                int audioCount = ReadInt(stream);
                for (; 0 < audioCount; audioCount--)
                {
                    string key = LUT[ReadInt(stream)];
                    string value = ReadString(stream);
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
            };
            foreach (var credit in credits.Zip(creditIds, (str, id) => (str, id)))
            {
                _file.SetCredit(credit.id, credit.str);
            }
        }

        private string ReadString(Stream stream)
        {
            int len = ReadInt(stream);
            string s = ReadString(stream, len, IsUsingLittleEndian ? Encoding.Unicode : Encoding.BigEndianUnicode);
            ReadInt(stream); // padding
            return s;
        }
    }
}
