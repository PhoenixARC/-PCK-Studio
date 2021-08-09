using System.Collections.Generic;

namespace stonevox
{
    public enum Message
    {
        WidgetEnable,
        WidgetMouseEnter,
        WidgetMouseLeave,
        WidgetKeyPress,
        WidgetFocus,
        WidgetFocusLost,
        WidgetStartTranslation,
        WidgetEndTranslation,
        ColorSelectionChanged,
        ColorSelectionUpdate,
        ColorSelectionCommit,

        TextboxTextCommited,

        WindowOpened,
        WindowClosed,

        StatusStripUpdate,

        ModelImported,
        ModelRemoved,
        ModelRenamed,
        ActiveMatrixChanged,
        ActiveModelChanged,

        WidgetMouseDown = 100,
        WidgetMouseDoubleClick = 101,
        WidgetMouseUp = 102,
        WidgetMouseScroll = 103,
        WidgetMouseOver = 104,
        WidgetMouseMove = 105
    }

    // allows widgets or others to handle the broadcast and stop it from carring further
    public enum BroadcastMessageReturn
    {
        Funnel,
        Stop
    }

    public class BroadcastMessage
    {
        public Message messgae;
        public Widget widget;
        public object[] args;

        public bool HasWidget { get { return widget != null; } }

        public T Arg<T>(int index)
        {
            return (T)args[index];
        }
    }

    public delegate void BroadcastMessageHandler(Message message, Widget windget, object[] args);

    public class Broadcaster : Singleton<Broadcaster>
    {
        GUI gui;

        public List<BroadcastMessageHandler> handlers;

        public Broadcaster()
             : base()
        {
            handlers = new List<BroadcastMessageHandler>();
        }

        public void SetGUI(GUI gui)
        {
            this.gui = gui;
        }

        public void Broadcast(Message message, params object[] args)
        {
            Broadcast(message, null, args);
        }

        public void Broadcast(Message message, Widget widget, params object[] args)
        {
            if ((int)message < 100)
                gui.Dirty = true;

            BroadcastMessage m = new BroadcastMessage()
            {
                messgae = message,
                widget = widget,
                args = args
            };

            handlers.ForEach(t => t(message, widget, args));

            foreach (var guiWidget in gui.widgets)
            {
                if (m.HasWidget)
                {
                    if (guiWidget.ID != m.widget.ID)
                        guiWidget.HandleMessageRecieved(m);
                }
                else
                    guiWidget.HandleMessageRecieved(m);
            }
        }
    }
}
