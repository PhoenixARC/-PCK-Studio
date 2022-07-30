using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PckStudio.Classes.FileTypes
{
    public class PCKProperties : List<ValueTuple<string, string>> // class because `using` is file scoped :|
    {
        public bool HasProperty(string property)
        {
            return GetProperty(property) != default;
        }

        public ValueTuple<string, string> GetProperty(string property)
        {
            return this.FirstOrDefault(p => p.Item1.Equals(property));
        }

        public void SetProperty(string property, string value)
        {
            if (HasProperty(property))
            {
                this[IndexOf(GetProperty(property))] = (property, value);
                return;
            }
            Add((property, value));
        }

    }
}
