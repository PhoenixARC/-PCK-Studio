using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;

namespace stonevox
{
    public class VoxelSideBuffer : IDisposable
    {
        public const int DEFAULT_MATRIX_SIZE = 499;

        private int vertexarrayID;
        private int vertexbufferID;

        private List<int> bufferholes;
        private int bufferposition = 1;

        public VoxelSideBuffer()
        {
            bufferholes = new List<int>();
        }

        public bool GetNextBufferIndex(out int index)
        {
            if (bufferholes.Count > 0)
            {
                int toreturn = bufferholes[bufferholes.Count - 1];
                bufferholes.RemoveAt(bufferholes.Count - 1);
                index = toreturn;
                return true;
            }
            if (bufferposition + 1 < DEFAULT_MATRIX_SIZE)
            {
                int t = bufferposition;
                bufferposition++;
                index = t;
                return true;
            }
            index = -1;
            return false;
        }

        public void GenerateVertexArrray_Buffer()
        {
            GLUtils.CreateVertexArraysQBF(sizeof(float) * 16 * DEFAULT_MATRIX_SIZE, out vertexarrayID, out vertexbufferID);
        }

        public void RemoveBuffer(ref float[] buffer, ref int voxelbufferindex)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexbufferID);
            GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)(sizeof(float) * 16 * voxelbufferindex), (IntPtr)(sizeof(float) * 16), buffer);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            int reff = voxelbufferindex;
            this.bufferholes.Add(reff);
            voxelbufferindex = -1;
        }

        public void FillBuffer(ref float[] buffer, int bufferindex, int x, int y, int z, int color)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexbufferID);
            GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)(sizeof(float) * 16 * bufferindex), (IntPtr)(sizeof(float) * 16), buffer);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void Render()
        {
            GL.BindVertexArray(vertexarrayID);
            GL.DrawArrays(PrimitiveType.Quads, 0, bufferposition * 4);
            GL.BindVertexArray(0);
        }

        public void Dispose()
        {
            GL.DeleteBuffer(vertexbufferID);
            GL.DeleteVertexArray(vertexarrayID);
        }
    }
}
