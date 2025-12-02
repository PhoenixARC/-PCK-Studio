using System;

namespace PckStudio.Core.IO.Java
{
    interface IVersion : IEquatable<Version>
    {
        string ToString(string seperator);
    }
}
