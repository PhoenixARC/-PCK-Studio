using OpenTK;
using OpenTK.Input;
using System;
using System.Collections.Generic;

namespace stonevox
{
    public delegate void WidgetKeyHandler(Widget widget, KeyboardKeyEventArgs e);
    public delegate void WidgetKeyPressHandler(Widget widget, KeyPressEventArgs e);
    public delegate void WidgetMouseHandler(Widget widget, MouseButtonEventArgs e);
    public delegate void WidgetMouseMoveHandler(Widget widget, MouseMoveEventArgs e);
    public delegate void WidgetMouseWheelHandler(Widget widget, MouseWheelEventArgs e);

    public delegate void WidgetBroadcastMessageHandler(Widget widget, Message m, Widget w, object[] args);


    public class WidgetEventHandler
    {
        public WidgetKeyHandler Keydownhandler;
        public WidgetKeyHandler Keyuphandler;
        public WidgetKeyPressHandler Keypresshandler;
        public WidgetMouseHandler mousedownhandler;
        public WidgetMouseHandler mousedoubleclick;
        public WidgetMouseHandler mouseuphandler;
        public WidgetMouseMoveHandler mousemovehandler;
        public WidgetMouseWheelHandler mousewheelhandler;
    }

    public class WidgetTranslation
    {
        public string name { get; set; }

        public Vector2 destination;

        // in seconds
        public float translationTime { get; set; }
        public Vector2 Destination { get { return destination; } set { destination = value; } }

        public float time;

        public WidgetTranslation()
        {
            name = "";
        }
    }

    public class Widget
    {
        private bool enabled;
        private Widget parent;

        public int ID;
        public string Name;
        public WidgetEventHandler handler;

        public Vector2 location;
        public Vector2 size;
        public WidgetAppearence appearence;

        public Widget Parent { get { return parent; } set { parent = value;  parent.children.Add(this); Enable = parent.Enable; } }

        public float Absolute_X { get { return DetermineLocationX(); } }
        public float Absolute_Y { get { return DetermineLocationY(); } }
        public float Width { get { return size.X; } set { size.X = value; } }
        public float Height { get { return size.Y; } set { size.Y = value; } }

        public bool Drag;
        public virtual bool Enable {  get { return enabled; } set { enabled = value; UpdateEnable(value); } }

        public WidgetTranslation translation;

        public Dictionary<string, WidgetTranslation> translations;

        public WidgetData data;

        public Dictionary<string, object> customData = new Dictionary<string, object>();

        public List<Widget> children = new List<Widget>();

        public MouseCursor cursor;

        public string StatusText;

        private DateTime lastClickTime;

        public bool focused;

        void UpdateEnable(bool value)
        {
            children.ForEach(t => t.Enable = value);
            Singleton<GUI>.INSTANCE.Dirty = true;
        }

        float DetermineLocationX()
        {
            if (Parent == null)
                return location.X;
            else
            {
                float parentX = 0;
                return RecursiveParentLocationX(Parent, ref parentX) + location.X;
            }
        }

        float RecursiveParentLocationX(Widget widget, ref float value)
        {
            if (widget.Parent == null)
                return value + widget.location.X;
            else
            {
                value += widget.location.X;
                return (RecursiveParentLocationX(widget.Parent, ref value));
            }
        }

        float DetermineLocationY()
        {
            if (Parent == null)
                return location.Y;
            else
            {
                float parentz = 0;
                return RecursiveParentLocationz(Parent, ref parentz) + location.Y;
            }
        }

        float RecursiveParentLocationz(Widget widget, ref float value)
        {
            if (widget.Parent == null)
                return value + widget.location.Y;
            else
            {
                value += widget.location.Y;
                return (RecursiveParentLocationz(widget.Parent, ref value));
            }
        }

        public Widget()
        {
            Enable = true;
            ID = -1;
            handler = new WidgetEventHandler();
            appearence = new WidgetAppearence(this);
            translations = new Dictionary<string, WidgetTranslation>();
        }

        public Widget(int id) : this()
        {
            ID = id;
        }

        public virtual void HandleKeyDown(KeyboardKeyEventArgs e)
        {
            handler.Keydownhandler?.Invoke(this, e);

        }

        public virtual void HandleKeyUp(KeyboardKeyEventArgs e)
        {
            handler.Keyuphandler?.Invoke(this, e);
        }

        public virtual void HandleKeyPress(KeyPressEventArgs e)
        {
            handler.Keypresshandler?.Invoke(this, e);

            Singleton<Broadcaster>.INSTANCE.Broadcast(Message.WidgetKeyPress, this, e);
        }

        public virtual void HandleMouseMove(MouseMoveEventArgs e)
        {
            handler.mousemovehandler?.Invoke(this, e);

            Singleton<Broadcaster>.INSTANCE.Broadcast(Message.WidgetMouseMove, this, e);
        }

        public virtual void HandleMouseDown(MouseButtonEventArgs e)
        {
            if (lastClickTime != new DateTime() && DateTime.Now - lastClickTime <= TimeSpan.FromSeconds(.5f))
            {
                HandleMouseDoubleClick(e);
            }

            Drag = true;

            handler.mousedownhandler?.Invoke(this, e);

            Singleton<Broadcaster>.INSTANCE.Broadcast(Message.WidgetMouseDown, this, e);

            lastClickTime = DateTime.Now;
        }


        public virtual void HandleMouseDoubleClick(MouseButtonEventArgs e)
        {
            handler.mousedoubleclick?.Invoke(this, e);
            Singleton<Broadcaster>.INSTANCE.Broadcast(Message.WidgetMouseDoubleClick, this, e);
        }

        public virtual void HandleMouseUp(MouseButtonEventArgs e)
        {
            handler.mouseuphandler?.Invoke(this, e);

            Singleton<Broadcaster>.INSTANCE.Broadcast(Message.WidgetMouseUp, this, e);
        }

        public virtual void HandleMouseWheel(MouseWheelEventArgs e)
        {
            handler.mousewheelhandler?.Invoke(this, e);

            Singleton<Broadcaster>.INSTANCE.Broadcast(Message.WidgetMouseScroll, this, e);
        }

        public virtual void HandleMouseEnter()
        {
        }

        public virtual void HandleMouseLeave()
        {
        }

        public virtual void HandleMouseOver()
        {
        }

        public virtual void HandleFocusedGained()
        {
        }

        public virtual void HandleFocusedLost()
        {
        }

        public virtual void HandleMessageRecieved(BroadcastMessage message)
        {
        }

        public virtual void SetBounds(float? x, float? z, float? width = null, float? height = null)
        {
            float parentX = Parent == null ? 0 : Parent.Absolute_X;
            float parentz = Parent == null ? 0 : Parent.Absolute_Y;

            if (x.HasValue)
                location.X = Scale.hPosScale(x.Value) - parentX;

            if (z.HasValue)
                location.Y = Scale.vPosScale(z.Value) - parentz;

            if (width.HasValue)
                size.X = Scale.hSizeScale(width.Value);

            if (height.HasValue)
                size.Y = Scale.vSizeScale(height.Value);

            Singleton<GUI>.INSTANCE.Dirty = true;
        }

        public virtual void SetBoundsNoScaling(float? x, float? z, float? width = null, float? height = null)
        {
            float parentX = Parent == null ? 0 : Parent.Absolute_X;
            float parentz = Parent == null ? 0 : Parent.Absolute_Y;

            if (x.HasValue)
                location.X = x.Value - parentX;

            if (z.HasValue)
                location.Y = z.Value - parentz;

            if (width.HasValue)
                size.X = width.Value;

            if (height.HasValue)
                size.Y = height.Value;

            Singleton<GUI>.INSTANCE.Dirty = true;
        }

        public virtual void Update(FrameEventArgs e)
        {
            if (translation != null)
            {
                Vector2 newLocation;
                translation.time += 1000;
                Vector2.Lerp(ref location, ref translation.destination, (float)translation.time / translation.translationTime*(float)e.Time, out newLocation);
                SetBoundsNoScaling(newLocation.X, newLocation.Y, null, null);

                if ((newLocation - translation.destination).Length <= .001f)
                {
                    translation.time = 0;
                    SetBoundsNoScaling(translation.Destination.X, translation.Destination.Y, null, null);

                    // call events

                    translation = null;
                }
            }
        }

        public virtual void Render()
        {
            if (Enable)
            {
                appearence.Render(Absolute_X, Absolute_Y, size.X, size.Y);
            }
        }

        // this should return a new instnace.. but for now
        public virtual Widget FromWidgetData(WidgetData data)
        {
            this.ID = data.ID;
            this.Name = data.Name;
            // enable

            if (data.ParentID != -1)
            {
                Parent = Singleton<GUI>.INSTANCE.widgets.Find((e) => e.ID == data.ParentID);
            }

            this.SetBounds(data.Location.X, data.Location.Y, data.Size.X, data.Size.Y);

            appearence.Clear();
            foreach (var appData in data.appearenceData)
            {
                appearence.AddAppearence(appData.Name, appData.ToAppearence());
            }

            //translations.Clear();
            //translations.AddRange(data.translations);

            WidgetCommands.handlers.TryGetValue(Name, out handler);

            return this;
        }

        public virtual WidgetData ToWidgetData()
        {
            WidgetData data = new WidgetData();
            data.Name = this.Name;
            data.ID = this.ID;
            data.Enable = this.Enable;
            data.Location = new Vector2(Scale.hUnPosScale(Absolute_X), Scale.vUnPosScale(Absolute_Y));
            data.Size = new Vector2(Scale.hUnSizeScale(size.X), Scale.vUnSizeScale(size.Y));
            appearence.Foreach((e) => { data.appearenceData.Add(e.ToData()); });
            return data;
        }

        public int GetNextAvailableID()
        {
            return Singleton<GUI>.INSTANCE.NextAvailableWidgeID;
        }

        public void DoTranslation(string name)
        {
            translations.TryGetValue(name, out translation);
            translation.time = 0;
        }
    }
}
