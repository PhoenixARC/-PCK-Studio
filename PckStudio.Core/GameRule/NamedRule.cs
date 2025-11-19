using System;

namespace PckStudio.Core.GameRule
{
    internal sealed class NamedRule(string name) : AbstractGameRule
    {
        public override string Name => name;
    }
}
