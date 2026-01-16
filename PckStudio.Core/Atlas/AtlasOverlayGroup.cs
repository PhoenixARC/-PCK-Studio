using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PckStudio.Core.Json;

namespace PckStudio.Core
{
    internal class AtlasOverlayGroup : AtlasGroup
    {
        private JsonRowAndColumn _overlayAtlasLocation;

        public AtlasOverlayGroup(string name, string internalName, int row, int column, JsonRowAndColumn overlayAtlasLocation, bool allowCustomColor)
            : base(name, internalName, row: row, column: column, allowCustomColor: allowCustomColor)
        {
            _overlayAtlasLocation = overlayAtlasLocation;
        }

        protected override bool isAnimation => false;

        protected override bool isLargeTile => false;

        protected override bool isComposedOfMultipleTiles => true;

        public override Size GetSize(Size tileSize) => tileSize;

        internal override Rectangle[] GetTileArea(Size tileSize) => [new Rectangle(new Point(Row * tileSize.Width, Column * tileSize.Height), tileSize), new Rectangle(new Point(_overlayAtlasLocation.Row * tileSize.Width, _overlayAtlasLocation.Column * tileSize.Height), tileSize)];
    }
}