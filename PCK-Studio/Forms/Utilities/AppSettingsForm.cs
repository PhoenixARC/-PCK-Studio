using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CBH.Controls;
using MetroFramework.Forms;
using PckStudio.Properties;
using PckStudio.ToolboxItems;

namespace PckStudio.Forms.Utilities
{
    public partial class AppSettingsForm : ThemeForm
    {
        public AppSettingsForm()
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

        private void showPresenceCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.ShowRichPresence = showPresenceCheckBox.Checked;
        }

        private void LoadCheckboxState(CrEaTiiOn_CustomCheckBox checkBox, EventHandler eventHandler, bool state)
        {
            checkBox.CheckedStateChanged -= eventHandler;
            checkBox.Checked = state;
            checkBox.CheckedStateChanged += eventHandler;
        }

        private void LoadSettings()
        {
            LoadCheckboxState(autoSaveCheckBox, autoSaveCheckBox_CheckedChanged, Settings.Default.AutoSaveChanges);
            LoadCheckboxState(endianCheckBox, endianCheckBox_CheckedChanged, Settings.Default.UseLittleEndianAsDefault);
            LoadCheckboxState(autoLoadPckCheckBox, autoLoadPckCheckBox_CheckedChanged, Settings.Default.LoadSubPcks);
            LoadCheckboxState(showPresenceCheckBox, showPresenceCheckBox_CheckedChanged, Settings.Default.ShowRichPresence);
        }

        private void AppBehaviorSettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.Save();
        }
    }
}
