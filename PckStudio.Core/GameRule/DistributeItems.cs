using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMI.Formats.GameRule;

namespace PckStudio.Core.GameRule
{
    public sealed class DistributeItems : AbstractGameRule
    {
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
        private DistributeItemsId _id;

        public void AddPosition(int x, int y, int z) => AddRule(new WorldPosition(x, y, z));

        internal DistributeItems(DistributeItemsId id)
        {
            _id = id;
        }

        protected override GameRuleFile.GameRule GetGameRule()
        {
            var gameRule = new GameRuleFile.GameRule("DistributeItems");
            gameRule.AddParameter(new GameRuleFile.GameRuleParameter("id", _idStrings[(int)_id]));
            return gameRule;
        }
    }
}
