using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Drawing;
using System.IO;

namespace stonevox
{
    public class ExporterSVP : IExporter
    {
        public string extension
        {
            get
            {
                return ".svp";
            }
        }

        const int version = 1;
        int colorpalletflag = 0;

        public void write(string path, string name, QbModel model)
        {
            Client.OpenGLContextThread.Add(() =>
            {
                int Wwidth = Client.window.Width;
                int Wheight = Client.window.Height;

                int framebuffer = GL.GenBuffer();
                GL.BindFramebuffer(FramebufferTarget.FramebufferExt, framebuffer);

                int color = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, color);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, Wwidth, Wheight, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
                GL.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0Ext, TextureTarget.Texture2D, color, 0);

                int depth = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, depth);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent24, Wwidth, Wheight, 0, PixelFormat.DepthComponent, PixelType.UnsignedByte, IntPtr.Zero);
                GL.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.DepthAttachmentExt, TextureTarget.Texture2D, depth, 0);

                GL.BindFramebuffer(FramebufferTarget.FramebufferExt, 0);

                GL.BindFramebuffer(FramebufferTarget.FramebufferExt, framebuffer);
                GL.DrawBuffers(1, new DrawBuffersEnum[] { DrawBuffersEnum.ColorAttachment0 });

                GL.ClearColor(0, 0, 0, 0);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                int minx = 10000;
                int miny = 10000;
                int minz = 10000;
                int maxx = 0;
                int maxy = 0;
                int maxz = 0;
                int sizex = 0;
                int sizey = 0;
                int sizez = 0;

                foreach (var matrix in model.matrices)
                {
                    if (!matrix.Visible) continue;
                    if (matrix.minx < minx)
                        minx = matrix.minx;
                    if (matrix.maxx > maxx)
                        maxx = matrix.maxx;

                    if (matrix.miny < miny)
                        miny = matrix.miny;
                    if (matrix.maxy > maxy)
                        maxy = matrix.maxy;

                    if (matrix.minz < minz)
                        minz = matrix.minz;
                    if (matrix.maxz > maxz)
                        maxz = matrix.maxz;
                }

                sizex = maxx - minx;
                sizey = maxy - miny;
                sizez = maxz - minz;

                float backup = 0;

                if (sizey * 1.5f > 20)
                    backup = sizey * 1.5f;
                else if (sizex * 1.5f > 20)
                    backup = sizex * 1.5f;
                else backup = 20;

                var centerpos = new Vector3((minx + ((maxx - minx) / 2)), (miny + ((maxy - miny) / 2)), (minz + ((maxz - minz) / 2)));
                var position = centerpos + new Vector3(.5f, sizey * .65f, backup);
                Vector3 direction;

                Vector3.Subtract(ref centerpos, ref position, out direction);
                direction.Normalize();

                var cameraright = Vector3.Cross(direction, VectorUtils.UP);
                var cameraup = Vector3.Cross(cameraright, direction);

                var view = Matrix4.LookAt(position, position + direction, cameraup);
                var modelviewprojection = view * Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45), (float)Wwidth / (float)Wheight, 1, 300);

                Shader voxelShader = ShaderUtil.GetShader("qb");

                voxelShader.UseShader();
                voxelShader.WriteUniform("modelview", modelviewprojection);

                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                model.RenderAll(voxelShader);

                string fullpath = Path.Combine(path, name + extension);
                using (FileStream f = new FileStream(fullpath, FileMode.OpenOrCreate))
                {
                    var bit = Screenshot.ScreenShot(ReadBufferMode.ColorAttachment0);
                    bit = cropImage(bit, new Rectangle(Wwidth /4, 0, Wwidth - ((Wwidth/4)*2), Wheight));
                    bit = bit.ResizeImage(ResizeKeepAspect(bit.Size, 400, 400));
                    byte[] buffer = new byte[0];
                    using (MemoryStream m = new MemoryStream())
                    {
                        bit.Save(m, System.Drawing.Imaging.ImageFormat.Png);
                        buffer = m.ToArray();
                    }

                    using (BinaryWriter w = new BinaryWriter(f))
                    {
                        w.Write(version);
                        w.Write((int)buffer.Length);
                        w.Write(buffer);

                        // note - just in case i allow extending the color pattet
                        // which i probably will 
                        w.Write(colorpalletflag);

                        w.Write(model.version);
                        w.Write(model.colorFormat);
                        w.Write(model.zAxisOrientation);
                        w.Write(model.compressed);
                        w.Write(model.visibilityMaskEncoded);

                        w.Write((uint)model.matrices.Count);

                        for (int i = 0; i < model.numMatrices; i++)
                        {
                            QbMatrix m = model.matrices[i];
                            if (!m.Visible) continue;

                            int startx = Math.Min(0, m.minx);
                            int starty = Math.Min(0, m.miny);
                            int startz = Math.Min(0, m.minz);

                            int width = (int)(Math.Abs(Math.Min(0, m.minx)) + m.maxx + 1);
                            int height = (int)(Math.Abs(Math.Min(0, m.miny)) + m.maxy + 1);
                            int length = (int)(Math.Abs(Math.Min(0, m.minz)) + m.maxz + 1);

                            if (width < m.size.X)
                                width = (int)m.size.X;

                            if (height < m.size.Y)
                                height = (int)m.size.Y;

                            if (length < m.size.Z)
                                length = (int)m.size.Z;

                            w.Write(m.name);
                            w.Write((uint)width);
                            w.Write((uint)height);
                            w.Write((uint)length);

                            w.Write((int)m.position.X);
                            w.Write((int)m.position.Y);
                            w.Write((int)m.position.Z);

                            if (model.compressed == 0)
                            {
                                Voxel voxel;
                                for (int z = startz; z < startz + length; z++)
                                    for (int y = starty; y < starty + height; y++)
                                        for (int x = startx; x < startx + width; x++)
                                        {
                                            int zz = model.zAxisOrientation == (int)0 ? z : (int)(length - z - 1);

                                            if (m.voxels.TryGetValue(m.GetHash(x, y, zz), out voxel))
                                            {
                                                Colort c = m.colors[voxel.colorindex];
                                                w.Write((byte)(c.R * 255));
                                                w.Write((byte)(c.G * 255));
                                                w.Write((byte)(c.B * 255));
                                                w.Write(voxel.alphamask);
                                            }
                                            else
                                            {
                                                w.Write((byte)0);
                                                w.Write((byte)0);
                                                w.Write((byte)0);
                                                w.Write((byte)0);
                                            }
                                        }
                            }
                        }
                    }

                    GL.BindFramebuffer(FramebufferTarget.FramebufferExt, 0);
                    GL.DeleteTexture(color);
                    GL.DeleteTexture(depth);
                    GL.DeleteFramebuffer(framebuffer);
                }
            });
        }

        Size ResizeKeepAspect(Size CurrentDimensions, int maxWidth, int maxHeight)
        {
            int newHeight = CurrentDimensions.Height;
            int newWidth = CurrentDimensions.Width;
            if (maxWidth > 0 && newWidth > maxWidth) //WidthResize
            {
                Decimal divider = Math.Abs((Decimal)newWidth / (Decimal)maxWidth);
                newWidth = maxWidth;
                newHeight = (int)Math.Round((Decimal)(newHeight / divider));
            }
            if (maxHeight > 0 && newHeight > maxHeight) //HeightResize
            {
                Decimal divider = Math.Abs((Decimal)newHeight / (Decimal)maxHeight);
                newHeight = maxHeight;
                newWidth = (int)Math.Round((Decimal)(newWidth / divider));
            }
            return new Size(newWidth, newHeight);
        }

        private Bitmap cropImage(Bitmap img, Rectangle cropArea)
        {
            Bitmap bmpImage = new Bitmap(img);
            return bmpImage.Clone(cropArea, bmpImage.PixelFormat);
        }
    }
}
