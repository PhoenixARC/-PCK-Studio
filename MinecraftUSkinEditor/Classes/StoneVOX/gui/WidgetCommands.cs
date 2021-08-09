using System.Collections.Generic;

namespace stonevox
{
    public static class WidgetCommands
    {
        public static Dictionary<string, WidgetEventHandler> handlers = new Dictionary<string, WidgetEventHandler>();

        static WidgetCommands()
        {
        }
    }
}
