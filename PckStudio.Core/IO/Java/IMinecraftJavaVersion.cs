using System;

namespace PckStudio.Core.IO.Java
{
    interface IMinecraftJavaVersion : IEquatable<Version>
    {
        string ToString(string seperator);
    }
}
