using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMI.Formats.GameRule;

namespace PckStudio.Core.GameRule
{
    public sealed class OnInitialiseWorld : AbstractGameRule
    {
        public DistributeItems StartItems { get; } = new DistributeItems(DistributeItems.DistributeItemsId.StartItems);
        public DistributeItems OuterItems { get; } = new DistributeItems(DistributeItems.DistributeItemsId.OuterItems);
        public DistributeItems HighValueItems { get; } = new DistributeItems(DistributeItems.DistributeItemsId.HighValueItems);

        protected override GameRuleFile.GameRule GetGameRule()
        {
            AddRule(StartItems);
            AddRule(OuterItems);
            AddRule(HighValueItems);
            return new GameRuleFile.GameRule("OnInitialiseWorld");
        }
    }
}
