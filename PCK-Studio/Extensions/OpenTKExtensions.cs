using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace PckStudio.Extensions
{
    internal static class OpenTKExtensions
    {
        internal static Matrix4 Pivoted(this Matrix4 rotation, Vector3 pivot)
        {
            var model = Matrix4.CreateTranslation(pivot);
            model *= rotation;
            model *= Matrix4.CreateTranslation(pivot).Inverted();
            return model;
        }

        public static Vector3 Abs(Vector3 value)
        {
            return new Vector3(Math.Abs(value.X), Math.Abs(value.Y), Math.Abs(value.Z));
        }
    }
}
