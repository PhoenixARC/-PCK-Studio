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
            buildLabel.Text = $"[Beta] {ApplicationBuildInfo.BetaBuildVersion}@{CommitInfo.BranchName}";
#elif DEBUG
            buildLabel.Text = $"[Debug] {CommitInfo.BranchName}@{CommitInfo.CommitHash}";
#else
            buildLabel.Text = string.Empty;
#endif
        }
    }
}
