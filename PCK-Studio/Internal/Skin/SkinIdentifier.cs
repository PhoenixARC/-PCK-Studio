using System;
using System.Globalization;

namespace PckStudio.Internal.Skin
{
    public sealed class SkinIdentifier : IFormattable
    {
        public int Id { get; }

        public SkinIdentifier(int id)
        {
            Id = id;
        }

        public static implicit operator int(SkinIdentifier _this) => _this.Id;

        public string ToString(string format, IFormatProvider formatProvider) => Id.ToString(format, formatProvider);

        public string ToString(string format) => Id.ToString(format, NumberFormatInfo.CurrentInfo);

        public override string ToString() => Id.ToString(NumberFormatInfo.CurrentInfo);
    }
}
