using System;

namespace stonevox
{
    public class GUIAppearenceDataTypeAttribute : Attribute
    {
        public Type Type;

        public GUIAppearenceDataTypeAttribute(Type Type)
        {
            this.Type = Type;
        }

        public override string ToString()
        {
            return Type.ToString();
        }
    }
}
