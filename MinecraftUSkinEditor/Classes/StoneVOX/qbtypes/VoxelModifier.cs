namespace stonevox
{
    public class VoxelModifier
    {
        public int x;
        public int y;
        public int z;

        public VoxelSide side;
        public VoxleModifierAction action;

        public VoxelModifier(int x, int y, int z, VoxelSide side, VoxleModifierAction action)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.side = side;
            this.action = action;
        }
    }

    public enum VoxleModifierAction
    {
        NONE,
        ADD,
        REMOVE,
        RECOLOR
    }
}