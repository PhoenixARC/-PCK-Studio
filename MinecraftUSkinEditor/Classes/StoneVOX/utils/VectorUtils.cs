using OpenTK;

namespace stonevox
{
    public static class VectorUtils
    {
        private static Vector3 up = new Vector3(0, 1, 0);
        private static Vector3 down = new Vector3(0, -1, 0);
        private static Vector3 left = new Vector3(-1, 0, 0);
        private static Vector3 right = new Vector3(1, 0, 0);
        private static Vector3 forward = new Vector3(0, 0, 1);
        private static Vector3 back = new Vector3(0, 0, -1);

        public static Vector3 UP { get { return up; } }
        public static Vector3 DOWN { get { return down; } }
        public static Vector3 FORWARD { get { return forward; } }
        public static Vector3 BACK { get { return back; } }
        public static Vector3 LEFT { get { return left; } }
        public static Vector3 RIGHT { get { return right; } }
    }
}
