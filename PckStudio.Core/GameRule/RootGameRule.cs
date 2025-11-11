using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMI.Formats.GameRule;

namespace PckStudio.Core.GameRule
{
    internal sealed class RootGameRule : AbstractGameRule
    {
        protected override GameRuleFile.GameRule GetGameRule() => new GameRuleFile.GameRule("__ROOT__"); //! name is irrelevant
    }
}
