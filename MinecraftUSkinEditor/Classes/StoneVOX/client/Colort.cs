using OpenTK.Graphics;

namespace stonevox
{
    public struct Colort
    {
        public float R;
        public float G;
        public float B;

        public Colort(float r, float g, float b)
        {
            this.R = r;
            this.G = g;
            this.B = b;
        }

        public static implicit operator Colort(Color4 color)
        {
            return new Colort(color.R, color.G, color.B);
        }

        public static implicit operator Color4(Colort color)
        {
            return new Color4(color.R, color.G, color.B, 1.0f);
        }
    }
}