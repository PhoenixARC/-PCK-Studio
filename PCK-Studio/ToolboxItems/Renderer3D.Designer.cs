using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace PckStudio.ToolboxItems
{
    public partial class Renderer3D : UserControl
    {

        // UserControl overrides dispose to clean up the component list.
        [DebuggerNonUserCode()]
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && components is not null)
                {
                    components.Dispose();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        // Required by the Windows Form Designer
        private System.ComponentModel.IContainer components;

        // NOTE: The following procedure is required by the Windows Form Designer
        // It can be modified using the Windows Form Designer.  
        // Do not modify it using the code editor.
        [DebuggerStepThrough()]
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            GlControl = new OpenTK.GLControl();
            GlControl.Paint += new PaintEventHandler(GlControl_Paint);
            GlControl.MouseDown += new MouseEventHandler(GlControl_MouseDown);
            GlControl.MouseUp += new MouseEventHandler(GlControl_MouseUp);
            timMove = new Timer(components);
            timMove.Tick += new EventHandler(Move_Tick);
            timPaint = new Timer(components);
            timPaint.Tick += new EventHandler(Paint_Tick);
            SuspendLayout();
            // 
            // GlControl
            // 
            GlControl.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            GlControl.BackColor = Color.Black;
            GlControl.Location = new Point(0, 0);
            GlControl.Name = "GlControl";
            GlControl.Size = new Size(150, 150);
            GlControl.TabIndex = 0;
            GlControl.VSync = true;
            // 
            // timMove
            // 
            timMove.Enabled = true;
            timMove.Interval = 20;
            // 
            // timPaint
            // 
            timPaint.Enabled = true;
            timPaint.Interval = 1;
            // 
            // Renderer3D
            // 
            AutoScaleDimensions = new SizeF(6.0f, 13.0f);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(GlControl);
            Name = "Renderer3D";
            MouseWheel += new MouseEventHandler(Renderer3D_MouseWheel);
            ResumeLayout(false);

        }

        internal OpenTK.GLControl GlControl;
        internal Timer timMove;
        internal Timer timPaint;
    }
}