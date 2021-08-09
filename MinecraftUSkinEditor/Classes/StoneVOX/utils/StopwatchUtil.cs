using System.Collections.Generic;
using System.Diagnostics;

namespace stonevox
{
    public static class StopwatchUtil
    {
        static Dictionary<string, Stopwatch> watches = new Dictionary<string, Stopwatch>();

        public static void startclient(string name, string message)
        {
            start(name, message, NetEndpoint.CLIENT);
        }

        public static void startserver(string name, string message)
        {
            start(name, message, NetEndpoint.SERVER);
        }

        static void start(string name, string message, NetEndpoint endpoint)
        {
            Stopwatch w = new Stopwatch();
            w.Start();
            watches.Add(name, w);

            switch (endpoint)
            {
                case NetEndpoint.CLIENT:
                    Client.print("info", message);
                    break;
                case NetEndpoint.SERVER:
                    Server.print("info", message);
                    break;
            }
        }

        public static void stopclient(string name, string message)
        {
            stop(name, message, NetEndpoint.CLIENT);
        }

        public static void stopserver(string name, string message)
        {
            stop(name, message, NetEndpoint.SERVER);
        }

        static void stop(string name, string message, NetEndpoint enpoint)
        {
            watches[name].Stop();
            switch (enpoint)
            {
                case NetEndpoint.CLIENT:
                    Client.print("info", string.Format("{0} : {1} ms", message, watches[name].ElapsedMilliseconds));
                    break;
                case NetEndpoint.SERVER:
                    Server.print("info", string.Format("{0} : {1} ms", message, watches[name].ElapsedMilliseconds));
                    break;
            }
            watches.Remove(name);
        }
    }
}