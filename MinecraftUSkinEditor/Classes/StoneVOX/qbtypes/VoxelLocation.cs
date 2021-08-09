namespace stonevox
{
    public struct VoxelLocation
    {
        public int x;
        public int y;
        public int z;

        public VoxelLocation(RaycastHit hit)
            : this (hit, true)
        {
        }

        public VoxelLocation(RaycastHit hit, bool modifyBySideCollided = false)
        {
            if (!modifyBySideCollided)
            {
                x = hit.x;
                y = hit.y;
                z = hit.z;
            }
            else
            {
                x = SideUtil.modify_x(hit.x, hit.side);
                y = SideUtil.modify_y(hit.y, hit.side);
                z = SideUtil.modify_z(hit.z, hit.side);
            }
        }
    }
}
