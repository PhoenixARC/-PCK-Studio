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
    }
}
