using PckStudio.Classes.FileTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PckStudio.Classes.IO.Model
{
    internal class ModelFileWriter : StreamDataWriter
    {
        private ModelFile _modelFile;
        private int _fileVersion;
        public static void Write(Stream stream, ModelFile modelFile, int fileVersion = 1)
        {
            new ModelFileWriter(modelFile, fileVersion).WriteToStream(stream);
        }

        public ModelFileWriter(ModelFile modelFile, int fileVersion) : base(false)
        {
            _modelFile = modelFile;
            if (fileVersion < 0 || fileVersion > 2)
                throw new InvalidDataException(nameof(fileVersion));
            _fileVersion = fileVersion;
        }

        protected override void WriteToStream(Stream stream)
        {
            WriteInt(stream, _fileVersion);
            WriteInt(stream, _modelFile.Models.Count);
            foreach (var model in _modelFile.Models)
            {
                WriteString(stream, model.name);
                WriteInt(stream, model.textureSize.Width);
                WriteInt(stream, model.textureSize.Height);
                WriteInt(stream, model.parts.Count);
                foreach (var part in model.parts)
                {
                    WriteString(stream, part.name);

                    if (_fileVersion > 1)
                    {
                        WriteString(stream, model.parts[0].name.Equals(part.name) ? string.Empty : model.parts[0].name);
                    }

                    WriteFloat(stream, part.position.x);
                    WriteFloat(stream, part.position.y);
                    WriteFloat(stream, part.position.z);
                    
                    WriteFloat(stream, part.rotation.yaw);
                    WriteFloat(stream, part.rotation.pitch);
                    WriteFloat(stream, part.rotation.roll);

                    if (_fileVersion > 0)
                    {
                        WriteFloat(stream, 0.0f);
                        WriteFloat(stream, 0.0f);
                        WriteFloat(stream, 0.0f);
                    }

                    WriteInt(stream, part.Boxes.Count);
                    foreach (var box in part.Boxes)
                    {
                        WriteFloat(stream, box.Position.x);
                        WriteFloat(stream, box.Position.y);
                        WriteFloat(stream, box.Position.z);
                        
                        WriteInt(stream, box.Size.width);
                        WriteInt(stream, box.Size.height);
                        WriteInt(stream, box.Size.length);

                        WriteFloat(stream, box.U);
                        WriteFloat(stream, box.V);
                        WriteFloat(stream, box.Scale);

                        WriteBool(stream, box.Mirror);
                    }
                }
            }
        }

        private void WriteString(Stream stream, string s)
        {
            WriteShort(stream, (short)s.Length);
            WriteString(stream, s, Encoding.ASCII);
        }
    }
}
