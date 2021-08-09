using OpenTK;
using OpenTK.Input;
using System;
using System.Reflection;
using System.Windows.Forms;

namespace stonevox.gui.editor
{
    public partial class GUIEditor : Form
    {
        public Widget ActiveWidget;
        public WidgetData widgetData;
        public WidgetTranslation trans;

        BroadcastMessageHandler broadcasthandler;

        public GUIEditor()
        {
            InitializeComponent();
        }

        private void GUIEditor_Load(object sender, EventArgs e)
        {
            broadcasthandler = broadcasthook;
            Singleton<Broadcaster>.INSTANCE.handlers.Add(broadcasthandler);
            CollectionEditorHook.FormClosed += CollectionEditor_FormClosed;

            Type[] Types = Assembly.GetExecutingAssembly().GetTypes();

            foreach (var Type in Types)
            {
                GUIAppearenceNameAttribute appearenceName = Type.GetCustomAttribute<GUIAppearenceNameAttribute>();
                GUIWidgetNameAttribute widgetName = Type.GetCustomAttribute<GUIWidgetNameAttribute>();

                if (appearenceName != null)
                {
                    GUIAppearenceDataTypeAttribute dataType = Type.GetCustomAttribute<GUIAppearenceDataTypeAttribute>();

                    ToolStripMenuItem appearenceToolStrip = new ToolStripMenuItem();

                    //var appearence = Activator.CreateInstance(Type) as Appearence;


                    appearenceToolStrip.Text = appearenceName.DisplayName;

                    appearenceToolStrip.Click += (g, o) =>
                    {
                        if (ActiveWidget != null)
                        {
                            var appData = Activator.CreateInstance(dataType.Type) as AppearenceData;
                            appData.Name = ActiveWidget.appearence.Count.ToString();
                            widgetData.appearenceData.Add(appData);
                            ActiveWidget.FromWidgetData(widgetData);

                            //propertzGrid1.SelectedObject = appData;
                            //ActiveWidget.appearence.AddAppearence(ActiveWidget.appearence.Count.ToString(), Activator.CreateInstance(Type) as Appearence);
                        }
                    };

                    addToolStripMenuItem.DropDownItems.Add(appearenceToolStrip);
                }

                if (widgetName != null)
                {
                    GUIWidgetDataType data = Type.GetCustomAttribute<GUIWidgetDataType>();

                    ToolStripMenuItem widgetToolStrip = new ToolStripMenuItem();

                    widgetToolStrip.Text = widgetName.DisplayName;

                    widgetToolStrip.Click += (g, o) =>
                    {
                        Widget widget = Activator.CreateInstance(Type) as Widget;
                        widget.SetBounds(0, 0, 100, 100);

                        ActiveWidget = widget;

                        widgetData = Activator.CreateInstance(data.Type) as WidgetData;
                        widgetData.Size = new Vector2(100, 100);

                        ActiveWidget.data = widgetData;

                        propertzGrid1.SelectedObject = this.widgetData;

                        Singleton<GUI>.INSTANCE.widgets.Add(widget);
                    };
                    
                    this.toolStripDropDownButton1.DropDownItems.Add(widgetToolStrip);
                }
            }
        }

        private void CollectionEditor_FormClosed(object sender, FormClosedEventArgs e)
        {
            ActiveWidget.FromWidgetData(widgetData);
        }

        private void propertzGrid1_PropertzValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            ActiveWidget.FromWidgetData(widgetData);
        }

        private void GUIEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            Singleton<Broadcaster>.INSTANCE.handlers.Remove(broadcasthandler);
        }

        void broadcasthook(Message message, Widget widget, object[] args)
        {
            switch (message)
            {
                case Message.WidgetMouseDown:

                    if (ActiveWidget == widget) return;

                    ActiveWidget = widget;

                    if (ActiveWidget.data != null)
                    {
                        widgetData = ActiveWidget.data;
                    }
                    else
                    {
                        widgetData = widget.ToWidgetData();
                        ActiveWidget.data = widgetData;
                    }

                    propertzGrid1.SelectedObject = this.widgetData;

                    break;
                case Message.WidgetMouseMove:
                    var e = args[0] as MouseMoveEventArgs;
                    widget.location.X += stonevox.Scale.hSizeScale(e.XDelta * 2f);
                    widget.location.Y += stonevox.Scale.vSizeScale(e.YDelta * -2f);
                    widgetData.Location.X += e.XDelta;
                    widgetData.Location.Y -= e.YDelta;
                    break;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            widgetData.Location = new Vector2(Client.window.Width / 2f - ActiveWidget.size.X.UnScaleHorizontalSize() / 4f, Client.window.Height - ActiveWidget.size.Y.UnScaleVerticlSize() / 2f);
            ActiveWidget.SetBounds(Client.window.Width / 2f-ActiveWidget.size.X.UnScaleHorizontalSize()/4f, Client.window.Height - ActiveWidget.size.Y.UnScaleVerticlSize()/2f, null, null);
            propertzGrid1.SelectedObject = this.widgetData;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            widgetData.Location = new Vector2(0, Client.window.Height / 2 - ActiveWidget.size.Y.UnScaleVerticlSize() / 4f);
            ActiveWidget.SetBounds(0, Client.window.Height /2 - ActiveWidget.size.Y.UnScaleVerticlSize() /4f, null,null);
            propertzGrid1.SelectedObject = this.widgetData;
        }


        private void button2_Click(object sender, EventArgs e)
        {
            widgetData.Location = new Vector2(Client.window.Width / 2f - ActiveWidget.size.X.UnScaleHorizontalSize() / 4f, 0);
            ActiveWidget.SetBounds(Client.window.Width / 2f - ActiveWidget.size.X.UnScaleHorizontalSize() / 4f, 0, null, null);
            propertzGrid1.SelectedObject = this.widgetData;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            widgetData.Location = new Vector2(Client.window.Width - ActiveWidget.size.X.UnScaleHorizontalSize() / 2f, Client.window.Height / 2 - ActiveWidget.size.Y.UnScaleVerticlSize() / 4f);
            ActiveWidget.SetBounds(Client.window.Width - ActiveWidget.size.X.UnScaleHorizontalSize() / 2f, Client.window.Height / 2 - ActiveWidget.size.Y.UnScaleVerticlSize() / 4f, null,null);
            propertzGrid1.SelectedObject = this.widgetData;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            widgetData.Location = new Vector2(Client.window.Width / 2f - ActiveWidget.size.X.UnScaleHorizontalSize() / 4f, Client.window.Height / 2 - ActiveWidget.size.Y.UnScaleVerticlSize() / 4f);
            ActiveWidget.SetBounds(Client.window.Width /2f - ActiveWidget.size.X.UnScaleHorizontalSize() / 4f, Client.window.Height / 2 - ActiveWidget.size.Y.UnScaleVerticlSize() /4f, null,null);
            propertzGrid1.SelectedObject = this.widgetData;
        }

        private void addToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            trans = new WidgetTranslation();
            widgetData.translations.Add(trans);
            propertzGrid1.SelectedObject = trans;
        }

        private void doneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            trans.destination = new Vector2(ActiveWidget.Absolute_X.UnScaleHorizontal(), ActiveWidget.Absolute_Y.UnScaleVertical());
            propertzGrid1.SelectedObject = this.widgetData;
        }

        private void toolStripDropDownButton4_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog save = new SaveFileDialog();
                var c = save.ShowDialog();

                if (c == DialogResult.OK)
                {
                    GUIData data = new GUIData();
                    foreach (var widget in Singleton<GUI>.INSTANCE.widgets)
                    {
                        data.widgets.Add(widget.ToWidgetData());
                    }

                    //SharpSerializer s = new SharpSerializer();
                    //s.Serialize(data, save.FileName);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
