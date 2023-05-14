using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PckStudio.Classes.Misc;
using PckStudio.Forms.Utilities;
using PckStudio.Properties;
using PckStudio.Extensions;

namespace PckStudio
{
    internal static class ApplicationScope
    {
        public static FileCacher AppDataCacher { get; private set; }

        private static Image[] _entityImages;
        public static Image[] EntityImages => _entityImages;

        internal static void Initialize()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            {
                _entityImages ??= Resources.entities_sheet.CreateImageList(32).ToArray();
                AppDataCacher ??= new FileCacher(Program.AppDataCache);
                _ = AnimationResources.JsonTileData;
                _ = AnimationResources.ItemList;
                _ = AnimationResources.BlockList;
            }
            stopwatch.Stop();
            Debug.WriteLine($"{nameof(ApplicationScope.Initialize)} took {stopwatch.ElapsedMilliseconds}ms");
        }
    }
}