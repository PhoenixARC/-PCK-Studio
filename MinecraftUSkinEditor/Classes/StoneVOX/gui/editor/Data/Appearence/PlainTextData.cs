namespace stonevox
{
    public class PlainTextData : AppearenceData
    {
        public string Text { get; set; }
        public ColorData Color { get; set; }
        public bool AutoSize { get; set; }
        public float OffsetX { get; set; }
        public float OffsetY { get; set; }
        public QuickFont.QFontAlignment Alignment { get; set; }

        public PlainTextData()
        {
            Text = "";
            Color = new ColorData(System.Drawing.Color.White);
        }

        public PlainTextData(bool autosize, string text, ColorData color, QuickFont.QFontAlignment alignment, float offsetX, float offsetY)
        {
            Text = text;
            AutoSize = autosize;
            Color = color;
            Alignment = alignment;
            OffsetX = offsetX;
            OffsetY = offsetY;
        }

        public override Appearence ToAppearence()
        {
            return new PlainText(AutoSize, Text, Color);
        }
    }
}
