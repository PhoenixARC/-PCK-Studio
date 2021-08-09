using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace stonevox
{
    [GUIAppearenceName("Plain Background")]
    [GUIAppearenceDataType(typeof(PlainBackgroundData))]
    public class PlainBackground : Appearence
    {
        public Color4 color;

        public PlainBackground()
        {

        }

        public PlainBackground(Color4 color)
        {
            this.color = color;
        }

        public override void Initialize()
        {
        }

        public override void Render(float x, float y, float width, float height)
        {
            GL.Color4(color);
            GL.Begin(PrimitiveType.Quads);

            GL.Vertex2(x, y);
            GL.Vertex2(x + width, y);
            GL.Vertex2(x + width, y + height);
            GL.Vertex2(x, y + height);

            GL.End();
        }

        public override Appearence FromData(AppearenceData data)
        {
            PlainBackgroundData bdata = data as PlainBackgroundData;
            return new PlainBackground(bdata.Color);
        }
        public override AppearenceData ToData()
        {
            PlainBackgroundData data = new PlainBackgroundData()
            {
                Color = this.color
            };
            return data;
        }
    }
}
