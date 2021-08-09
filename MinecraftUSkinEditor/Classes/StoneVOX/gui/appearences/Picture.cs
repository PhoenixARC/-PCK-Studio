using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Windows.Forms;

namespace stonevox
{
    [GUIAppearenceName("Picture")]
    [GUIAppearenceDataType(typeof(PictureData))]
    public class Picture : Appearence
    {
        public enum RenderOptions
        {
            None,
            FlipHorizontal,
            FlipVertical
        }

        string filePath;

        public Texture2D texture;

        public bool AutoResizeWidget;

        public Color4 color = Color4.White;

        public RenderOptions options;

        public Picture() { AutoResizeWidget = true; }

        public Picture(string filepath) : this()
        {
            filePath = filepath;
        }

        public override void Initialize()
        {
            texture = GLUtils.LoadImage(Application.StartupPath + filePath);
            MatchTextureBounds(Owner);
        }

        public override void Render(float x, float y, float width, float height)
        {
            if (texture.TextureID == -1) return;


            GL.Enable(EnableCap.Texture2D);

            GL.BindTexture(TextureTarget.Texture2D, texture.TextureID);

            GL.Color4(color);
            GL.Begin(PrimitiveType.Quads);

            switch (options)
            {
                case RenderOptions.None:
                    /*
                        
                        3----2
                        |    |
                        0----1

                    */

                    GL.TexCoord2(0f, 1f);
                    GL.Vertex2(x, y);

                    GL.TexCoord2(1f, 1f);
                    GL.Vertex2(x + width, y);

                    GL.TexCoord2(1f, 0f);
                    GL.Vertex2(x + width, y + height);

                    GL.TexCoord2(0f, 0f);
                    GL.Vertex2(x, y + height);
                    break;
                case RenderOptions.FlipHorizontal:

                    GL.TexCoord2(1f, 1f);
                    GL.Vertex2(x, y);

                    GL.TexCoord2(0f, 1f);
                    GL.Vertex2(x + width, y);

                    GL.TexCoord2(0f, 0f);
                    GL.Vertex2(x + width, y + height);

                    GL.TexCoord2(1f, 0f);
                    GL.Vertex2(x, y + height);

                    break;
                case RenderOptions.FlipVertical:

                    GL.TexCoord2(0f, 0f);
                    GL.Vertex2(x, y);

                    GL.TexCoord2(1f, 0f);
                    GL.Vertex2(x + width, y);

                    GL.TexCoord2(1f, 1f);
                    GL.Vertex2(x + width, y + height);

                    GL.TexCoord2(0f, 1f);
                    GL.Vertex2(x, y + height);

                    break;
            }
            GL.End();

            GL.Disable(EnableCap.Texture2D);
        }

        public void MatchTextureBounds(Widget widget)
        {
            widget.size.X = Scale.hSizeScale(texture.Width) * 2 * Singleton<GUI>.INSTANCE.scale;
            widget.size.Y = Scale.vSizeScale(texture.Height)*2 * Singleton<GUI>.INSTANCE.scale;
        }

        public void SetImage(string path)
        {
            filePath = path;
            texture = GLUtils.LoadImage(Application.StartupPath + filePath);

            if (AutoResizeWidget)
                MatchTextureBounds(Owner);
        }

        public override Appearence FromData(AppearenceData data)
        {
            PictureData _data = data as PictureData;
            return new Picture(_data.FilePath);
        }
        public override AppearenceData ToData()
        {
            return new PictureData(filePath);
        }
    }
}
