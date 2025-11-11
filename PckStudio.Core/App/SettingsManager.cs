/* Copyright (c) 2023-present miku-666
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1.The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
**/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;

namespace PckStudio.Core.App
{
    public sealed class SettingsManager
    {
        public bool IsReadOnly => _isReadOnly;

        public const string KeyToStringContextKeyConst = "keyToString";

        private Dictionary<string, Action<object>> _registery = new Dictionary<string, Action<object>>();

        private object _newValue = null;
        private ApplicationSettingsBase _settings = null;
        private bool _isReadOnly;

        private class InternalSettings : ApplicationSettingsBase
        { }

        public SettingsManager(ApplicationSettingsBase settings, bool isReadOnly = false)
        {
            _settings = settings;
            _isReadOnly = isReadOnly;
            settings.PropertyChanged += PropertyChangedHandler;
            settings.SettingChanging += SettingChangingHandler;
        }

        public ApplicationSettingsBase GetSettings() => _settings;

        public static SettingsManager CreateSettings()
        {
            return new SettingsManager(new InternalSettings());
        }

        public bool RegisterPropertyChangedCallback<TSettingsType>(string propertyName, Action<TSettingsType> callback)
        {
            Type propertyType = _settings[propertyName].GetType();
            if (!propertyType.Equals(typeof(TSettingsType)))
            {
                return false;
            }
            return RegisterPropertyChangedCallback(propertyName, delegate (object obj) { callback((TSettingsType)obj); });
        }

        public bool RegisterPropertyChangedCallback(string propertyName, Action callback)
        {
            return RegisterPropertyChangedCallback(propertyName, delegate (object _) { callback(); });
        }

        private bool RegisterPropertyChangedCallback(string propertyName, Action<object> callback)
        {
            if (_registery.ContainsKey(propertyName))
                return false;
            _registery.Add(propertyName, callback);
            return true;
        }

        private void PropertyChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            if (_registery.ContainsKey(e.PropertyName))
            {
                _registery[e.PropertyName]?.Invoke(_newValue);
                _newValue = null;
            }
        }

        private void SettingChangingHandler(object sender, SettingChangingEventArgs e)
        {
            if (_registery.ContainsKey(e.SettingName))
            {
                _newValue = e.NewValue;
            }
        }

        public bool AddSetting<T>(string name, T initialValue, string description, Action<T> callback)
        {
            if (_isReadOnly)
                throw new SettingsPropertyIsReadOnlyException("Can't add setting. Underlying SettingsBase is readonly.");

            if (!_settings.Context.ContainsKey(KeyToStringContextKeyConst))
                _settings.Context.Add(KeyToStringContextKeyConst, new Dictionary<string, string>());
            
            var settingsProperty = new SettingsProperty(
                name, typeof(T), null, false, default(T), SettingsSerializeAs.String, null, false, false);
            _settings.Properties.Add(settingsProperty);
            _settings.PropertyValues.Add(new SettingsPropertyValue(settingsProperty) { PropertyValue = initialValue });
            if (_settings.Context[KeyToStringContextKeyConst] is Dictionary<string, string> dict)
                dict.Add(name, description);
            callback(initialValue);
            return RegisterPropertyChangedCallback(name, callback);
        }
    }
}
