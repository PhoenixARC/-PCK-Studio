using OpenTK;
using System;

namespace stonevox
{
    public class QbMatrixSide : IDisposable
    {
        public QbMatrix owner;
        private float[] buffer;
        private float cubesize = .5f;
        private float lightscale = 1;
        public Vector3 normal;

        private Side side;
        public VoxelBuffer vBuffer;

        public QbMatrixSide(Side side)
        {
            this.side = side;
            vBuffer = new VoxelBuffer();
            buffer = new float[16];
            switch (side)
            {
                case Side.Left:
                    lightscale = .9f;
                    normal = new Vector3(1, 0, 0);
                    break;
                case Side.Right:
                    lightscale = .9f;
                    normal = new Vector3(-1, 0, 0);
                    break;
                case Side.Top:
                    lightscale = 1f;
                    normal = new Vector3(0, 1, 0);
                    break;
                case Side.Bottom:
                    lightscale = 1f;
                    normal = new Vector3(0, -1, 0);
                    break;
                case Side.Front:
                    lightscale = .8f;
                    normal = new Vector3(0, 0, 1);
                    break;
                case Side.Back:
                    lightscale = .8f;
                    normal = new Vector3(0, 0, -1);
                    break;
            }
        }

        public void GenerateVertexBuffers()
        {
            vBuffer.GenerateNewBuffer();
        }

        public int getnextbufferindex()
        {
            return vBuffer.GetNextBufferIndex();
        }

        public void removebuffer(ref int voxelbufferindex)
        {
            if (voxelbufferindex == -1) return;

            Array.Clear(buffer, 0, buffer.Length);
            vBuffer.RemoveBuffer(ref buffer, ref voxelbufferindex);
        }

        public void fillbuffer(int bufferindex, int x, int y, int z, int color)
        {
            switch (side)
            {
                case Side.Front:

                    buffer[0] = -cubesize + x;
                    buffer[1] = -cubesize + y;
                    buffer[2] = cubesize + z;

                    buffer[4] = cubesize + x;
                    buffer[5] = -cubesize + y;
                    buffer[6] = cubesize + z;

                    buffer[8] = cubesize + x;
                    buffer[9] = cubesize + y;
                    buffer[10] = cubesize + z;

                    buffer[12] = -cubesize + x;
                    buffer[13] = cubesize + y;
                    buffer[14] = cubesize + z;

                    break;
                case Side.Back:

                    buffer[0] = -cubesize + x;
                    buffer[1] = cubesize + y;
                    buffer[2] = -cubesize + z;

                    buffer[4] = cubesize + x;
                    buffer[5] = cubesize + y;
                    buffer[6] = -cubesize + z;

                    buffer[8] = cubesize + x;
                    buffer[9] = -cubesize + y;
                    buffer[10] = -cubesize + z;

                    buffer[12] = -cubesize + x;
                    buffer[13] = -cubesize + y;
                    buffer[14] = -cubesize + z;

                    break;
                case Side.Top:

                    buffer[0] = -cubesize + x;
                    buffer[1] = cubesize + y;
                    buffer[2] = cubesize + z;

                    buffer[4] = cubesize + x;
                    buffer[5] = cubesize + y;
                    buffer[6] = cubesize + z;

                    buffer[8] = cubesize + x;
                    buffer[9] = cubesize + y;
                    buffer[10] = -cubesize + z;

                    buffer[12] = -cubesize + x;
                    buffer[13] = cubesize + y;
                    buffer[14] = -cubesize + z;

                    break;
                case Side.Bottom:

                    buffer[0] = -cubesize + x;
                    buffer[1] = -cubesize + y;
                    buffer[2] = -cubesize + z;



                    buffer[4] = cubesize + x;
                    buffer[5] = -cubesize + y;
                    buffer[6] = -cubesize + z;

                    buffer[8] = cubesize + x;
                    buffer[9] = -cubesize + y;
                    buffer[10] = cubesize + z;



                    buffer[12] = -cubesize + x;
                    buffer[13] = -cubesize + y;
                    buffer[14] = cubesize + z;

                    break;
                case Side.Right:

                    buffer[0] = -cubesize + x;
                    buffer[1] = -cubesize + y;
                    buffer[2] = -cubesize + z;

                    buffer[4] = -cubesize + x;
                    buffer[5] = -cubesize + y;
                    buffer[6] = cubesize + z;

                    buffer[8] = -cubesize + x;
                    buffer[9] = cubesize + y;
                    buffer[10] = cubesize + z;

                    buffer[12] = -cubesize + x;
                    buffer[13] = cubesize + y;
                    buffer[14] = -cubesize + z;

                    break;
                case Side.Left:

                    buffer[0] = cubesize + x;
                    buffer[1] = cubesize + y;
                    buffer[2] = -cubesize + z;

                    buffer[4] = cubesize + x;
                    buffer[5] = cubesize + y;
                    buffer[6] = cubesize + z;

                    buffer[8] = cubesize + x;
                    buffer[9] = -cubesize + y;
                    buffer[10] = cubesize + z;

                    buffer[12] = cubesize + x;
                    buffer[13] = -cubesize + y;
                    buffer[14] = -cubesize + z;

                    break;
            }

            buffer[3] = color;
            buffer[7] = color;
            buffer[11] = color;
            buffer[15] = color;

            vBuffer.FillBuffer(ref buffer, bufferindex, x, y, z, color);
        }

        public void fillbuffer(ref int voxelbufferindex, int x, int y, int z, int color)
        {
            if (voxelbufferindex > -1) return;

            int bufferindex = getnextbufferindex();
            voxelbufferindex = bufferindex;

            fillbuffer(voxelbufferindex, x, y, z, color);

            #region // mapbuffer
            //IntPtr pointer = GL.MapBuffer(BufferTarget.ArrayBuffer, BufferAccess.WriteOnly);

            //if ((int)pointer > (int)IntPtr.Zero)
            //{
            //    unsafe
            //    {
            //        fixed (float* SystemMemory = &buffer[0])
            //        {
            //            float* VideoMemory = (float*)pointer;
            //            for (int i = 0; i < buffer.Length; i++)
            //                VideoMemory[i] = buffer[i+4*28*bufferindex];
            //        }
            //    }
            //}
            //GL.UnmapBuffer(BufferTarget.ElementArrayBuffer);
            //GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            #endregion
        }

        public void updatevoxel(Voxel voxel)
        {
            int bufferindex = -1;

            switch (side)
            {
                case Side.Front:
                    bufferindex = voxel.front;
                    break;
                case Side.Back:
                    bufferindex = voxel.back;
                    break;
                case Side.Top:
                    bufferindex = voxel.top;
                    break;
                case Side.Bottom:
                    bufferindex = voxel.bottom;
                    break;
                case Side.Right:
                    bufferindex = voxel.right;
                    break;
                case Side.Left:
                    bufferindex = voxel.left;
                    break;
            }

            fillbuffer(bufferindex, voxel.x, voxel.y, voxel.z, voxel.colorindex);
        }

        public void Render(Shader shader)
        {
            shader.WriteUniform("light", lightscale);
            vBuffer.Render();
        }

        public void Render()
        {
            vBuffer.Render();
        }

        public void Dispose()
        {
            vBuffer.Dispose();
        }
    }
}