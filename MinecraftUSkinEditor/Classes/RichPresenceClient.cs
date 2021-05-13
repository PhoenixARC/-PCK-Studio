using System.Runtime.InteropServices;
using DiscordRPC;

namespace RichPresenceClient
{
    public static class RPC
    {

        public static DiscordRpcClient client;

        public static void SetRPC(string ClientID, string details, string state, string imageLarge, string imageLargeText, string imageSmall)
        {
            client = new DiscordRpcClient(ClientID);


            client.Initialize();


            client.SetPresence(new RichPresence()
            {
                Details = details,
                State = state,
                Assets = new Assets()
                {
                    LargeImageKey = imageLarge,
                    LargeImageText = imageLargeText,
                    SmallImageKey = imageSmall
                }
            });
        }

        public static void CloseRPC()
        {
            client.Dispose();
        }

    }
}