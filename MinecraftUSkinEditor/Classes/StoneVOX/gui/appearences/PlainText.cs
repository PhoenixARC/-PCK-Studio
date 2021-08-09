using OpenTK;
using QuickFont;
using System.Drawing;

namespace stonevox
{
    [GUIAppearenceName("Plain Text")]
    [GUIAppearenceDataType(typeof(PlainTextData))]
    public class PlainText : Appearence
    {
        public static SizeF MeasureString(string text)
        {
            SizeF _return = Client.window.Qfont.Measure(text);
            _return.Width *= 2f;
            _return.Height *= 2f;

            return _return;
        }

        public override bool Enabled
        {
            get
            {
                return base.Enabled;
            }

            set
            {
                base.Enabled = value;
            }
        }

        public bool AutoSize;
        public string AppendText = "";

        public float offsetX;
        public float offsetY;

        private string text = "";
        private Color color = Color.White;
        public QFontAlignment Alignment;
        public Color Color { get { return color; } set { color = value; } }
        public string Text { get { return text; } set { text = value; ResizeWidget(); } }

        public PlainText()
        {
        }
        
        public PlainText(bool autoSize, string text, Color color)
            :this(autoSize, text, color, QFontAlignment.Left)
        {
        }

        public PlainText(bool autoSize, string text, Color color, QFontAlignment alignment)
            :this(autoSize, text, color, alignment, 0, 0)
        {
        }

        public PlainText(bool autoSize, string text, Color color, QFontAlignment alignment, float offsetX, float offsetY)
        {
            this.AutoSize = autoSize;
            this.text = text;
            this.Color = color;
            this.Alignment = alignment;
            this.offsetX = offsetX;
            this.offsetY = offsetY;
        }

        public override void Initialize()
        {
        }

        public override void Render(float x, float y, float width, float height)
        {
            float xx = x.UnScaleHorizontal();
            float yy = Client.window.Height - Scale.vUnPosScale(y) - height.UnScaleVerticlSize()*.50f;

            QFont.Begin();
            Client.window.Qfont.Options.Colour = color;
            Client.window.Qfont.Print(text + AppendText, Alignment, new Vector2(xx+offsetX , yy+offsetY));
            QFont.End();
        }

        void ResizeWidget()
        {
            if (AutoSize)
                MatchWidgetToBounds(Owner);
        }

        public void MatchWidgetToBounds(Widget widget)
        {
            var size = Client.window.Qfont.Measure(text);

            widget.SetBounds(null, null, size.Width*2f , size.Height*2f);
        }

        public SizeF MesaureString()
        {
            return MeasureString(this.text);
        }

        public SizeF MesaureString(string text)
        {
            var size = Client.window.Qfont.Measure(text);
            size.Width *= 2f;
            size.Height *= 2f;
            return size;
        }

        public override Appearence FromData(AppearenceData data)
        {
            PlainTextData _data = data as PlainTextData;
            return new PlainText(_data.AutoSize, _data.Text, _data.Color, _data.Alignment, _data.OffsetX, _data.OffsetY);
        }
        public override AppearenceData ToData()
        {
            return new PlainTextData(AutoSize, text, color, Alignment, offsetX, offsetY);
        }

        public PlainText Clone()
        {
            return FromData(ToData()) as PlainText;
        }
    }
}
