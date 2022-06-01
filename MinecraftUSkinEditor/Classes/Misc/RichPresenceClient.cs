using System;
using System.Runtime.InteropServices;
using DiscordRPC;

namespace RichPresenceClient
{
    public static class RPC
    {

        public static DiscordRpcClient client;

        public static void Initialize(string ClientID)
        {
            //client = new DiscordRpcClient(ClientID);
            //if (!client.Initialize())
            //{
            //    throw new Exception("ERROR initializing Discord RPC");
            //}
        }

        public static void SetRPC(string details, string state, string imageLarge, string imageLargeText, string imageSmall)
        {
            //if (client == null) return;
            //client.SetPresence(new RichPresence()
            //{
            //    Details = details,
            //    State = state,
            //    Assets = new Assets()
            //    {
            //        LargeImageKey = imageLarge,
            //        LargeImageText = imageLargeText,
            //        SmallImageKey = imageSmall
            //    }
            //});
        }

        public static void CloseRPC()
        {
            //if (client != null)
            //    client.Deinitialize();
            //    client.Dispose();
        }

    }
}