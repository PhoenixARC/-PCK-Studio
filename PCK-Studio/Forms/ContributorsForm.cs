using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework.Forms;
using PckStudio.Internal;
using PckStudio.ToolboxItems;

namespace PckStudio.Forms
{
    public partial class ContributorsForm : MetroForm
    {
        public ContributorsForm()
        {
            InitializeComponent();
#if false
            Task.Run(GetContributors);
#endif
            string buildConfig = "";
#if BETA
            buildConfig = "Beta";
#elif DEBUG
            buildConfig = "Debug";
#elif RELEASE
            buildConfig = "Release";
#else
            buildConfig = "unknown";
#endif
            buildLabel.Text = $"Verion: {Application.ProductVersion}\nBuild Config: {buildConfig}\nBranch: {CommitInfo.BranchName}@{CommitInfo.CommitHash}";
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            foreach (var contributorsName in ApplicationScope.Contributors)
            {
                if (InvokeRequired)
                    Invoke(() => contributorsLayoutPanel.Controls.Add(new GithubUserPanel(contributorsName)));
                else
                    contributorsLayoutPanel.Controls.Add(new GithubUserPanel(contributorsName));
            }
        }
    }
}
