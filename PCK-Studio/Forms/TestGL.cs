using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PckStudio.Properties;

namespace PckStudio.Forms
{
    public partial class TestGL : Form
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.Run(new TestGL());
        }

        public TestGL()
        {
            InitializeComponent();
        }

        private void TestGL_Load(object sender, EventArgs e)
        {
            renderer3D1.Refresh();
        }
    }
}
