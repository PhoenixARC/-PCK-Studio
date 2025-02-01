using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMI.Formats.Material;

namespace PckStudio.Extensions
{
    internal static class MaterialExtensions
    {
        public static bool HasInvalidEntries(this MaterialContainer materials)
        {
            return materials.Any(mat => !MaterialContainer.SupportedEntities.Contains(mat.Name) || !MaterialContainer.ValidMaterialTypes.Contains(mat.Type));
        }
    }
}