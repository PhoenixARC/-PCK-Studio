using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;

namespace stonevox
{
    public class Floor : Singleton<Floor>
    {
        public static int MinSize = 15;
        Camera camera;
        Broadcaster broadcaster;

        public Color4 color = Color4.Transparent;

       float width =15;
       float length = 15;

       public float x = 0;
       public float y = 0;
       public float z = 0;

        Vector3 up = new Vector3(0, 1, 0);
        Vector3 h = Vector3.Zero;

        public Floor(Camera camera, Broadcaster broadcaster)
            : base()
        {
            this.camera = camera;
            this.broadcaster = broadcaster;

            broadcaster.handlers.Add((message, e, t) => 
            {
                if (message == Message.ActiveMatrixChanged)
                {
                    QbMatrix m = t[0] as QbMatrix;
                    m.MatchFloorToSize();
                }
            });
        }

        public void SetFloorSize(float x, float y, float z, float width, float length)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.width = width;
            this.length = length;
        }

        public void Render()
        {
            float cubesize = .5f;

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref camera.projection);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref camera.view);

            GL.Begin(PrimitiveType.Quads);

            GL.Color4(color);
            GL.Vertex3(-cubesize + x,
             -.5F + y,
             cubesize*length*2f + z+cubesize);

            GL.Vertex3(cubesize*width*2f + x+cubesize,
            -.5F + y,
            cubesize*length*2f + z+cubesize);

            GL.Vertex3(cubesize*width*2f + x+cubesize,
            -.5F + y,
             -cubesize + z);

            GL.Vertex3(-cubesize*2f + x +cubesize,
             -.5F + y,
             -cubesize + z);
            GL.End();
        }

        public bool RayTest(Raycaster raycaster, ref RaycastHit hit)
        {
            // 0 1 2
            // 0 2 3
            float cubesize = .5f;

            for (float x = this.x; x <= this.x+width; x++)
                for (float z = this.z; z <= this.z + length; z++)
                {
                    if (raycaster.RayTestTriangle(ref up, -cubesize + x, -.5F + y, cubesize + z,
                                             cubesize + x, -.5F + y, cubesize + z,
                                             cubesize + x, -.5F + y, -cubesize + z,
                                             out h) || raycaster.RayTestTriangle(ref up, -cubesize + x, -.5F + y, cubesize + z,
                                             cubesize + x, -.5F  + y, -cubesize + z,
                                             -cubesize + x, -.5F + y, -cubesize + z, out h))
                    {
                        int hx, hy, hz;
                        hx = (int)Math.Round(h.X, 0);
                        hy = (int)Math.Round(h.Y, 0);
                        hz = (int)Math.Round(h.Z, 0);

                        hit.distance = raycaster.distance(hx * .5f, hy * .5f + .5f, hz * .5f);
                        hit.x = hx;
                        hit.y = (int)y-1;
                        hit.z = hz;
                        hit.side = Side.Top;
                        return true;
                    }
                }
            return false;
        }
    }
}
