using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace stonevox
{
    public class GUIID
    {
        public const int HSV_H = 500;
        public const int HSV_S = 501;
        public const int HSV_V = 502;
        public const int RGB_R = 503;
        public const int RGB_G = 504;
        public const int RGB_B = 505;

        public const int MATRIX_LISTBOX_WINDOW = 700;
        public const int MATRIX_SIZE_TEXTBOX = 701;
        public const int IO_WINDOW = 900;
        public const int COLOR_PICKER_WINDOW = 600;

        public const int COLORQUAD = 601;

        public const int START_COLOR_SELECTORS = 1000;

        public const int STATUS_TEXT = 750;

        public const int GRIDOPTIONS = 800;

        public const int ACTIVE_MATRIX_NAME = 850;

        public const int HACKYSELECTIONTOOL = 950;
    }

    public class GUI : Singleton<GUI>
    {
        public float scale = 1.0f;

        public List<Widget> widgets;
        private Input input;
        private GLWindow window;
        private QbManager manager;

        private int widgetIDs = 100000;
        public int NextAvailableWidgeID { get { widgetIDs++; return widgetIDs; } }

        private int lastWidgetOverIndex = -1;
        public Widget lastWidgetOver { get { return widgets[lastWidgetOverIndex]; } }

        private int lastWidgetFocusedID = -1;
        public Widget lastWidgetFocused { get { return widgets[lastWidgetFocusedID]; } }

        public bool OverWidget { get { return lastWidgetOverIndex != -1; } }
        public bool FocusingWidget { get { return lastWidgetFocusedID != -1; } }

        public bool Visible = true;

        int activecolorindex = 9;
        List<Color4> colorpallete = new List<Color4>();

        public bool Dirty = true;
        int framebuffer;
        int color;

        public GUI(GLWindow window, QbManager manager, Input input)
             : base()
        {
            this.window = window;
            this.manager = manager;
            Singleton<Broadcaster>.INSTANCE.SetGUI(this);

            framebuffer = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.FramebufferExt, framebuffer);

            color = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, color);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, window.Width, window.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
            GL.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0Ext, TextureTarget.Texture2D, color, 0);

            GL.BindFramebuffer(FramebufferTarget.FramebufferExt, 0);

            window.Resize += (e, o) =>
            {
                UISaveState();
                ConfigureUI(window.Width);
                UILoadState();

                GL.BindTexture(TextureTarget.Texture2D, color);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, window.Width, window.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
                GL.BindTexture(TextureTarget.Texture2D, 0);
            };

            this.input = input;
            widgets = new List<Widget>();

            input.AddHandler(new InputHandler()
            {
                Keydownhandler = (e) =>
                {
                    if (lastWidgetFocusedID != -1 && lastWidgetFocused.Enable)
                    {
                        lastWidgetFocused.HandleKeyDown(e);
                    }
                },
                Keypresshandler = (e) =>
                {
                    if (lastWidgetFocusedID != -1 && lastWidgetFocused.Enable)
                    {
                        lastWidgetFocused.HandleKeyPress(e);
                    }
                },

                mousedownhandler = (e) =>
                {
                    if (lastWidgetOverIndex != -1 && lastWidgetOver.Enable)
                    {
                        if (lastWidgetOverIndex != lastWidgetFocusedID)
                        {
                            if (lastWidgetFocusedID != -1)
                                lastWidgetFocused.HandleFocusedLost();

                            lastWidgetFocusedID = lastWidgetOverIndex;
                            lastWidgetOver?.HandleFocusedGained();
                        }

                        lastWidgetOver.HandleMouseDown(e);
                    }
                    else
                    {
                        if (lastWidgetFocusedID != -1 && lastWidgetFocused.Enable)
                        {
                            lastWidgetFocused.HandleFocusedLost();
                            lastWidgetFocusedID = -1;
                        }
                    }
                },
                mousemovehandler = (e) =>
                {
                    if (lastWidgetFocusedID != -1 && lastWidgetFocused.Enable)
                    {
                        if (lastWidgetFocused.Drag)
                        {
                            lastWidgetFocused.HandleMouseMove(e);
                        }
                    }
                },
                mouseuphandler = (e) =>
                {
                    if (lastWidgetOverIndex != -1 && lastWidgetOver.Enable)
                    {
                        lastWidgetOver.Drag = false;
                        lastWidgetOver.HandleMouseUp(e);

                        //float scaledmouseX = (float)Scale.hPosScale(input.mousex);
                        //float scaledmousez = (float)Scale.vPosScale(input.mouseY);

                        //if (!isMouseWithin(scaledmouseX, scaledmousez, lastWidgetOver))
                        //{
                        //    Console.WriteLine("mouse leasve");
                        //    lastWidgetOver.HandleMouseLeave();
                        //    lastWidgetOverIndex = -1;
                        //}
                    }
                },
                mousewheelhandler = (e) =>
                {
                    if (lastWidgetOverIndex != -1 && lastWidgetOver.Enable)
                    {
                        lastWidgetOver.HandleMouseWheel(e);
                    }
                }
            });

            //SharpSerializer s = new SharpSerializer();

            //if (File.Exists(Application.StartupPath + @"\data\gui\Standard.svui"))
            //{
            //    GUIData data = s.Deserialize(Application.StartupPath + @"\data\gui\Standard.svui") as GUIData;
            //    foreach (var widget in data.widgets)
            //    {
            //        //widgets.Add(widget.)
            //    }
            //}
        }

        public void Update(FrameEventArgs e)
        {
            for (int i = widgets.Count - 1; i > -1; i--)
            {
                if (widgets[i].Enable)
                    widgets[i].Update(e);
            }

            if (input.mousedx != 0 || input.mousedy != 0)
            {
                float scaledmouseX = (float)Scale.hPosScale(input.mousex);
                float scaledmouseY = (float)Scale.vPosScale(input.mousey);


                // we were over a control
                if (lastWidgetOverIndex != -1)
                {
                    if (!lastWidgetOver.Drag)
                    {
                        bool change = false;
                        if (!isMouseWithin(scaledmouseX, scaledmouseY, lastWidgetOver) || !lastWidgetOver.Enable)
                        {
                            change = true;
                        }

                        // are within our lastoverwidget
                        if (!change)
                        {
                            // check widgets above, if we're over then reset lastover
                            for (int i = widgets.Count - 1; i > lastWidgetOverIndex; i--)
                            {
                                if (widgets[i].Enable)
                                    if (isMouseWithin(scaledmouseX, scaledmouseY, widgets[i]))
                                    {
                                        lastWidgetOver.HandleMouseLeave();
                                        lastWidgetOverIndex = i;
                                        lastWidgetOver.HandleMouseEnter();
                                        lastWidgetOver.HandleMouseOver();
                                        change = true;
                                        break;
                                    }
                            }
                        }
                        // not within our lastoverwidget
                        else
                        {
                            change = false;
                            // check all widgets including lower ones...
                            for (int i = widgets.Count - 1; i > -1; i--)
                            {
                                if (widgets[i].Enable)

                                    if (i != lastWidgetOverIndex && isMouseWithin(scaledmouseX, scaledmouseY, widgets[i]))
                                    {
                                        lastWidgetOver.HandleMouseLeave();
                                        lastWidgetOverIndex = i;
                                        lastWidgetOver.HandleMouseEnter();
                                        lastWidgetOver.HandleMouseOver();
                                        change = true;
                                        break;
                                    }
                            }
                            // not over anything
                            if (!change)
                            {
                                lastWidgetOver.HandleMouseLeave();
                                lastWidgetOverIndex = -1;
                            }
                        }
                    }
                    else
                    {
                        // were dragging... release left mouse to let go
                        if (!input.mousedown(MouseButton.Left))
                        {
                            lastWidgetOver.HandleMouseLeave();
                            lastWidgetOverIndex = -1;

                            // if we don't test again we'd have to wait till next frame to mouse over something...
                            // i think there a potential double mouse over / mouse enter here??
                            for (int i = widgets.Count - 1; i > -1; i--)
                            {
                                if (widgets[i].Enable)

                                    if (isMouseWithin(scaledmouseX, scaledmouseY, widgets[i]))
                                    {
                                        lastWidgetOverIndex = i;
                                        lastWidgetOver.HandleMouseEnter();
                                        lastWidgetOver.HandleMouseOver();
                                        break;
                                    }
                            }
                        }
                    }
                }
                else
                {
                    for (int i = widgets.Count - 1; i > -1; i--)
                    {
                        if (widgets[i].Enable)

                            if (isMouseWithin(scaledmouseX, scaledmouseY, widgets[i]))
                            {
                                lastWidgetOverIndex = i;
                                lastWidgetOver.HandleMouseEnter();
                                lastWidgetOver.HandleMouseOver();
                                break;
                            }
                    }
                }

                if (lastWidgetOverIndex == -1)
                {
                    Singleton<Raycaster>.INSTANCE.Enabled = true;

                    if (Client.window.Cursor != Singleton<BrushManager>.INSTANCE.currentBrush.Cursor)
                        Client.window.Cursor = Singleton<BrushManager>.INSTANCE.currentBrush.Cursor;
                }
                else
                {
                    Singleton<Raycaster>.INSTANCE.Enabled = false;

                    MouseCursor cursor = lastWidgetOver.cursor != null ? lastWidgetOver.cursor : MouseCursor.Default;

                    if (Client.window.Cursor != cursor)
                        Client.window.Cursor = cursor;
                }
            }
        }

        public void Render()
        {
            if (!Visible) return;

            GL.Disable(EnableCap.DepthTest);

            Setup2D();

            if (Dirty)
            {
                Dirty = false;

                //if (lastWidgetOverIndex >-1)
                //{
                //    Label status = Get<Label>(GUIID.STATUS_TEXT);
                //    string current = status.text;
                //    string previous = lastWidgetOver.StatusText;

                //    if (!string.IsNullOrEmpty(previous) && current != previous)
                //        status.text = previous;
                //    else
                //        status.text = "";
                //    Console.WriteLine("changed text");
                //}

                GL.BindFramebuffer(FramebufferTarget.FramebufferExt, framebuffer);
                GL.DrawBuffers(1, new DrawBuffersEnum[] { DrawBuffersEnum.ColorAttachment0 });

                GL.ClearColor(0, 0, 0, 0f);
                GL.Clear(ClearBufferMask.ColorBufferBit);

                Render2D();

                GL.BindFramebuffer(FramebufferTarget.FramebufferExt, 0);
            }

            float x = -1;
            float y = -1;
            float width = 2;
            float height = 2;

            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, color);
            GL.Color4(Color4.White);
            GL.Begin(PrimitiveType.Quads);

            GL.TexCoord2(0f, 0f);
            GL.Vertex2(x, y);

            GL.TexCoord2(1f, 0f);
            GL.Vertex2(x + width, y);

            GL.TexCoord2(1f, 1f);
            GL.Vertex2(x + width, y + height);

            GL.TexCoord2(0f, 1f);
            GL.Vertex2(x, y + height);

            GL.End();
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.Disable(EnableCap.Texture2D);

            GL.Enable(EnableCap.DepthTest);
        }

        void Setup2D()
        {
            ShaderUtil.ResetShader();
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(-1f, 1f, -1f, 1f, -1, 1);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
        }

        void Render2D()
        {
            for (int i = 0; i < widgets.Count; i++)
            {
                widgets[i].Render();
            }
        }

        bool isMouseWithin(float x, float y, Widget widget)
        {
            // absolute x.z and sizes should alreadz be in scale
            float widget_x = widget.Absolute_X;
            float widget_y = widget.Absolute_Y;

            float widget_width = widget.size.X;
            float widget_height = widget.size.Y;

            return (x <= widget_x + widget_width && x >= widget_x && y >= widget_y && y <= widget_y + widget_height);
        }

        public T Get<T>(int ID) where T : Widget
        {
            var widget = widgets.Where((e) => { return e.ID == ID; });

            if (widget.Count() > 0)
                return (T)widget.First();
            else
            {
                return null;
            }
        }

        void UISaveState()
        {
        }

        void UILoadState()
        {
        }

        void ConfigureUI(int width)
        {
        }

        void BuildUI()
        {
        }

        void Build_HackySelectionTool()
        {
        }

        void Build_IOWindow()
        {
        }

        void Build_ModelTabs()
        {
        }

        void Build_BrushToolbar()
        {
        }

        void Build_ColorPicker()
        {
        }

        void Build_ColorToolbar()
        {

        }
        void Build_MatrixList()
        {
        }

        void Build_Screenshot()
        {
        }

        private static Bitmap cropImage(Bitmap img, Rectangle cropArea)
        {
            Bitmap bmpImage = new Bitmap(img);
            return bmpImage.Clone(cropArea, bmpImage.PixelFormat);
        }
    }
}
