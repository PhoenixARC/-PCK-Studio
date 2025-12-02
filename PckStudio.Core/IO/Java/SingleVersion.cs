using System;

namespace PckStudio.Core.IO.Java
{
    class SingleVersion(Version version) : IVersion
    {
        private readonly Version _version = version;

        public SingleVersion(string version) : this(new Version(version)) { }

        public bool Equals(Version other) => _version.Equals(other);

        public string ToString(string _) => _version.ToString();
    }
}
