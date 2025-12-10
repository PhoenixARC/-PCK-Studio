using System.Numerics;

namespace PckStudio.Core
{
    public class Box(Vector3 position, Vector3 size, Vector2 uv, float inflate, bool mirror)
    {
        public Vector3 Position { get; } = position;
        public Vector3 Size { get; } = size;
        public Vector2 Uv { get; } = uv;
        public float Inflate { get; } = inflate;
        public bool Mirror { get; } = mirror;
    }
}
