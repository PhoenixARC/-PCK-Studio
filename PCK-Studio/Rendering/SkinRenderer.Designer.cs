using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PckStudio.Rendering
{
    internal partial class SkinRenderer
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
            animationTimer = new System.Windows.Forms.Timer(components);
            moveTimer.Tick += new EventHandler(moveTimer_Tick);
            animationTimer.Tick += new EventHandler(animationTimer_Tick);
            SuspendLayout();
            // 
            // moveTimer
            // 
            moveTimer.Enabled = true;
            moveTimer.Interval = 10;
            // 
            // animationTimer
            // 
            animationTimer.Enabled = false;
            animationTimer.Interval = 50;
            Name = "Renderer3D";
            ResumeLayout(false);
        }

        private System.Windows.Forms.Timer moveTimer;
        private System.Windows.Forms.Timer animationTimer;
    }
}
