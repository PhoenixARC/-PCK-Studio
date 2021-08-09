using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace stonevox
{
    [GUIAppearenceName("Plain Border")]
    [GUIAppearenceDataType(typeof(PlainBorderData))]
    public class PlainBorder : Appearence
    {
        public float borderWidth;
        public float borderScaled;
        public Color4 color;

        public PlainBorder() { }

        public PlainBorder(float borderWidth, Color4 color)
        {
            this.borderWidth = borderWidth;
            this.borderScaled = Scale.vSizeScale(borderWidth);
            this.color = color;
        }

        public override void Initialize()
        {
        }

        public override void Render(float x, float y, float width, float height)
        {
            GL.Color4(color);
            GL.LineWidth(borderWidth);

            GL.Begin(PrimitiveType.Lines);

            GL.Vertex2(x, y);
            GL.Vertex2(x + width, y);

            GL.Vertex2(x + width, y - borderScaled*.5f);
            GL.Vertex2(x + width, y + height +borderScaled*.75f);

            GL.Vertex2(x, y + height);
            GL.Vertex2(x + width, y + height);

            GL.Vertex2(x, y + height + borderScaled*.5f);
            GL.Vertex2(x, y - borderScaled);

            GL.End();
            GL.LineWidth(1);
        }

        public override Appearence FromData(AppearenceData data)
        {
            PlainBorderData _data = data as PlainBorderData;
            return new PlainBorder(_data.BorderWidth, _data.Color);
        }
        public override AppearenceData ToData()
        {
            return new PlainBorderData()
            {
                BorderWidth = borderWidth,
                Color = color
            };
        }
    }
}
