using System;
using System.Collections.Generic;
using OMI.Formats.Pck;

namespace PckStudio.Core.DLC
{
    public class DLCPackageContent
    {
        public static DLCPackageContent Empty => new DLCPackageContent(nameof(Empty), default);

        public bool IsEmpty { get; }

        public string Name { get; }
        public PckFile MainPck { get; }

        public bool HasDataFolder => DataFolder != null;
        public DLCDataFolderContent DataFolder { get; }

        public record DLCDataFolderContent
        {
            public NamedData<PckFile> TexturePck { get; }
            public List<NamedData<byte[]>> Files { get; }

            public DLCDataFolderContent(NamedData<PckFile> texturePck, NamedData<byte[]>[] files)
            {
                TexturePck = texturePck;
                Files = new List<NamedData<byte[]>>(files);
            }

            public void AddFile(NamedData<byte[]> namedData) => Files.Add(namedData);
            public void AddFiles(NamedData<byte[]>[] namedData) => Files.AddRange(namedData);
            public void AddFile(string name, byte[] data) => AddFile(new NamedData<byte[]>(name, data));
        }

        public DLCPackageContent(string name, PckFile mainPck, NamedData<PckFile> texturePck, NamedData<byte[]>[] dataFiles)
            : this(name, mainPck, new(texturePck, dataFiles ?? Array.Empty<NamedData<byte[]>>()))
        {
        }

        public DLCPackageContent(string name, PckFile mainPck, DLCDataFolderContent dataFolderContent)
        {
            MainPck = mainPck;
            DataFolder = dataFolderContent;
            Name = name;
            IsEmpty = mainPck is null;
        }

        public DLCPackageContent(string name, PckFile mainPck) : this(name, mainPck, default) { }
    }
}