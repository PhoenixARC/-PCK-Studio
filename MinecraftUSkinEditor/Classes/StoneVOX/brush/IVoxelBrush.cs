using OpenTK;
using OpenTK.Input;
using System.Collections.Generic;

namespace stonevox
{
    public interface IVoxelBrush
    {
        VoxelBrushType BrushType { get; }
        bool Active { get; set; }
        string CursorPath { get; }
        MouseCursor Cursor { get; set; }

        void Enable();
        void Disable();

        bool OnRaycastHitchanged(Input input, QbMatrix matrix, RaycastHit hit, ref Colort color, MouseButtonEventArgs e);

        void EnumerateVolume(VoxelVolume volume, VoxelVolume currentVolume, QbMatrix matrix, ref Colort color, Dictionary<double, VoxelUndoData> modifiedVoxels);
        void CleanLastVolume(VoxelVolume volume, VoxelVolume currentVolume, QbMatrix matrix, Dictionary<double, VoxelUndoData> modifiedVoxels);

        void AddVolume(VoxelVolume volume, QbMatrix matrix, ref Colort color, Dictionary<double, VoxelUndoData> modifiedVoxels);
        void RemoveVolume(VoxelVolume volume, QbMatrix matrix, Dictionary<double, VoxelUndoData> modifiedVoxels);
    }
}