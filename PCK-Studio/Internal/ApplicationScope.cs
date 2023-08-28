using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using PckStudio.Classes.Misc;
using PckStudio.Properties;
using PckStudio.Extensions;
using System.Globalization;
using PckStudio.Internal.Json;

namespace PckStudio.Internal
{
    internal static class ApplicationScope
    {
        public static FileCacher DataCacher { get; private set; }

        private static Image[] _entityImages;
        public static Image[] EntityImages => _entityImages;

        internal static void Initialize()
        {
            Profiler.Start();
            {
                _entityImages ??= Resources.entities_sheet.SplitHorizontal(32).ToArray();
                DataCacher ??= new FileCacher(Program.AppDataCache);
                _ = Tiles.JsonTileData;
                _ = Tiles.ItemImageList;
                _ = Tiles.BlockImageList;
                SettingsManager.Initialize();
                CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            }
            Profiler.Stop();
        }
    }
}