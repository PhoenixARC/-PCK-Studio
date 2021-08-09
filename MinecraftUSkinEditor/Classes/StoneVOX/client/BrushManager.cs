using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace stonevox
{
    public class BrushManager : Singleton<BrushManager>
    {
        public Colort brushColor = new Colort(.3f, .2f, .8f);
        public IVoxelBrush currentBrush;
        public IVoxelBrush previousBrush;

        public Dictionary<VoxelBrushType, IVoxelBrush> brushes;

        private GLWindow window;

        public BrushManager(GLWindow window, Input input)
            : base()
        {
            this.window = window;

            brushes = new Dictionary<VoxelBrushType, IVoxelBrush>();
            brushes.Add(VoxelBrushType.Add, new BrushAdd());
            brushes.Add(VoxelBrushType.Remove, new BrushRemove());
            brushes.Add(VoxelBrushType.Recolor, new BrushRecolor());
            brushes.Add(VoxelBrushType.MatrixSelect, new BrushMatrixSelection());
            brushes.Add(VoxelBrushType.ColorSelect, new BrushColorSelection());
            brushes.Add(VoxelBrushType.Select, new BrushVoxelSelection());

            input.AddHandler(new InputHandler()
            {
                Keydownhandler = (e) =>
                {
                    if (e.Key == Key.Tab)
                    {
                        NextBrush();
                    }

                    var gui = Singleton<GUI>.INSTANCE;


                    if (e.Key == Key.B)
                        SetCurrentBrush(VoxelBrushType.Add);
                    else if (e.Key == Key.R)
                        SetCurrentBrush(VoxelBrushType.Recolor);
                    else if (e.Key == Key.F)
                        SetCurrentBrush(VoxelBrushType.Remove);
                }
            });

            window.SVReizeEvent += (e, o) =>
            {
                var values = Enum.GetValues(typeof(VoxelBrushType));
                var enumer = values.GetEnumerator();
                while (enumer.MoveNext())
                {
                    string path = brushes[(VoxelBrushType)enumer.Current].CursorPath;

                    if (string.IsNullOrEmpty(path))
                    {
                        brushes[(VoxelBrushType)enumer.Current].Cursor = OpenTK.MouseCursor.Default;
                        continue;
                    }

                    Bitmap bitmap = new Bitmap(path);

                    if (window.Width <= 1280)
                        bitmap = bitmap.ResizeImage(new Size((int)(bitmap.Width * .75f), (int)(bitmap.Height * .75f)));
                    else if (window.Width <= 1400)
                        bitmap = bitmap.ResizeImage(new Size((int)(bitmap.Width * .8f), (int)(bitmap.Height * .8f)));

                    bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
                    var data = bitmap.LockBits(
                        new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                        System.Drawing.Imaging.ImageLockMode.ReadOnly,
                        System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

                    // super hacks
                    if ((VoxelBrushType)enumer.Current == VoxelBrushType.MatrixSelect)
                        brushes[(VoxelBrushType)enumer.Current].Cursor = new OpenTK.MouseCursor(
                            data.Width / 2, data.Height / 2, data.Width, data.Height, data.Scan0);
                    else
                        brushes[(VoxelBrushType)enumer.Current].Cursor = new OpenTK.MouseCursor(
                            0, 0, data.Width, data.Height, data.Scan0);

                    bitmap.Dispose();
                }
            };

            NextBrush();
        }

        public void SetCurrentBrush(VoxelBrushType type)
        {
            if (previousBrush != null)
            {
                currentBrush.Disable();

                //super super hacks
                if (currentBrush.BrushType != VoxelBrushType.Select || currentBrush.BrushType != VoxelBrushType.MatrixSelect || currentBrush.BrushType != VoxelBrushType.ColorSelect)
                    previousBrush = currentBrush;
            }
            else
            {
                if (currentBrush?.BrushType != VoxelBrushType.Select || currentBrush?.BrushType != VoxelBrushType.MatrixSelect || currentBrush?.BrushType != VoxelBrushType.MatrixSelect)
                    previousBrush = currentBrush;
            }
            currentBrush = brushes[type];
            currentBrush.Enable();

            // ensure onselectionchanged is called when changing brushes
            // even if the raycaster isn't over a new voxel
            if (Singleton<Raycaster>.INSTANCE != null)
                Singleton<Raycaster>.INSTANCE.lastHit = new RaycastHit() { distance = 10000 };

            if (Singleton<GUI>.INSTANCE?.OverWidget == false)
                window.Cursor = currentBrush.Cursor;
        }

        public void NextBrush()
        {
            var values = Enum.GetValues(typeof(VoxelBrushType));
            var enumer = values.GetEnumerator();
            enumer.MoveNext();
            VoxelBrushType first = (VoxelBrushType)enumer.Current;

            if (currentBrush != null)
            {
                do
                {
                    if ((VoxelBrushType)enumer.Current == currentBrush.BrushType)
                    {
                        if (enumer.MoveNext())
                        {
                            // kinda hacky but so is the matrix selection tool...
                            if ((VoxelBrushType)enumer.Current == VoxelBrushType.Select || (VoxelBrushType)enumer.Current == VoxelBrushType.MatrixSelect || currentBrush.BrushType == VoxelBrushType.MatrixSelect)
                                continue;
                            SetCurrentBrush((VoxelBrushType)enumer.Current);
                            return;
                        }
                    }
                } while (enumer.MoveNext());
            }

            SetCurrentBrush(first);
        }

        public bool onselectionchanged(Input input, QbMatrix matrix, RaycastHit hit, MouseButtonEventArgs e = null)
        {
            if (matrix != null && !matrix.Visible) return true;
            return currentBrush.OnRaycastHitchanged(input, matrix, hit, ref brushColor, e);
        }
    }
}
