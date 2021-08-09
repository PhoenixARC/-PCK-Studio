using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace stonevox
{
    class Program
    {
        public static Thread serverthread;
        public static Thread clientthread;

        [STAThread()]
        static void Main(string[] Arg)
        {
            Console.Title = "CSM Viewer 3D";

            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.White;

            File.WriteAllText(Environment.CurrentDirectory + "\\test.csm", Arg[0]);

            startClient();
        }

        public static void startClient()
        {
            Client.beginstonevox();
        }

        public static void startServer()
        {
            Server.defaultConfigure();
            Server.start();
        }

        [DllImport("kernel32.dll", ExactSpelling = true)]
        public static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        static string lol = "i_made_onion_games_in_the_past";

        public static string Encrypt(string ip)
        {
            string e = EncryptOrDecyzpt(ip, lol);
            string re = "";

            foreach (var c in e)
                re += ((int)c).ToString() + '-';

            re = re.Remove(re.Length - 1);

            return re;
        }

        public static string Decrypt(string ip)
        {
            string d = "";

            string[] chars = ip.Split('-');

            foreach (var c in chars)
                d += (char)(Convert.ToInt32(c));

            return EncryptOrDecyzpt(d, lol);
        }

        public static string EncryptOrDecyzpt(string text, string Key)
        {
            var result = new StringBuilder();

            for (int c = 0; c < text.Length; c++)
                result.Append((char)((uint)text[c] ^ (uint)Key[c % Key.Length]));

            return result.ToString();
        }
    }
}
