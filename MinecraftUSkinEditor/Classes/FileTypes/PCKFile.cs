using System;
using System.Collections.Generic;
using System.IO;

namespace PckStudio.Classes.FileTypes
{
    public class PCKFile
    {
        public int type { get; }
        public List<string> meta_data { get; } = new List<string>();
        public List<FileData> file_entries { get; } = new List<FileData>();

        public class FileData
        {
            // only apllys when the PCKFile is type 3
            public enum EDLCType : int
            {
                DLCSkinFile         = 0,  // *.png
		        DLCCapeFile         = 1,  // *.png
                DLCTextureFile      = 2,  // *.png
                DLCUIDataFile       = 3,  // *.fui ????
                // DLCInfoFile         = 4, // "0" file
                // DLCTexturePackInfoFile = 5, // x16Info.pck
                DLCLocalisationFile = 6,  // languages.loc/localisation.loc
                DLCGameRulesFile    = 7,  // *.grf
                DLCAudioFile        = 8,  // audio.pck
                DLCColourTableFile  = 9,  // colours.col
                DLCGameRulesHeader  = 10, // *.grh
                DLCSkinDataFile     = 11, // *.pck made up name  -Miku
		        DLCModelsFile       = 12, // models.bin
                DLCBehavioursFile   = 13, // behaviours.bin
                DLCMaterialFile     = 14, // entityMaterials.bin
            }

            public string name { get; set; }
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

            public FileData(FileData file) : this(file.name, file.type)
            {
                properties = file.properties;
                SetData(file.data);
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

        public void ValidateMeta()
        {
            file_entries.ForEach(file =>
            {
                file.properties.ForEach(pair =>
                {
                    if (!meta_data.Contains(pair.Item1))
                        meta_data.Add(pair.Item1);
                });
            });
        }

        public bool HasFile(string name, int type)
        {
            return GetFile(name, type) != null;
        }

        public FileData GetFile(string name, int type)
        {
            foreach (var file in file_entries)
            {
                if (file.name == name && file.type == type)
                    return file;
            }
            return null;
        }
    }
}