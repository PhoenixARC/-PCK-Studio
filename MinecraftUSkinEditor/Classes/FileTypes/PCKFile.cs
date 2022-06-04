using PckStudio.Classes.IO;
using System;
using System.Collections.Generic;
using System.IO;

namespace PckStudio.Classes.FileTypes
{
    public class PCKFile
    {
        public int type { get; } = -1;
        public Dictionary<string, int> meta_data { get; } = new Dictionary<string, int>();
        public List<FileData> file_entries { get; set; } = new List<FileData>();

        public class FileData
        {
            public enum EDLCType : int
            {
                DLCSkinFile,
		        DLCCapeFile,
		        DLCTextureFile,
		        DLCUIDataFile,
		        DLCLocalisationFile = 6,
		        DLCGameRulesFile,
		        DLCAudioFile,
		        DLCColourTableFile,
		        DLCGameRulesHeader,
		        DLCModelsFile = 12,
		        DLCBehavioursFile,
		        DLCMaterialFile,
            }

            public string name;
            public int type { get; }
            public byte[] data => _data;
            public int size => _size;
            public PCKProperties properties { get; set; } = new PCKProperties();

            private byte[] _data = new byte[0];
            private int _size = 0;

            public FileData(string name, int type)
            {
                this.type = type;
                this.name = name;
            }

            public FileData(string name, int type, int dataSize)
            {
                this.type = type;
                this.name = name;
                _size = dataSize;
                _data = new byte[dataSize];
            }

            public void SetData(byte[] data)
            {
                _data = data;
                _size = data.Length;
            }

        }

        public PCKFile(int type)
        {
            this.type = type;
        }
    }
}