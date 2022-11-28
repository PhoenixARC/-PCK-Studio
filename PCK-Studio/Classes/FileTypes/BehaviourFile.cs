using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PckStudio.Classes.FileTypes
{
    public class BehaviourFile
    {
        public List<RiderPositionOverride> overrides { get; } = new List<RiderPositionOverride>();
        public struct RiderPositionOverride
        {
            public string name;
            public List<PositionOverride> overrides { get; }

            public RiderPositionOverride(string name) : this()
            {
                this.name = name;
                overrides = new List<PositionOverride>();
            }

            public struct PositionOverride
            {
                public bool _1;
                public bool _2;
                public float x, y, z;
            }
        }
    }
}
