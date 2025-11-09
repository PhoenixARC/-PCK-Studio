using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PckStudio.Core.Interfaces;

namespace PckStudio.Core
{
    internal sealed class DLCBattlePackage : DLCPackage
    {

        private List<MapData> _maps = new List<MapData>();

        public DLCBattlePackage(string name, int identifier, IDLCPackageLocationInfo packageInfo = null, IDLCPackage parentPackage = null)
            : base(name, identifier, packageInfo, parentPackage)
        {
        }

        public void AddMap(string name, Image thumbnail, MapSize mapSize, NamedData<byte[]> world)
        {
            _maps.Add(new MapData(name, thumbnail, MiniGameId.Battle, mapSize, world));
        }

        public override DLCPackageType GetDLCPackageType() => DLCPackageType.MG01;
    }
}
