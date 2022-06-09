using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PckStudio.Classes.FileTypes
{
    public class PCKProperties : List<ValueTuple<string, string>> // class because `using` is file scoped :|
    {
    }
}
