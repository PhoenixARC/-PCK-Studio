using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cyotek.Data.Nbt;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using OMI;
using PckStudio.Core.Extensions;

namespace PckStudio.Core
{
    public class SaveData
    {
        public NbtDocument LevelData { get; }
        public IDictionary<Guid, NbtDocument> Players { get; }


        private IDictionary<string, byte[]> _worldArchive;

        public SaveData(IDictionary<string, byte[]> worldArchive)
        {
            _worldArchive = worldArchive;
            if (_worldArchive.TryGetValue("levels.dat", out byte[] levelData))
            {
                Stream stream = new MemoryStream(levelData);
                LevelData = NbtDocument.LoadDocument(stream);
            }
            Players = _worldArchive
                .Where(kv => kv.Key.StartsWith("players/"))
                .ToDictionary(
                    kv => Guid.Parse(Path.GetFileNameWithoutExtension(kv.Key)), 
                    kv => NbtDocument.LoadDocument(new MemoryStream(kv.Value)));
        }
    }


    public class MapReader
    {
        public static SaveData OpenSaveData(Stream stream)
        {
            EndiannessAwareBinaryReader reader = new EndiannessAwareBinaryReader(stream, ByteOrder.BigEndian);
            _ = reader.ReadInt32();
            int bufferSize = reader.ReadInt32();
            byte[] buffer = new byte[bufferSize];
            using Stream decompStream = new InflaterInputStream(stream);
            decompStream.Read(buffer, 0, bufferSize);
            reader = new EndiannessAwareBinaryReader(new MemoryStream(buffer), System.Text.Encoding.BigEndianUnicode, ByteOrder.BigEndian);

            int fileStartOffset = reader.ReadInt32();
            int fileCount = reader.ReadInt32();
            reader.BaseStream.Seek(fileStartOffset, SeekOrigin.Begin);
            Dictionary<string, byte[]> res = new Dictionary<string, byte[]>();
            for (int i = 0; i < fileCount; i++)
            {
                string path = reader.ReadString(64).Replace('\\', '/');
                int size = reader.ReadInt32();
                int offset = reader.ReadInt32();
                long timestemp = reader.ReadInt64();

                long origin = reader.BaseStream.Position;
                reader.BaseStream.Seek(offset, SeekOrigin.Begin);
                byte[] data = reader.ReadBytes(size);
                reader.BaseStream.Seek(origin, SeekOrigin.Begin);

                res.Add(path, data);
            }
            return new SaveData(res);
        }
    }
}
