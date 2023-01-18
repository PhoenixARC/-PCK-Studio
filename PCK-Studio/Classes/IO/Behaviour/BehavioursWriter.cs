using PckStudio.Classes.FileTypes;
using System;
using System.IO;
using System.Text;

namespace PckStudio.Classes.IO.Behaviour
{
    internal class BehavioursWriter : StreamDataWriter
    {
        private BehaviourFile behaviourFile;

        public static void Write(Stream stream, BehaviourFile file)
        {
            new BehavioursWriter(file).WriteToStream(stream);
        }

        public BehavioursWriter(BehaviourFile file) : base(false)
        {
            behaviourFile = file;
        }

        protected override void WriteToStream(Stream stream)
        {
            WriteInt(stream, 0);
            WriteInt(stream, behaviourFile.entries.Count);
            foreach (var entry in behaviourFile.entries)
            {
                WriteString(stream, entry.name);
                WriteInt(stream, entry.overrides.Count);
                foreach(var posOverride in entry.overrides)
				{
                    WriteBool(stream, posOverride._1);
                    WriteBool(stream, posOverride._2);
                    WriteFloat(stream, posOverride.x);
                    WriteFloat(stream, posOverride.y);
                    WriteFloat(stream, posOverride.z);
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