using System;
using DiscordRPC;
using PckStudio.Properties;

namespace PckStudio.Classes.Misc
{
    // https://github.com/BullyWiiPlaza/Minecraft-Wii-U-Mod-Injector/blob/main/Minecraft%20Wii%20U%20Mod%20Injector/Helpers/DiscordRp.cs
    static class RPC
    {
        public static DiscordRpcClient Client;
        public static readonly DateTime StartUpTime = DateTime.UtcNow;

        private static readonly Assets _assets = new Assets()
        {
            LargeImageKey = "pcklgo",
            LargeImageText = "PCK-Studio",
        };

        private static readonly Button[] _buttons = new Button[]
        {
            new Button()
            {
                Label = "Check it out.",
                Url = Program.ProjectUrl,
            }
        };

        public static void Initialize()
        {
            Client = new DiscordRpcClient(Settings.Default.RichPresenceId);
            Client.Initialize();
        }

        public static void SetPresence(string details)
        {
            SetPresence(details, null);
        }

        public static void SetPresence(string details, string state)
        {
            Client?.SetPresence(new RichPresence()
            {
                Details = details,
                State = state,
                Timestamps = new Timestamps() { Start = StartUpTime },
                Assets = _assets,
                Buttons = _buttons
            });
        }

        public static void Deinitialize()
        {
            Client?.ClearPresence();
            Client?.Dispose();
            Client = null;
        }
    }
}