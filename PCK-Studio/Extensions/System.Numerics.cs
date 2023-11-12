namespace PckStudio.Extensions
{
    internal static class NumericsExtensions
    {
        internal static OpenTK.Vector3 ToOpenTKVector(this System.Numerics.Vector3 vector3)
        {
            return new OpenTK.Vector3(vector3.X, vector3.Y, vector3.Z);
        }
        
        internal static OpenTK.Vector2 ToOpenTKVector(this System.Numerics.Vector2 vector2)
        {
            return new OpenTK.Vector2(vector2.X, vector2.Y);
        }
    }
}
