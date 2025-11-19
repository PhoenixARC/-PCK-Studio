using System.Drawing;

namespace PckStudio.Core.Extensions
{
    public readonly struct BlendOption(Color color, BlendMode blendMode)
    {
        internal readonly Color Color = color;
        internal readonly BlendMode BlendMode = blendMode;

        public static BlendOption Add(Color color) => new BlendOption(color, BlendMode.Add);
        public static BlendOption Subtract(Color color) => new BlendOption(color, BlendMode.Subtract);
        public static BlendOption Multiply(Color color) => new BlendOption(color, BlendMode.Multiply);
        public static BlendOption Average(Color color) => new BlendOption(color, BlendMode.Average);
        public static BlendOption DescendingOrder(Color color) => new BlendOption(color, BlendMode.DescendingOrder);
        public static BlendOption AscendingOrder(Color color) => new BlendOption(color, BlendMode.AscendingOrder);
        public static BlendOption Screen(Color color) => new BlendOption(color, BlendMode.Screen);
        public static BlendOption Overlay(Color color) => new BlendOption(color, BlendMode.Overlay);
    }
}
