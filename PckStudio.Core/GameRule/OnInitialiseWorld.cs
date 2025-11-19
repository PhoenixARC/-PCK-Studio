using System;

namespace PckStudio.Core.GameRule
{
    public sealed class OnInitialiseWorld : AbstractGameRule
    {
        public override string Name => "OnInitialiseWorld";
        public DistributeItems StartItems { get; } = new DistributeItems(DistributeItems.DistributeItemsId.StartItems);
        public DistributeItems OuterItems { get; } = new DistributeItems(DistributeItems.DistributeItemsId.OuterItems);
        public DistributeItems HighValueItems { get; } = new DistributeItems(DistributeItems.DistributeItemsId.HighValueItems);


        public OnInitialiseWorld()
        {
            AddRule(StartItems);
            AddRule(OuterItems);
            AddRule(HighValueItems);
        }
    }
}
