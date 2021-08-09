using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;
using System;
using System.Collections.Generic;

namespace stonevox
{
    public class BrushColorSelection : IVoxelBrush
    {
        public bool Active { get; set; }

        public VoxelBrushType BrushType
        {
            get
            {
                return VoxelBrushType.ColorSelect;
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
                return "./data/images/cursor_eyedrop.png";
            }
        }

        public BrushColorSelection()
        {
            Singleton<Input>.INSTANCE.AddHandler(new InputHandler()
            {
                Keydownhandler = (e) =>
                {
                    if (Singleton<GUI>.INSTANCE.OverWidget) return;
                    if (!Active && e.Shift)
                    {
                        Singleton<BrushManager>.INSTANCE.SetCurrentBrush(BrushType);
                    }
                },
                Keyuphandler = (e) =>
                {
                    if (Singleton<GUI>.INSTANCE.OverWidget) return;

                    if (Active && (e.Key == Key.ShiftLeft || e.Key == Key.ShiftRight))
                    {
                        var clientbrush = Singleton<BrushManager>.INSTANCE;
                        clientbrush.SetCurrentBrush(clientbrush.previousBrush.BrushType);
                    }
                }
            });
        }

        public bool OnRaycastHitchanged(Input input, QbMatrix matrix, RaycastHit hit, ref Colort color, MouseButtonEventArgs e)
        {
            if ((e != null && e.IsPressed && e.Button == MouseButton.Left) || (e == null && input.mousedown(MouseButton.Left)))
            {
                QbMatrix mat = Singleton<QbManager>.INSTANCE.ActiveModel.matrices[hit.matrixIndex];
                if (mat != null)
                {
                    Voxel voxel;
                    if (mat.voxels.TryGetValue(mat.GetHash(hit.x, hit.y, hit.z), out voxel))
                    {
                    }
                }
                return true;
            }
            return false;
        }

        public void Disable()
        {
            Active = false;
            Singleton<Raycaster>.INSTANCE.Mode = RaycastMode.ActiveMatrix;
        }

        public void Enable()
        {
            Active = true;
            Singleton<Raycaster>.INSTANCE.Mode = RaycastMode.MatrixColorSelection;
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
