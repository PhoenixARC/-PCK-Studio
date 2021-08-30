using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PckStudio.Forms
{
    public partial class Promo : MetroFramework.Forms.MetroForm
    {
        string data;
        public Promo()
        {
            InitializeComponent();
        }

        private void Promo_Load(object sender, EventArgs e)
        {
            
            try
            {
                using (WebClient getData = new WebClient())
                {
                    data = getData.DownloadString(PckStudio.Program.baseurl + "Promo/PromoFC");
                    //data = "k_EPynYjxmc";

                    webBrowser1.ScrollBarsEnabled = false;

                    var embed = "<html><head>" +
                    "<meta http-equiv=\"X-UA-Compatible\" content=\"IE=Edge\"/>" +
                    "</head><body style=\"background-color: #000000;\">" +
                    "<iframe width=\"720\" height=\"439\" src=\"{0}\"" +
                    "frameborder = \"0\" allow = \"autoplay; encrypted-media\" allowfullscreen></iframe>" +
                    "</body></html>";
                    var url = "https://www.youtube.com/embed/" + data;
                    this.webBrowser1.DocumentText = string.Format(embed, url);
                }
            }
            catch
            {
                this.Close();
            }
        }

        private void buttonOpenInBrowser_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.youtube.com/watch?v=" + data  );
            //MessageBox.Show("https://www.youtube.com/watch?v=" + data);
        }
    }
}
