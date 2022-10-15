using PckStudio.Classes.FileTypes;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace PckStudio.Classes.IO.Model
{
    public class ModelReader : StreamDataReader
    {
        public static ModelFile Read(Stream stream, bool useLittleEndian = false)
        {
            return new ModelReader(useLittleEndian).ReadFromStream(stream);
        }

        private ModelReader(bool useLittleEndian) : base(useLittleEndian)
        {
        }

        private ModelFile ReadFromStream(Stream stream)
        {
            var modelFile = new ModelFile();
            int version = ReadInt(stream);
            int modelCount = ReadInt(stream);
            for (; 0 < modelCount; modelCount--)
            {
                string name = ReadString(stream);
                int width = ReadInt(stream);
                int height = ReadInt(stream);
                var model = new ModelFile.Model(name, width, height);

                int partCount = ReadInt(stream);
                for (; 0 < partCount; partCount--)
                {
                    string partname = ReadString(stream);
                    float x = ReadFloat(stream);
                    float y = ReadFloat(stream);
                    float z = ReadFloat(stream);

                    float yaw = ReadFloat(stream);
                    float pitch = ReadFloat(stream);
                    float roll = ReadFloat(stream);
                    var part = new ModelFile.Model.Part(partname, (x, y, z), (yaw, pitch, roll));
                    if (version > 0)
                    {
                        float _1 = ReadFloat(stream);
                        float _2 = ReadFloat(stream);
                        float _3 = ReadFloat(stream);
                        Debug.WriteLine("[{0}]: {1}, {2}, {3}", nameof(ModelReader), _1, _2, _3);
                    }

                    int boxCount = ReadInt(stream);
                    for (; 0 < boxCount; boxCount--)
                    {
                        var pos = (ReadFloat(stream), ReadFloat(stream), ReadFloat(stream));
                        var size = (ReadInt(stream), ReadInt(stream), ReadInt(stream));
                        float u = ReadFloat(stream), v = ReadFloat(stream);
                        float scale = ReadFloat(stream);
                        bool mirrored = ReadBool(stream);
                        part.AddBox(pos, size, u, v, scale, mirrored);
                    }
                    model.AddPart(part);
                }
                modelFile.AddModel(model);

            }
            return modelFile;
        }

        private string ReadString(Stream stream)
        {
            short length = ReadShort(stream);
            return ReadString(stream, length, Encoding.ASCII);
        }

    }
}
