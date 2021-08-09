namespace stonevox
{
    public struct VoxelVolume
    {
        public int minx;
        public int miny;
        public int minz;
        public int maxx;
        public int maxy;
        public int maxz;

        public VoxelVolume(VoxelLocation start, VoxelLocation end)
        {
            minx = start.x < end.x ? start.x : end.x;
            miny = start.y < end.y ? start.y : end.y;
            minz = start.z < end.z ? start.z : end.z;
            maxx = start.x > end.x ? start.x : end.x;
            maxy = start.y > end.y ? start.y : end.y;
            maxz = start.z > end.z ? start.z : end.z;
        }

        public bool ContainsPoint(int x, int y, int z)
        {
            if (x < minx || y < miny || z < minz
                || x > maxx || y > maxy || z > maxz)
                return false;
            return true;
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2} : {3} {4} {5}", minx, miny, minz, maxx, maxy, maxz);
        }

        public static VoxelVolume NEGATIVE_ZERO { get {
                return new VoxelVolume()
                {
                    maxx = -1,
                    maxy = -1,
                    maxz = -1,
                    minx = -1,
                    miny = -1,
                    minz = -1
                };
            }
        }
    }
}
