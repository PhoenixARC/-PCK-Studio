using System.IO;
using System.Linq;

namespace stonevox
{
    public class ImporterQb : IImporter
    {
        public string extension
        {
            get { return ".qb"; }
        }

        public QbModel read(string path)
        {
            StopwatchUtil.startclient("regularqbread", "Begin Qb read");

            QbModel model = _read(path);

            model.GenerateVertexBuffers();
            model.FillVertexBuffers();

            StopwatchUtil.stopclient("regularqbread", "End Qb read");

            return model;
        }

        public QbModel _read(string path)
        {
            QbModel model = new QbModel(path.Split('\\').Last().Split('.').First());

            using (FileStream f = new FileStream(path, FileMode.Open))
            using (BinaryReader reader = new BinaryReader(f))
            {
                model.version = reader.ReadUInt32();
                model.colorFormat = reader.ReadUInt32();
                model.zAxisOrientation = reader.ReadUInt32();
                model.compressed = reader.ReadUInt32();
                model.visibilityMaskEncoded = reader.ReadUInt32();
                model.setmatrixcount(reader.ReadUInt32());

                for (int i = 0; i < model.numMatrices; i++)
                {
                    QbMatrix m = model.matrices[i];

                    byte l = reader.ReadByte();
                    m.name = System.Text.Encoding.Default.GetString(reader.ReadBytes(l));
                    m.setsize((int)reader.ReadUInt32(), (int)reader.ReadUInt32(), (int)reader.ReadUInt32());
                    m.position = new OpenTK.Vector3(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());

                    byte r;
                    byte g;
                    byte b;
                    byte a;
                    int zz;

                    if (model.compressed == 0)
                    {

                        for (int z = 0; z < m.size.Z; z++)
                            for (int y = 0; y < m.size.Y; y++)
                                for (int x = 0; x < m.size.X; x++)
                                {
                                    r = reader.ReadByte();
                                    g = reader.ReadByte();
                                    b = reader.ReadByte();
                                    a = reader.ReadByte();
                                    zz = model.zAxisOrientation == (int)0 ? z : (int)(m.size.Z - z - 1);

                                    if (a != 0)
                                    {
                                        m.voxels.GetOrAdd(m.GetHash(x, y, zz), new Voxel(x, y, zz, a, m.getcolorindex(r, g, b, model.colorFormat)));
                                    }
                                }
                    }
                    else
                    {
                        for (int z = 0; z < m.size.Z; z++)
                        {
                            zz = model.zAxisOrientation == 0 ? z : (int)m.size.Z - z - 1;
                            int index = 0;
                            while (true)
                            {
                                r = reader.ReadByte();
                                g = reader.ReadByte();
                                b = reader.ReadByte();
                                a = reader.ReadByte();
                                if (r == 6 && g == 0 && b == 0 && a == 0) // NEXTSLICEFLAG
                                {
                                    break;
                                }
                                else
                                {
                                    if (r == 2 && g == 0 && b == 0 && a == 0) //CODEFLAG
                                    {
                                        uint count = reader.ReadUInt32();
                                        r = reader.ReadByte();
                                        g = reader.ReadByte();
                                        b = reader.ReadByte();
                                        a = reader.ReadByte();
                                        if (a != 0)
                                        {
                                            for (int j = 0; j < count; j++)
                                            {
                                                int x = index % (int)m.size.X;
                                                int y = index / (int)m.size.X;
                                                index++;
                                                m.voxels.GetOrAdd(m.GetHash(x, y, zz), new Voxel(x, y, zz, a, m.getcolorindex(r, g, b, model.colorFormat)));
                                            }
                                        }
                                        else
                                        {
                                            index += (int)count;
                                        }
                                    }
                                    else
                                    {
                                        int x = index % (int)m.size.X;
                                        int y = index / (int)m.size.X;
                                        index++;
                                        if (a != 0)
                                        {
                                            m.voxels.GetOrAdd(m.GetHash(x, y, zz), new Voxel(x, y, zz, a, m.getcolorindex(r, g, b, model.colorFormat)));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return model;
        }
    }
}