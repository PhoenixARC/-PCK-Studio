using OpenTK;
using System;
using System.Threading;

namespace stonevox
{
    public enum RaycastMode
    {
        ActiveMatrix,
        FullModel,
        MatrixSelection, // super hacks
        MatrixColorSelection
    }

    public class Raycaster : Singleton<Raycaster>
    {
        Vector3 top = new Vector3(0, 1, 0);
        Vector3 bottom = new Vector3(0, -1, 0);
        Vector3 left = new Vector3(1, 0, 0);
        Vector3 right = new Vector3(-1, 0, 0);
        Vector3 front = new Vector3(0, 0, 1);
        Vector3 back = new Vector3(0, 0, -1);

        public Vector3 near = new Vector3();
        public Vector3 far = new Vector3();

        float clientwidth;
        float clientheight;

        Camera camera;
        Input input;
        GLWindow window;
        Selection selection;
        Floor floor;
        QbManager manager;
        GUI gui;

        public Vector3 rayOrigin;
        public Vector3 rayDirection;

        public bool HasHit;

        bool enabled = true;
        public bool Enabled { get { return enabled; } set { enabled = value; if (!value) selection.handledselectionchange = true; } }
        public bool testdirt = false;

        public RaycastMode Mode = RaycastMode.ActiveMatrix;

        float dot;
        float t;
        float coordRatio;
        Vector3 intPoint = new Vector3();
        double accuracy = 0.0008d;
        double fullArea;
        double subTriangle1;
        double subTriangle2;
        double subTriangle3;
        double totalSubAreas;
        float cubesize = .5f;
        float d;
        Matrix4 a;
        Vector4 _in = new Vector4();
        Vector4 _out;
        Vector3 p1;
        Vector3 p2;
        Vector3 p3;
        Vector3 s1;
        Vector3 s2;
        Vector3 s3;
        bool _front;
        bool _back;
        bool _left;
        bool _right;
        bool _top;
        bool _bottom;
        Vector3 testout;

        public RaycastHit lastHit = new RaycastHit()
        {
            distance = 10000
        };

        Thread thread;

        public Raycaster(GLWindow window, Camera camera, Selection selection, Floor floor, Input input, QbManager manager, GUI gui)
        {
            this.window = window;
            this.camera = camera;
            this.input = input;
            this.floor = floor;
            this.selection = selection;
            this.manager = manager;
            this.gui = gui;

            clientwidth = window.Width;
            clientheight = window.Height;
            window.Resize += (e, o) =>
            {
                clientwidth = window.Width;
                clientheight = window.Height;
            };

            thread = new Thread(RaycastTask);
            thread.Start();
        }

        void RaycastTask()
        {
            while (true)
            {
                if (!enabled || gui.OverWidget)
                {
                    HasHit = false;
                    lastHit.distance = 10000;
                    Thread.Sleep(200);
                    continue;
                }

                if (!camera.freecam)
                    ScreenToMouseRay(input.mousex, input.mousey);
                else
                    ScreenToMouseRay(window.Width / 2, window.Height / 2);
                RaycastHit hit = new RaycastHit();
                hit.distance = 10000;

                switch (Mode)
                {
                    case RaycastMode.ActiveMatrix:
                        if (!manager.ActiveMatrix.Visible) break;
                        hit = RaycastTest(camera.position, manager.ActiveMatrix);

                        if (hit.distance != 10000)
                        {
                            HasHit = true;
                            hit.matrixIndex = manager.ActiveMatrixIndex;
                            if (!hit.matches(lastHit))
                            {
                                lastHit = hit;
                                selection.dirty = true;
                            }
                        }
                        else if (hit.distance == 10000)
                        {
                            if (floor.RayTest(this, ref hit))
                            {
                                HasHit = true;
                                hit.matrixIndex = manager.ActiveMatrixIndex;
                                if (!hit.matches(lastHit))
                                {
                                    lastHit = hit;
                                    selection.dirty = true;
                                }
                                Thread.Sleep(15);
                                continue;
                            }
                            selection.handledselectionchange = true;
                            HasHit = false;
                        }
                        lastHit = hit;
                        break;
                    case RaycastMode.FullModel:

                        for (int i = 0; i < manager.ActiveModel.numMatrices; i++)
                        {
                            if (!manager.ActiveModel.matrices[i].Visible) continue;
                            RaycastHit tempHit = RaycastTest(camera.position, manager.ActiveModel.matrices[i]);

                            if (tempHit.distance < hit.distance && !tempHit.matches(hit))
                            {
                                manager.ActiveModel.activematrix = i;
                                tempHit.matrixIndex = i;
                                hit = tempHit;
                            }
                        }

                        if (hit.distance != 10000)
                        {
                            HasHit = true;
                            if (!hit.matches(lastHit))
                            {
                                lastHit = hit;
                                selection.dirty = true;
                            }
                        }
                        else if (hit.distance == 10000)
                        {
                            selection.handledselectionchange = true;
                            HasHit = false;
                        }
                        lastHit = hit;
                        break;

                    case RaycastMode.MatrixSelection: // super hacks

                        int index = 0;
                        for (int i = 0; i < manager.ActiveModel.numMatrices; i++)
                        {
                            if (!manager.ActiveModel.matrices[i].Visible) continue;
                            RaycastHit tempHit = RaycastTest(camera.position, manager.ActiveModel.matrices[i]);

                            if (tempHit.distance != 10000 && tempHit.distance < hit.distance && !tempHit.matches(hit))
                            {
                                index = i;
                                tempHit.matrixIndex = i;
                                hit = tempHit;
                            }
                        }

                        if (hit.distance != 10000)
                        {
                            HasHit = true;
                        }
                        else if (hit.distance == 10000)
                        {
                            HasHit = false;
                        }
                        if (!hit.matches(lastHit))
                        {
                            // ohhh my...
                            // super super hacks
                            if (HasHit)
                                Singleton<BrushManager>.INSTANCE.onselectionchanged(input, manager.ActiveModel.matrices[index], hit);
                            else
                                Singleton<BrushManager>.INSTANCE.onselectionchanged(input, null, hit);

                        }
                        lastHit = hit;

                        break;

                    case RaycastMode.MatrixColorSelection: // super hacks

                        int indexx = 0;
                        for (int i = 0; i < manager.ActiveModel.numMatrices; i++)
                        {
                            if (!manager.ActiveModel.matrices[i].Visible) continue;
                            RaycastHit tempHit = RaycastTest(camera.position, manager.ActiveModel.matrices[i]);

                            if (tempHit.distance != 10000 && tempHit.distance < hit.distance && !tempHit.matches(hit))
                            {
                                indexx = i;
                                tempHit.matrixIndex = i;
                                hit = tempHit;
                            }
                        }

                        if (hit.distance != 10000)
                        {
                            HasHit = true;
                        }
                        else if (hit.distance == 10000)
                        {
                            HasHit = false;
                        }
                        if (!hit.matches(lastHit))
                        {
                            Client.OpenGLContextThread.Add(() => Singleton<Selection>.INSTANCE.UpdateVisibleSelection());
                            // ohhh my...
                            // super super hacks
                            if (HasHit)
                                Singleton<BrushManager>.INSTANCE.onselectionchanged(input, manager.ActiveModel.matrices[indexx], hit);
                            else
                                Singleton<BrushManager>.INSTANCE.onselectionchanged(input, null, hit);

                        }
                        lastHit = hit;

                        break;
                }
                Thread.Sleep(15);
            }
        }

        public void UnProject(float x, float y, float z, ref Matrix4 modelview, ref Matrix4 projection, out Vector3 value)
        {
            Matrix4.Mult(ref modelview, ref projection, out a);
            a.Invert();

            _in.X = (x) / clientwidth * 2f - 1f;
            _in.Y = (y) / clientheight * 2f - 1f;
            _in.Z = 2f * z - 1f;
            _in.W = 1;

            Vector4.Transform(ref _in, ref a, out _out);

            if (_out.W != 0f)
                _out.W = 1f / _out.W;

            value.X = _out.X * _out.W;
            value.Y = _out.Y * _out.W;
            value.Z = _out.Z * _out.W;
        }

        public void ScreenToMouseRay(int mouseX, int mouseY)
        {
            UnProject(mouseX, mouseY, 0f, ref camera.view, ref camera.projection, out near);
            UnProject(mouseX, mouseY, 1f, ref camera.view, ref camera.projection, out far);
            Vector3.Subtract(ref far, ref near, out rayDirection);
        }

        public bool RayTestTriangle(ref Vector3 planeNormal, float p1x, float p1y, float p1z, float p2x, float p2y,
           float p2z, float p3x, float p3y, float p3z, out Vector3 _out)
        {
            dot = rayDirection.X * planeNormal.X + rayDirection.Y * planeNormal.Y + rayDirection.Z * planeNormal.Z;
            t = 0;
            if (dot == 0)
            {
                _out = Vector3.Zero;
                return false;
            }
            coordRatio =
                    p1x * planeNormal.X + p1y * planeNormal.Y + p1z * planeNormal.Z - planeNormal.X * rayOrigin.X
                            - planeNormal.Y * rayOrigin.Y - planeNormal.Z * rayOrigin.Z;
            t = coordRatio / dot;
            if (t < 0)
            {
                _out = Vector3.Zero;
                return false;
            }

            intPoint.X = rayOrigin.X + t * rayDirection.X;
            intPoint.Y = rayOrigin.Y + t * rayDirection.Y;
            intPoint.Z = rayOrigin.Z + t * rayDirection.Z;

            fullArea = CalculateTriangleArea(p1x, p1y, p1z, p2x, p2y, p2z, p3x, p3y, p3z);
            subTriangle1 = CalculateTriangleArea(p1x, p1y, p1z, p2x, p2y, p2z, intPoint.X, intPoint.Y, intPoint.Z);
            subTriangle2 = CalculateTriangleArea(p2x, p2y, p2z, p3x, p3y, p3z, intPoint.X, intPoint.Y, intPoint.Z);
            subTriangle3 = CalculateTriangleArea(p1x, p1y, p1z, p3x, p3y, p3z, intPoint.X, intPoint.Y, intPoint.Z);

            totalSubAreas = subTriangle1 + subTriangle2 + subTriangle3;

            if (Math.Abs(fullArea - totalSubAreas) < accuracy)
            {
                _out = intPoint;
                return true;
            }
            else
            {
                _out = Vector3.Zero;
                return false;
            }
        }

        private double CalculateTriangleArea(float p1x, float p1y, float p1z, float p2x, float p2y, float p2z,
            float p3x, float p3y, float p3z)
        {
            // this is about 1/3 faster than all the math.sqrt
            // 1/2 | (x₃ - x₁) x (x₃ - x₂) | 
            p1.X = p1x;
            p1.Y = p1y;
            p1.Z = p1z;
            p2.X = p2x;
            p2.Y = p2y;
            p2.Z = p2z;
            p3.X = p3x;
            p3.Y = p3y;
            p3.Z = p3z;

            Vector3.Subtract(ref p3, ref p1, out s1);
            Vector3.Subtract(ref p3, ref p2, out s2);

            Vector3.Cross(ref s1, ref s2, out s3);

            double value = Math.Abs(s3.X + s3.Y + s3.Z);
            return value * .5d;

            //double a = Math.Sqrt((p2x - p1x) * (p2x - p1x) + (p2y - p1y) * (p2y - p1y) + (p2z - p1z) * (p2z - p1z));
            //double b = Math.Sqrt((p3x - p2x) * (p3x - p2x) + (p3y - p2y) * (p3y - p2y) + (p3z - p2z) * (p3z - p2z));
            //double c = Math.Sqrt((p3x - p1x) * (p3x - p1x) + (p3y - p1y) * (p3y - p1y) + (p3z - p1z) * (p3z - p1z));
            //double s = (a + b + c) / 2d;
            //return Math.Sqrt(s * (s - a) * (s - b) * (s - c));
        }

        bool RayIntersectsPlane(ref Vector3 normal, ref Vector3 rayVector)
        {
            float denom = 0;
            Vector3.Dot(ref normal, ref rayVector, out denom);
            if (denom < .3f)
            {
                return true;
            }

            return false;
        }

        public float distance(float x, float y, float z)
        {
            float num = rayOrigin.X - x;
            float num2 = rayOrigin.Y - y;
            float num3 = rayOrigin.Z - z;
            float num4 = num * num + num2 * num2 + num3 * num3;
            return (float)Math.Sqrt((double)num4);
        }

        public RaycastHit RaycastTest(Vector3 camerapos, QbMatrix m)
        {
            RaycastHit hitpoint = new RaycastHit();
            hitpoint.distance = 10000;
            rayOrigin = camerapos;

            _front = RayIntersectsPlane(ref front, ref camera.direction);
            _back = RayIntersectsPlane(ref back, ref camera.direction);
            _top = RayIntersectsPlane(ref top, ref camera.direction);
            _bottom = RayIntersectsPlane(ref bottom, ref camera.direction);
            _right = RayIntersectsPlane(ref right, ref camera.direction);
            _left = RayIntersectsPlane(ref left, ref camera.direction);

            //var outp =string.Format("front {0},back {1},top {2},bottom {3},left {4},right {5},", _front, _back, _top, _bottom, _left, _right);
            //Console.WriteLine(outp);

            // RayTestTriangle region... kinda think of like quad treeing
            //Vector3[] f1 = new Vector3[] { new Vector3(0, 0, 0), new Vector3(m.size.X/2f, m.size.Y /2f, m.size.Z/2f) };
            //Vector3[] f2 = new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, 0, 0) };
            //Vector3[] f3 = new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, 0, 0) };
            //Vector3[] f4 = new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, 0, 0) };
            //Vector3[] f5 = new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, 0, 0) };
            //Vector3[] f6 = new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, 0, 0) };
            //Vector3[] f7 = new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, 0, 0) };
            //Vector3[] f8 = new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, 0, 0) };

            foreach (var v in m.voxels.Values)
            {
                if (testdirt)
                {
                    if (v.dirty)
                    {
                        ////front
                        if (_front && (RayTestTriangle(ref front, -cubesize + v.x, -cubesize + v.y, cubesize + v.z,
                                          cubesize + v.x, -cubesize + v.y, cubesize + v.z,
                                          cubesize + v.x, cubesize + v.y, cubesize + v.z,
                                          out testout) || RayTestTriangle(ref front, -cubesize + v.x, -cubesize + v.y, cubesize + v.z,
                                              cubesize + v.x, cubesize + v.y, cubesize + v.z,
                                              -cubesize + v.x, cubesize + v.y, cubesize + v.z, out testout)))
                        {
                            d = distance(v.x * .5f, v.y * .5f, v.z * .5f + .5f);
                            if (d < hitpoint.distance)
                            {
                                hitpoint.distance = d;
                                hitpoint.x = v.x;
                                hitpoint.y = v.y;
                                hitpoint.z = v.z;
                                hitpoint.side = Side.Front;
                            }
                        }

                        ////bavk
                        if (_back && (RayTestTriangle(ref back, -cubesize + v.x, -cubesize + v.y, -cubesize + v.z,
                                         cubesize + v.x, -cubesize + v.y, -cubesize + v.z,
                                         cubesize + v.x, cubesize + v.y, -cubesize + v.z,
                                         out testout) || RayTestTriangle(ref back, -cubesize + v.x, -cubesize + v.y, -cubesize + v.z,
                                               cubesize + v.x, cubesize + v.y, -cubesize + v.z,
                                               -cubesize + v.x, cubesize + v.y, -cubesize + v.z, out testout)))
                        {
                            d = distance(v.x * .5f, v.y * .5f, v.z * .5f - .5f);
                            if (d < hitpoint.distance)
                            {
                                hitpoint.distance = d;
                                hitpoint.x = v.x;
                                hitpoint.y = v.y;
                                hitpoint.z = v.z;
                                hitpoint.side = Side.Back;
                            }
                        }


                        //top
                        if (_top && (RayTestTriangle(ref top, -cubesize + v.x, cubesize + v.y, cubesize + v.z,
                                             cubesize + v.x, cubesize + v.y, cubesize + v.z,
                                             cubesize + v.x, cubesize + v.y, -cubesize + v.z,
                                             out testout) || RayTestTriangle(ref top, -cubesize + v.x, cubesize + v.y, cubesize + v.z,
                                             cubesize + v.x, cubesize + v.y, -cubesize + v.z,
                                             -cubesize + v.x, cubesize + v.y, -cubesize + v.z, out testout)))
                        {
                            d = distance(v.x * .5f, v.y * .5f + .5f, v.z * .5f);
                            if (d < hitpoint.distance)
                            {
                                hitpoint.distance = d;
                                hitpoint.x = v.x;
                                hitpoint.y = v.y;
                                hitpoint.z = v.z;
                                hitpoint.side = Side.Top;
                            }
                        }

                        ////bottom
                        if (_bottom && (RayTestTriangle(ref bottom, -cubesize + v.x, -cubesize + v.y, cubesize + v.z,
                                             cubesize + v.x, -cubesize + v.y, cubesize + v.z,
                                             cubesize + v.x, -cubesize + v.y, -cubesize + v.z,
                                             out testout) || RayTestTriangle(ref bottom, -cubesize + v.x, -cubesize + v.y, cubesize + v.z,
                                                cubesize + v.x, -cubesize + v.y, -cubesize + v.z,
                                                -cubesize + v.x, -cubesize + v.y, -cubesize + v.z, out testout)))
                        {
                            d = distance(v.x * .5f, v.y * .5f - .5f, v.z * .5f);
                            if (d < hitpoint.distance)
                            {
                                hitpoint.distance = d;
                                hitpoint.x = v.x;
                                hitpoint.y = v.y;
                                hitpoint.z = v.z;
                                hitpoint.side = Side.Bottom;
                            }
                        }

                        ////left
                        if (_left && (RayTestTriangle(ref left, cubesize + v.x, -cubesize + v.y, -cubesize + v.z,
                                             cubesize + v.x, -cubesize + v.y, cubesize + v.z,
                                             cubesize + v.x, cubesize + v.y, cubesize + v.z,
                                             out testout) || RayTestTriangle(ref left, cubesize + v.x, -cubesize + v.y, -cubesize + v.z,
                                              cubesize + v.x, cubesize + v.y, cubesize + v.z,
                                              cubesize + v.x, cubesize + v.y, -cubesize + v.z, out testout)))
                        {
                            d = distance(v.x * .5f + .5f, v.y * .5f, v.z * .5f);
                            if (d < hitpoint.distance)
                            {
                                hitpoint.distance = d;
                                hitpoint.x = v.x;
                                hitpoint.y = v.y;
                                hitpoint.z = v.z;
                                hitpoint.side = Side.Left;
                            }
                        }

                        ////right
                        if (_right && (RayTestTriangle(ref right, -cubesize + v.x, -cubesize + v.y, -cubesize + v.z,
                                             -cubesize + v.x, -cubesize + v.y, cubesize + v.z,
                                             -cubesize + v.x, cubesize + v.y, cubesize + v.z,
                                             out testout) || RayTestTriangle(ref right, -cubesize + v.x, -cubesize + v.y, -cubesize + v.z,
                                               -cubesize + v.x, cubesize + v.y, cubesize + v.z,
                                               -cubesize + v.x, cubesize + v.y, -cubesize + v.z, out testout)))
                        {
                            d = distance(v.x * .5f - .5f, v.y * .5f, v.z * .5f);
                            if (d < hitpoint.distance)
                            {
                                hitpoint.distance = d;
                                hitpoint.x = v.x;
                                hitpoint.y = v.y;
                                hitpoint.z = v.z;
                                hitpoint.side = Side.Right;
                            }
                        }
                    }
                    else
                    {
                        //front
                        if (_front && ((v.alphamask & 32) == 32))
                        {
                            if ((RayTestTriangle(ref front, -cubesize + v.x, -cubesize + v.y, cubesize + v.z,
                                         cubesize + v.x, -cubesize + v.y, cubesize + v.z,
                                         cubesize + v.x, cubesize + v.y, cubesize + v.z,
                                         out testout) || RayTestTriangle(ref front, -cubesize + v.x, -cubesize + v.y, cubesize + v.z,
                                             cubesize + v.x, cubesize + v.y, cubesize + v.z,
                                             -cubesize + v.x, cubesize + v.y, cubesize + v.z, out testout)))
                            {
                                d = distance(v.x * .5f, v.y * .5f, v.z * .5f + .5f);
                                if (d < hitpoint.distance)
                                {
                                    hitpoint.distance = d;
                                    hitpoint.x = v.x;
                                    hitpoint.y = v.y;
                                    hitpoint.z = v.z;
                                    hitpoint.side = Side.Front;
                                }
                            }
                        }

                        //bavk
                        if (_back && ((v.alphamask & 64) == 64))
                        {
                            if ((RayTestTriangle(ref back, -cubesize + v.x, -cubesize + v.y, -cubesize + v.z,
                                        cubesize + v.x, -cubesize + v.y, -cubesize + v.z,
                                        cubesize + v.x, cubesize + v.y, -cubesize + v.z,
                                        out testout) || RayTestTriangle(ref back, -cubesize + v.x, -cubesize + v.y, -cubesize + v.z,
                                              cubesize + v.x, cubesize + v.y, -cubesize + v.z,
                                              -cubesize + v.x, cubesize + v.y, -cubesize + v.z, out testout)))
                            {
                                d = distance(v.x * .5f, v.y * .5f, v.z * .5f - .5f);
                                if (d < hitpoint.distance)
                                {
                                    hitpoint.distance = d;
                                    hitpoint.x = v.x;
                                    hitpoint.y = v.y;
                                    hitpoint.z = v.z;
                                    hitpoint.side = Side.Back;
                                }
                            }
                        }

                        //top
                        if (_top && ((v.alphamask & 8) == 8))
                        {
                            if ((RayTestTriangle(ref top, -cubesize + v.x, cubesize + v.y, cubesize + v.z,
                                             cubesize + v.x, cubesize + v.y, cubesize + v.z,
                                             cubesize + v.x, cubesize + v.y, -cubesize + v.z,
                                             out testout) || RayTestTriangle(ref top, -cubesize + v.x, cubesize + v.y, cubesize + v.z,
                                             cubesize + v.x, cubesize + v.y, -cubesize + v.z,
                                             -cubesize + v.x, cubesize + v.y, -cubesize + v.z, out testout)))
                            {
                                d = distance(v.x * .5f, v.y * .5f + .5f, v.z * .5f);
                                if (d < hitpoint.distance)
                                {
                                    hitpoint.distance = d;
                                    hitpoint.x = v.x;
                                    hitpoint.y = v.y;
                                    hitpoint.z = v.z;
                                    hitpoint.side = Side.Top;
                                }
                            }
                        }

                        //bottom
                        if (_bottom && ((v.alphamask & 16) == 16))
                        {
                            if ((RayTestTriangle(ref bottom, -cubesize + v.x, -cubesize + v.y, cubesize + v.z,
                                            cubesize + v.x, -cubesize + v.y, cubesize + v.z,
                                            cubesize + v.x, -cubesize + v.y, -cubesize + v.z,
                                            out testout) || RayTestTriangle(ref bottom, -cubesize + v.x, -cubesize + v.y, cubesize + v.z,
                                               cubesize + v.x, -cubesize + v.y, -cubesize + v.z,
                                               -cubesize + v.x, -cubesize + v.y, -cubesize + v.z, out testout)))
                            {
                                d = distance(v.x * .5f, v.y * .5f - .5f, v.z * .5f);
                                if (d < hitpoint.distance)
                                {
                                    hitpoint.distance = d;
                                    hitpoint.x = v.x;
                                    hitpoint.y = v.y;
                                    hitpoint.z = v.z;
                                    hitpoint.side = Side.Bottom;
                                }
                            }
                        }

                        //left
                        if (_left && ((v.alphamask & 2) == 2))
                        {
                            if ((RayTestTriangle(ref left, cubesize + v.x, -cubesize + v.y, -cubesize + v.z,
                                             cubesize + v.x, -cubesize + v.y, cubesize + v.z,
                                             cubesize + v.x, cubesize + v.y, cubesize + v.z,
                                             out testout) || RayTestTriangle(ref left, cubesize + v.x, -cubesize + v.y, -cubesize + v.z,
                                              cubesize + v.x, cubesize + v.y, cubesize + v.z,
                                              cubesize + v.x, cubesize + v.y, -cubesize + v.z, out testout)))
                            {
                                d = distance(v.x * .5f + .5f, v.y * .5f, v.z * .5f);
                                if (d < hitpoint.distance)
                                {
                                    hitpoint.distance = d;
                                    hitpoint.x = v.x;
                                    hitpoint.y = v.y;
                                    hitpoint.z = v.z;
                                    hitpoint.side = Side.Left;
                                }
                            }
                        }

                        //right
                        if (_right && ((v.alphamask & 4) == 4))
                        {
                            if ((RayTestTriangle(ref right, -cubesize + v.x, -cubesize + v.y, -cubesize + v.z,
                                            -cubesize + v.x, -cubesize + v.y, cubesize + v.z,
                                            -cubesize + v.x, cubesize + v.y, cubesize + v.z,
                                            out testout) || RayTestTriangle(ref right, -cubesize + v.x, -cubesize + v.y, -cubesize + v.z,
                                              -cubesize + v.x, cubesize + v.y, cubesize + v.z,
                                              -cubesize + v.x, cubesize + v.y, -cubesize + v.z, out testout)))
                            {
                                d = distance(v.x * .5f - .5f, v.y * .5f, v.z * .5f);
                                if (d < hitpoint.distance)
                                {
                                    hitpoint.distance = d;
                                    hitpoint.x = v.x;
                                    hitpoint.y = v.y;
                                    hitpoint.z = v.z;
                                    hitpoint.side = Side.Right;
                                }
                            }
                        }
                    }
                }
                else if (v.alphamask > 0)
                {
                    if (v.dirty)
                        continue;
                    //front
                    if (_front && ((v.alphamask & 32) == 32 || m.IsDirty(v.x, v.y, v.z + 1)))
                    {
                        if ((RayTestTriangle(ref front, -cubesize + v.x, -cubesize + v.y, cubesize + v.z,
                                         cubesize + v.x, -cubesize + v.y, cubesize + v.z,
                                         cubesize + v.x, cubesize + v.y, cubesize + v.z,
                                         out testout) || RayTestTriangle(ref front, -cubesize + v.x, -cubesize + v.y, cubesize + v.z,
                                             cubesize + v.x, cubesize + v.y, cubesize + v.z,
                                             -cubesize + v.x, cubesize + v.y, cubesize + v.z, out testout)))
                        {
                            d = distance(v.x * .5f, v.y * .5f, v.z * .5f + .5f);
                            if (d < hitpoint.distance)
                            {
                                hitpoint.distance = d;
                                hitpoint.x = v.x;
                                hitpoint.y = v.y;
                                hitpoint.z = v.z;
                                hitpoint.side = Side.Front;
                            }
                        }
                    }

                    //bavk
                    if (_back && ((v.alphamask & 64) == 64 || m.IsDirty(v.x, v.y, v.z - 1)))
                    {
                        if ((RayTestTriangle(ref back, -cubesize + v.x, -cubesize + v.y, -cubesize + v.z,
                                        cubesize + v.x, -cubesize + v.y, -cubesize + v.z,
                                        cubesize + v.x, cubesize + v.y, -cubesize + v.z,
                                        out testout) || RayTestTriangle(ref back, -cubesize + v.x, -cubesize + v.y, -cubesize + v.z,
                                              cubesize + v.x, cubesize + v.y, -cubesize + v.z,
                                              -cubesize + v.x, cubesize + v.y, -cubesize + v.z, out testout)))
                        {
                            d = distance(v.x * .5f, v.y * .5f, v.z * .5f - .5f);
                            if (d < hitpoint.distance)
                            {
                                hitpoint.distance = d;
                                hitpoint.x = v.x;
                                hitpoint.y = v.y;
                                hitpoint.z = v.z;
                                hitpoint.side = Side.Back;
                            }
                        }
                    }


                    //top
                    if (_top && ((v.alphamask & 8) == 8) || m.IsDirty(v.x, v.y + 1, v.z))
                    {
                        if ((RayTestTriangle(ref top, -cubesize + v.x, cubesize + v.y, cubesize + v.z,
                                            cubesize + v.x, cubesize + v.y, cubesize + v.z,
                                            cubesize + v.x, cubesize + v.y, -cubesize + v.z,
                                            out testout) || RayTestTriangle(ref top, -cubesize + v.x, cubesize + v.y, cubesize + v.z,
                                            cubesize + v.x, cubesize + v.y, -cubesize + v.z,
                                            -cubesize + v.x, cubesize + v.y, -cubesize + v.z, out testout)))
                        {
                            d = distance(v.x * .5f, v.y * .5f + .5f, v.z * .5f);
                            if (d < hitpoint.distance)
                            {
                                hitpoint.distance = d;
                                hitpoint.x = v.x;
                                hitpoint.y = v.y;
                                hitpoint.z = v.z;
                                hitpoint.side = Side.Top;
                            }
                        }
                    }

                    //bottom
                    if (_bottom && ((v.alphamask & 16) == 16) || m.IsDirty(v.x, v.y - 1, v.z))
                    {
                        if ((RayTestTriangle(ref bottom, -cubesize + v.x, -cubesize + v.y, cubesize + v.z,
                                           cubesize + v.x, -cubesize + v.y, cubesize + v.z,
                                           cubesize + v.x, -cubesize + v.y, -cubesize + v.z,
                                           out testout) || RayTestTriangle(ref bottom, -cubesize + v.x, -cubesize + v.y, cubesize + v.z,
                                              cubesize + v.x, -cubesize + v.y, -cubesize + v.z,
                                              -cubesize + v.x, -cubesize + v.y, -cubesize + v.z, out testout)))
                        {
                            d = distance(v.x * .5f, v.y * .5f - .5f, v.z * .5f);
                            if (d < hitpoint.distance)
                            {
                                hitpoint.distance = d;
                                hitpoint.x = v.x;
                                hitpoint.y = v.y;
                                hitpoint.z = v.z;
                                hitpoint.side = Side.Bottom;
                            }
                        }
                    }

                    //left
                    if (_left && ((v.alphamask & 2) == 2 || m.IsDirty(v.x + 1, v.y, v.z)))
                    {
                        if ((RayTestTriangle(ref left, cubesize + v.x, -cubesize + v.y, -cubesize + v.z,
                      cubesize + v.x, -cubesize + v.y, cubesize + v.z,
                      cubesize + v.x, cubesize + v.y, cubesize + v.z,
                      out testout) || RayTestTriangle(ref left, cubesize + v.x, -cubesize + v.y, -cubesize + v.z,
                       cubesize + v.x, cubesize + v.y, cubesize + v.z,
                       cubesize + v.x, cubesize + v.y, -cubesize + v.z, out testout)))
                        {
                            d = distance(v.x * .5f + .5f, v.y * .5f, v.z * .5f);
                            if (d < hitpoint.distance)
                            {
                                hitpoint.distance = d;
                                hitpoint.x = v.x;
                                hitpoint.y = v.y;
                                hitpoint.z = v.z;
                                hitpoint.side = Side.Left;
                            }
                        }
                    }

                    //right
                    if (_right && ((v.alphamask & 4) == 4 || m.IsDirty(v.x - 1, v.y, v.z)))
                    {
                        if ((RayTestTriangle(ref right, -cubesize + v.x, -cubesize + v.y, -cubesize + v.z,
                                            -cubesize + v.x, -cubesize + v.y, cubesize + v.z,
                                            -cubesize + v.x, cubesize + v.y, cubesize + v.z,
                                            out testout) || RayTestTriangle(ref right, -cubesize + v.x, -cubesize + v.y, -cubesize + v.z,
                                              -cubesize + v.x, cubesize + v.y, cubesize + v.z,
                                              -cubesize + v.x, cubesize + v.y, -cubesize + v.z, out testout)))
                        {
                            d = distance(v.x * .5f - .5f, v.y * .5f, v.z * .5f);
                            if (d < hitpoint.distance)
                            {
                                hitpoint.distance = d;
                                hitpoint.x = v.x;
                                hitpoint.y = v.y;
                                hitpoint.z = v.z;
                                hitpoint.side = Side.Right;
                            }
                        }
                    }
                }
            }
            return hitpoint;
        }

    }
    public class RaycastHit
    {
        public float distance;
        public int x;
        public int y;
        public int z;
        public Side side;
        public int matrixIndex;

        public RaycastHit()
        {
        }

        public RaycastHit(int x, int y, int z, float distance)
        {

        }

        public bool matches(RaycastHit other)
        {
            return this.x == other.x && this.y == other.y && this.z == other.z && this.side == other.side;
        }

        public override string ToString()
        {
            return string.Format("SIDE : {0} \nLocation : {1} : {2} : {3}", side.ToString(), x, y, z);
        }
    }
}
