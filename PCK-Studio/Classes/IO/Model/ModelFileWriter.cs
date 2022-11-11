using PckStudio.Classes.FileTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PckStudio.Classes.IO.Model
{
    internal class ModelWriter : StreamDataWriter
    {
        private ModelFile _modelFile;
        public static void Write(Stream stream, ModelFile modelFile)
        {
            new ModelWriter(modelFile, false).WriteToStream(stream);
        }

        public ModelWriter(ModelFile modelFile, bool littleEndian) : base(littleEndian)
        {
            _modelFile = modelFile;
        }

        protected override void WriteToStream(Stream stream)
        {
            int version = 0;
            WriteInt(stream, version);
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
                    WriteFloat(stream, part.position.x);
                    WriteFloat(stream, part.position.y);
                    WriteFloat(stream, part.position.z);
                    
                    WriteFloat(stream, part.rotation.yaw);
                    WriteFloat(stream, part.rotation.pitch);
                    WriteFloat(stream, part.rotation.roll);

                    if (version > 0)
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
