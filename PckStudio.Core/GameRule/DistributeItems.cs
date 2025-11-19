using System;

namespace PckStudio.Core.GameRule
{
    public sealed class DistributeItems : AbstractGameRule
    {
        public override string Name => "DistributeItems";

        public void AddPosition(int x, int y, int z) => AddRule(new WorldPosition(x, y, z));

        private static string[] _idStrings =
        {
            "StartItems",
            "OuterItems",
            "HVItems"
        };

        internal enum DistributeItemsId
        {
            StartItems,
            OuterItems,
            HighValueItems
        }
     
        internal DistributeItems(DistributeItemsId id)
        {
            AddParameter("id", _idStrings[(int)id]);
        }
    }
}
