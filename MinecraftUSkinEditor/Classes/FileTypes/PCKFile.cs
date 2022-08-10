using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PckStudio.Classes.FileTypes
{
    public class PCKFile
    {
        public readonly int type;
        public List<FileData> Files { get; } = new List<FileData>();

        public class FileData
        {
            public enum EDLCType : int
            {
                DLCSkinFile         = 0,  // *.png
		        DLCCapeFile         = 1,  // *.png
                DLCTextureFile      = 2,  // *.png
                DLCUIDataFile       = 3,  // *.fui ????
                /// <summary>
                /// "0" file
                /// </summary>
                DLCInfoFile         = 4,
                /// <summary>
                /// (x16|x32|x64)Info.pck
                /// </summary>
                DLCTexturePackInfoFile = 5,
                /// <summary>
                /// languages.loc/localisation.loc
                /// </summary>
                DLCLocalisationFile = 6,
                /// <summary>
                /// GameRules.grf
                /// </summary>
                DLCGameRulesFile    = 7,
                /// <summary>
                /// audio.pck
                /// </summary>
                DLCAudioFile        = 8,
                /// <summary>
                /// colours.col
                /// </summary>
                DLCColourTableFile  = 9,
                /// <summary>
                /// GameRules.grh
                /// </summary>
                DLCGameRulesHeader  = 10,
                /// <summary>
                /// Skins.pck
                /// </summary>
                DLCSkinDataFile     = 11,
                /// <summary>
                /// models.bin
                /// </summary>
		        DLCModelsFile       = 12,
                /// <summary>
                /// behaviours.bin
                /// </summary>
                DLCBehavioursFile   = 13,
                /// <summary>
                /// entityMaterials.bin
                /// </summary>
                DLCMaterialFile     = 14,
            }

            public string filepath { get; set; }
            public int type { get; set; }
            public byte[] data => _data;
            public int size => _size;
            public PCKProperties properties { get; set; } = new PCKProperties();

            private byte[] _data = new byte[0];
            private int _size = 0;

            public FileData(string path, int type)
            {
                this.type = type;
                filepath = path;
            }

            public FileData(string path, int type, int dataSize) : this(path, type)
            {
                _size = dataSize;
                _data = new byte[dataSize];
            }

            public FileData(FileData file) : this(file.filepath, file.type)
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

        public List<string> GatherPropertiesList()
        {
            var LUT = new List<string>();
            Files.ForEach(file => file.properties.ForEach(pair =>
                {
                    if (!LUT.Contains(pair.Item1))
                        LUT.Add(pair.Item1);
                })
            );
            return LUT;
        }

        /// <summary>
        /// Checks wether a file with <paramref name="filepath"/> and <paramref name="type"/> exists
        /// </summary>
        /// <param name="filepath">Path to the file in the pck</param>
        /// <param name="type">Type of the file <see cref="FileData.EDLCType"/></param>
        /// <returns>True when file exists, otherwise false </returns>
        public bool HasFile(string filepath, int type)
        {
            return GetFile(filepath, type) is FileData;
        }

        /// <summary>
        /// Tries to get a file with <paramref name="filepath"/> and <paramref name="type"/>.
        /// </summary>
        /// <param name="filepath">Path to the file in the pck</param>
        /// <param name="type">Type of the file <see cref="FileData.EDLCType"/></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public bool TryGetFile(string filepath, int type, out FileData file)
        {
            file = GetFile(filepath, type);
            return file is FileData;
        }

        /// <summary>
        /// Gets the first file that Equals <paramref name="filepath"/> and <paramref name="type"/>
        /// </summary>
        /// <param name="filepath">Path to the file in the pck</param>
        /// <param name="type">Type of the file <see cref="FileData.EDLCType"/></param>
        /// <returns>FileData if found, otherwise null</returns>
        public FileData GetFile(string filepath, int type)
        {
            return Files.FirstOrDefault(file => file.filepath.Equals(filepath) && file.type.Equals(type));
        }
    }
}