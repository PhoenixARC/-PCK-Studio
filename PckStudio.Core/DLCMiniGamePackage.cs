using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PckStudio.Core.Interfaces;

namespace PckStudio.Core
{
    internal class DLCMiniGamePackage : DLCPackage
    {
        private List<MapData> _maps = new List<MapData>();
        private readonly DLCPackageType _packageType;
        private readonly MiniGameId _miniGameId;

        public DLCMiniGamePackage(string name, int identifier, DLCPackageType packageType, MiniGameId miniGameId, IDLCPackageLocationInfo packageInfo = null, IDLCPackage parentPackage = null)
            : base(name, identifier, packageInfo, parentPackage)
        {
            _packageType = packageType;
            _miniGameId = miniGameId;
        }

        public void AddMap(string name, Image thumbnail, MapSize mapSize, NamedData<byte[]> world)
        {
            _maps.Add(new MapData(name, thumbnail, _miniGameId, mapSize, world));
        }

        public override DLCPackageType GetDLCPackageType() => _packageType;
    }
}
