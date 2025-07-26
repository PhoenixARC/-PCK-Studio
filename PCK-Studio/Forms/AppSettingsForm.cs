using MetroFramework.Controls;
using PckStudio.Properties;
using PckStudio.ToolboxItems;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Windows.Forms;

namespace PckStudio.Forms
{
    public partial class AppSettingsForm : ThemeForm
    {
        private ApplicationSettingsBase _applicationSettings;

        public AppSettingsForm()
            : this(Settings.Default)
        {
        }

        public AppSettingsForm(ApplicationSettingsBase applicationSettings)
        {
            InitializeComponent();
            _applicationSettings = applicationSettings;
            LoadSettings();
        }

        private static Dictionary<string, string> CheckBoxText = new Dictionary<string, string>()
        {
            ["ShowRichPresence"] = "Show Rich Presence",
            ["AutoSaveChanges"] = "Auto Save",
            ["UseLittleEndianAsDefault"] = "Open as Little Endian",
            ["AutoUpdate"] = "Auto Update",
            ["LoadSubPcks"] = "Load Sub Pcks",
            ["UsePrerelease"] = "Use Prerelease",
        };

        private void CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.Tag is string settingsKey && _applicationSettings[settingsKey] is bool)
            {
                _applicationSettings[settingsKey] = checkBox.Checked;
            }
        }

        private void LoadSettings()
        {
            foreach (SettingsPropertyValue item in _applicationSettings.PropertyValues)
            {
                Debug.WriteLine($"{item.Property.Name}: {item.Property.PropertyType}");
                if (!item.Property.Attributes.ContainsKey(typeof(UserScopedSettingAttribute)) || item.Property.PropertyType != typeof(bool))
                    continue;
                var checkBox = new MetroCheckBox
                {
                    Name = item.Name,
                    Tag = item.Name,
                    Text = CheckBoxText.ContainsKey(item.Name) ? CheckBoxText[item.Name] : item.Name,
                    Checked = (bool)item.PropertyValue,

                    AutoSize = true,
                    Theme = MetroFramework.MetroThemeStyle.Dark,
                    Style = MetroFramework.MetroColorStyle.White,
                };
                checkBox.CheckedChanged += CheckBox_CheckedChanged;
                flowLayoutPanel.Controls.Add(checkBox);
            }
        }

        private void AppBehaviorSettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _applicationSettings.Save();
        }
    }
}