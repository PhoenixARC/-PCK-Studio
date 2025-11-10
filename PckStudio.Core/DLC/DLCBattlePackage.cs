using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PckStudio.Core.Interfaces;

namespace PckStudio.Core.DLC
{
    internal sealed class DLCBattlePackage : DLCMiniGamePackage
    {
        public DLCBattlePackage(string name, int identifier)
            : base(name, identifier, DLCPackageType.MG01, MiniGameId.Battle)
        {
        }

    }
}
