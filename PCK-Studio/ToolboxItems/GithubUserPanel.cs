using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Octokit;
using MetroFramework.Controls;
using System.Drawing;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using PckStudio.Internal;
using System.Drawing.Imaging;

namespace PckStudio.ToolboxItems
{
    public partial class GithubUserPanel : MetroUserControl
    {
        private Author _contributor;

        public GithubUserPanel()
        {
            InitializeComponent();
        }

        public GithubUserPanel(Author contributor) : this()
        {
            _contributor = contributor;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (DesignMode)
                return;

            Visible = false;
            Task.Run(LoadAuthor);
        }

        private void LoadAuthor()
        {
            // TODO: find a better way to check if the avatar has changed since last cache.
            string cacheKey = Convert.ToBase64String(Encoding.Default.GetBytes(_contributor.AvatarUrl));

            if (!ApplicationScope.DataCacher.HasFileCached(cacheKey))
            {
                using (WebClient webClient = new WebClient())
                {
                    Stream avatarImgStream = webClient.OpenRead(_contributor.AvatarUrl);
                    MemoryStream ms = new MemoryStream();
                    new Bitmap(avatarImgStream).Save(ms, ImageFormat.Png);
                    avatarImgStream.Flush();
                    avatarImgStream.Dispose();
                    ApplicationScope.DataCacher.Cache(ms.ToArray(), cacheKey);
                }
            }

            Image avatarUserImg = Image.FromFile(ApplicationScope.DataCacher.GetCachedFilepath(cacheKey));

            Action setUiElements = () =>
            {
                userPictureBox.Image = avatarUserImg;
                userNameLabel.Text = _contributor.Login;
                aboutButton.Text = "Github profile";
                aboutButton.Click += (s, e) => Process.Start(_contributor.HtmlUrl);
                Visible = true;
            };

            if (InvokeRequired)
            {
                Invoke(setUiElements);
                return;
            }
            setUiElements();
        }
    }
}
