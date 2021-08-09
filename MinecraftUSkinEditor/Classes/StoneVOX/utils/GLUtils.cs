using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;

namespace stonevox
{
    public class GLUtils
    {
        public static int CreateIndexBuffer(int size)
        {
            ushort[] data = new ushort[size * 6];

            for (ushort i = 0; i < size; i++)
            {
                data[i * 6] = (ushort)(i * 4);
                data[i * 6 + 1] = (ushort)(i * 4 + 1);
                data[i * 6 + 2] = (ushort)(i * 4 + 2);
                data[i * 6 + 3] = (ushort)(i * 4);
                data[i * 6 + 4] = (ushort)(i * 4 + 2);
                data[i * 6 + 5] = (ushort)(i * 4 + 3);
            }

            int id = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, id);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(sizeof(ushort) * data.Length), data, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            return id;
        }


        public static void CreateVertexArraysQBF(int size, out int vertexArrayID, out int vertexBufferID)
        {
            Shader s = ShaderUtil.GetShader("qb");

            vertexBufferID = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferID);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(size), IntPtr.Zero, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            vertexArrayID = GL.GenVertexArray();
            GL.BindVertexArray(vertexArrayID);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferID);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(s.getartributelocation("position"), 3, VertexAttribPointerType.Float, false, sizeof(float) * 4, 0);

            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(s.getartributelocation("color"), 1, VertexAttribPointerType.Float, false, sizeof(float) * 4, sizeof(float) * 3);

            //GL.EnableVertexAttribArray(2);
            //GL.VertexAttribPointer(s.getartributelocation("light"), 1, VertexAttribPointerType.Float, false, sizeof(float) * 7, sizeof(float) * 6);

            //GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
            GL.BindVertexArray(0);
        }

        static Dictionary<string, Texture2D> loadedTextures = new Dictionary<string, Texture2D>();

        public static Texture2D LoadImage(string path)
        {
            Texture2D _out = null;

            if (loadedTextures.TryGetValue(path, out _out))
            {
                return _out;
            }

            try
            {
                using (Bitmap bitmap = new Bitmap(path))
                {
                    int textureID = GL.GenTexture();

                    GL.BindTexture(TextureTarget.Texture2D, textureID);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

                    BitmapData bmp_data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp_data.Width, bmp_data.Height, 0,
                        OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);

                    bitmap.UnlockBits(bmp_data);



                    Texture2D loadedTexture = new Texture2D()
                    {
                        TextureID = textureID,
                        Width = bitmap.Width,
                        Height = bitmap.Height
                    };

                    loadedTextures.Add(path, loadedTexture);

                    return loadedTexture;
                }
            }
            catch (Exception ex)
            {
                Debug.Print(ex.ToString());
                return new Texture2D();
            }
        }
    }

    // this is dumb, but it's whatever i don't need something amazing or clean
    public class Texture2D
    {
        public int TextureID;
        public int Width;
        public int Height;

        public Texture2D()
        {
            TextureID = -1;
        }

        public Color4 ReadPixel(int x, int y)
        {
            float[] data = new float[Width * Height];
            GL.BindTexture(TextureTarget.Texture2D, TextureID);
            GL.GetTexImage(TextureTarget.Texture2D, 0, OpenTK.Graphics.OpenGL4.PixelFormat.Rgba, PixelType.Float, data);

            int index = (y * Width) * 4 + x *4;
            GL.BindTexture(TextureTarget.Texture2D, 0);
            return new Color4(data[index], data[index + 1], data[index + 2], data[index + 3]);
        }
    }
}
