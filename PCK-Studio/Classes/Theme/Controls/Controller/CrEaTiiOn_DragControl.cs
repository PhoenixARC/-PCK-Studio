#region Imports
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;
#endregion

namespace CBH.Controls
{
    internal class CrEaTiiOn_DragControl : Component
    {
        private Control controler;

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr o, int msg, int wParams, int IParam);

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        public Control TargetControl
        {
            get => this.controler;
            set
            {
                this.controler = value;
                this.controler.MouseDown += new MouseEventHandler(this.Controler_MouseDown);
            }
        }

        private void Controler_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;
            CrEaTiiOn_DragControl.ReleaseCapture();
            CrEaTiiOn_DragControl.SendMessage(this.TargetControl.FindForm().Handle, 161, 2, 0);
        }
    }
}
