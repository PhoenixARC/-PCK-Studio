namespace stonevox
{
    public static class Scale
    {
        static float minh;
        static float maxh;

        static float minv;
        static float maxv;

        static float aspect;

        static float screenN1 = -1.0f;
        static float screen1 = 1.0f;

        public static void SetHScaling(float min, float max)
        {
            minh = min;
            maxh = max;
        }

        public static void SetVScaling(float min, float max)
        {
            minv = min;
            maxv = max;
        }

        public static void SetAspectRatio(float width, float height)
        {
            aspect = width / height;

            // screenN1 = -1.0f * aspect;
            // screen1 = 1.0f * aspect;
        }

        // value
        // min screen 0
        // max screen max
        // minscale lowest value
        // maxscale hight value
        public static float scale(float value, float min, float max, float minScale, float maxScale)
        {
            float scaled = (float)(minScale + (double)(value - min) / (max - min) * (maxScale - minScale));
            return scaled;
        }

        public static float hPosScale(float value)
        {
            return scale(value, minh, maxh, screenN1, screen1);
        }

        public static float vPosScale(float value)
        {
            return scale(value, minv, maxv, screenN1, screen1);
        }

        public static float hSizeScale(float value)
        {
            return scale(value, minh, maxh, 0, screen1);
        }

        public static float vSizeScale(float value)
        {
            return scale(value, minv, maxv, 0, screen1);
        }

        public static float hUnPosScale(float value)
        {
            return scale(value, screenN1, screen1, minh, maxh);
        }

        public static float vUnPosScale(float value)
        {
            return scale(value, screenN1, screen1, minv, maxv);
        }

        public static float hUnSizeScale(float value)
        {
            return scale(value, 0, screen1, minh, maxh);
        }

        public static float vUnSizeScale(float value)
        {
            return scale(value, 0, screen1, minv, maxv);
        }
    }
}
