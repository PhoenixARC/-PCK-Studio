using System;

namespace stonevox
{
    public class GUIWidgetNameAttribute : Attribute
    {
        private string DisplayeName;
        public string DisplayName {  get { return DisplayeName; } }

        public GUIWidgetNameAttribute(string DisplayName)
        {
            this.DisplayeName = DisplayName;
        }
    }
}
