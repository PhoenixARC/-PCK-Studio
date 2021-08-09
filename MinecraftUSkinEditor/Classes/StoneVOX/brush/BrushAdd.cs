using OpenTK;
using OpenTK.Input;
using System.Collections.Generic;
using System;

namespace stonevox
{
    public class BrushAdd : IVoxelBrush
    {
        ToolState state = ToolState.Start;
        VoxelLocation startPosition;
        VoxelLocation endPosition;
        VoxelVolume volume;
        VoxelVolume lastvolume;
        QbMatrix lastmatrix;
        Dictionary<double, VoxelUndoData> modifiedVoxels = new Dictionary<double, VoxelUndoData>();
        public MouseCursor Cursor { get; set; }

        public VoxelBrushType BrushType { get { return VoxelBrushType.Add; } }

        public bool Active { get; set; }

        public string CursorPath
        {
            get
            {
                return "./data/images/target_cursor.png";
            }
        }

        public BrushAdd()
        {
        }

        public bool OnRaycastHitchanged(Input input, QbMatrix matrix, RaycastHit hit, ref Colort color, MouseButtonEventArgs e)
        {
            return true;
        }

        public void EnumerateVolume(VoxelVolume volume, VoxelVolume currentVolume, QbMatrix matrix, ref Colort color, Dictionary<double, VoxelUndoData> modifiedVoxels)
        {
            for (int z = currentVolume.minz; z <= currentVolume.maxz; z++)
                for (int y = currentVolume.miny; y <= currentVolume.maxy; y++)
                    for (int x = currentVolume.minx; x <= currentVolume.maxx; x++)
                    {
                        if (!volume.ContainsPoint(x,y,z) && !modifiedVoxels.ContainsKey(matrix.GetHash(x,y,z)))
                            modifiedVoxels.Add(matrix.GetHash(x, y, z), new VoxelUndoData(matrix.Add(x, y, z, color)));
                    }
        }
        public void CleanLastVolume(VoxelVolume volume, VoxelVolume currentVolume, QbMatrix matrix, Dictionary<double, VoxelUndoData> modifiedVoxels)
        {
            for (int z = volume.minz; z <= volume.maxz; z++)
                for (int y = volume.miny; y <= volume.maxy; y++)
                    for (int x = volume.minx; x <= volume.maxx; x++)
                    {
                        if (!currentVolume.ContainsPoint(x, y, z))
                        {
                            if (modifiedVoxels[matrix.GetHash(x, y, z)].changed)
                            {
                                matrix.Remove(x, y, z, false, false);
                                modifiedVoxels.Remove(matrix.GetHash(x, y, z));
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
            VoxelUndoData _temp;
            for (int z = volume.minz; z <= volume.maxz; z++)
                for (int y = volume.miny; y <= volume.maxy; y++)
                    for (int x = volume.minx; x <= volume.maxx; x++)
                    {
                        if (modifiedVoxels.TryGetValue(matrix.GetHash(x, y, z), out _temp) && _temp.changed)
                        {
                            matrix.Remove(x, y, z, false, false);
                            modifiedVoxels.Remove(matrix.GetHash(x, y, z));
                        }
                    }
        }

        public void AddVolume(VoxelVolume volume, QbMatrix matrix, ref Colort color, Dictionary<double, VoxelUndoData> modifiedVoxels)
        {
            double hash;
            for (int z = volume.minz; z <= volume.maxz; z++)
                for (int y = volume.miny; y <= volume.maxy; y++)
                    for (int x = volume.minx; x <= volume.maxx; x++)
                    {
                        hash = matrix.GetHash(x, y, z);
                        if (!modifiedVoxels.ContainsKey(hash))
                            modifiedVoxels.Add(hash, new VoxelUndoData(matrix.Add(x, y, z, color)));
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

        public void Dispose()
        {
        }

    }
}