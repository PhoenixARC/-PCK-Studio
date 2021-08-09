namespace stonevox
{
    public class TextBoxData : WidgetData
    {
		public string Text { get; set; }
        public ColorData Color { get; set; }
        public int CharDisplaySize { get; set; }

        public TextBoxData() { }

        public TextBoxData(string text, ColorData color, int charDisplaySize)
        {
            Text = text;
            Color = color;
            CharDisplaySize = charDisplaySize;
        }

    }

    // dont need this much datat erere
}
