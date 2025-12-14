using System;
using System.Globalization;

namespace PckStudio.Core.Skin
{
    public sealed class SkinIdentifier : IFormattable
    {
        private readonly int _id;

        public SkinIdentifier(int id)
        {
            _id = id;
        }

        public static implicit operator int(SkinIdentifier @this) => @this._id;

        public string ToString(string format, IFormatProvider formatProvider) => _id.ToString(format, formatProvider);

        public string ToString(string format) => _id.ToString(format, NumberFormatInfo.CurrentInfo);

        public override string ToString() => _id.ToString(NumberFormatInfo.CurrentInfo);
    }
}
