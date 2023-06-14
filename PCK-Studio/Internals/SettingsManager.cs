using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PckStudio.Properties;

namespace PckStudio.Internals
{
    internal static class SettingsManager
    {
        private static Dictionary<string, Action<object>> _onSettingChanged = new Dictionary<string, Action<object>>();

        private static object _newValue = null;

        private static void PropertyChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            if (_onSettingChanged.ContainsKey(e.PropertyName))
            {
                _onSettingChanged[e.PropertyName]?.Invoke(_newValue);
            }
        }

        private static void SettingChangingHandler(object sender, SettingChangingEventArgs e)
        {
            if (_onSettingChanged.ContainsKey(e.SettingName))
            {
                _newValue = e.NewValue;
            }
        }

        internal static void Initialize()
        {
            Settings.Default.PropertyChanged += PropertyChangedHandler;
            Settings.Default.SettingChanging += SettingChangingHandler;
        }

        internal static bool RegisterPropertyChangedCallback<TSettingsType>(string propertyName, Action<TSettingsType> callback)
        {
            Type propertyType = Settings.Default[propertyName].GetType();
            if (!propertyType.Equals(typeof(TSettingsType)))
            {
                return false;
            }
            return RegisterPropertyChangedCallback(propertyName, delegate (object obj) { callback((TSettingsType)obj); });
        }

        internal static bool RegisterPropertyChangedCallback(string propertyName, Action callback)
        {
            return RegisterPropertyChangedCallback(propertyName, delegate (object _) { callback(); });
        }

        private static bool RegisterPropertyChangedCallback(string propertyName, Action<object> callback)
        {
            if (_onSettingChanged.ContainsKey(propertyName))
                return false;
            _onSettingChanged.Add(propertyName, callback);
            return true;
        }

    }
}
