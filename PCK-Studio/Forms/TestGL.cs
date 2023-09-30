using PckStudio.ToolboxItems;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PckStudio.ToolboxItems;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace PckStudio.Forms
{
    public partial class TestGL : Form
    {
        public TestGL()
        {
            InitializeComponent();
        }

        internal Bitmap Skin = Properties.Resources.steve;
        internal void UpdateImage()
        {
            var Image = new Bitmap(Skin.Width, Skin.Height); // Create the skin preview bitmao
            if (!(Skin.Width == 64 && Skin.Height == 64)) // Check the skin resolution
            {
                MessageBox.Show("Skin isn't valid.", "Error");
                return;
            }
            // ****************Writing pixels to the preview****************
            for (byte Y = 0, loopTo = (byte)(Skin.Height - 1); Y <= loopTo; Y++)
            {
                for (byte X = 0, loopTo1 = (byte)(Skin.Width - 1); X <= loopTo1; X++)
                    Image.SetPixel(X, Y, Skin.GetPixel(X / 2, Y / 2));
            }
            // *************************************************************
            renderer3D1.Skin = Skin;
            renderer3D1.Refresh(); // Render
        }
        private void TestGL_Load(object sender, EventArgs e)
        {
            renderer3D1.InDesignMode = false;
            UpdateImage(); // Load preview
        }
    }
}
