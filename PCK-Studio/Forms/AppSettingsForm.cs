using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Windows.Forms;
using MetroFramework.Controls;
using MetroFramework.Forms;
using PckStudio.Internal.App;
using PckStudio.Properties;

namespace PckStudio.Forms
{
    public partial class AppSettingsForm : MetroForm
    {
        private ApplicationSettingsBase _applicationSettings;
        internal const string keyToStringContextKey = "keyToString";

        public AppSettingsForm()
            : this("Application Settings", Settings.Default, new Dictionary<string, string>()
            {
                ["ShowRichPresence"] = "Show Rich Presence",
                ["AutoSaveChanges"] = "Auto Save",
                ["UseLittleEndianAsDefault"] = "Open as Little Endian",
                ["AutoUpdate"] = "Auto Update",
                ["LoadSubPcks"] = "Load Sub Pcks",
                ["UsePrerelease"] = "Use Prerelease",
            })
        {
        }

        public AppSettingsForm(string title, ApplicationSettingsBase applicationSettings, Dictionary<string, string> keyToStringMap = null)
        {
            InitializeComponent();
            Text = title;
            _applicationSettings = applicationSettings;
            if (keyToStringMap is not null)
                _applicationSettings.Context.Add(SettingsManager.KeyToStringContextKeyConst, keyToStringMap);
            LoadSettings();
        }

        static bool ContextTryGetKeyToString(SettingsContext context, string key, out string value)
        {
            value = default;
            return
                context.ContainsKey(SettingsManager.KeyToStringContextKeyConst) &&
                context[SettingsManager.KeyToStringContextKeyConst] is Dictionary<string, string> keyToString &&
                keyToString.TryGetValue(key, out value);
        }

        static Control CreateCheckBox(SettingsPropertyValue propertyValue, SettingsBase settings)
        {
            var control = new MetroCheckBox
            {
                Name = propertyValue.Name,
                Tag = propertyValue.Name,
                Text = ContextTryGetKeyToString(settings.Context, propertyValue.Name, out string name) ? name : propertyValue.Name,
                Checked = (bool)propertyValue.PropertyValue,

                AutoSize = true,
                Theme = MetroFramework.MetroThemeStyle.Dark,
                Style = MetroFramework.MetroColorStyle.White,
            };

            void CheckBox_CheckedChanged(object sender, EventArgs e)
            {
                if (sender is CheckBox checkBox && checkBox.Tag is string settingsKey && settings[settingsKey] is bool)
                {
                    settings[settingsKey] = checkBox.Checked;
                }
            }

            control.CheckedChanged += CheckBox_CheckedChanged;
            return control;
        }

        delegate Control ControlCreateDelegate(SettingsPropertyValue propertyValue, SettingsBase settings);

        Dictionary<Type, ControlCreateDelegate> _typeToControl = new Dictionary<Type, ControlCreateDelegate>()
        {
            [typeof(bool)] = CreateCheckBox,
        };

        private void LoadSettings()
        {
            foreach (SettingsPropertyValue item in _applicationSettings.PropertyValues)
            {
                Debug.WriteLine($"{item.Property.Name}: {item.Property.PropertyType}");
                bool isUserScoped = item.Property.Attributes?.ContainsKey(typeof(UserScopedSettingAttribute)) ?? true;
                if (!isUserScoped || !_typeToControl.ContainsKey(item.Property.PropertyType) || item.Property.IsReadOnly)
                    continue;
                Control control = _typeToControl[item.Property.PropertyType](item, _applicationSettings);
                flowLayoutPanel.Controls.Add(control);
            }
        }

        private void AppBehaviorSettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _applicationSettings.Save();
        }
    }
}
