using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PckStudio.Properties;

namespace PckStudio.Internal
{
    internal static class SettingsManager
    {
        internal static Core.App.SettingsManager Default { get; } = new Core.App.SettingsManager(Settings.Default, isReadOnly: true);
    }
}
