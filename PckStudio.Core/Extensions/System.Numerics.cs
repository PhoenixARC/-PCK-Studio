namespace PckStudio.Core.Extensions
{
    public static class NumericsExtensions
    {
        //internal static Cube ToCube(this SkinBOX skinBOX) => skinBOX.ToCube(0f);

        //internal static Cube ToCube(this SkinBOX skinBOX, float inflate, bool flipZMapping = false)
        //    => new Cube(skinBOX.Pos.ToOpenTKVector(), skinBOX.Size.ToOpenTKVector(), skinBOX.UV.ToOpenTKVector(), skinBOX.Scale + inflate, skinBOX.Mirror, flipZMapping);
        public static OpenTK.Vector3 ToOpenTKVector(this System.Numerics.Vector3 vector3)
        {
            return new OpenTK.Vector3(vector3.X, vector3.Y, vector3.Z);
        }

        public static OpenTK.Vector2 ToOpenTKVector(this System.Numerics.Vector2 vector2)
        {
            return new OpenTK.Vector2(vector2.X, vector2.Y);
        }

        public static System.Numerics.Vector3 ToNumericsVector(this OpenTK.Vector3 vector3)
        {
            return new System.Numerics.Vector3(vector3.X, vector3.Y, vector3.Z);
        }

        public static System.Numerics.Vector2 ToNumericsVector(this OpenTK.Vector2 vector2)
        {
            return new System.Numerics.Vector2(vector2.X, vector2.Y);
        }
    }
}
