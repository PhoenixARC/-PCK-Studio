using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Windows.Forms;

namespace MechanikaDesign.WinForms.UI.ColorPicker
{
    [DefaultEvent("ColorChanged")]
    public partial class ColorHexagon: UserControl
    {
        # region Fields
        private const float COEFFCIENT = 0.824f;
        private ColorHexagonElement[] hexagonElements = new ColorHexagonElement[0x93];
        private float[] matrix1 = new float[] { -0.5f, -1f, -0.5f, 0.5f, 1f, 0.5f };
        private float[] matrix2 = new float[] { 0.824f, 0f, -0.824f, -0.824f, 0f, 0.824f };
        private int oldSelectedHexagonIndex = -1;
        private int sectorMaximum = 7;
        private int selectedHexagonIndex = -1;
        #endregion

        #region Events
        public delegate void ColorChangedEventHandler(object sender, ColorChangedEventArgs args);
        public event ColorChangedEventHandler ColorChanged; 

        #endregion

        #region Properties
        public Color SelectedColor
        {
            get
            {
                if (this.selectedHexagonIndex < 0)
                {
                    return Color.Empty;
                }
                return this.hexagonElements[this.selectedHexagonIndex].CurrentColor;
            }
        }
        #endregion

        #region Constructors
        public ColorHexagon()
        {
            base.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            base.SetStyle(ControlStyles.UserPaint, true);
            base.SetStyle(ControlStyles.Opaque, true);
            base.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            base.SetStyle(ControlStyles.ResizeRedraw, true);
            base.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            for (int i = 0; i < this.hexagonElements.Length; i++)
            {
                this.hexagonElements[i] = new ColorHexagonElement();
            }

            InitializeComponent();
        }
        #endregion

        #region Methods/Functions
        private void DrawHexagonHighlighter(int selectedHexagonIndex)
        {
            if (selectedHexagonIndex != this.oldSelectedHexagonIndex)
            {
                if (this.oldSelectedHexagonIndex >= 0)
                {
                    this.hexagonElements[this.oldSelectedHexagonIndex].IsHovered = false;
                    base.Invalidate(this.hexagonElements[this.oldSelectedHexagonIndex].BoundingRectangle);
                }
                this.oldSelectedHexagonIndex = selectedHexagonIndex;
                if (this.oldSelectedHexagonIndex >= 0)
                {
                    this.hexagonElements[this.oldSelectedHexagonIndex].IsHovered = true;
                    base.Invalidate(this.hexagonElements[this.oldSelectedHexagonIndex].BoundingRectangle);
                }
            }
        }

        private int GetHexagonIndexFromCoordinates(int xCoordinate, int yCoordinate)
        {
            for (int i = 0; i < this.hexagonElements.Length; i++)
            {
                if (this.hexagonElements[i].BoundingRectangle.Contains(xCoordinate, yCoordinate))
                {
                    return i;
                }
            }
            return -1;
        }

        private int GetHexgaonWidth(int availableHeight)
        {
            int num = availableHeight / (2 * this.sectorMaximum);
            if ((((int) Math.Floor((double) (((double) num) / 2.0))) * 2) < num)
            {
                num--;
            }
            return num;
        }

        private void InitializeGrayscaleHexagons(ref Rectangle clientRectangle, int hexagonWidth, 
                                                 ref int centerOfMiddleHexagonX, ref int centerOfMiddleHexagonY,
                                                 ref int index)
        {
            int red = 0xff;
            int num4 = 0x11;
            int num3 = 0x10;
            int num5 = (((clientRectangle.Width - (7 * hexagonWidth)) / 2) + clientRectangle.X) - (hexagonWidth / 3);
            
            centerOfMiddleHexagonX = num5;
            centerOfMiddleHexagonY = clientRectangle.Bottom;
            for (int i = 0; i < num3; i++)
            {
                this.hexagonElements[index].CurrentColor = Color.FromArgb(red, red, red);
                this.hexagonElements[index].SetHexagonPoints((float)centerOfMiddleHexagonX, (float)centerOfMiddleHexagonY, hexagonWidth);
                centerOfMiddleHexagonX += hexagonWidth;
                index++;
                if (i == 7)
                {
                    centerOfMiddleHexagonX = num5 + (hexagonWidth / 2);
                    centerOfMiddleHexagonY += (int)(hexagonWidth * 0.824f);
                }
                red -= num4;
            }
        }

        private void InitializeHexagons()
        {
            Rectangle clientRectangle = base.ClientRectangle;
            clientRectangle.Offset(0, -8);
            if (clientRectangle.Height < clientRectangle.Width)
            {
                clientRectangle.Inflate(-(clientRectangle.Width - clientRectangle.Height) / 2, 0);
            }
            else
            {
                clientRectangle.Inflate(0, -(clientRectangle.Height - clientRectangle.Width) / 2);
            }

            int hexagonWidth = this.GetHexgaonWidth(Math.Min(clientRectangle.Height, clientRectangle.Width));
            int centerOfMiddleHexagonX = (clientRectangle.Left + clientRectangle.Right) / 2;
            int centerOfMiddleHexagonY = (clientRectangle.Top + clientRectangle.Bottom) / 2;
         
            centerOfMiddleHexagonY -= hexagonWidth;
            this.hexagonElements[0].CurrentColor = Color.White;
            this.hexagonElements[0].SetHexagonPoints((float)centerOfMiddleHexagonX, (float)centerOfMiddleHexagonY, hexagonWidth);
            int index = 1;
            for (int i = 1; i < this.sectorMaximum; i++)
            {
                float yCoordinate = centerOfMiddleHexagonY;
                float xCoordinate = centerOfMiddleHexagonX + (hexagonWidth * i);
                for (int j = 0; j < (this.sectorMaximum - 1); j++)
                {
                    int num9 = (int)(hexagonWidth * this.matrix2[j]);
                    int num10 = (int)(hexagonWidth * this.matrix1[j]);
                    for (int k = 0; k < i; k++)
                    {
                        double num12 = ((0.936 * (this.sectorMaximum - i)) / ((double)this.sectorMaximum)) + 0.12;
                        float colorQuotient = GetColorQuotient(xCoordinate - centerOfMiddleHexagonX, yCoordinate - centerOfMiddleHexagonY);
                        this.hexagonElements[index].SetHexagonPoints(xCoordinate, yCoordinate, hexagonWidth);
                        this.hexagonElements[index].CurrentColor = ColorFromRGBRatios((double)colorQuotient, num12, 1.0);
                        yCoordinate += num9;
                        xCoordinate += num10;
                        index++;
                    }
                }
            }
            clientRectangle.Y -= hexagonWidth + (hexagonWidth / 2);
            this.InitializeGrayscaleHexagons(ref clientRectangle, hexagonWidth, ref centerOfMiddleHexagonX, ref centerOfMiddleHexagonY, ref index);
        }

        #endregion

        #region Overridden Methods

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (this.selectedHexagonIndex >= 0)
                {
                    this.hexagonElements[this.selectedHexagonIndex].IsSelected = false;
                    base.Invalidate(this.hexagonElements[this.selectedHexagonIndex].BoundingRectangle);
                }
                this.selectedHexagonIndex = -1;
                if (this.oldSelectedHexagonIndex >= 0)
                {
                    this.selectedHexagonIndex = this.oldSelectedHexagonIndex;
                    this.hexagonElements[this.selectedHexagonIndex].IsSelected = true;
                    if (this.ColorChanged != null)
                    {
                        this.ColorChanged(this, new ColorChangedEventArgs(this.SelectedColor));
                    }
                    base.Invalidate(this.hexagonElements[this.selectedHexagonIndex].BoundingRectangle);
                }
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            this.DrawHexagonHighlighter(-1);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            this.DrawHexagonHighlighter(this.GetHexagonIndexFromCoordinates(e.X, e.Y));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (this.BackColor == Color.Transparent)
            {
                base.OnPaintBackground(e);
            }

            Graphics g = e.Graphics;
            using (SolidBrush brush = new SolidBrush(this.BackColor))
            {
                g.FillRectangle(brush, base.ClientRectangle);
            }

            g.SmoothingMode = SmoothingMode.AntiAlias;
            foreach (ColorHexagonElement element in this.hexagonElements)
            {
                element.Paint(g);
            }
           
            if (this.oldSelectedHexagonIndex >= 0)
            {
                this.hexagonElements[this.oldSelectedHexagonIndex].Paint(g);
            }
            if (this.selectedHexagonIndex >= 0)
            {
                this.hexagonElements[this.selectedHexagonIndex].Paint(g);
            }
            base.OnPaint(e);
        }

        protected override void OnResize(EventArgs e)
        {
            this.InitializeHexagons();
            base.OnResize(e);
        }

        #endregion

        #region Color Helper Functions
        private static float GetColorQuotient(float value1, float value2)
        {
            return (float)((Math.Atan2((double)value2, (double)value1) * 180.0) / 3.1415926535897931);
        }

        private static int GetColorChannelValue(float value1, float value2, float value3)
        {
            if (value3 > 360f)
            {
                value3 -= 360f;
            }
            else if (value3 < 0f)
            {
                value3 += 360f;
            }
            if (value3 < 60f)
            {
                value1 += ((value2 - value1) * value3) / 60f;
            }
            else if (value3 < 180f)
            {
                value1 = value2;
            }
            else if (value3 < 240f)
            {
                value1 += ((value2 - value1) * (240f - value3)) / 60f;
            }
            return (int)(value1 * 255f);
        }

        private static Color ColorFromRGBRatios(double value1, double value2, double value3)
        {
            int num;
            int num2;
            int num3;
            if (value3 == 0.0)
            {
                num = num2 = num3 = (int)(value2 * 255.0);
            }
            else
            {
                float num4;
                if (value2 <= 0.5)
                {
                    num4 = (float)(value2 + (value2 * value3));
                }
                else
                {
                    num4 = (float)((value2 + value3) - (value2 * value3));
                }
                float num5 = ((float)(2.0 * value2)) - num4;
                num = GetColorChannelValue(num5, num4, (float)(value1 + 120.0));
                num2 = GetColorChannelValue(num5, num4, (float)value1);
                num3 = GetColorChannelValue(num5, num4, (float)(value1 - 120.0));
            }
            return Color.FromArgb(num, num2, num3);
        }
        #endregion

    }

    #region HexagaonElement Class
    internal class ColorHexagonElement
    {
        #region Fields

        private Rectangle boundingRect = Rectangle.Empty;
        private Color hexagonColor = Color.Empty;
        private Point[] hexagonPoints = new Point[6];
        private bool isHovered;
        private bool isSelected;

        #endregion

        #region Methods

        public void Paint(Graphics g)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddPolygon(this.hexagonPoints);
            path.CloseAllFigures();
            using (SolidBrush brush = new SolidBrush(this.hexagonColor))
            {
                g.FillPath(brush, path);
            }
            if (this.isHovered || this.isSelected)
            {
                SmoothingMode smoothingMode = g.SmoothingMode;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using (Pen pen = new Pen(Color.FromArgb(0x2a, 0x5b, 150), 2f))
                {
                    g.DrawPath(pen, path);
                }
                using (Pen pen2 = new Pen(Color.FromArgb(150, 0xb1, 0xef), 1f))
                {
                    g.DrawPath(pen2, path);
                }
                g.SmoothingMode = smoothingMode;
            }
            path.Dispose();
        }

        public void SetHexagonPoints(float xCoordinate, float yCoordinate, int hexagonWidth)
        {
            float num = hexagonWidth * 0.5773503f;
            this.hexagonPoints[0] = new Point((int)Math.Floor((double)(xCoordinate - (hexagonWidth / 2))), ((int)Math.Floor((double)(yCoordinate - (num / 2f)))) - 1);
            this.hexagonPoints[1] = new Point((int)Math.Floor((double)xCoordinate), ((int)Math.Floor((double)(yCoordinate - (hexagonWidth / 2)))) - 1);
            this.hexagonPoints[2] = new Point((int)Math.Floor((double)(xCoordinate + (hexagonWidth / 2))), ((int)Math.Floor((double)(yCoordinate - (num / 2f)))) - 1);
            this.hexagonPoints[3] = new Point((int)Math.Floor((double)(xCoordinate + (hexagonWidth / 2))), ((int)Math.Floor((double)(yCoordinate + (num / 2f)))) + 1);
            this.hexagonPoints[4] = new Point((int)Math.Floor((double)xCoordinate), ((int)Math.Floor((double)(yCoordinate + (hexagonWidth / 2)))) + 1);
            this.hexagonPoints[5] = new Point((int)Math.Floor((double)(xCoordinate - (hexagonWidth / 2))), ((int)Math.Floor((double)(yCoordinate + (num / 2f)))) + 1);
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddPolygon(this.hexagonPoints);
                this.boundingRect = Rectangle.Round(path.GetBounds());
                this.boundingRect.Inflate(2, 2);
            }
        }

        #endregion

        #region Properties

        public Rectangle BoundingRectangle
        {
            get { return this.boundingRect; }
        }

        public Color CurrentColor
        {
            get { return this.hexagonColor; }
            set { this.hexagonColor = value; }
        }

        public bool IsHovered
        {
            get { return this.isHovered; }
            set { this.isHovered = value; }
        }

        public bool IsSelected
        {
            get { return this.isSelected; }
            set { this.isSelected = value; }
        }

        #endregion
    }
    #endregion
}
