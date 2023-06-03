using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework.Forms;
using PckStudio.Properties;

namespace PckStudio.Forms.Utilities
{
    public partial class AppBehaviorSettingsForm : MetroForm
    {
        public AppBehaviorSettingsForm()
        {
            InitializeComponent();
            LoadSettings();
        }

        private void autoSaveCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.AutoSaveChanges = autoSaveCheckBox.Checked;
        }

        private void endianCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.UseLittleEndianAsDefault = endianCheckBox.Checked;
        }

        private void autoLoadPckCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.LoadSubPcks = autoLoadPckCheckBox.Checked;
        }

        private void LoadSettings()
        {
            autoSaveCheckBox.Checked = Settings.Default.AutoSaveChanges;
            endianCheckBox.Checked = Settings.Default.UseLittleEndianAsDefault;
            autoLoadPckCheckBox.Checked = Settings.Default.LoadSubPcks;
        }

        private void AppBehaviorSettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.Save();
        }
    }
}
