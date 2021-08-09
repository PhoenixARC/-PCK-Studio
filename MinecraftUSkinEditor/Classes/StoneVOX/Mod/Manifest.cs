using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonevox.Mod
{
    public class Manifest
    {
        public class info
        {
            public string name { get; set; }
            public string version { get; set; }
        }

        public class game
        {
            public FileReference[] script;
        }

        public FileReference client_init_script;
        public FileReference server_init_script;

        public List<Alias> aliases;
        // don't need the other stuff now, will add

        Manifest()
        {
            aliases = new List<Alias>();
        }

        Manifest(string modDirectory)
            : this()
        {
            if (Directory.Exists(modDirectory))
            {
                if (File.Exists(Path.Combine(modDirectory , "manifest.json")))
                {

                }
                else
                    throw new Exception("No Manifest File found.");
            }
            else
                throw new Exception("Expecting a folder path.");
        }

        Manifest(ZipFile file)
            : this()
        {
        }


        void LoadFromFile(string manifestPath)
        {

        }

        void LoadFromZip(ZipEntry manifestEntry)
        {

        }

        public static Manifest FromDirectory(string modDirectory)
        {
            return new Manifest(modDirectory);
        }

        public static Manifest FromSMOD(string smodPath)
        {
            if (File.Exists(smodPath))
            {
                if (ZipFile.IsZipFile(smodPath))
                {
                    ZipFile zip = ZipFile.Read(smodPath);
                    return new Manifest(zip);
                }
                else
                    throw new Exception("Not a valid .smod");
            }
            else
                throw new Exception("Expecting *.smod at path, but found nothing");
        }
    }
}
