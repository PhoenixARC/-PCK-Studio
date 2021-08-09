using OpenTK;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Windows.Forms;

namespace stonevox
{
    public class QbMatrix : IDisposable
    {
        public bool Visible = true;

        public string name;
        public Vector3 position;
        public Vector3 centerposition;
        public Vector3 size;
        public Colort highlight;

        private Stack<int> colorindexholes;
        public Colort[] colors;
        public Colort[] matrixcolors;
        //public Colort[] wireframecolors;
        //public Colort[] outlinecolors;
        public ConcurrentDictionary<double, Voxel> voxels;
        public ConcurrentStack<VoxelModifier> modifiedvoxels;

        public int minx = 10000;
        public int miny = 10000;
        public int minz = 10000;
        public int maxx = 0;
        public int maxy = 0;
        public int maxz = 0;
        public int sizex = 0;
        public int sizey =0;       
        public int sizez =0;

        private QbMatrixSide left;
        private QbMatrixSide right;
        private QbMatrixSide top;
        private QbMatrixSide bottom;
        private QbMatrixSide front;
        private QbMatrixSide back;

        private Voxel voxel;
        private VoxelModifier modified;

        private int colorIndex = 0;

        public QbMatrix()
        {
            highlight = new Colort(1f, 1f, 1f);
            matrixcolors = new Colort[128];
            colors = matrixcolors;
            colorindexholes = new Stack<int>();
            //wireframecolors = new Colort[64];
            //outlinecolors = new Colort[64];
            voxels = new ConcurrentDictionary<double, Voxel>();
            modifiedvoxels = new ConcurrentStack<VoxelModifier>();

            left = new QbMatrixSide(Side.Left);
            right = new QbMatrixSide(Side.Right);
            top = new QbMatrixSide(Side.Top);
            bottom = new QbMatrixSide(Side.Bottom);
            front = new QbMatrixSide(Side.Front);
            back = new QbMatrixSide(Side.Back);
        }

        public void setsize(int x, int y, int z, bool updateCenterPosition = false)
        {
            size = new Vector3(x, y, z);

            if (updateCenterPosition)
            {
                minx = 0;
                miny = 0;
                minz = 0;

                maxx = x;
                maxy = y;
                maxz = z;

                sizex = x;
                sizey = y;
                sizez = z;

                centerposition = new Vector3((minx + ((maxx - minx) / 2)), (miny + ((maxy - miny) / 2)), (minz + ((maxz - minz) / 2)));
            }
        }

        public int GetColorIndex(float r, float g, float b)
        {
            Colort c;
            for (int i = 0; i < colors.Length; i++)
            {
                c = colors[i];
                if (c.R == r && c.G == g && c.B == b)
                    return (int)i;
            }

            if (colorindexholes.Count > 0)
            {
                matrixcolors[colorindexholes.Pop()] = new Colort(r, g, b);
            }
            else
                matrixcolors[colorIndex] = new Colort(r, g, b);

            // wireframes and outline....
            // right now my hsv to rbg is not working correctly and fails generating a few colors
            // later on this will be used over doing the conversion on a shader

            //Color4 color = new Color4(r, g, b, 1);

            //double hue, sat, vi;
            //ColorConversion.ColorToHSV(color.ToSystemDrawingColor(), out hue, out sat, out vi);

            //var outline = ColorConversion.ColorFromHSV(hue, (sat + .5d).Clamp(0, 1), vi );
            //outlinecolors[colorIndex] = outline.ToColor4();

            //var wireframe = ColorConversion.ColorFromHSV(hue, (sat + .1d).Clamp(0,1), (vi+.1d).Clamp(0,1));
            //wireframecolors[colorIndex] = wireframe.ToColor4();

            if (colorIndex + 1 >= colors.Length)
            {
                CleanUnsuedColorIndexs();

                if (colorindexholes.Count > 0)
                {
                    int newindex = colorindexholes.Pop();
                    matrixcolors[newindex] = new Colort(r, g, b);
                    return newindex;
                }
                else
                {
                    MessageBox.Show("StoneVox only supports 128 active colors at once. This is quite the issue, try reducing the amount of colors you are using. Since there is no room the last color will now be overwritten with this new color.", "Out of Colors");
                    matrixcolors[matrixcolors.Length - 1] = new Colort(r, g, b);
                    return matrixcolors.Length - 1;
                }
            }

            colorIndex++;

            return colorIndex - 1;
        }

        public int GetColorIndex(float r, float g, float b, uint colorFormat)
        {
            if (colorFormat == 1)
            {
                float tmp = r;
                r = b;
                b = tmp;
            }

            Colort c;
            for (int i = 0; i < colors.Length; i++)
            {
                c = colors[i];
                if (c.R == r && c.G == g && c.B == b)
                    return (int)i;
            }

            if (colorindexholes.Count > 0)
            {
                matrixcolors[colorindexholes.Pop()] = new Colort(r, g, b);
            }           
            else
                matrixcolors[colorIndex] = new Colort(r, g, b);

            // wireframes and outline....
            // right now my hsv to rbg is not working correctly and fails generating a few colors
            // later on this will be used over doing the conversion on a shader

            //Color4 color = new Color4(r, g, b, 1);

            //double hue, sat, vi;
            //ColorConversion.ColorToHSV(color.ToSystemDrawingColor(), out hue, out sat, out vi);

            //var outline = ColorConversion.ColorFromHSV(hue, (sat + .5d).Clamp(0, 1), vi );
            //outlinecolors[colorIndex] = outline.ToColor4();

            //var wireframe = ColorConversion.ColorFromHSV(hue, (sat + .1d).Clamp(0,1), (vi+.1d).Clamp(0,1));
            //wireframecolors[colorIndex] = wireframe.ToColor4();

            if (colorIndex + 1 >= colors.Length)
            {
                CleanUnsuedColorIndexs();

                if (colorindexholes.Count > 0)
                {
                    int newindex = colorindexholes.Pop();
                    matrixcolors[newindex] = new Colort(r, g, b);
                    return newindex;
                }
                else
                {
                    MessageBox.Show("StoneVox only supports 128 active colors at once. This is quite the issue, try reducing the amount of colors you are using. Since there is no room the last color will now be overwritten with this new color.", "Out of Colors");
                    matrixcolors[matrixcolors.Length - 1] = new Colort(r, g, b);
                    return matrixcolors.Length - 1;
                }
            }

            colorIndex++;

            return colorIndex-1;
        }

        public int getcolorindex(byte r, byte g, byte b)
        {
            return GetColorIndex(r / 256f, g / 256f, b / 256f);
        }

        public int getcolorindex(byte r, byte g, byte b, uint colorFormat)
        {
            return GetColorIndex(r / 256f, g / 256f, b / 256f, colorFormat);
        }

        public bool GetColorIndex_Alphamask(int x, int y, int z, out int colorindex, out byte alphamask)
        {
            if (voxels.TryGetValue(GetHash(x, y, z), out voxel))
            {
                colorindex = voxel.colorindex;
                alphamask = voxel.alphamask;
                return true;
            }
            colorindex = -1;
            alphamask = 0;
            return false;
        }

        public void GenerateVertexBuffers()
        {
            left.GenerateVertexBuffers();
            right.GenerateVertexBuffers();
            front.GenerateVertexBuffers();
            back.GenerateVertexBuffers();
            top.GenerateVertexBuffers();
            bottom.GenerateVertexBuffers();
        }

        public void FillVertexBuffers()
        {
            foreach (var c in voxels.Values)
            {
                if (c.x < minx)
                    minx = c.x;
                if (c.x > maxx)
                    maxx = c.x;

                if (c.y < miny)
                    miny = c.y;
                if (c.y > maxy)
                    maxy = c.y;

                if (c.z < minz)
                    minz = c.z;
                if (c.z > maxz)
                    maxz = c.z;

                if (c.alphamask > 1)
                {
                    //front
                    if ((c.alphamask & 32) == 32)
                    {
                        front.fillbuffer(ref c.front, c.x, c.y, c.z, c.colorindex);
                    }

                    //back
                    if ((c.alphamask & 64) == 64)
                    {
                        back.fillbuffer(ref c.back, c.x, c.y, c.z, c.colorindex);
                    }

                    //top
                    if ((c.alphamask & 8) == 8)
                    {
                        top.fillbuffer(ref c.top, c.x, c.y, c.z, c.colorindex);
                    }

                    //bottom
                    if ((c.alphamask & 16) == 16)
                    {
                        bottom.fillbuffer(ref c.bottom, c.x, c.y, c.z, c.colorindex);
                    }

                    //left
                    if ((c.alphamask & 2) == 2)
                    {
                        left.fillbuffer(ref c.left, c.x, c.y, c.z, c.colorindex);
                    }

                    //right
                    if ((c.alphamask & 4) == 4)
                    {
                        right.fillbuffer(ref c.right, c.x, c.y, c.z, c.colorindex);
                    }
                }
            }

            sizex =       maxx - minx;
            sizey =       maxy - miny;
            sizez =       maxz - minz;

            centerposition = new Vector3((minx + ((maxx - minx) / 2)), (miny + ((maxy - miny) / 2)), (minz + ((maxz - minz) / 2)));
        }

        public void Render(Shader shader)
        {
            if (!Visible) return;
            //foreach(var c in voxels.Values)
            //{
            ////    Debug.Print(c.front.ToString());
            ////    Debug.Print(c.back.ToString());
            ////    Debug.Print(c.left.ToString());
            ////    Debug.Print(c.right.ToString());
            //    Debug.Print(c.top.ToString());
            //    //Debug.Print(c.bottom.ToString());
            //}

            while (modifiedvoxels.Count > 0)
            {
                if (modifiedvoxels.TryPop(out modified))
                {
                    switch (modified.action)
                    {
                        case VoxleModifierAction.NONE:
                            break;
                        case VoxleModifierAction.ADD:
                            break;
                        case VoxleModifierAction.REMOVE:
                            break;
                        case VoxleModifierAction.RECOLOR:
                            break;
                    }
                }
            }

            shader.WriteUniform("highlight", new Vector3(highlight.R, highlight.G, highlight.B));

            unsafe
            {
                fixed (float* pointer = &colors[0].R)
                {
                    ShaderUtil.GetShader("qb").WriteUniformArray("colors", colorIndex, pointer);
                }
            }

            if (RayIntersectsPlane(ref front.normal, ref Singleton<Camera>.INSTANCE.direction))
            {
                front.Render(shader);
            }
            if (RayIntersectsPlane(ref back.normal, ref Singleton<Camera>.INSTANCE.direction))
            {
                back.Render(shader);
            }
            if (RayIntersectsPlane(ref top.normal, ref Singleton<Camera>.INSTANCE.direction))
            {
                top.Render(shader);
            }
            if (RayIntersectsPlane(ref bottom.normal, ref Singleton<Camera>.INSTANCE.direction))
            {
                bottom.Render(shader);
            }
            if (RayIntersectsPlane(ref left.normal, ref Singleton<Camera>.INSTANCE.direction))
            {
                left.Render(shader);
            }
            if (RayIntersectsPlane(ref right.normal, ref Singleton<Camera>.INSTANCE.direction))
            {
                right.Render(shader);
            }
        }

        public void Render()
        {
            if (!Visible) return;

            if (RayIntersectsPlane(ref front.normal, ref Singleton<Camera>.INSTANCE.direction))
            {
                front.Render();
            }
            if (RayIntersectsPlane(ref back.normal, ref Singleton<Camera>.INSTANCE.direction))
            {
                back.Render();
            }
            if (RayIntersectsPlane(ref top.normal, ref Singleton<Camera>.INSTANCE.direction))
            {
                top.Render();
            }
            if (RayIntersectsPlane(ref bottom.normal, ref Singleton<Camera>.INSTANCE.direction))
            {
                bottom.Render();
            }
            if (RayIntersectsPlane(ref left.normal, ref Singleton<Camera>.INSTANCE.direction))
            {
                left.Render();
            }
            if (RayIntersectsPlane(ref right.normal, ref Singleton<Camera>.INSTANCE.direction))
            {
                right.Render();
            }
        }

        public void RenderAll(Shader shader)
        {
            if (!Visible) return;

            shader.WriteUniform("highlight", new Vector3(highlight.R, highlight.G, highlight.B));

            unsafe
            {
                fixed (float* pointer = &colors[0].R)
                {
                    ShaderUtil.GetShader("qb").WriteUniformArray("colors", colorIndex, pointer);
                }
            }

            front.Render(shader);
            back.Render(shader);
            top.Render(shader);
            bottom.Render(shader);
            left.Render(shader);
            right.Render(shader);
        }

        public void RenderAll()
        {
            if (!Visible) return;

            front.Render();
            back.Render();
            top.Render();
            bottom.Render();
            left.Render();
            right.Render();
        }

        public void UseMatrixColors()
        {
            colors = matrixcolors;
        }

        //public void UseWireframeColors()
        //{
        //    colors = wireframecolors;
        //}

        //public void UseOutlineColors()
        //{
        //    colors = outlinecolors;
        //}

        private static bool RayIntersectsPlane(ref Vector3 normal, ref Vector3 rayVector)
        {
            float denom = 0;
            Vector3.Dot(ref normal, ref rayVector, out denom);
            if (denom < .3f)
            {
                return true;
            }

            return false;
        }

        public void Dispose()
        {
            left.Dispose();
            right.Dispose();
            top.Dispose();
            bottom.Dispose();
            front.Dispose();
            back.Dispose();
        }

        // VOXEL MODIFIING
        //
        #region


        public void Clean()
        {
            List<double> toremove = new List<double>();
            minx = 10000;
            miny = 10000;
            minz = 10000;
            maxx = 0;
            maxy = 0;
            maxz = 0;
            sizex = 0;
            sizey = 0;
            sizez = 0;

            foreach (var v in voxels.Values)
            {
                if (v.dirty)
                    v.dirty = false;

                if (v.alphamask == 0)
                {
                    toremove.Add(GetHash(v.x, v.y, v.z));
                    continue;
                }

                if (v.x < minx)
                    minx = v.x;
                if (v.x > maxx)
                    maxx = v.x;

                if (v.y < miny)
                    miny = v.y;
                if (v.y > maxy)
                    maxy = v.y;

                if (v.z < minz)
                    minz = v.z;
                if (v.z > maxz)
                    maxz = v.z;
            }

            for (int i = 0; i < toremove.Count; i++)
                voxels.TryRemove(toremove[i], out voxel);

            sizex = maxx - minx;
            sizey = maxy - miny;
            sizez = maxz - minz;

            if (sizex == -10000 && sizey == -10000 && sizez == -10000)
            {
                minx = 0;
                miny = 0;
                minz = 0;

                maxx = (int)size.X;
                maxy = (int)size.Y;
                maxz = (int)size.Z;
                sizex =(int)size.X;
                sizey =(int)size.Y;
                sizez =(int)size.Z;
            }

            centerposition = new Vector3((minx + ((maxx- minx) /2)), (miny + ((maxy- miny) / 2)) , (minz + ((maxz - minz) / 2)));

            MatchFloorToSize();
        }

        public void MatchFloorToSize()
        {
            float x = Math.Min(0 - 1, minx - 1);
            float y = Math.Min(0, miny);
            float z = Math.Min(0 - 1, minz - 1);

            int width = (int)(Math.Abs(Math.Min(0 - 1, minx - 1)) + maxx + 1);
            int height = (int)(Math.Abs(Math.Min(0, miny)) + maxy + 1);
            int length = (int)(Math.Abs(Math.Min(0 - 1, minz - 1)) + maxz + 1);

            if (width < size.X + 1)
                width = (int)size.X + 1;

            if (height < size.Y)
                height = (int)size.Y;

            if (length < size.Z + 1)
                length = (int)size.Z + 1;

        }

        public double GetHash(int x, int y, int z)
        {
            return ((double)x * 23.457154879791d + (double)y) * 31.154879416546d + (double)z;
        }

        public bool IsDirty(int x, int y, int z)
        {
            Voxel voxel;
            if (voxels.TryGetValue(GetHash(x, y, z), out voxel))
            {
                return voxel.dirty;
            }
            else
                return false;
        }

        public void UpdateVoxel()
        {
            //front
            if ((voxel.alphamask & 32) == 32)
            {
                front.fillbuffer(ref voxel.front, voxel.x, voxel.y, voxel.z, voxel.colorindex);
            }
            else
                front.removebuffer(ref voxel.front);

            //back
            if ((voxel.alphamask & 64) == 64)
            {
                back.fillbuffer(ref voxel.back, voxel.x, voxel.y, voxel.z, voxel.colorindex);
            }
            else
                back.removebuffer(ref voxel.back);

            //top
            if ((voxel.alphamask & 8) == 8)
            {
                top.fillbuffer(ref voxel.top, voxel.x, voxel.y, voxel.z, voxel.colorindex);
            }
            else
                top.removebuffer(ref voxel.top);

            //bottom
            if ((voxel.alphamask & 16) == 16)
            {
                bottom.fillbuffer(ref voxel.bottom, voxel.x, voxel.y, voxel.z, voxel.colorindex);
            }
            else
                bottom.removebuffer(ref voxel.bottom);

            //left
            if ((voxel.alphamask & 2) == 2)
            {
                left.fillbuffer(ref voxel.left, voxel.x, voxel.y, voxel.z, voxel.colorindex);
            }
            else
                left.removebuffer(ref voxel.left);

            //right
            if ((voxel.alphamask & 4) == 4)
            {
                right.fillbuffer(ref voxel.right, voxel.x, voxel.y, voxel.z, voxel.colorindex);
            }
            else
                right.removebuffer(ref voxel.right);
        }

        public byte GetAlphaMask(int x, int y, int z)
        {
            byte alpha = 1;

            if (voxels.TryGetValue(GetHash(x, y, z + 1), out voxel))
            {
                if (voxel.alphamask <= 1)
                    alpha += 32;
            }
            else alpha += 32;

            if (voxels.TryGetValue(GetHash(x, y, z - 1), out voxel))
            {
                if (voxel.alphamask <= 1)
                    alpha += 64;
            }
            else alpha += 64;

            if (voxels.TryGetValue(GetHash(x, y + 1, z), out voxel))
            {
                if (voxel.alphamask <= 1)
                    alpha += 8;
            }
            else alpha += 8;

            if (voxels.TryGetValue(GetHash(x, y - 1, z), out voxel))
            {
                if (voxel.alphamask <= 1)
                    alpha += 16;
            }
            else alpha += 16;

            if (voxels.TryGetValue(GetHash(x - 1, y, z), out voxel))
            {
                if (voxel.alphamask <= 1)
                    alpha += 4;
            }
            else alpha += 4;

            if (voxels.TryGetValue(GetHash(x + 1, y, z), out voxel))
            {
                if (voxel.alphamask <= 1)
                    alpha += 2;
            }
            else alpha += 2;

            return alpha >= 1 ? alpha : (byte)0;
        }

        public bool Remove(int x, int y, int z, bool setDirty = true, bool ignoreDirt = true)
        {
            if (voxels.TryGetValue(GetHash(x, y, z), out voxel))
            {
                if (ignoreDirt && voxel.dirty || voxel.alphamask == 0) return false;

                if (voxel.alphamask > 1)
                {
                    voxel.alphamask = 0;
                    UpdateVoxel();
                }
                else
                    voxel.alphamask = 0;

                voxel.dirty = setDirty;

                if (voxels.TryGetValue(GetHash(x, y, z + 1), out voxel))
                {
                    if (voxel.alphamask > 0 && (voxel.alphamask & 64) != 64)
                    {
                        voxel.alphamask += 64;
                        UpdateVoxel();
                    }
                }


                if (voxels.TryGetValue(GetHash(x, y, z - 1), out voxel))
                {
                    if (voxel.alphamask > 0 && (voxel.alphamask & 32) != 32)
                    {
                        voxel.alphamask += 32;
                        UpdateVoxel();
                    }
                }

                if (voxels.TryGetValue(GetHash(x, y + 1, z), out voxel))
                {
                    if (voxel.alphamask > 0 && (voxel.alphamask & 16) != 16)
                    {
                        voxel.alphamask += 16;
                        UpdateVoxel();
                    }
                }

                if (voxels.TryGetValue(GetHash(x, y - 1, z), out voxel))
                {
                    if (voxel.alphamask > 0 && (voxel.alphamask & 8) != 8)
                    {
                        voxel.alphamask += 8;
                        UpdateVoxel();
                    }
                }

                if (voxels.TryGetValue(GetHash(x + 1, y, z), out voxel))
                {
                    if (voxel.alphamask > 0 && (voxel.alphamask & 4) != 4)
                    {
                        voxel.alphamask += 4;
                        UpdateVoxel();
                    }
                }

                if (voxels.TryGetValue(GetHash(x - 1, y, z), out voxel))
                {
                    if (voxel.alphamask > 0 && (voxel.alphamask & 2) != 2)
                    {
                        voxel.alphamask += 2;
                        UpdateVoxel();
                    }
                }

                return true;
                //voxels.TryRemove(gethash(x, y, z), out voxel);
            }

            return false;
        }

        public void Remove(int x, int y, int z)
        {
            Remove(x, y, z, true, false);
        }

        public bool Add(int x, int y, int z, Colort color)
        {
            if (voxels.TryGetValue(GetHash(x, y, z), out voxel))
            {
                if (voxel.alphamask != 0) return false;
                else
                {
                    if (!voxels.TryRemove(GetHash(x, y, z), out voxel)) return false;
                    voxel = new Voxel(x, y, z, GetAlphaMask(x, y, z), GetColorIndex(color.R, color.G, color.B));
                    if (voxels.TryAdd(GetHash(x, y, z), voxel))
                    {
                        voxel.dirty = true;
                        UpdateVoxel();

                        if (voxels.TryGetValue(GetHash(x, y, z + 1), out voxel))
                        {
                            if ((voxel.alphamask & 64) == 64)
                            {
                                voxel.alphamask -= 64;
                                UpdateVoxel();
                            }
                        }

                        if (voxels.TryGetValue(GetHash(x, y, z - 1), out voxel))
                        {
                            if ((voxel.alphamask & 32) == 32)
                            {
                                voxel.alphamask -= 32;
                                UpdateVoxel();
                            }
                        }

                        if (voxels.TryGetValue(GetHash(x, y + 1, z), out voxel))
                        {
                            if ((voxel.alphamask & 16) == 16)
                            {
                                voxel.alphamask -= 16;
                                UpdateVoxel();
                            }
                        }

                        if (voxels.TryGetValue(GetHash(x, y - 1, z), out voxel))
                        {
                            if ((voxel.alphamask & 8) == 8)
                            {
                                voxel.alphamask -= 8;
                                UpdateVoxel();
                            }
                        }

                        if (voxels.TryGetValue(GetHash(x + 1, y, z), out voxel))
                        {
                            if ((voxel.alphamask & 4) == 4)
                            {
                                voxel.alphamask -= 4;
                                UpdateVoxel();
                            }
                        }

                        if (voxels.TryGetValue(GetHash(x - 1, y, z), out voxel))
                        {
                            if ((voxel.alphamask & 2) == 2)
                            {
                                voxel.alphamask -= 2;
                                UpdateVoxel();
                            }
                        }

                        return true;
                    }
                    return false;
                }
            }
            else
            {
                voxel = new Voxel(x, y, z, GetAlphaMask(x, y, z), GetColorIndex(color.R, color.G, color.B));
                if (voxels.TryAdd(GetHash(x, y, z), voxel))
                {
                    voxel.dirty = true;
                    UpdateVoxel();

                    if (voxels.TryGetValue(GetHash(x, y, z + 1), out voxel))
                    {
                        if ((voxel.alphamask & 64) == 64)
                        {
                            voxel.alphamask -= 64;
                            UpdateVoxel();
                        }
                    }

                    if (voxels.TryGetValue(GetHash(x, y, z - 1), out voxel))
                    {
                        if ((voxel.alphamask & 32) == 32)
                        {
                            voxel.alphamask -= 32;
                            UpdateVoxel();
                        }
                    }

                    if (voxels.TryGetValue(GetHash(x, y + 1, z), out voxel))
                    {
                        if ((voxel.alphamask & 16) == 16)
                        {
                            voxel.alphamask -= 16;
                            UpdateVoxel();
                        }
                    }

                    if (voxels.TryGetValue(GetHash(x, y - 1, z), out voxel))
                    {
                        if ((voxel.alphamask & 8) == 8)
                        {
                            voxel.alphamask -= 8;
                            UpdateVoxel();
                        }
                    }

                    if (voxels.TryGetValue(GetHash(x + 1, y, z), out voxel))
                    {
                        if ((voxel.alphamask & 4) == 4)
                        {
                            voxel.alphamask -= 4;
                            UpdateVoxel();
                        }
                    }

                    if (voxels.TryGetValue(GetHash(x - 1, y, z), out voxel))
                    {
                        if ((voxel.alphamask & 2) == 2)
                        {
                            voxel.alphamask -= 2;
                            UpdateVoxel();
                        }
                    }

                    return true;
                }

                return false;
            }
        }

        public void Color(Vector3 location, Colort color)
        {
            Color((int)location.X, (int)location.Y, (int)location.Z, color);
        }

        public void Color(int x, int y, int z, Colort color)
        {
            Color(x, y, z, GetColorIndex(color.R, color.G, color.B));
        }

        public void Color(int x, int y, int z, int colorindex, bool setDirty = true, bool ignoreDirt = true)
        {
            if (voxels.TryGetValue(GetHash(x, y, z), out voxel) && (ignoreDirt || !voxel.dirty))
            {
                int currentColor = voxel.colorindex;
                voxel.dirty = setDirty;
                voxel.colorindex = colorindex;
                if (voxel.alphamask > 1)
                {
                    if (currentColor != colorindex)
                    {
                        _color(x, y, z);
                    }
                }
            }
        }

        private void _color(int x, int y, int z)
        {
            //front
            if ((voxel.alphamask & 32) == 32)
            {
                front.updatevoxel(voxel);
            }

            //bavoxelk
            if ((voxel.alphamask & 64) == 64)
            {
                back.updatevoxel(voxel);
            }

            //top
            if ((voxel.alphamask & 8) == 8)
            {
                top.updatevoxel(voxel);
            }

            //bottom
            if ((voxel.alphamask & 16) == 16)
            {
                bottom.updatevoxel(voxel);
            }

            //left
            if ((voxel.alphamask & 2) == 2)
            {
                left.updatevoxel(voxel);
            }

            //right
            if ((voxel.alphamask & 4) == 4)
            {
                right.updatevoxel(voxel);
            }
        }

        void CleanUnsuedColorIndexs()
        {
            //Colort[] ncolors = new Colort[64];
            //Dictionary<int, int> activecolors = new Dictionary<int, int>();

            //int newindex = 0; 

            //foreach (var voxel in voxels.Values)
            //{
            //    if (!activecolors.ContainsKey(voxel.colorindex))
            //    {
            //        activecolors.Add(voxel.colorindex, newindex);
            //        newindex++;
            //    }
            //}

            //var keys = activecolors.Keys;

            //for (int i = 0; i < keys.Count; i++)
            //{
            //    ncolors[i] = colors[keys.ElementAt(i)];
            //}

            List<int> usedindexs = new List<int>();

            foreach (var voxel in voxels.Values)
            {
                if (!usedindexs.Contains(voxel.colorindex))
                {
                    usedindexs.Add(voxel.colorindex);
                }
            }

            Stack<int> unused = new Stack<int>();

            for (int i = 0; i < colors.Length; i++)
            {
                if (!usedindexs.Contains(i))
                    unused.Push(i);
            }

            colorindexholes = unused;
        }

        public void MoveUp()
        {
            // for every voxel, starting top down
            for (int x = minx; x < maxx+1; x++)
                for (int z = minz; z < maxz+1; z++)
                    for (int y = maxy+1; y > miny-2; y--)
                    {
                        double hash = GetHash(x, y, z);

                        // if we have a voxel to move up...
                        if (voxels.TryGetValue(hash, out voxel))
                        {
                            int newColor = voxel.colorindex;

                            // if we have voxel above our current one, recolor instead of adding
                            // recoloring = better performance
                            if (voxels.TryGetValue(GetHash(x, y + 1, z), out voxel))
                            {
                                Color(x, y + 1, z, newColor);
                            }
                            else
                                Add(x, y + 1, z, colors[newColor]);
                        }
                        else
                        // we don't have a voxel to move up... so remove the one above
                            if (voxels.TryGetValue(GetHash(x, y + 1, z), out voxel))
                            {
                                Remove(x, y + 1, z);
                            }

                        if (y == miny - 1)
                            Remove(x, y, z);
                    }

            Clean();
        }


        public void MoveDown()
        {
            // for every voxel, starting bottom up
            for (int x = minx; x < maxx + 1; x++)
                for (int z = minz; z < maxz + 1; z++)
                    for (int y = miny; y < maxy +2; y++)
                    {
                        double hash = GetHash(x, y, z);

                        // if we have a voxel to move up...
                        if (voxels.TryGetValue(hash, out voxel))
                        {
                            int newColor = voxel.colorindex;

                            // if we have voxel above our current one, recolor instead of adding
                            // recoloring = better performance
                            if (voxels.TryGetValue(GetHash(x, y - 1, z), out voxel))
                            {
                                Color(x, y - 1, z, newColor);
                            }
                            else
                                Add(x, y - 1, z, colors[newColor]);
                        }
                        else
                        // we don't have a voxel to move up... so remove the one above
                            if (voxels.TryGetValue(GetHash(x, y - 1, z), out voxel))
                        {
                            Remove(x, y - 1, z);
                        }

                        if (y == maxy + 2)
                            Remove(x, y, z);
                    }

            Clean();
        }

        public void MoveLeft()
        {
            for (int y = miny; y < maxy + 1; y++)
                for (int z = minz; z < maxz + 1; z++)
                    for (int x = maxx+1; x > minx-2; x--)
                    {
                        double hash = GetHash(x, y, z);

                        // if we have a voxel to move up...
                        if (voxels.TryGetValue(hash, out voxel))
                        {
                            int newColor = voxel.colorindex;

                            // if we have voxel above our current one, recolor instead of adding
                            // recoloring = better performance
                            if (voxels.TryGetValue(GetHash(x+1, y, z), out voxel))
                            {
                                Color(x+1, y, z, newColor);
                            }
                            else
                                Add(x+1, y, z, colors[newColor]);
                        }
                        else
                        // we don't have a voxel to move up... so remove the one above
                            if (voxels.TryGetValue(GetHash(x+1, y, z), out voxel))
                        {
                            Remove(x+1, y, z);
                        }

                        if (x == minx - 1)
                            Remove(x, y, z);
                    }

            Clean();
        }

        public void MoveRight()
        {
            for (int y = miny; y < maxy + 1; y++)
                for (int z = minz; z < maxz + 1; z++)
                    for (int x = minx -2; x < maxx +1; x++)
                    {
                        double hash = GetHash(x, y, z);

                        // if we have a voxel to move up...
                        if (voxels.TryGetValue(hash, out voxel))
                        {
                            int newColor = voxel.colorindex;

                            // if we have voxel above our current one, recolor instead of adding
                            // recoloring = better performance
                            if (voxels.TryGetValue(GetHash(x - 1, y, z), out voxel))
                            {
                                Color(x - 1, y, z, newColor);
                            }
                            else
                                Add(x - 1, y, z, colors[newColor]);
                        }
                        else
                        // we don't have a voxel to move up... so remove the one above
                            if (voxels.TryGetValue(GetHash(x - 1, y, z), out voxel))
                        {
                            Remove(x - 1, y, z);
                        }

                        if (x == maxx)
                            Remove(x, y, z);
                    }

            Clean();
        }

        public void MoveBack()
        {
            for (int y = miny; y < maxy +1; y++)
                for (int x = minx; x < maxx+1 ; x++)
                    for (int z = minz-2; z < maxz+1; z++)
                    {
                        double hash = GetHash(x, y, z);

                        // if we have a voxel to move up...
                        if (voxels.TryGetValue(hash, out voxel))
                        {
                            int newColor = voxel.colorindex;

                            // if we have voxel above our current one, recolor instead of adding
                            // recoloring = better performance
                            if (voxels.TryGetValue(GetHash(x, y, z - 1), out voxel))
                            {
                                Color(x, y, z - 1, newColor);
                            }
                            else
                                Add(x, y, z - 1, colors[newColor]);
                        }
                        else
                        {
                            // we don't have a voxel to move up... so remove the one above
                            if (voxels.TryGetValue(GetHash(x, y, z - 1), out voxel))
                            {
                                Remove(x, y, z - 1);
                            }
                        }

                        if (z == maxz)
                        {
                            Remove(x, y, z);
                        }
                    }

            Clean();
        }

        public void MoveForward()
        {
            for (int y = miny; y < maxy + 1; y++)
                for (int x = minx; x < maxx + 1; x++)
                    for (int z = maxz +1; z > minz -2; z--)
                    {
                        double hash = GetHash(x, y, z);

                        // if we have a voxel to move up...
                        if (voxels.TryGetValue(hash, out voxel))
                        {
                            int newColor = voxel.colorindex;

                            // if we have voxel above our current one, recolor instead of adding
                            // recoloring = better performance
                            if (voxels.TryGetValue(GetHash(x, y, z + 1), out voxel))
                            {
                                Color(x, y, z + 1, newColor);
                            }
                            else
                                Add(x, y, z + 1, colors[newColor]);
                        }
                        else
                        {
                            // we don't have a voxel to move up... so remove the one above
                            if (voxels.TryGetValue(GetHash(x, y, z + 1), out voxel))
                            {
                                Remove(x, y, z + 1);
                            }
                        }

                        if (z == minz -1)
                        {
                            Remove(x, y, z);
                        }
                    }

            Clean();
        }

        #endregion

        #region NewVoxelEditing

        public void Add(VoxelVolume volume, ref Colort color)
        {
            int colorIndex = GetColorIndex(color.R, color.G, color.B);
            double hash = 0;
            // for all inner points in volume...
            for (int z = volume.minz + 1; z <= volume.maxz - 1; z++)
                for (int y = volume.miny + 1; y <= volume.maxy - 1; y++)
                    for (int x = volume.minx + 1; x <= volume.maxx - 1; x++)
                    {
                        hash = GetHash(x, y, z);
                        if (!voxels.TryGetValue(hash, out voxel))
                        {
                            // alphamask = 1 because we know we're this point is inside the voxel volume
                            Voxel voxel = new Voxel(x, y, z, (byte)1, colorIndex);
                            voxel.dirty = true;
                            voxels.TryAdd(hash, voxel);

                            // since this is new voxel we don't need to update any visible geometry
                            // (again it is interior voxel)
                        }
                        // we have a voxel here already
                        else
                        {
                            // nuke all visible geometry of voxel
                            voxel.alphamask = 1;
                            voxel.dirty = true;
                            UpdateVoxel();
                        }
                    }

            var insideVolume = new VoxelVolume()
            {
                minx = volume.minx + 1,
                miny = volume.miny + 1,
                minz = volume.minz + 1,
                maxx = volume.maxx - 1,
                maxy = volume.maxy - 1,
                maxz = volume.maxz - 1,
            };

            // for all exterior voxels
            for (int z = volume.minz; z <= volume.maxz; z++)
                for (int y = volume.miny; y <= volume.maxy; y++)
                    for (int x = volume.minx; x <= volume.maxx; x++)
                    {
                        if (!insideVolume.ContainsPoint(x,y, z))
                            Add(x, y, z, color);
                    }
        }

        public void Add(VoxelVolume volume, VoxelVolume preVolume, ref Colort color)
        {
            int colorIndex = GetColorIndex(color.R, color.G, color.B);
            double hash = 0;
            // for all inner points in volume...
            for (int z = volume.minz + 1; z <= volume.maxz - 1; z++)
                for (int y = volume.miny + 1; y <= volume.maxy - 1; y++)
                    for (int x = volume.minx + 1; x <= volume.maxx - 1; x++)
                    {
                        // if point not inside prevoius volume
                        if (!preVolume.ContainsPoint(x, y, z))
                        {
                            hash = GetHash(x, y, z);
                            if (!voxels.TryGetValue(hash, out voxel))
                            {
                                // alphamask = 1 because we know we're this point is inside the voxel volume
                                Voxel voxel = new Voxel(x, y, z, (byte)1, colorIndex);
                                voxels.TryAdd(hash, voxel);

                                // since this is new voxel we don't need to update any visible geometry
                                // (again it is interior voxel)
                            }
                            // we have a voxel here already
                            else
                            {
                                // nuke all visible geometry of voxel
                                voxel.alphamask = 1;
                                UpdateVoxel();
                            }
                        }
                    }

        }
        #endregion
    }
}