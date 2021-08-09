using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;

namespace stonevox
{
    public class WidgetData
    {
        public bool Enable { get; set; }

        public string Name { get; set; }
        public int ID { get; set; }
        public int ParentID { get; set; }

        public Vector2Data Location { get; set; }
        public Vector2Data Size { get; set; }

        [Editor(typeof(CollectionEditorHook), typeof(UITypeEditor))]
        public List<AppearenceData> appearenceData { get; set; }
        [Editor(typeof(CollectionEditorHook), typeof(UITypeEditor))]
        public List<WidgetTranslation> translations { get; set; }

        public WidgetData()
        {
            Name = "";
            ParentID = -1;
            appearenceData = new List<AppearenceData>();
            translations = new List<WidgetTranslation>();
            Location = new Vector2Data();
            Size = new Vector2Data();
        }
    }
}
