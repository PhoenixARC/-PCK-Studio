using System;

namespace PckStudio.Core.IO.Java
{
    class VersionRange(Version min, Version max) : IMinecraftJavaVersion
    {
        private readonly Version _min = min;
        private readonly Version _max = max;

        public VersionRange(string min, string max) : this(new Version(min), new Version(max)) { }

        public bool Equals(Version other) => _min <= other && other <= _max;

        public string ToString(string seperator) => $"{_min}{seperator}{_max}";
    }
}
