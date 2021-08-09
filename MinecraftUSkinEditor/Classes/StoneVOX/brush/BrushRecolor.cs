using OpenTK;
using OpenTK.Input;
using System.Collections.Generic;

namespace stonevox
{
    public class BrushRecolor : IVoxelBrush
    {
        ToolState state = ToolState.Start;
        VoxelLocation startPosition;
        VoxelLocation endPosition;
        VoxelVolume volume;
        VoxelVolume lastvolume;
        QbMatrix lastmatrix;
        Dictionary<double, VoxelUndoData> modifiedVoxels = new Dictionary<double, VoxelUndoData>();
        public MouseCursor Cursor { get; set; }


        public VoxelBrushType BrushType { get { return VoxelBrushType.Recolor; } }

        public bool Active { get; set; }

        public string CursorPath
        {
            get
            {
                return "./data/images/cursor_paint.png";
            }
        }

        public BrushRecolor()
        {
            Singleton<Input>.INSTANCE.AddHandler(new InputHandler()
            {
                mouseuphandler = (e) =>
                {
                    if (!Active) return;

                    if (e.Button == MouseButton.Right && Singleton<Input>.INSTANCE.Keydown(Key.AltLeft) && state == ToolState.Base)
                    {
                        state = ToolState.Disabled;
                        if (Singleton<Input>.INSTANCE.mouseup(MouseButton.Left))
                        {
                            CleanForToolReset();
                            state = ToolState.Start;
                        }
                    }

                    if (state == ToolState.Disabled && e.Button == MouseButton.Left)
                    {
                        state = ToolState.Start;
                        CleanForToolReset();
                    }
                }
            });
        }

        public bool OnRaycastHitchanged(Input input, QbMatrix matrix, RaycastHit hit, ref Colort color, MouseButtonEventArgs e)
        {
            lastmatrix = matrix;
            switch (state)
            {
                case ToolState.Start:
                    if ((e != null && e.IsPressed && e.Button == MouseButton.Left) || (e == null && input.mousedown(MouseButton.Left)))
                    {
                        state = ToolState.Base;
                        Singleton<Raycaster>.INSTANCE.testdirt = true;
                        startPosition = new VoxelLocation(hit, false);
                        endPosition = new VoxelLocation(hit, false);
                        volume = new VoxelVolume(startPosition, endPosition);
                        modifiedVoxels.Clear();
                        EnumerateVolume(volume, lastvolume, matrix, ref color, modifiedVoxels);
                        lastvolume = volume;
                        return true;
                    }
                    break;
                case ToolState.Base:
                    if ((e != null && e.IsPressed && e.Button == MouseButton.Left) || (e == null && input.mousedown(MouseButton.Left)))
                    {
                        endPosition = new VoxelLocation(hit, false);
                        volume = new VoxelVolume(startPosition, endPosition);

                        EnumerateVolume(volume, lastvolume, matrix, ref color, modifiedVoxels);
                        CleanLastVolume(lastvolume, volume, matrix, modifiedVoxels);
                        lastvolume = volume;

                        return true;
                    }
                    else if ((e != null && !e.IsPressed && e.Button == MouseButton.Left) || (e == null && input.mouseup(MouseButton.Left)))
                    {
                        state = ToolState.Start;
                        lastvolume = VoxelVolume.NEGATIVE_ZERO;
                        Singleton<UndoRedo>.INSTANCE.AddUndo(BrushType, matrix, volume, color, modifiedVoxels);
                        return true;
                    }
                    break;
                case ToolState.Limit:
                    break;
            }
            return false;
        }

        public void EnumerateVolume(VoxelVolume volume, VoxelVolume currentVolume, QbMatrix matrix, ref Colort color, Dictionary<double, VoxelUndoData> modifiedVoxels)
        {
            double hash;
            Voxel voxel = null;
            for (int z = volume.minz; z <= volume.maxz; z++)
                for (int y = volume.miny; y <= volume.maxy; y++)
                    for (int x = volume.minx; x <= volume.maxx; x++)
                    {
                        hash = matrix.GetHash(x, y, z);

                        if (!modifiedVoxels.ContainsKey(hash) && matrix.voxels.TryGetValue(hash, out voxel) && voxel.alphamask > 1)
                        {
                            modifiedVoxels.Add(hash, new VoxelUndoData(matrix.voxels[hash].colorindex, 0));
                            matrix.Color(x, y, z, color);
                        }
                    }
        }

        public void CleanLastVolume(VoxelVolume volume, VoxelVolume currentVolume, QbMatrix matrix, Dictionary<double, VoxelUndoData> modifiedVoxels)
        {
            double hash;
            VoxelUndoData voxel = new VoxelUndoData();
            for (int z = volume.minz; z <= volume.maxz; z++)
                for (int y = volume.miny; y <= volume.maxy; y++)
                    for (int x = volume.minx; x <= volume.maxx; x++)
                    {
                        if (!currentVolume.ContainsPoint(x, y, z))
                        {
                            hash = matrix.GetHash(x, y, z);
                            if (modifiedVoxels.TryGetValue(hash, out voxel))
                            {
                                matrix.Color(x, y, z, voxel.colorindex, false, true);
                                modifiedVoxels.Remove(hash);
                            }
                        }
                    }
        }

        void CleanForToolReset()
        {
            RemoveVolume(volume, lastmatrix, modifiedVoxels);
            modifiedVoxels.Clear();
            lastvolume = VoxelVolume.NEGATIVE_ZERO;
        }

        public void RemoveVolume(VoxelVolume volume, QbMatrix matrix, Dictionary<double, VoxelUndoData> modifiedVoxels)
        {
            double hash;
            VoxelUndoData voxel = new VoxelUndoData();
            for (int z = volume.minz; z <= volume.maxz; z++)
                for (int y = volume.miny; y <= volume.maxy; y++)
                    for (int x = volume.minx; x <= volume.maxx; x++)
                    {
                        hash = matrix.GetHash(x, y, z);
                        if (modifiedVoxels.TryGetValue(hash, out voxel))
                        {
                            matrix.Color(x, y, z, voxel.colorindex, false, true);
                            modifiedVoxels.Remove(hash);
                        }
                    }
        }

        public void AddVolume(VoxelVolume volume, QbMatrix matrix, ref Colort color, Dictionary<double, VoxelUndoData> modifiedVoxels)
        {
            double hash;
            Voxel voxel = null;
            for (int z = volume.minz; z <= volume.maxz; z++)
                for (int y = volume.miny; y <= volume.maxy; y++)
                    for (int x = volume.minx; x <= volume.maxx; x++)
                    {
                        hash = matrix.GetHash(x, y, z);

                        if (!modifiedVoxels.ContainsKey(hash) && matrix.voxels.TryGetValue(hash, out voxel) && voxel.alphamask > 1)
                        {
                            modifiedVoxels.Add(hash, new VoxelUndoData(matrix.voxels[hash].colorindex, 0));
                            matrix.Color(x, y, z, color);
                        }
                    }
        }

        enum ToolState
        {
            Disabled,
            Start,
            Base,
            Limit
        }

        public void Enable()
        {
            Active = true;
        }

        public void Disable()
        {
            Active = false;
        }

    }
}