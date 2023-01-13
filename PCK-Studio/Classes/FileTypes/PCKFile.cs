using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PckStudio.Classes.FileTypes
{
    public class PCKFile
    {
        public int type { get; }
        public List<FileData> Files { get; } = new List<FileData>();

        public const string XMLVersionString = "XMLVERSION";

        public class FileData
        {
            public enum FileType : int
            {
                SkinFile         = 0,  // *.png
		        CapeFile         = 1,  // *.png
                TextureFile      = 2,  // *.png
                UIDataFile       = 3,  // *.fui ????
                /// <summary>
                /// "0" file
                /// </summary>
                InfoFile         = 4,
                /// <summary>
                /// (x16|x32|x64)Info.pck
                /// </summary>
                TexturePackInfoFile = 5,
                /// <summary>
                /// languages.loc/localisation.loc
                /// </summary>
                LocalisationFile = 6,
                /// <summary>
                /// GameRules.grf
                /// </summary>
                GameRulesFile    = 7,
                /// <summary>
                /// audio.pck
                /// </summary>
                AudioFile        = 8,
                /// <summary>
                /// colours.col
                /// </summary>
                ColourTableFile  = 9,
                /// <summary>
                /// GameRules.grh
                /// </summary>
                GameRulesHeader  = 10,
                /// <summary>
                /// Skins.pck
                /// </summary>
                SkinDataFile     = 11,
                /// <summary>
                /// models.bin
                /// </summary>
		        ModelsFile       = 12,
                /// <summary>
                /// behaviours.bin
                /// </summary>
                BehavioursFile   = 13,
                /// <summary>
                /// entityMaterials.bin
                /// </summary>
                MaterialFile     = 14,
            }

            public string Filename {
                get => filename;
                set => filename = value.Replace('\\', '/');
            }

            public FileType Filetype { get; set; }
            public byte[] Data => _data;
            public int Size => _data is null ? 0 : _data.Length;
            public PCKProperties Properties { get; } = new PCKProperties();

            private string filename;
            private byte[] _data = new byte[0];

            public FileData(string name, FileType type)
            {
                Filetype = type;
                Filename = name;
            }

            public FileData(string name, FileType type, int dataSize) : this(name, type)
            {
                _data = new byte[dataSize];
            }

            public FileData(FileData file) : this(file.Filename, file.Filetype)
            {
                Properties = file.Properties;
                SetData(file.Data);
            }

            public void SetData(byte[] data)
            {
                _data = data;
            }

        }

        public PCKFile(int type)
        {
            this.type = type;
        }

        public List<string> GetPropertyList()
        {
            var LUT = new List<string>();
            Files.ForEach(file => file.Properties.ForEach(pair =>
                {
                    if (!LUT.Contains(pair.property))
                        LUT.Add(pair.property);
                })
            );
            return LUT;
        }

        /// <summary>
        /// Checks wether a file with <paramref name="filepath"/> and <paramref name="type"/> exists
        /// </summary>
        /// <param name="filepath">Path to the file in the pck</param>
        /// <param name="type">Type of the file <see cref="FileData.FileType"/></param>
        /// <returns>True when file exists, otherwise false </returns>
        public bool HasFile(string filepath, FileData.FileType type)
        {
            return GetFile(filepath, type) is FileData;
        }

        /// <summary>
        /// Gets the first file that Equals <paramref name="filepath"/> and <paramref name="type"/>
        /// </summary>
        /// <param name="filepath">Path to the file in the pck</param>
        /// <param name="type">Type of the file <see cref="FileData.FileType"/></param>
        /// <returns>FileData if found, otherwise null</returns>
        public FileData GetFile(string filepath, FileData.FileType type)
        {
            return Files.FirstOrDefault(file => file.Filename.Equals(filepath) && file.Filetype.Equals(type));
        }

        /// <summary>
        /// Tries to get a file with <paramref name="filepath"/> and <paramref name="type"/>.
        /// </summary>
        /// <param name="filepath">Path to the file in the pck</param>
        /// <param name="type">Type of the file <see cref="FileData.FileType"/></param>
        /// <param name="file">If succeeded <paramref name="file"/> will be non-null, otherwise null</param>
        /// <returns>True if succeeded, otherwise false</returns>
        public bool TryGetFile(string filepath, FileData.FileType type, out FileData file)
        {
            file = GetFile(filepath, type);
            return file is FileData;
        }
    }
}