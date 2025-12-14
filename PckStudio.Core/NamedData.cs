using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PckStudio.Core
{
    public readonly struct NamedData<T>(string name, T value)
    {
        public readonly string Name = name;
        public readonly T Value = value;

        public static implicit operator NamedData<T>(KeyValuePair<string , T> kvp) => new NamedData<T>(kvp.Key, kvp.Value);
    }
}
