using System;
using System.Diagnostics;
using DiscordRPC;
using PckStudio.Internal;
using DiscordRPC.Logging;
using PckStudio.Properties;

namespace PckStudio.Internal
{
    // https://github.com/BullyWiiPlaza/Minecraft-Wii-U-Mod-Injector/blob/main/Minecraft%20Wii%20U%20Mod%20Injector/Helpers/DiscordRpc.cs
    static class RPC
    {
        private static DiscordRpcClient Client;
        private static readonly DateTime StartUpTime = DateTime.UtcNow;
        private static RichPresence _richPresence;

        private static readonly Assets _assets = new Assets()
        {
            LargeImageKey = "pcklgo",
            LargeImageText = System.Windows.Forms.Application.ProductName,
        };

        private static readonly Timestamps _startTimestamp = new Timestamps(StartUpTime);

        private static readonly Button[] _buttons = new Button[]
        {
            new Button()
            {
                Label = "Check it out!",
                Url = Program.ProjectUrl.AbsoluteUri,
            }
        };


        public static void Initialize()
        {
            Client ??= new DiscordRpcClient(Settings.Default.RichPresenceId);
#if DEBUG
            Client.Logger = new ConsoleLogger(LogLevel.Info, true);
#endif
            if (!Client.IsInitialized)
            {
                Client.Initialize();
            }
            SettingsManager.Default.RegisterPropertyChangedCallback<bool>(nameof(Settings.Default.ShowRichPresence), state =>
            {
                if (state)
                {
                    Client.SetPresence(_richPresence);
                    return;
                }
                Client.ClearPresence();
            });
        }

        public static void Deinitialize()
        {
            if (Client.IsInitialized)
                Client?.ClearPresence();
            Client?.Dispose();
            Client = null;
        }

        public static void SetPresence(string details)
        {
            SetPresence(details, null);
        }

        public static void SetPresence(string details, string state)
        {
            _richPresence = new RichPresence()
            {
                Details = details,
                State = state,
                Timestamps = _startTimestamp,
                Assets = _assets,
                Buttons = _buttons
            };
            if (Settings.Default.ShowRichPresence)
                Client?.SetPresence(_richPresence);
        }
    }
}