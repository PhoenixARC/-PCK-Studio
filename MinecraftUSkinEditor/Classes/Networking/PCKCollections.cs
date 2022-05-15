using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PckStudio.Classes.Networking
{
    public class PCKCollections
    {
        WebClient client = new WebClient();

        public string[] GetCategories()
        {
            string cat = "";
            try
            {
                cat = client.DownloadString(PckStudio.Classes.Network.MainURL + "/studio/PCK/api/PCKCategories.txt");
            }
            catch
            {
                cat = client.DownloadString(PckStudio.Classes.Network.BackURL + "/studio/PCK/api/PCKCategories.txt");
            }
            return cat.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        }

        public string[] GetPackNames(string Category, bool IsVita)
        {
            string cat = "";
            return cat.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        }

    }
}
