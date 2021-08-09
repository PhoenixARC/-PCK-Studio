using System;

namespace stonevox
{
    public class GUIAppearenceNameAttribute : Attribute
    {
        private string DisplayeName;
        public string DisplayName { get { return DisplayeName; } }

        public GUIAppearenceNameAttribute(string DisplayName)
        {
            this.DisplayeName = DisplayName;
        }
    }
}
