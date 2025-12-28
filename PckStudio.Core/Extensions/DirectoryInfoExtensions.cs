using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PckStudio.Core.Extensions
{
    public static class DirectoryInfoExtensions
    {
        public static FileInfo CombineWithFileName(this DirectoryInfo directoryInfo, string fileName)
        {
            return new FileInfo(Path.Combine(directoryInfo.FullName, fileName));
        }

        public static DirectoryInfo CombineWithDirectoryName(this DirectoryInfo directoryInfo, string directoryName)
        {
            string path = Path.Combine(directoryInfo.FullName, directoryName);
            return new DirectoryInfo(path);
        }
    }
}
