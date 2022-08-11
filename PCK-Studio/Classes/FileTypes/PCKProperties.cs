using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PckStudio.Classes.FileTypes
{
    public class PCKProperties : List<(string property, string value)>
    {
        public bool HasProperty(string property)
        {
            return GetProperty(property) != default;
        }

        public (string, string) GetProperty(string property)
        {
            return this.FirstOrDefault(p => p.property.Equals(property));
        }
        
        public (string, string)[] GetProperties(string property)
        {
            return FindAll(p => p.property == property).ToArray();
        }

        public bool HasMoreThanOneOf(string property)
        {
            return GetProperties(property).Length > 1;
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
