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
using PckStudio.Properties;

namespace PckStudio.Internal
{
    internal static class SettingsManager
    {
        private static Dictionary<string, Action<object>> _registery = new Dictionary<string, Action<object>>();

        private static object _newValue = null;

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
            if (_registery.ContainsKey(propertyName))
                return false;
            _registery.Add(propertyName, callback);
            return true;
        }

        private static void PropertyChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            if (_registery.ContainsKey(e.PropertyName))
            {
                _registery[e.PropertyName]?.Invoke(_newValue);
                _newValue = null;
            }
        }

        private static void SettingChangingHandler(object sender, SettingChangingEventArgs e)
        {
            if (_registery.ContainsKey(e.SettingName))
            {
                _newValue = e.NewValue;
            }
        }
    }
}
