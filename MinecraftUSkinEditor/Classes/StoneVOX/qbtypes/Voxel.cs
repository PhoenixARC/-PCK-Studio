namespace stonevox
{
    public enum Side
    {
        Left,
        Right,
        Top,
        Bottom,
        Front,
        Back
    }

    public class Voxel
    {
        public int colorindex;
        public byte alphamask;
        public bool dirty;

        public int left;
        public int right;
        public int top;
        public int bottom;
        public int front;
        public int back;

        public int x;
        public int y;
        public int z;

        public Voxel(int x, int y, int z, byte a, int colorindex)
        {
            this.x = x;
            this.y = y;
            this.z = z;

            this.alphamask = a;
            this.colorindex = colorindex;

            left = -1;
            right = -1;
            top = -1;
            bottom = -1;
            front = -1;
            back = -1;
        }

        public override string ToString()
        {
            return string.Format("voxel : {0}, {1}, {2}", x, y, z);
        }
    }
}