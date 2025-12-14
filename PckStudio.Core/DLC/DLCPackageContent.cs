using System;
using System.Collections.Generic;
using OMI.Formats.Pck;

namespace PckStudio.Core.DLC
{
    public class DLCPackageContent
    {
        public static DLCPackageContent Empty => new DLCPackageContent(default);

        internal bool IsEmpty { get; }

        internal PckFile MainPck { get; }

        internal DLCDataFolderContent DataFolder { get; }

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

        public DLCPackageContent(PckFile mainPck, NamedData<PckFile> texturePck, NamedData<byte[]>[] dataFiles)
            : this(mainPck, new(texturePck, dataFiles ?? Array.Empty<NamedData<byte[]>>()))
        {
        }

        public DLCPackageContent(PckFile mainPck, DLCDataFolderContent dataFolderContent)
        {
            MainPck = mainPck;
            DataFolder = dataFolderContent;
            IsEmpty = mainPck is null;
        }

        public DLCPackageContent(PckFile mainPck) : this(mainPck, default) { }
    }
}