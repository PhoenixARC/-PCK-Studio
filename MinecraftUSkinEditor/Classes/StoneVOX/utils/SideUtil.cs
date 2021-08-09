namespace stonevox
{
    public static class SideUtil
    {
        public static int modify_x(int x, Side side)
        {
            switch (side)
            {
                case Side.Left:
                    x += 1;
                    break;
                case Side.Right:
                    x -= 1;
                    break;
            }
            return x;
        }

        public static int modify_y(int y, Side side)
        {
            switch (side)
            {
                case Side.Top:
                    y += 1;
                    break;
                case Side.Bottom:
                    y -= 1;
                    break;
            }
            return y;
        }

        public static int modify_z(int z, Side side)
        {
            switch (side)
            {
                case Side.Front:
                    z += 1;
                    break;
                case Side.Back:
                    z -= 1;
                    break;
            }
            return z;
        }

        public static void modify(ref int x, ref int y, ref int z, Side side)
        {
            switch (side)
            {
                case Side.Left:
                    x += 1;
                    break;
                case Side.Right:
                    x -= 1;
                    break;
                case Side.Top:
                    y += 1;
                    break;
                case Side.Bottom:
                    y -= 1;
                    break;
                case Side.Front:
                    z += 1;
                    break;
                case Side.Back:
                    z -= 1;
                    break;
            }
        }

        public static void modifyinverse(ref int x, ref int y, ref int z, Side side)
        {
            switch (side)
            {
                case Side.Left:
                    x -= 1;
                    break;
                case Side.Right:
                    x += 1;
                    break;
                case Side.Top:
                    y += 1;
                    break;
                case Side.Bottom:
                    y -= 1;
                    break;
                case Side.Front:
                    z += 1;
                    break;
                case Side.Back:
                    z -= 1;
                    break;
            }
        }
    }

}