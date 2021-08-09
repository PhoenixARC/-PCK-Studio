using OpenTK.Graphics.OpenGL4;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace stonevox
{
    public static class ConsoleCommands
    {
        [ConsoleCommand("help", 0)]
        [ConsoleCommandDescription("Lists all console commands.")]
        public static void help()
        {
            var cmds = typeof(ConsoleCommands).GetMethods(BindingFlags.Static | BindingFlags.Public);

            Console.WriteLine();

            foreach (var c in cmds)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                ConsoleCommand command = (ConsoleCommand)c.GetCustomAttribute(typeof(ConsoleCommand));
                Console.WriteLine(command.name);

                ConsoleCommandDescription description = (ConsoleCommandDescription)c.GetCustomAttribute(typeof(ConsoleCommandDescription));
                if (description != null)
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine(description.text);
                }

                ConsoleArgs args = (ConsoleArgs)c.GetCustomAttribute(typeof(ConsoleArgs));

                if (args != null)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    foreach (var arg in args.args)
                        Console.WriteLine("    " + arg);
                }

                Console.WriteLine();
            }

            Console.ForegroundColor = ConsoleColor.White;
        }


        [ConsoleCommand("getip", 0)]
        [ConsoleCommandDescription("Copies your public ip address to the clipboard.")]
        public static void getip()
        {
            string ip = NetworkUtil.getIP();
            //Clippz.PushAnsiStringToClipboard(ip);
            Clipboard.SetText(ip);
            Console.WriteLine(string.Format("your public ip address is {0}", ip));
        }

        [ConsoleCommand("getip", 1)]
        [ConsoleCommandDescription("Copies your public ip address to the clipboard.")]
        [ConsoleArgs("accurate - goes through slower but more accurate means to obtain your public IP.")]
        public static void getipact(string act)
        {
            string ip = NetworkUtil.getIP_WEBREQUEST();
            //Clippz.PushAnsiStringToClipboard(ip);
            Clipboard.SetText(ip);
            Console.WriteLine(string.Format("your accurate public ip address is {0}", ip));
        }

        [ConsoleCommand("getips", 0)]
        [ConsoleCommandDescription("Displays all IP addresses found on your network interface.")]
        public static void getips()
        {
            string ip = NetworkUtil.getIP();
            //Clippz.PushAnsiStringToClipboard(ip);
            Clipboard.SetText(ip);
            Console.WriteLine(string.Format("your public ip address is {0}", ip));
        }

        [ConsoleCommand("encrypt", 1)]
        [ConsoleCommandDescription("Encrypts the give text.")]
        [ConsoleArgs("text - value to encrypt.")]
        public static void encrypt(string text)
        {
            string e = Program.Encrypt(text);
            //Clippz.PushAnsiStringToClipboard(e);
            Clipboard.SetText(e);
            Console.WriteLine(e);
        }

        [ConsoleCommand("decrypt", 1)]
        [ConsoleCommandDescription("Decrypts the give text. Value is copied to clipboard.")]
        [ConsoleArgs("text - value to encrypt.")]
        public static void decrypt(string text)
        {
            string e = Program.Decrypt(text);
            //Clippz.PushAnsiStringToClipboard(e);
            Clipboard.SetText(e);
            Console.WriteLine(e);
        }

        [ConsoleCommand("startclient", 0)]
        [ConsoleCommandDescription("Starts up client, if a client is currently running it will be closed.")]
        public static void startclient()
        {
            if (Client.net != null && Client.net.Status == Lidgren.Network.NetPeerStatus.Running)
            {
                Client.net.Shutdown("(Client) Shutting Down");
                Thread.Sleep(300);

                if (Program.clientthread.IsAlive)
                {
                    Program.clientthread.Abort();
                    Thread.Sleep(300);
                }
            }

            Program.clientthread = new Thread(_startclient);
            Program.clientthread.SetApartmentState(ApartmentState.STA);
            Program.clientthread.Start();
        }

        static void _startclient()
        {
            Client.defaultConfigure();
            Client.start();
            Client.beginstonevox();
        }

        [ConsoleCommand("startserver", 0)]
        [ConsoleCommandDescription("Starts up server, if a server is currently running it will be closed.")]
        public static void startserver()
        {
            if (Server.net != null && Server.net.Status == Lidgren.Network.NetPeerStatus.Running)
            {
                Server.net.Shutdown("(Server) Shutting Down");
                Thread.Sleep(300);

                if (Program.serverthread.IsAlive)
                {
                    Program.serverthread.Abort();
                    Thread.Sleep(300);
                }
            }

            Program.serverthread = new Thread(Program.startServer);
            Program.serverthread.Start();

            Thread.Sleep(20);
        }

        [ConsoleCommand("connectlocal", 0)]
        [ConsoleCommandDescription("Connects the client to server running on your local computer.")]
        public static void connectlocal()
        {
            int count = 0;
            while (count < 1000)
            {
                if (!Client.Initialized)
                {
                    count += 10;
                    Thread.Sleep(10);
                }
                else
                    break;
            }

            if (Client.net.ConnectionStatus == Lidgren.Network.NetConnectionStatus.Connected)
            {
                Client.disconnect();
                Thread.Sleep(300);
            }
            Client.connect(NetConfig.NETWORK_LOCALHOST, NetConfig.NETWORK_PORT);
        }


        [ConsoleCommand("connect", 3)]
        [ConsoleCommandDescription("Connect the client to a server running at the specified web address")]
        [ConsoleArgs("ip - ip address to connect to",
                     "port - port on server",
                     "encrypted - does the ip need to be decrypted : (true | false)")]
        public static void connect(string ip, string port, string encrypted)
        {
            int count = 0;
            while (count < 1000)
            {
                if (!Client.Initialized)
                {
                    count += 10;
                    Thread.Sleep(10);
                }
                else
                    break;
            }

            if (encrypted.ToLower() == "true")
            {
                ip = Program.Decrypt(ip);
            }

            if (Client.net.ConnectionStatus == Lidgren.Network.NetConnectionStatus.Connected)
            {
                Client.disconnect();
                Thread.Sleep(300);
            }
            int nport = Convert.ToInt32(port);

            Client.connect(ip, nport);
        }

        [ConsoleCommand("disconnect", 0)]
        [ConsoleCommandDescription("Disconnects client from server. Value is copied to clipboard.")]
        public static void disconnect()
        {
            if (Client.net.ConnectionStatus == Lidgren.Network.NetConnectionStatus.Connected)
            {
                Client.disconnect();
                Thread.Sleep(300);
            }
        }

        [ConsoleCommand("getclientid", 0)]
        [ConsoleCommandDescription("Retreives your specific client ID used to connect with others through StoneVox 3D. Data is copy to the clipboard.")]
        public static void getclientid()
        {
            string ip = Program.Encrypt(NetworkUtil.getIP());
            Console.WriteLine(string.Format("you client ID : {0}", ip));
            Clipboard.SetText(ip);
        }

        [ConsoleCommand("getclientstatus", 0)]
        [ConsoleCommandDescription("Displays the clients current network status.")]
        public static void getclientstatus()
        {
            if (Client.net != null)
                Client.print("info", "Connection Status " + Client.net.ConnectionStatus.ToString());
        }

        [ConsoleCommand("getserverconnectioncount", 0)]
        [ConsoleCommandDescription("If a server is running on this computer, Displays the amount of clients connected to the server")]
        public static void getserverconnectioncount()
        {
            if (Server.net != null)
                Server.print("info", "Connection Count " + Server.net.ConnectionsCount.ToString());
        }

        [ConsoleCommand("loadqb", 1)]
        [ConsoleCommandDescription("Loads a .qb file.")]
        [ConsoleArgs("filepath - path to file to load")]
        public static void loadqb(string path)
        {
            if (!File.Exists(path))
            {
                Client.print("error", $"Error : loadqb -File {path}, was not found.");
                return;
            }

            Client.OpenGLContextThread.Add(() =>
            {
                ImportExportUtil.Import(path);
            });
        }

        [ConsoleCommand("loadqbnetworking", 1)]
        [ConsoleCommandDescription("Loads a .qb file internally, then sends the loaded model to all clients connected.")]
        [ConsoleArgs("filepath - path to file to load")]
        public static void loadqbnetworking(string path)
        {
            if (Client.net != null && Client.net.ConnectionsCount == 0)
            {
                Client.print("error", "Error : loadqbnetworking - is meant for network loading of files. Connect your client to a server before running this command, or try using \"loadqb\"");
                return;
            }

            Client.OpenGLContextThread.Add(() =>
            {

                StopwatchUtil.startclient("internalqbread", "Begin Qb Read");
                ImporterQb importer = new ImporterQb();
                QbModel model = importer._read(path);

                StopwatchUtil.stopclient("internalqbread", "End Qb Read");

                StopwatchUtil.startclient("clientwriteqbpacket", "Begin Qb Packet Write");
                var packet = PacketWriter.write<Packet_QbImported>(NetEndpoint.CLIENT);
                packet.write(model);
                packet.send();
                StopwatchUtil.stopclient("clientwriteqbpacket", "End qb Packet Write");

            });
        }


        [ConsoleCommand("fps", 1)]
        [ConsoleCommandDescription("SV will try to match this frame-rate.")]
        [ConsoleArgs("fps - frames per second to shoot for")]
        public static void fps(string s_fps)
        {
            try
            {
                int fps = Convert.ToInt32(s_fps);
                if (fps <= 0)
                    throw new Exception();
                Client.window.targetFps = fps;
                Client.window.TargetRenderFrequency = fps;
                Client.window.TargetUpdateFrequency = fps;
            }
            catch
            {
                Client.print("info", "input value must be non decimal and greater then 0");
            }
        }

        [ConsoleCommand("openglsupport", 0)]
        [ConsoleCommandDescription("Gives feedback about what version of OpenGL you GPU supports.")]
        public static void openglsupport()
        {
            Client.OpenGLContextThread.Add(() =>
            {
                Console.WriteLine();

                var osversion= Environment.OSVersion.VersionString;
                Client.print("debug", osversion);

                string majorminor = GL.GetString(StringName.Version);
                Client.print("debug", string.Format("Available OpenGL version : {0}", majorminor));

                string vendor = GL.GetString(StringName.Vendor);
                Client.print("debug", string.Format("Vendor : {0}", vendor));

                string renderer = GL.GetString(StringName.Renderer);
                Client.print("debug", string.Format("Available Render version : {0}", renderer));

                string glslversion = GL.GetString(StringName.ShadingLanguageVersion);
                Client.print("debug", string.Format("Available Shader version : {0}", glslversion));

                string numberextensions = GL.GetString(StringName.Extensions);
                string[] supports = numberextensions.Split(' ');

                Console.WriteLine();
                Client.print("debug", string.Format("Available Extensions : {0}", supports.Length));

                for (int i = 0; i < supports.Length; i++)
                {
                    Client.print("debug_data", string.Format("    {0}", supports[i]));
                }

                Client.print("debug", "Shader test");
                int shadererrorcount = 0;

                string vertexshaderpath = "./data/shaders/voxel.vs";
                string fragmentshaderpath = "./data/shaders/voxel.fs";

                int vertexshaderid = GL.CreateShader(ShaderType.VertexShader);
                int fragmentshaderid = GL.CreateShader(ShaderType.FragmentShader);

                using (StreamReader r = new StreamReader(vertexshaderpath))
                    GL.ShaderSource(vertexshaderid, r.ReadToEnd());
                using (StreamReader r = new StreamReader(fragmentshaderpath))
                    GL.ShaderSource(fragmentshaderid, r.ReadToEnd());

                GL.CompileShader(vertexshaderid);

                string vertexshadererror = GL.GetShaderInfoLog(vertexshaderid);

                GL.CompileShader(fragmentshaderid);

                string fragmentshadererror = GL.GetShaderInfoLog(fragmentshaderid);

                int shaderid = GL.CreateProgram();
                GL.AttachShader(shaderid, vertexshaderid);
                GL.AttachShader(shaderid, fragmentshaderid);
                GL.LinkProgram(shaderid);

                string shadererror = GL.GetProgramInfoLog(shaderid);

                if (!string.IsNullOrEmpty(vertexshadererror))
                {
                    Client.print("debug_data", string.Format("    {0}", vertexshadererror));
                    shadererrorcount++;
                }

                if (!string.IsNullOrEmpty(fragmentshadererror))
                {
                    Client.print("debug_data", string.Format("    Fragment shader failed compiling : {0}", fragmentshadererror));
                    shadererrorcount++;
                }

                if (!string.IsNullOrEmpty(shadererror))
                {
                    Client.print("debug_data", string.Format("    Shader failed linking: {0}", shadererror));
                    shadererrorcount++;
                }

                if (shadererrorcount > 0)
                {
                    Client.print("debug", "Shader test failed");
                }
                else
                    Client.print("debug", "Shader completed with no errors");

                Console.WriteLine();
                Client.print("info", "Would you like to export info to desktop. (yes/no)");
                string t = Console.ReadLine();
                if (t.ToLower().Contains("y") || t.ToLower().Contains("es"))
                {
                    Client.print("debug", "Info exported to desktop.");

                    using (FileStream file = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "//stonevox_log.txt", FileMode.Create))
                    {
                        using (StreamWriter s = new StreamWriter(file))
                        {
                            s.WriteLine(osversion);
                            s.WriteLine(string.Format("Available OpenGL version : {0}", majorminor));
                            s.WriteLine(string.Format("Vendor : {0}", vendor));
                            s.WriteLine(string.Format("Available Render version : {0}", renderer));
                            s.WriteLine(string.Format("Available Shader version : {0}", glslversion));

                            s.WriteLine();
                            s.WriteLine(string.Format("Available Extensions : {0}", supports.Length));

                            for (int i = 0; i < supports.Length; i++)
                            {
                                s.WriteLine(string.Format("    {0}", supports[i]));
                            }

                            s.WriteLine();

                            s.WriteLine("Shader test");


                            if (!string.IsNullOrEmpty(vertexshadererror))
                            {
                                s.WriteLine(string.Format("   {0}", vertexshadererror));
                            }

                            if (!string.IsNullOrEmpty(fragmentshadererror))
                            {
                                s.WriteLine(string.Format("   Fragment shader failed compiling : {0}", fragmentshadererror));
                            }

                            if (!string.IsNullOrEmpty(shadererror))
                            {
                                s.WriteLine(string.Format("   Shader failed linking: {0}", shadererror));
                            }

                            if (shadererrorcount > 0)
                            {
                                s.WriteLine("Shader test failed");
                            }
                            else
                                s.WriteLine("Shader completed with no errors");
                        }
                    }
                }
                else
                {

                }
            });
        }
    }

    public class ConsoleCommand : System.Attribute
    {
        public string name;
        public int argcount;

        public ConsoleCommand(string cmdname, int argcount)
            : base()
        {
            name = cmdname;
            this.argcount = argcount;
        }
    }

    public class ConsoleCommandDescription : System.Attribute
    {
        public string text;

        public ConsoleCommandDescription(string text)
        {
            this.text = text;
        }
    }

    public class ConsoleArgs : System.Attribute
    {
        public string[] args;

        public ConsoleArgs(params string[] args)
        {
            this.args = args;
        }
    }
}