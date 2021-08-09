using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System;

namespace stonevox
{
    public class Wireframe : Singleton<Wireframe>, IRenderer
    {
        // eventually i'll get around to implementing a single-pass wireframe shader

        Shader voxelShader;
        Shader wireframeShader;

        Camera camera;
        Selection selection;
        Floor floor;

        public bool drawWireframe = true;
        public WireframeType wireFrameType = WireframeType.SmartOutline;

        public Wireframe(Camera camera, Selection selection, Floor floor, Input input)
            : base()
        {
            voxelShader = ShaderUtil.CreateShader("qb", "./data/shaders/voxel.vs", "./data/shaders/voxel.fs");
            wireframeShader = ShaderUtil.CreateShader("wireframe_qb", "./data/shaders/wireframe.vs", "./data/shaders/wireframe.fs");

            this.camera = camera;
            this.selection = selection;
            this.floor = floor;

            // bug mouse leaving matrix while dragging tools

            input.AddHandler(new InputHandler()
            {
                Keydownhandler = (e) =>
                {
                    bool shift = Singleton<Input>.INSTANCE.Keydown(OpenTK.Input.Key.ShiftLeft);

                    if (e.Key == Key.G && !e.Shift)
                    {
                        drawWireframe = !drawWireframe;
                    }
                    else if (e.Key == Key.G && e.Shift)
                    {
                        MoveNext();
                    }
                }
            });
        }

        public void MoveNext()
        {
            drawWireframe = true;
            var values = Enum.GetValues(typeof(WireframeType));

            var enumer = values.GetEnumerator();
            while (enumer.MoveNext())
            {
                if ((WireframeType)enumer.Current == wireFrameType)
                {
                    if (enumer.MoveNext())
                    {
                        wireFrameType = (WireframeType)enumer.Current;


                        Singleton<GUI>.INSTANCE.Dirty = true;
                        return;
                    }
                }
            }

            wireFrameType = WireframeType.WireframeBlack;

            Singleton<GUI>.INSTANCE.Dirty = true;
        }

        public void Render(QbModel model)
        {
            switch (wireFrameType)
            {
                case WireframeType.WireframeBlack:

                    if (drawWireframe)
                    {
                        GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                        GL.LineWidth(1);
                        wireframeShader.UseShader();
                        wireframeShader.WriteUniform("vHSV", new Vector3(1, 0f, 0));
                        wireframeShader.WriteUniform("modelview", camera.modelviewprojection);
                        model.getactivematrix.RenderAll(wireframeShader);
                    }
                    GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                    GL.LineWidth(2);
                    voxelShader.UseShader();
                    voxelShader.WriteUniform("modelview", camera.modelviewprojection);
                    selection.render(voxelShader);

                    GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                    model.RenderAll(voxelShader);
                    break;
                case WireframeType.WireframeColorMatch:
                    if (drawWireframe)
                    {
                        GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                        GL.LineWidth(1);
                        wireframeShader.UseShader();
                        wireframeShader.WriteUniform("vHSV", new Vector3(1, 1.1f, 1.1f));
                        wireframeShader.WriteUniform("modelview", camera.modelviewprojection);
                        model.getactivematrix.RenderAll(wireframeShader);
                    }
                    GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                    GL.LineWidth(2);
                    voxelShader.UseShader();
                    voxelShader.WriteUniform("modelview", camera.modelviewprojection);
                    selection.render(voxelShader);

                    GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                    model.RenderAll(voxelShader);
                    break;
                case WireframeType.SmartOutline:
                    if (drawWireframe)
                    {
                        GL.CullFace(CullFaceMode.Front);
                        GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                        GL.LineWidth(2);
                        wireframeShader.UseShader();
                        wireframeShader.WriteUniform("vHSV", new Vector3(1, 1.5f, 1.0f));
                        wireframeShader.WriteUniform("modelview", camera.modelviewprojection);
                        model.getactivematrix.RenderAll(wireframeShader);
                    }
                    GL.CullFace(CullFaceMode.Back);
                    GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                    GL.LineWidth(2);
                    voxelShader.UseShader();
                    voxelShader.WriteUniform("modelview", camera.modelviewprojection);
                    selection.render(voxelShader);

                    GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                    model.RenderAll(voxelShader);
                    break;
            }
        }
    }

    public enum WireframeType
    {
        WireframeBlack,
        WireframeColorMatch,
        SmartOutline
    }
}
