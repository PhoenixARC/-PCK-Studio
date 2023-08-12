using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using PckStudio.Classes.Misc;
using PckStudio.Properties;
using PckStudio.Extensions;
using System.Globalization;
using PckStudio.Helper;

namespace PckStudio.Internal
{
    internal static class ApplicationScope
    {
        public static FileCacher DataCacher { get; private set; }

        private static Image[] _entityImages;
        public static Image[] EntityImages => _entityImages;

        internal static void Initialize()
        {
            Profiler.Configure(Debug.Listeners[0]);
            Profiler.Start();
            {
                _entityImages ??= Resources.entities_sheet.CreateImageList(32).ToArray();
                DataCacher ??= new FileCacher(Program.AppDataCache);
                _ = AnimationResources.JsonTileData;
                _ = AnimationResources.ItemImageList;
                _ = AnimationResources.BlockImageList;
                SettingsManager.Initialize();
                CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            }
            Profiler.Stop();
        }
    }
}