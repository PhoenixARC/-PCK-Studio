using PckStudio.Classes.FileTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PckStudio.Classes.IO.Behaviour
{
    public class BehavioursReader : StreamDataReader<BehaviourFile>
    {
        public static BehaviourFile Read(Stream stream)
        {
            return new BehavioursReader().ReadFromStream(stream);
        }

        protected BehavioursReader() : base(false) // Doesn't seem that Behaviours uses little endian
        {
        }

        protected override BehaviourFile ReadFromStream(Stream stream)
        {
            BehaviourFile behaviourFile = new BehaviourFile();
            _ = ReadInt(stream);
            int riderPosOverrideCount = ReadInt(stream);
            for (int i = 0; i < riderPosOverrideCount; i++)
            {
                string name = ReadString(stream);
                var riderPositionOverride = new BehaviourFile.RiderPositionOverride(name);
                int posOverideCount = ReadInt(stream);
                for (; 0 < posOverideCount; posOverideCount--)
                {
                    var posOverride = new BehaviourFile.RiderPositionOverride.PositionOverride();
                    posOverride.EntityIsTamed = ReadBool(stream);
                    posOverride.EntityHasSaddle = ReadBool(stream);
                    posOverride.x = ReadFloat(stream);
                    posOverride.y = ReadFloat(stream);
                    posOverride.z = ReadFloat(stream);
                    riderPositionOverride.overrides.Add(posOverride);
                }
                behaviourFile.entries.Add(riderPositionOverride);
            }
            return behaviourFile;
        }

        private string ReadString(Stream stream)
        {
            short length = ReadShort(stream);
            return ReadString(stream, length, Encoding.ASCII);
        }
    }
}
