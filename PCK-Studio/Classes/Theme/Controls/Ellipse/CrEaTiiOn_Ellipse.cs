#region Imports
using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
#endregion

namespace CBH.Controls
{
    public class CrEaTiiOn_Ellipse : Component
    {
        private Control control;
        private int cornerRadius = 6;

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateRoundRectRgn(
          int nL,
          int nT,
          int nR,
          int nB,
          int nWidthEllipse,
          int nHeightEllipse);

        public Control TargetControl
        {
            get => this.control;
            set
            {
                this.control = value;
                this.control.SizeChanged += (EventHandler)((sender, eventArgs) => this.control.Region = Region.FromHrgn(CrEaTiiOn_Ellipse.CreateRoundRectRgn(0, 0, this.control.Width, this.control.Height, this.cornerRadius, this.cornerRadius)));
            }
        }

        public int CornerRadius
        {
            get => this.cornerRadius;
            set
            {
                this.cornerRadius = value;
                if (this.control == null)
                    return;
                this.control.Region = Region.FromHrgn(CrEaTiiOn_Ellipse.CreateRoundRectRgn(0, 0, this.control.Width, this.control.Height, this.cornerRadius, this.cornerRadius));
            }
        }
    }
}
