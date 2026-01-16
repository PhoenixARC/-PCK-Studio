using System;
using PckStudio.Core.Extensions;

namespace PckStudio.Core.IO.Java
{
    class SpecificVerions(params Version[] versions) : IMinecraftJavaVersion
    {
        private readonly Version[] _versions = versions;

        public bool Equals(Version other) => other?.EqualsAny(_versions) ?? default;

        public string ToString(string seperator) => _versions.ToString(seperator);
    }
}
