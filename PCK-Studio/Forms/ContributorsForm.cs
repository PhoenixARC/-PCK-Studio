using System;
using PckStudio.Controls;
using PckStudio.Internal.App;
using PckStudio.ToolboxItems;

namespace PckStudio.Forms
{
    public partial class ContributorsForm : ImmersiveForm
    {
        public ContributorsForm()
        {
            InitializeComponent();
            string buildConfig = "unknown";
#if BETA
            buildConfig = "Beta";
#elif DEBUG
            buildConfig = "Debug";
#elif RELEASE
            buildConfig = "Release";
#endif
            buildLabel.Text = $"Verion: {ApplicationScope.CurrentVersion}\nBuild Config: {buildConfig}\nBranch: {CommitInfo.BranchName}@{CommitInfo.CommitHash}";
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            foreach (Octokit.RepositoryContributor contributorsName in ApplicationScope.Contributors)
            {
                if (InvokeRequired)
                    Invoke(() => contributorsLayoutPanel.Controls.Add(new GithubUserPanel(contributorsName)));
                else
                    contributorsLayoutPanel.Controls.Add(new GithubUserPanel(contributorsName));
            }
        }
    }
}
