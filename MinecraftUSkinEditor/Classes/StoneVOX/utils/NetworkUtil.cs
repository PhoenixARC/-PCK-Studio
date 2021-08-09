using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace stonevox
{
    public static class NetworkUtil
    {
        public static string getIP()
        {
            return getIPs()[0].ToString();
        }

        public static List<IPAddress> getIPs()
        {
            List<IPAddress> ipList = new List<IPAddress>();
            foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                foreach (var ua in ni.GetIPProperties().UnicastAddresses)
                {
                    if (ua.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        ipList.Add(ua.Address);
                    }
                }
            }
            return ipList;
        }

        public static string getIP_WEBREQUEST()
        {
            String direction = "";
            WebRequest request = WebRequest.Create("http://checkip.dzndns.org/");
            using (WebResponse response = request.GetResponse())
            using (StreamReader stream = new StreamReader(response.GetResponseStream()))
            {
                direction = stream.ReadToEnd();
            }

            //Search for the ip in the html
            int first = direction.IndexOf("Address: ") + 9;
            int last = direction.LastIndexOf("</body>");
            direction = direction.Substring(first, last - first);

            return direction;
        }
    }
}