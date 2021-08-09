using System;
using System.IO;

namespace stonevox
{
    public class ExporterQb : IExporter
    {
        public string extension
        {
            get { return ".qb"; }
        }

        public void write(string path, string name, QbModel model)
        {
            string fullpath = Path.Combine(path, name + extension);
            using (FileStream f = new FileStream(fullpath, FileMode.OpenOrCreate))
            {
                using (BinaryWriter w = new BinaryWriter(f))
                {
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

                        int startx = Math.Min(0 , m.minx );
                        int starty = Math.Min(0, m.miny);
                        int startz = Math.Min(0 , m.minz );

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

                                            int r = (int)(c.R * 255f);
                                            int g = (int)(c.G * 255f);
                                            int b = (int)(c.B * 255f);

                                            w.Write((byte)r);
                                            w.Write((byte)g);
                                            w.Write((byte)b);
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
            }
        }
    }
}