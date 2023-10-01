using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace PckStudio.Rendering
{
    public partial class Renderer3D
    {
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
            timMove = new Timer(components);
            timMove.Tick += new EventHandler(Move_Tick);
            SuspendLayout();
            // 
            // timMove
            // 
            timMove.Enabled = true;
            timMove.Interval = 20;
#if DEBUG
            // 
            // debugLabel
            // 
            debugLabel = new System.Windows.Forms.Label();
            debugLabel.Enabled = true;
            debugLabel.Visible = true;
            debugLabel.AutoSize = true;
            debugLabel.Location = new Point(0, 0);
            debugLabel.BackColor = Color.Transparent;
            Controls.Add(debugLabel);
#endif
            // 
            // Renderer3D
            // 
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            BackColor = Color.Transparent;
            Location = new Point(0, 0);
            Size = new Size(150, 150);
            TabIndex = 0;
            VSync = true;
            AutoScaleDimensions = new SizeF(6.0f, 13.0f);
            AutoScaleMode = AutoScaleMode.Font;
            Name = "Renderer3D";
            ResumeLayout(false);

        }

        private Timer timMove;
#if DEBUG
        private System.Windows.Forms.Label debugLabel;
#endif
        
    }
}