using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PckStudio.Classes.FileTypes
{
    public class BehaviourFile
    {
        public List<RiderPositionOverride> entries { get; } = new List<RiderPositionOverride>();
        public class RiderPositionOverride
        {
            public string name;
            public List<PositionOverride> overrides { get; }

            public RiderPositionOverride(string name)
            {
                this.name = name;
                overrides = new List<PositionOverride>();
            }

            public class PositionOverride
            {
                public bool EntityIsTamed;
                public bool EntityHasSaddle;
                public float x, y, z;
            }
        }
    }
}
