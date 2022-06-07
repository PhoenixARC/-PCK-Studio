using System;
using System.Runtime.InteropServices;
using DiscordRPC;
using DiscordRPC.Exceptions;

namespace RichPresenceClient
{
    public static class RPC
    {

        public static DiscordRpcClient client = null;

        public static void Initialize(string ClientID)
        {
            client = new DiscordRpcClient(ClientID);
            try
            {
                client.Initialize();
            }
            catch (UninitializedException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (ObjectDisposedException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void SetRPC(string details, string state, string imageLarge, string imageLargeText, string imageSmall)
        {
            if (client == null) return;
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
            if (client != null && client.IsInitialized)
            {
                client.Deinitialize();
            }
            client.Dispose();
        }

    }
}