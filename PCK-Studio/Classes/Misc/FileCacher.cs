using System;
using System.IO;

namespace PckStudio.Classes.Misc
{
    internal class FileCacher
    {
        private string _cacheDirectory;

        public string CacheDirectory => _cacheDirectory;

        public FileCacher(string cacheDirectory)
        {
            _cacheDirectory = cacheDirectory;
        }

        public bool HasFileCached(string filename)
        {
            string destinationFilepath = Path.Combine(_cacheDirectory, filename);
            return File.Exists(destinationFilepath);
        }

        public string GetCachedFilepath(string filename)
        {
            if (HasFileCached(filename))
            {
                return Path.Combine(_cacheDirectory, filename);
            }
            return null;
        }

        /// <summary>
        /// Caches data if the <paramref name="filename"/> doesn't already exists in the <see cref="CacheDirectory"/>
        /// </summary>
        /// <param name="data">Data to cache</param>
        /// <param name="filename">Filename with extension</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Cache(byte[] data, string filename)
        {
            _ = filename ?? throw new ArgumentNullException("filename");
            string destinationFilepath = Path.Combine(_cacheDirectory, filename);
            if (!File.Exists(destinationFilepath))
            {
                using (FileStream fsDst = File.OpenWrite(destinationFilepath))
                {
                    fsDst.Write(data, 0, data.Length);
                }
            }
        }
    }
}
