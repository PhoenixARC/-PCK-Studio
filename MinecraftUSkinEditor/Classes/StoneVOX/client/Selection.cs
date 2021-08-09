using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System;

namespace stonevox
{
    public class Selection : Singleton<Selection>
    {
        int vertexbuffer;
        int vertexArray;

        public bool dirty;
        public bool Visible = true;

        float cubesize = .5f;
        float[] buffer;

        private BrushManager brushes;
        private Input input;
        private QbManager manager;
        private Floor floor;
        private GUI gui;
        public Raycaster raycaster;

        public bool handledselectionchange = true;
        private bool needscleaning;

        private RaycastHit lasthit = new RaycastHit()
        {
            distance = 10000
        };

        private Colort color;


        public Selection(GLWindow window, BrushManager tools, Input input, QbManager manager, Floor floor, GUI gui)
             : base()
        {
            this.brushes = tools;
            this.input = input;
            this.manager = manager;
            this.floor = floor;
            this.gui = gui;

            window.Resize += (e, a) =>
             {
             };

            color = new Colort(1, 0, 0);

            buffer = new float[16];


            input.AddHandler(new InputHandler()
            {
                mousedownhandler = (e) =>
                {
                    if (Singleton<GUI>.INSTANCE.OverWidget) return;

                    if (e.Button == MouseButton.Left && !dirty && handledselectionchange && Singleton<Raycaster>.INSTANCE.HasHit)
                    {
                        handledselectionchange = brushes.onselectionchanged(input, manager.ActiveMatrix, lasthit, e);

                        if (handledselectionchange)
                            needscleaning = true;
                    }
                },
                mouseuphandler = (e) =>
                {
                    if (e.Button == MouseButton.Left && !dirty && handledselectionchange)
                    {
                        handledselectionchange = brushes.onselectionchanged(input, manager.ActiveMatrix, lasthit, e);

                        if (handledselectionchange)
                            needscleaning = true;
                    }
                }
            });
        }

        public void GenerateVertexArray()
        {
            GLUtils.CreateVertexArraysQBF(sizeof(float) * 16, out vertexArray, out vertexbuffer);
        }

        public void update()
        {
            if (dirty)
            {
                dirty = false;
                UpdateVisibleSelection();

                handledselectionchange = brushes.onselectionchanged(input, manager.ActiveMatrix, lasthit);

                if (handledselectionchange)
                    needscleaning = true;
            }

            if (!handledselectionchange && raycaster.HasHit)
            {
                handledselectionchange = brushes.onselectionchanged(input, manager.ActiveMatrix, lasthit);
                if (handledselectionchange)
                    needscleaning = true;
            }

            if (needscleaning)
            {
                if (input.mouseup(MouseButton.Left))
                {
                    // change this...  have the tool decide to clean or not
                    needscleaning = false;
                    manager.ActiveMatrix.Clean();
                }
            }
        }

        public void UpdateVisibleSelection()
        {

            lasthit = Singleton<Raycaster>.INSTANCE.lastHit;
            switch (lasthit.side)
            {
                case Side.Front:

                    buffer[0] = -cubesize + lasthit.x;
                    buffer[1] = -cubesize + lasthit.y;
                    buffer[2] = cubesize + lasthit.z;

                    buffer[4] = cubesize + lasthit.x;
                    buffer[5] = -cubesize + lasthit.y;
                    buffer[6] = cubesize + lasthit.z;

                    buffer[8] = cubesize + lasthit.x;
                    buffer[9] = cubesize + lasthit.y;
                    buffer[10] = cubesize + lasthit.z;

                    buffer[12] = -cubesize + lasthit.x;
                    buffer[13] = cubesize + lasthit.y;
                    buffer[14] = cubesize + lasthit.z;

                    break;
                case Side.Back:

                    buffer[12] = -cubesize + lasthit.x;
                    buffer[13] = -cubesize + lasthit.y;
                    buffer[14] = -cubesize + lasthit.z;

                    buffer[8] = cubesize + lasthit.x;
                    buffer[9] = -cubesize + lasthit.y;
                    buffer[10] = -cubesize + lasthit.z;

                    buffer[4] = cubesize + lasthit.x;
                    buffer[5] = cubesize + lasthit.y;
                    buffer[6] = -cubesize + lasthit.z;

                    buffer[0] = -cubesize + lasthit.x;
                    buffer[1] = cubesize + lasthit.y;
                    buffer[2] = -cubesize + lasthit.z;

                    break;
                case Side.Top:

                    buffer[0] = -cubesize + lasthit.x;
                    buffer[1] = cubesize + lasthit.y;
                    buffer[2] = cubesize + lasthit.z;

                    buffer[4] = cubesize + lasthit.x;
                    buffer[5] = cubesize + lasthit.y;
                    buffer[6] = cubesize + lasthit.z;

                    buffer[8] = cubesize + lasthit.x;
                    buffer[9] = cubesize + lasthit.y;
                    buffer[10] = -cubesize + lasthit.z;

                    buffer[12] = -cubesize + lasthit.x;
                    buffer[13] = cubesize + lasthit.y;
                    buffer[14] = -cubesize + lasthit.z;

                    break;
                case Side.Bottom:

                    buffer[12] = -cubesize + lasthit.x;
                    buffer[13] = -cubesize + lasthit.y;
                    buffer[14] = cubesize + lasthit.z;

                    buffer[8] = cubesize + lasthit.x;
                    buffer[9] = -cubesize + lasthit.y;
                    buffer[10] = cubesize + lasthit.z;

                    buffer[4] = cubesize + lasthit.x;
                    buffer[5] = -cubesize + lasthit.y;
                    buffer[6] = -cubesize + lasthit.z;

                    buffer[0] = -cubesize + lasthit.x;
                    buffer[1] = -cubesize + lasthit.y;
                    buffer[2] = -cubesize + lasthit.z;

                    break;
                case Side.Right:

                    buffer[0] = -cubesize + lasthit.x;
                    buffer[1] = -cubesize + lasthit.y;
                    buffer[2] = -cubesize + lasthit.z;

                    buffer[4] = -cubesize + lasthit.x;
                    buffer[5] = -cubesize + lasthit.y;
                    buffer[6] = cubesize + lasthit.z;

                    buffer[8] = -cubesize + lasthit.x;
                    buffer[9] = cubesize + lasthit.y;
                    buffer[10] = cubesize + lasthit.z;

                    buffer[12] = -cubesize + lasthit.x;
                    buffer[13] = cubesize + lasthit.y;
                    buffer[14] = -cubesize + lasthit.z;

                    break;
                case Side.Left:

                    buffer[12] = cubesize + lasthit.x;
                    buffer[13] = -cubesize + lasthit.y;
                    buffer[14] = -cubesize + lasthit.z;

                    buffer[8] = cubesize + lasthit.x;
                    buffer[9] = -cubesize + lasthit.y;
                    buffer[10] = cubesize + lasthit.z;

                    buffer[4] = cubesize + lasthit.x;
                    buffer[5] = cubesize + lasthit.y;
                    buffer[6] = cubesize + lasthit.z;

                    buffer[0] = cubesize + lasthit.x;
                    buffer[1] = cubesize + lasthit.y;
                    buffer[2] = -cubesize + lasthit.z;
                    break;
            }

            buffer[3] = 0;
            buffer[7] = 0;
            buffer[11] = 0;
            buffer[15] = 0;

            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexbuffer);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, (IntPtr)(sizeof(float) * 16), buffer);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

        }

        public void render(Shader shader)
        {
            if (Visible && Singleton<Raycaster>.INSTANCE.lastHit.distance != 10000)
            {
                shader.WriteUniform("highlight", new Vector3(1, 1, 1));

                unsafe
                {
                    fixed (float* pointer = &color.R)
                    {
                        shader.WriteUniformArray("colors", 1, pointer);
                    }
                }

                GL.BindVertexArray(vertexArray);
                GL.DrawArrays(PrimitiveType.Quads, 0, 4);
                GL.BindVertexArray(0);
            }
        }
    }
}