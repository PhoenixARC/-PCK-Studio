using System.Collections.Generic;

namespace stonevox
{
    public class GUIData
    {
        public List<WidgetData> widgets { get; set; }

        public GUIData()
        {
            widgets = new List<WidgetData>();
        }
    }
}
