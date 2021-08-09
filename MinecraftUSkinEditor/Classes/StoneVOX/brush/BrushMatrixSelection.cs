using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;
using System;
using System.Collections.Generic;

namespace stonevox
{
    public class BrushMatrixSelection : IVoxelBrush
    {
        public bool Active { get; set; }

        public VoxelBrushType BrushType
        {
            get
            {
                return VoxelBrushType.MatrixSelect;
            }
        }

        public MouseCursor Cursor
        {
            get; set;
        }

        public string CursorPath
        {
            get
            {
               return "./data/images/target_cursor.png";
            }
        }

        private QbMatrix lastmatrix;

        public BrushMatrixSelection()
        {
            Singleton<Input>.INSTANCE.AddHandler(new InputHandler()
            {
                Keydownhandler = (e) =>
                {
                    if (Singleton<GUI>.INSTANCE.OverWidget) return;
                    if (!Active && e.Key == Key.Space)
                    {
                        Singleton<BrushManager>.INSTANCE.SetCurrentBrush(BrushType);
                    }
                },
                Keyuphandler = (e) =>
                {
                    if (Singleton<GUI>.INSTANCE.OverWidget) return;

                    if (Active && (e.Key == Key.Space || e.Key == Key.Space))
                    {
                        var clientbrush = Singleton<BrushManager>.INSTANCE;
                        clientbrush.SetCurrentBrush(clientbrush.previousBrush.BrushType);
                        if (lastmatrix != null)
                        {
                            Singleton<QbManager>.INSTANCE.ActiveMatrix = lastmatrix;
                            Singleton<Camera>.INSTANCE.TransitionToMatrix();
                            lastmatrix.highlight = Color4.White;
                            lastmatrix = null;
                        }
                    }
                }
            });
        }


        public bool OnRaycastHitchanged(Input input, QbMatrix matrix, RaycastHit hit, ref Colort color, MouseButtonEventArgs e)
        {
            if (matrix == null)
            {
                if (lastmatrix != null)
                {
                    lastmatrix.highlight = Color4.White;
                    lastmatrix = null;
                }
                return true;
            }

            if(matrix != lastmatrix)
            {
                if (lastmatrix != null)
                    lastmatrix.highlight = Color4.White;
                matrix.highlight = new Colort(1.5f, 1.5f, 1.5f);

                lastmatrix = matrix;
            }
            return true;
        }

        public void Disable()
        {
            Active = false;
            Singleton<Raycaster>.INSTANCE.Mode = RaycastMode.ActiveMatrix;
            Singleton<Selection>.INSTANCE.Visible = true;
        }

        public void Enable()
        {
            Active = true;
            Singleton<Raycaster>.INSTANCE.Mode = RaycastMode.MatrixSelection;
            Singleton<Selection>.INSTANCE.Visible = false;
        }

        public void AddVolume(VoxelVolume volume, QbMatrix matrix, ref Colort color, Dictionary<double, VoxelUndoData> modifiedVoxels)
        {
            throw new NotImplementedException();
        }

        public void CleanLastVolume(VoxelVolume volume, VoxelVolume currentVolume, QbMatrix matrix, Dictionary<double, VoxelUndoData> modifiedVoxels)
        {
            throw new NotImplementedException();
        }


        public void EnumerateVolume(VoxelVolume volume, VoxelVolume currentVolume, QbMatrix matrix, ref Colort color, Dictionary<double, VoxelUndoData> modifiedVoxels)
        {
            throw new NotImplementedException();
        }

        public void RemoveVolume(VoxelVolume volume, QbMatrix matrix, Dictionary<double, VoxelUndoData> modifiedVoxels)
        {
            throw new NotImplementedException();
        }
    }
}
