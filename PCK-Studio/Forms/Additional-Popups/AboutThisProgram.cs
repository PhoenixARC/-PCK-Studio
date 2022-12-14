﻿using System;
using System.Windows.Forms;
using MetroFramework.Forms;

namespace PckStudio
{
    public partial class AboutThisProgram : Form
    {
        int count = 0;
        public AboutThisProgram()
        {
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (++count == 5)
            {
                MessageBox.Show("🌸Miku🌸 was here!");
                count = 0;
            }
        }

        private void GitHubPageButton(object sender, EventArgs e)
        {
            //Launch browser to GitHub...
            System.Diagnostics.Process.Start("https://github.com/PhoenixARC/-PCK-Studio");
        }

        private void WebsiteButton_Click(object sender, EventArgs e)
        {
            //Launch browser to the PCK Studio website...
            System.Diagnostics.Process.Start("https://pckstudio.xyz/");
        }

        private void DiscordDevelopmentServerButton(object sender, EventArgs e)
        {
            //Launch browser to the Discord Server...
            System.Diagnostics.Process.Start("https://discord.gg/dAepk3Bhud");
        }
    }
}
