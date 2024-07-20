using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PckStudio.Extensions
{
    internal static class PictureBoxExtensions
    {
        public static bool IsAnimating(this PictureBox pictureBox)
        {
            var fi = typeof(PictureBox).GetField("currentlyAnimating", BindingFlags.NonPublic | BindingFlags.Instance);
            return (bool)fi.GetValue(pictureBox);
        }

        public static void Animate(this PictureBox pictureBox, bool animate)
        {
            var animateMethod = typeof(PictureBox).GetMethod("Animate", BindingFlags.NonPublic | BindingFlags.Instance,
            null, new Type[] { typeof(bool) }, null);
            animateMethod.Invoke(pictureBox, new object[] { animate });
        }

    }
}
