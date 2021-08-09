using OpenTK;
using OpenTK.Input;
using System;

namespace stonevox
{
    public class Camera : Singleton<Camera>
    {
        public Vector3 position;
        public Vector3 direction;

        public Matrix4 projection;
        public Matrix4 view;
        public Matrix4 modelviewprojection;

        private Input input;
        private QbManager manager;

        public Vector3 cameraright { get { return Vector3.Cross(direction, VectorUtils.UP); } }
        public Vector3 cameraup { get { return Vector3.Cross(cameraright, direction); } }

        float fov = 45f;
        float nearPlane = 1f;
        float farPlane = 300;

        bool dotransition;
        Vector3 startpos;
        Vector3 startdir;
        Vector3 _goto;
        Vector3 centerposition;
        float time = 0;

        public bool freecam;

        public Camera(GLWindow window, Input input, QbManager manager)
            : base()
        {
            this.input = input;
            this.manager = manager;

            window.Resize += (e, s) =>
                {
                    projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(fov), (float)window.Width / (float)window.Height, nearPlane, farPlane);
                };

            position = new Vector3(0f, 0f, 10f);
            direction = new Vector3(0f, 0f, 1f);
            direction.Normalize();

            projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(fov), (float)window.Width / (float)window.Height, nearPlane, farPlane);
            view = Matrix4.LookAt(position, position + direction, VectorUtils.UP);
            modelviewprojection = projection * view;

            InputHandler handler = new InputHandler()
            {
                Keydownhandler = (e) =>
                {
                    var gui = Singleton<GUI>.INSTANCE;


                    //freecam is super interesting... but need a bit of work
                    //if (e.Key == Key.C)
                    //{
                    //    freecam = !freecam;

                    //    if (freecam)
                    //    {
                    //        Client.window.CursorVisible = false;
                    //    }
                    //    else
                    //    {
                    //        Client.window.CursorVisible = true;
                    //    }
                    //}
                },


                mousewheelhandler = (e) =>
                {
                    if (!Singleton<GUI>.INSTANCE.OverWidget)
                    {
                        if (e.Delta < 0)
                        {
                            position -= direction * 6 * 1f;
                        }
                        else if (e.Delta > 0)
                        {
                            position += direction * 6 * 1f;
                        }
                    }
                    else if (Singleton<GUI>.INSTANCE.lastWidgetOver.Drag)
                    {
                        if (e.Delta < 0)
                        {
                            position -= direction * 6 * 1f;
                        }
                        else if (e.Delta > 0)
                        {
                            position += direction * 6 * 1f;
                        }
                    }
                }
            };
            input.AddHandler(handler);
        }

        public void update(float delta)
        {
            if (!Singleton<GUI>.INSTANCE.OverWidget)
            {
                if (input.Keydown(Key.Q))
                    position += Vector3.UnitY * -15f * delta;
                if (input.Keydown(Key.E))
                    position -= Vector3.UnitY * -15f * delta;

                if (input.Keydown(Key.W) || input.Keydown(Key.Up))
                {
                    position += direction * 25f * delta;
                }

                if (input.Keydown(Key.S) || input.Keydown(Key.Down))
                {
                    position -= direction * 25f * delta;
                }

                if (input.Keydown(Key.A) || input.Keydown(Key.Left))
                {
                    Vector3 camerar = cameraright;

                    position.X -= camerar.X * 25f * delta;
                    position.Z -= camerar.Z * 25f * delta;
                }

                if (input.Keydown(Key.D) || input.Keydown(Key.Right))
                {
                    Vector3 camerar = cameraright;

                    position.X += camerar.X * 25f * delta;
                    position.Z += camerar.Z * 25f * delta;
                }
            }

            if (input.mousedown(MouseButton.Right) || freecam)
            {
                Vector3 camright = cameraright;
                Vector3 camup = cameraup;

                float roty = (float)MathHelper.DegreesToRadians(-input.mousedx * .15f);
                float rotx = (float)MathHelper.DegreesToRadians(-input.mousedy * .15f);

                if (Math.Abs(direction.Y) >= .95f && Math.Sign(rotx) == Math.Sign(direction.Y))
                {
                    camright.Normalize();
                    camup.Normalize();

                    Vector3 focus = position - manager.ActiveMatrix.centerposition;
                    float length = focus.Length;
                    focus.Normalize();

                    focus = Vector3.Transform(focus, Quaternion.FromAxisAngle(camup, roty));

                    position = focus * length + manager.ActiveMatrix.centerposition;

                    direction = Vector3.Transform(direction, Quaternion.FromAxisAngle(camup, roty));
                    direction.Normalize();
                }
                else
                {
                    camright.Normalize();
                    camup.Normalize();

                    Vector3 focus = position - manager.ActiveMatrix.centerposition;
                    float length = focus.Length;
                    focus.Normalize();

                    if (!freecam)
                    {
                        focus = Vector3.Transform(focus, Quaternion.FromAxisAngle(camup, roty));
                        focus = Vector3.Transform(focus, Quaternion.FromAxisAngle(camright, rotx));
                    }

                    position = focus * length + manager.ActiveMatrix.centerposition;

                    direction = Vector3.Transform(direction, Quaternion.FromAxisAngle(camup, roty));
                    direction = Vector3.Transform(direction, Quaternion.FromAxisAngle(camright, rotx));
                    direction.Normalize();

                    direction.Y = direction.Y.Clamp(-.95f, .95f);
                }
            }

            if (input.mousedown(MouseButton.Middle))
            {
                Vector3 camright = Vector3.Cross(direction, Vector3.UnitY);

                camright.Normalize();

                Vector3 camup = Vector3.Cross(camright, direction);

                camup.Normalize();

                camright *= -input.mousedx;
                camup *= input.mousedy;

                camright += camup;

                position.X += camright.X * .06f;
                position.Y += camright.Y * .06f;
                position.Z += camright.Z * .06f;
            }

            if (dotransition)
            {
                time += delta;

                if (time >= .5f)
                {
                    position = _goto;
                    direction = (centerposition - position).Normalized();
                    dotransition = false;
                    time = 0;
                }
                else
                {
                    position = Vector3.Lerp(startpos, _goto, time / .5f);
                    direction = Vector3.Lerp(startdir, (centerposition - position).Normalized(), time / .5f);
                    direction.Normalize();
                }
            }

            view = Matrix4.LookAt(position, position + direction, Vector3.UnitY);
            modelviewprojection = view * projection;
        }

        public void LookAtModel(bool skipVoxels = false)
        {
            if (!skipVoxels)
            {
                int minx = 10000;
                int miny = 10000;
                int minz = 10000;
                int maxx = 0;
                int maxy = 0;
                int maxz = 0;
                int sizex = 0;
                int sizey = 0;
                int sizez = 0;

                foreach (var matrix in manager.ActiveModel.matrices)
                {
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
                position = centerpos + new Vector3(.5f, sizey * .65f, backup);

                Vector3.Subtract(ref centerpos, ref position, out direction);
                direction.Normalize();

                view = Matrix4.LookAt(position, position + direction, cameraup);
                modelviewprojection = Matrix4.CreateScale(.1f) * projection * view;
            }
            else
            {
                float sizey = manager.ActiveMatrix.sizey;
                float sizex = manager.ActiveMatrix.sizex;

                float backup = 0;

                if (sizey * 1.5f > 20)
                    backup = sizey * 1.5f;
                else if (sizex * 1.5f > 20)
                    backup = sizex * 1.5f;
                else backup = 20;

                var centerpos = manager.ActiveMatrix.centerposition;
                position = centerpos + new Vector3(0, sizey * .65f, backup*.7f);

                Vector3.Subtract(ref centerpos, ref position, out direction);
                direction.Normalize();

                view = Matrix4.LookAt(position, position + direction, cameraup);
                modelviewprojection = Matrix4.CreateScale(.1f) * projection * view;
            }
        }

        public void TransitionToMatrix()
        {
            startpos = position;
            startdir = direction;

            var mat = manager.ActiveMatrix;
            _goto = mat.centerposition;
            centerposition = mat.centerposition;

            float height = (mat.maxy - mat.miny);
            float width = (mat.maxx - mat.minx);
            float length = (mat.maxz - mat.minz);

            float distance;

            if (height < 10 && width < 10 && length < 10)
            {
                height = (mat.maxy - mat.miny) * 2.5f;
                width = (mat.maxx - mat.minx) * 2.5f;
                length = (mat.maxz - mat.minz) * 2.5f;

                distance = Math.Max(Math.Max(height, width), length);
            }
            else if (height < 18 && width < 18 && length < 18)
            {
                height = (mat.maxy - mat.miny) * 3.5f;
                width = (mat.maxx - mat.minx) * 3.5f;
                length = (mat.maxz - mat.minz) * 3.5f;

                distance = Math.Max(Math.Max(height, width), length);
            }
            else
            {
                height = (mat.maxy - mat.miny) * 1.5f;
                width = (mat.maxx - mat.minx) * 1.5f;
                length = (mat.maxz - mat.minz) * 1.5f;

                distance = Math.Max(Math.Max(height, width), length);
            }

            Vector3 offset = direction;
            if (Math.Abs(offset.X) > .5f)
                offset.X = 1f * -Math.Sign(direction.X);
            else
                offset.X = 0;
            if (Math.Abs(offset.Z) > .5f)
                offset.Z = 1f * -Math.Sign(direction.Z);
            else
                offset.Z = 0;
            if (Math.Abs(offset.Y) > .5f)
                offset.Y = 1f * -Math.Sign(direction.Y);
            else
                offset.Y = 0;

            if (offset.Y == 1 && offset.X == 0 && offset.Z == 0)
            {
                offset = direction;

                float starting = .5f;
                bool b = true;
                while (b)
                {
                    if (Math.Abs(offset.X) > starting)
                    {
                        offset.X = 1f * -Math.Sign(direction.X);
                        b = false;
                    }
                    if (Math.Abs(offset.Z) > starting)
                    {
                        offset.Z = 1f * -Math.Sign(direction.Z);
                        b = false;
                    }
                    starting -= .07f;
                }

                offset.Y = 0;
            }

            offset.Normalize();

            _goto += offset * distance;
            dotransition = true;
        }
    }
}
