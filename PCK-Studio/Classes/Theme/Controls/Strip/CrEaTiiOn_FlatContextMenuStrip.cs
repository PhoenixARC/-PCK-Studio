#region Imports
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
#endregion

namespace CBH.Controls
{
    public class CrEaTiiOn_FlatContextMenuStrip : ContextMenuStrip
    {
        protected override void OnTextChanged( EventArgs e )
        {
            base.OnTextChanged( e );
            Invalidate();
        }

        public CrEaTiiOn_FlatContextMenuStrip()
            : base()
        {
            Renderer = new ToolStripProfessionalRenderer( new TColorTable() );
            ShowImageMargin = false;
            ForeColor = Color.White;
            Font = new Font( "Tahoma", 8 );
        }

        protected override void OnPaint( PaintEventArgs e )
        {
            base.OnPaint( e );
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        }

        public class TColorTable : ProfessionalColorTable
        {
            [Category( "Colors" )]
            public Color _BackColor { get; set; } = Color.FromArgb( 24, 22, 43 );

            [Category( "Colors" )]
            public Color _CheckedColor { get; set; } = Color.FromArgb(250, 36, 38);

            [Category( "Colors" )]
            public Color _BorderColor { get; set; } = Color.FromArgb( 53, 58, 60 );

            public override Color ButtonSelectedBorder => _BackColor;

            public override Color CheckBackground => _CheckedColor;

            public override Color CheckPressedBackground => _CheckedColor;

            public override Color CheckSelectedBackground => _CheckedColor;

            public override Color ImageMarginGradientBegin => _CheckedColor;

            public override Color ImageMarginGradientEnd => _CheckedColor;

            public override Color ImageMarginGradientMiddle => _CheckedColor;

            public override Color MenuBorder => _BorderColor;

            public override Color MenuItemBorder => _BorderColor;

            public override Color MenuItemSelected => _CheckedColor;

            public override Color SeparatorDark => _BorderColor;

            public override Color ToolStripDropDownBackground => _BackColor;
        }
    }
}