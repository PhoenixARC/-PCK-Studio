using System;

namespace stonevox
{
    public class GUIWidgetDataType : Attribute
    {
        public Type Type;

        public GUIWidgetDataType(Type Type)
        {
            this.Type = Type;
        }

        public override string ToString()
        {
            return Type.ToString();
        }
    }
}
