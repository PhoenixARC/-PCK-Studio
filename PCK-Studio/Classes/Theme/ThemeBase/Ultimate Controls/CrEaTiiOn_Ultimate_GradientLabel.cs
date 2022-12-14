using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CBH.Ultimate.Controls
{
    public class CrEaTiiOn_Ultimate_GradientLabel: Label
    {

        private Color _FirstColor = Color.FromArgb(250, 36, 38);
        private Color _SecondColor = Color.FromArgb(75, 75, 75);

        [Category("CrEaTiiOn")]
        public string String { get { return Text; } set { Text = value; } }
        [Category("CrEaTiiOn")]
        public override Font Font { get => base.Font; set { base.Font = value; base.Font = value; } }

        [Category("CrEaTiiOn")]
        public Color FirstColor { get => _FirstColor; set { _FirstColor = value; Invalidate(); } }

        [Category("CrEaTiiOn")]
        public Color SecondColor { get => _SecondColor; set { _SecondColor = value; Invalidate(); } }

        public CrEaTiiOn_Ultimate_GradientLabel()
        {

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, Width, Height + 5), _FirstColor, _SecondColor, LinearGradientMode.Horizontal);
            e.Graphics.DrawString(String, Font, brush, 0, 0);
        }

    }
}
