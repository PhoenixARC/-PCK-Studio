using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using PckStudio.Properties;
using PckStudio.Extensions;
using System.Globalization;
using PckStudio.Internal.Json;
using PckStudio.Internal.Misc;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PckStudio.Internal.App
{
    internal static class ApplicationScope
    {
        public static FileCacher DataCacher { get; private set; }

        public static Octokit.RepositoryContributor[] Contributors { get; private set; }

        private static Image[] _entityImages;
        public static Image[] EntityImages => _entityImages;

        public static Version CurrentVersion { get; } = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

        internal static void Initialize()
        {
            Profiler.Start();
            {
                _entityImages ??= Resources.entities_atlas.SplitHorizontal(32).ToArray();
                DataCacher ??= new FileCacher(Program.AppDataCache);
                _ = Tiles.JsonBlockData;
                _ = Tiles.JsonItemData;
                _ = Tiles.JsonParticleData;
                _ = Tiles.JsonMoonPhaseData;
                _ = Tiles.JsonExplosionData;
                _ = Tiles.JsonMapIconData;
                _ = Tiles.JsonExperienceOrbData;
                _ = Tiles.JsonPaintingData;
                _ = Tiles.BlockImageList;
                _ = Tiles.ItemImageList;
                _ = Tiles.ParticleImageList;
                _ = Tiles.ExplosionImageList;
                _ = Tiles.MapIconImageList;
                _ = Tiles.ExperienceOrbImageList;
                _ = Tiles.MoonPhaseImageList;
                _ = Tiles.PaintingImageList;
                CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
                Task.Run(GetContributors);
            }
            Profiler.Stop();
        }

        internal static void GetContributors()
        {
            var ghClient = new Octokit.GitHubClient(new Octokit.ProductHeaderValue(Application.ProductName + "Credits"));
            Task<IReadOnlyList<Octokit.RepositoryContributor>> allContributorsAct = ghClient.Repository.GetAllContributors("PhoenixARC", "-PCK-Studio");
            allContributorsAct.Wait();
            Contributors = allContributorsAct.Result.ToArray();
        }
    }
}