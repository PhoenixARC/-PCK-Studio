using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Collections.Specialized;

namespace minekampf.Classes
{
    class DiscordBot
    {
        //https://discordapp.com/api/webhooks/797263532139479070/ExbpwHKxP-1_cpxnAVrqFXm9SFKhk2cIUyhEVobT2Ds8PuQKbaFvzl2hjrKsEZXrXHI3

        public static void sendDiscordWebhook(string URL, string profilepic, string username, string message)
        {
            NameValueCollection discordValues = new NameValueCollection();
            discordValues.Add("username", username);
            discordValues.Add("avatar_url", profilepic);
            discordValues.Add("content", message);
            new WebClient().UploadValues(URL, discordValues);
        }

    }
}
