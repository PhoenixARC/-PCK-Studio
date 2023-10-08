using System;
using System.Diagnostics;

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
            moveTimer = new System.Windows.Forms.Timer(components);
            moveTimer.Tick += new EventHandler(Move_Tick);
            SuspendLayout();
            // 
            // timMove
            // 
            moveTimer.Enabled = true;
            moveTimer.Interval = 20;
#if DEBUG
            // 
            // debugLabel
            // 
            debugLabel = new System.Windows.Forms.Label();
            debugLabel.Enabled = true;
            debugLabel.Visible = false;
            debugLabel.AutoSize = true;
            debugLabel.Location = new System.Drawing.Point(0, 0);
            debugLabel.BackColor = System.Drawing.Color.Transparent;
            Controls.Add(debugLabel);;
#endif
            // 
            // Renderer3D
            // 
            Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            BackColor = System.Drawing.Color.LightGray;
            Location = new System.Drawing.Point(0, 0);
            Size = new System.Drawing.Size(150, 150);
            TabIndex = 0;
            VSync = true;
            AutoScaleDimensions = new System.Drawing.SizeF(6.0f, 13.0f);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Name = "Renderer3D";
            ResumeLayout(false);

        }

        private System.Windows.Forms.Timer moveTimer;
#if DEBUG
        private System.Windows.Forms.Label debugLabel;
#endif
        
    }
}