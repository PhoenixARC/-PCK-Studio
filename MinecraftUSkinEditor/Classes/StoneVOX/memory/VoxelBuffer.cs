using System;
using System.Collections.Generic;

namespace stonevox
{
    public class VoxelBuffer : IDisposable
    {
        public List<VoxelSideBuffer> buffers;

        public VoxelBuffer()
        {
            buffers = new List<VoxelSideBuffer>();
        }

        public VoxelSideBuffer GenerateNewBuffer()
        {
            VoxelSideBuffer _return = new VoxelSideBuffer();
            _return.GenerateVertexArrray_Buffer();
            buffers.Add(_return);
            return _return;
        }

        public int GetNextBufferIndex()
        {
            int index = -1;
            for (int i = 0; i < buffers.Count; i++)
            {
                if (buffers[i].GetNextBufferIndex(out index))
                {
                    index = index + (i * VoxelSideBuffer.DEFAULT_MATRIX_SIZE);
                    break;
                }
            }
            if (index == -1)
            {
                GenerateNewBuffer().GetNextBufferIndex(out index);
                index = index + ( (buffers.Count-1) * VoxelSideBuffer.DEFAULT_MATRIX_SIZE);
            }
            return index;
        }

        int GetBufferIndex(int voxelindex)
        {
            //int mod = voxelindex % VoxelSideBuffer.DEFAULT_MATRIX_SIZE;
            return voxelindex / VoxelSideBuffer.DEFAULT_MATRIX_SIZE;
        }

        public void FillBuffer(ref float[] buffer, int voxelbufferindex, int x, int y, int z, int color)
        {
            int bIndex = GetBufferIndex(voxelbufferindex);
            buffers[bIndex].FillBuffer(ref buffer, voxelbufferindex - (bIndex * VoxelSideBuffer.DEFAULT_MATRIX_SIZE), x, y, z, color);
        }

        public void RemoveBuffer(ref float[] buffer, ref int voxelbufferindex)
        {
            int bIndex = GetBufferIndex(voxelbufferindex);
            voxelbufferindex -= (bIndex * VoxelSideBuffer.DEFAULT_MATRIX_SIZE);
            buffers[bIndex].RemoveBuffer(ref buffer, ref voxelbufferindex);
        }

        public void Render()
        {
            foreach (var buffer in buffers)
                buffer.Render();
        }

        public void Dispose()
        {
            foreach (var buffer in buffers)
                buffer.Dispose();
        }
    }
}
