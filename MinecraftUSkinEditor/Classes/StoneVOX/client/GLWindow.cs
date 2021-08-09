using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using QuickFont;
using stonevox.gui.editor;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Generic;

namespace stonevox
{
    // personally i'm thinking nothing should go here to access stuff, ie backcolor, input, gui, ect
    // added singleton classes to start this change
    public class GLWindow : SyncWindow
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        public static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        [DllImport("ole32.dll")]
        public static extern int OleInitialize(IntPtr pvReserved);

        [DllImport("ole32.dll")]
        public static extern int RegisterDragDrop(IntPtr hwnd, stonevox.IDropTarget pDropTarget);

        // this goes along with the comment below
        // these are all singeltons now
        Input input;
        Camera camera;
        Selection selection;
        BrushManager brushes;
        GUI gui;
        Broadcaster broadcaster;
        UndoRedo undoredo;
        Raycaster raycaster;
        Floor floor;
        QbManager manager;
        IRenderer renderer;

        // not sure about these yet, need another place
        // ... also the whole QbManager/Importer/Exporter thing i don't like
        // thinking some sort of qb manager class should be put in the _client space
        // following Singleton<?? "qb manager">
        //public QbModel model;
        public Shader voxelShader;

        public Color4 backcolor;

        public bool isfocused = true;

        public QFont Qfont;
        public QFont Qfont_1280;
        public QFont Qfont_1400;
        public QFont Qfont_1920;

        public event EventHandler SVReizeEvent;
        private int lastWidth;

        public string CSMText = "";

        public GLWindow(int width, int height, GraphicsMode graphicsmode, string CSMTextx)
            : base(width, height, graphicsmode)
        {
            CSMText = CSMTextx;
        }



        public DragDropTarget dnd;




        VoxelLocation startPosition;
        VoxelLocation endPosition;
        VoxelVolume volume;
        VoxelVolume lastvolume;
        QbMatrix lastmatrix;
        Dictionary<double, VoxelUndoData> modifiedVoxels = new Dictionary<double, VoxelUndoData>();


        protected override void OnLoad(EventArgs e)
        {
            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Title = String.Format("StoneVox 3D - version {0}", version);

            GL.Viewport(0, 0, Width, Height);
            Qfont_1280 = new QFont("data\\fonts\\Bigfish.ttf", 11.2f, new QFontBuilderConfiguration(true, false));
            Qfont_1400 = new QFont("data\\fonts\\Bigfish.ttf", 12f, new QFontBuilderConfiguration(true, false));
            Qfont_1920 = new QFont("data\\fonts\\Bigfish.ttf", 15, new QFontBuilderConfiguration(true, false));
            if (Width <= 1280)
            {
                Qfont = Qfont_1280;
            }
            else if (Width < 1400)
            {
                Qfont = Qfont_1400;
            }
            else
            {
                Qfont = Qfont_1920;
            }

            this.Qfont.Options.Colour = Color.White;
            //this.Qfont.Options.TransformToViewport = new TransformViewport(-1,-1,2,2);

            Scale.SetHScaling(0, Width);
            Scale.SetVScaling(0, Height);

            ShaderUtil.CreateShader("quad_interpolation", "./data/shaders/QuadInterpolation.vs", "./data/shaders/QuadInterpolation.fs");

            broadcaster = new Broadcaster();
            manager = new QbManager(broadcaster);
            input = new Input(this);
            camera = new Camera(this, input, manager);
            brushes = new BrushManager(this, input);
            floor = new Floor(camera, broadcaster);
            gui = new GUI(this, manager, input);
            selection = new Selection(this,brushes, input, manager, floor, gui);
            renderer = new Wireframe(camera, selection, floor, input);
            undoredo = new UndoRedo(input);

            selection.GenerateVertexArray();

            if(!manager.HasModel)
                manager.AddEmpty();
            camera.LookAtModel(true);

            backcolor = new Color4(60, 60, 60, 0);

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);

            int ole_hresult = OleInitialize(IntPtr.Zero);
            IntPtr handle = FindWindowByCaption(IntPtr.Zero, Title);
            dnd = new DragDropTarget();
            int dnd_hresult = RegisterDragDrop(handle, dnd);

            raycaster = new Raycaster(this, camera, selection, floor, input, manager, gui);
            selection.raycaster = raycaster;

            Client.Initialized = true;
            base.OnLoad(e);

            CSM csm = new CSM(CSMText);
            foreach(Box box in csm.boxes)
            {
                CreateBox(box.ParentName, -box.x, -box.y, -box.z, box.SizeX, box.SizeY, box.SizeZ, new Colort(.3f, .5f, .8f));
                Console.WriteLine("CreateBox(\"" + box.ParentName + "\", " + -box.x + ", " + -box.y + ", " + -box.z + ", " + box.SizeX + ", " + box.SizeY + ", " + box.SizeZ + ", new Colort(.3f, .5f, .8f));");
            }



            SetForegroundWindow(WindowInfo.Handle);

        }

        public void CreateBox(string Parent, int x, int y, int z, int sizeX, int sizeY, int sizeZ, Colort col)
        {
            sizeX -= 1;
            sizeY -= 1;
            sizeZ -= 1;
            switch (Parent)
            {
                case ("HEAD"):
                    y += 16;
                    col = new Colort(.3f, .5f, .8f);
                    break;
                case ("BODY"):
                    z += 4;
                    y += 12;
                    col = new Colort(.5f, .2f, .8f);
                    break;
                case ("ARM0"):
                    z += 4;
                    y += 10;
                    x -= 3;
                    col = new Colort(.3f, .2f, .5f);
                    break;
                case ("ARM1"):
                    z += 4;
                    y += 10;
                    x += 11;
                    col = new Colort(.3f, .5f, .5f);
                    break;
                case ("LEG0"):
                    x += 2;
                    z += 4;
                    col = new Colort(.3f, .2f, .1f);
                    break;
                case ("LEG1"):
                    x += 6;
                    z += 4;
                    col = new Colort(.1f, .2f, .5f);
                    break;
            }



            RaycastHit hit = new RaycastHit();
            hit.distance = 10000;
            hit.x = x;
            hit.y = y;
            hit.z = z;

            RaycastHit hit2 = new RaycastHit();
            hit2.distance = hit.distance - (sizeX + sizeZ + sizeY);
            hit2.y = y + sizeY;
            hit2.x = x + sizeX;
            hit2.z = z + sizeZ;

            int index = 0;

            for (int i = 0; i < manager.ActiveModel.numMatrices; i++)
            {
                if (!manager.ActiveModel.matrices[i].Visible) continue;
                RaycastHit tempHit = raycaster.RaycastTest(camera.position, manager.ActiveModel.matrices[i]);

                if (tempHit.distance != 10000 && tempHit.distance < hit.distance && !tempHit.matches(hit))
                {
                    index = i;
                }
            }


            startPosition = new VoxelLocation(hit);
            endPosition = new VoxelLocation(hit2);
            volume = new VoxelVolume(startPosition, endPosition);
            modifiedVoxels.Clear();
            EnumerateVolume(lastvolume, volume, manager.ActiveModel.matrices[index], ref col, modifiedVoxels);
            
        }

        public void EnumerateVolume(VoxelVolume volume, VoxelVolume currentVolume, QbMatrix matrix, ref Colort color, Dictionary<double, VoxelUndoData> modifiedVoxels)
        {
            for (int z = currentVolume.minz; z <= currentVolume.maxz; z++)
                for (int y = currentVolume.miny; y <= currentVolume.maxy; y++)
                    for (int x = currentVolume.minx; x <= currentVolume.maxx; x++)
                    {
                        if (!volume.ContainsPoint(x, y, z) && !modifiedVoxels.ContainsKey(matrix.GetHash(x, y, z)))
                            modifiedVoxels.Add(matrix.GetHash(x, y, z), new VoxelUndoData(matrix.Add(x, y, z, color)));
                        
                    }
        }


        protected override void OnClosing(CancelEventArgs e)
        {
            Environment.Exit(0);
            base.OnClosing(e);
        }
        public override void Dispose()
        {
            Qfont_1280.Dispose();
            Qfont_1400.Dispose();
            Qfont_1920.Dispose();
            manager.Dispose();
            base.Dispose();
        }

        protected override void OnFocusedChanged(EventArgs e)
        {
            if (!Focused)
            {
                isfocused = false;
                raycaster.Enabled = false;
            }
            else
            {
                isfocused = true;
                raycaster.Enabled = true;
            }

            var handle = FindWindowByCaption(IntPtr.Zero, "StoneVox - Open File");
            if (handle != IntPtr.Zero)
                SetForegroundWindow(handle);

            handle = FindWindowByCaption(IntPtr.Zero, "StoneVox - Save File");
            if (handle != IntPtr.Zero)
                SetForegroundWindow(handle);

            base.OnFocusedChanged(e);
        }
        protected override void OnWindowStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                raycaster.Enabled = false;

            }
            else if (WindowState == WindowState.Normal || WindowState == WindowState.Maximized)
            {
                raycaster.Enabled = true;
            }
            base.OnWindowStateChanged(e);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            QFont.ForceViewportRefresh();

            Scale.SetHScaling(0, Width);
            Scale.SetVScaling(0, Height);

            if (Width <= 1280 && lastWidth != 1280)
            {
                lastWidth = 1280;
                Qfont = Qfont_1280;
                SVReizeEvent?.Invoke(this, EventArgs.Empty);
            }
            else if (Width >1280 && Width <= 1400 && lastWidth !=1400)
            {
                lastWidth = 1400;
                Qfont = Qfont_1400;
                SVReizeEvent?.Invoke(this, EventArgs.Empty);
            }
            else if (Width > 1400 && lastWidth != 1920)
            {
                lastWidth = 1920;
                Qfont = Qfont_1920;
                SVReizeEvent?.Invoke(this, EventArgs.Empty);
            }

            base.OnResize(e);
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);
            input.handleKeydown(e);

            if (e.Control && e.Key == Key.O)
            {
                var open = new OpenFileDialog();
                open.Multiselect = false;
                open.Title = "StoneVox - Open File";
                open.DefaultExt = ".qb";

                open.FileOk += (s, o) =>
                {
                    Client.OpenGLContextThread.Add(() =>
                    {
                        ImportExportUtil.Import(open.FileName);
                    });
                };

                Thread thread = new Thread(() => 
                {
                    open.ShowDialog();
                });

                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
            }
            if (e.Control && e.Key == Key.S)
            {
                var save = new SaveFileDialog();
                save.Title = "StoneVox - Save File";
                save.Filter = "StoneVox Project (.svp)|*.svp|Qubicle Binary (.qb)|*.qb|Wavefront OBJ (.obj)|*.obj|All files (*.*)|*.*";
                save.DefaultExt = ".svp";

                save.FileOk += (s, t) =>
                {
                    QbModel model = manager.ActiveModel;
                    model.name = save.FileName.Split('\\').Last();
                    if (model.name.Contains('.'))
                        model.name = model.name.Split('.').First();
                    broadcaster.Broadcast(Message.ModelRenamed, model, model.name);
                    ImportExportUtil.Export(save.FileName.Split('\\').Last().Replace(model.name, ""), model.name, Path.GetDirectoryName(save.FileName), model);
                };

                Thread thread = new Thread(() =>
                {
                    save.ShowDialog();
                });

                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
            }
            else if (e.Control && e.Key == Key.F12)
            {
                GUIEditor editor = new GUIEditor();
                editor.Show();
            }
            //else if (e.Key == Key.T)
            //{
            //    Stopwatch w = Stopwatch.StartNew();
            //    var model = Singleton<QbManager>.INSTANCE.ActiveMatrix;

            //    Colort color = new Colort(1f, 0f, 0f);

            //    var volume = new VoxelVolume()
            //    {
            //        minx = 0,
            //        maxx = 100,
            //        miny = 0,
            //        maxy = 100,
            //        minz = 0,
            //        maxz = 100
            //    };

            //    model.Add(volume, ref color);

            //    w.Stop();
            //    Console.WriteLine("add " + w.ElapsedMilliseconds.ToString());
            //}
            //else if (e.Key == Key.T)
            //{
            //    Stopwatch w = Stopwatch.StartNew();
            //    var model = Singleton<QbManager>.INSTANCE.ActiveMatrix;

            //    Colort color = new Colort(1f, 0f, 0f);
            //    for (int x = 0; x < 100; x++)
            //        for (int z = 0; z < 100; z++)
            //        for (int y = 0; y< 100; y++)
            //                model.Add(x, y, z, color);

            //    w.Stop();
            //    Console.WriteLine("add " +w.ElapsedMilliseconds.ToString());
            //}

            //else if (e.Key == Key.Y)
            //{
            //    Stopwatch w = Stopwatch.StartNew();
            //    var model = Singleton<QbManager>.INSTANCE.ActiveMatrix;

            //    Colort color = new Colort(1f, 0f, 0f);
            //    for (int x = 0; x < 100; x++)
            //        for (int z = 0; z < 100; z++)
            //            for (int y = 0; y < 100; y++)

            //                model.Remove(x, y, z);

            //    w.Stop();
            //    Console.WriteLine("earse " + w.ElapsedMilliseconds.ToString());
            //}
        }
        protected override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            input.handleKeyup(e);
            base.OnKeyUp(e);
        }
        protected override void OnKeyPress(OpenTK.KeyPressEventArgs e)
        {
            input.handleKeypress(e);
            base.OnKeyPress(e);
        }

        // mouse
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            input.handlemousedown(e);
            base.OnMouseDown(e);
        }
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            input.handlemouseup(e);
            base.OnMouseUp(e);
        }
        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            if (input != null)
            {
                input.mousex = e.X;
                input.mousey = Height - e.Y;

                input.mousedx = e.XDelta;
                input.mousedy = e.YDelta;

                // when i implement something good for following the mouse around quickly
                //Raycaster.testlocations.Push(new Vector2(e.X, Height - e.Y));

                input.handlemousemove(e);

            }

            base.OnMouseMove(e);
        }
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            input.handlemousewheel(e);
            base.OnMouseWheel(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (!isfocused)
            {
                input.update();
                base.OnUpdateFrame(e);
                return;
            }

            input.update();
            gui.Update(e);

            camera.update((float)e.Time);
            selection.update();

            base.OnUpdateFrame(e);
        }

        double ee = 0;
        int fps = 0;

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            if (!isfocused)
            {
                Client.update();
                base.OnRenderFrame(e);
                return;
            }
              //  Console.WriteLine(raycaster.Enabled.ToString());
            GL.ClearColor(backcolor.R, backcolor.G, backcolor.B, backcolor.A);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            ee += e.Time;
            fps++;

            if (ee > 1)
            {
                ee = 0;
                Title = "StoneVox fps : " + fps.ToString();
                fps = 0;
            }

            renderer.Render(manager.ActiveModel);

            ShaderUtil.ResetShader();

            floor.Render();

            gui.Render();

            SwapBuffers();

            Client.update();

            base.OnRenderFrame(e);
        }
    }
}
