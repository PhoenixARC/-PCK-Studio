using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace PckStudio.Extensions
{
    internal static class OpenTkMatrixExtensions
    {
        internal static Matrix4 Pivoted(this Matrix4 target, Vector3 translation, Vector3 pivot)
        {
            var model = Matrix4.CreateTranslation(translation);
            model *= Matrix4.CreateTranslation(pivot);
            model *= target;
            model *= Matrix4.CreateTranslation(pivot).Inverted();
            return model;
        }
    }
}
