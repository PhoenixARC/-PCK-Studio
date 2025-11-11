using System;
using System.Reflection;
using System.Windows.Forms;

namespace PckStudio.Core.Extensions
{
    public static class PictureBoxExtensions
    {
        public static bool IsAnimating(this PictureBox pictureBox)
        {
            FieldInfo fi = typeof(PictureBox).GetField("currentlyAnimating", BindingFlags.NonPublic | BindingFlags.Instance);
            return (bool)fi.GetValue(pictureBox);
        }

        public static void Animate(this PictureBox pictureBox, bool animate)
        {
            MethodInfo animateMethod = typeof(PictureBox).GetMethod("Animate", BindingFlags.NonPublic | BindingFlags.Instance,
            null, new Type[] { typeof(bool) }, null);
            animateMethod.Invoke(pictureBox, new object[] { animate });
        }

    }
}
