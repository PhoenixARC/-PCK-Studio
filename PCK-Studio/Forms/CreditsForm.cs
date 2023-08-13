using System;
using System.Windows.Forms;
using MetroFramework.Forms;
using PckStudio.Internal;

namespace PckStudio.Forms
{
    public partial class CreditsForm : MetroForm
    {
        public CreditsForm()
        {
            InitializeComponent();
#if BETA
            buildLabel.Text = $"Build Config: Beta\nBuild Version: {ApplicationBuildInfo.BetaBuildVersion}\n Branch: {CommitInfo.BranchName}";
#elif DEBUG
            buildLabel.Text = $"Build Config: Debug\nBranch: {CommitInfo.BranchName}\nCommit Id: {CommitInfo.CommitHash}";
#else
            buildLabel.Text = string.Empty;
#endif
        }
    }
}
