using Lidgren.Network;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Threading;

namespace stonevox
{
    public class Client
    {
        public static GLWindow window;

        public static NetClient net;
        public static NetPeerConfiguration config = new NetPeerConfiguration(NetConfig.NETWORK_IDENTIFIER);
        private static NetIncomingMessage message;

        public static string ID;

        static ManualResetEvent waithandle;

        public static List<Action> OpenGLContextThread = new List<Action>();

        public static bool Initialized;

        public static void defaultConfigure()
        {
            print("info", "Configured with default values");
            config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
            config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);

            ID = Program.Encrypt(NetworkUtil.getIP());
        }

        public static void start()
        {
            net = new NetClient(config);

            net.RegisterReceivedCallback(new SendOrPostCallback((e) =>
            {
                message = net.ReadMessage();
                messageRecieved(message);
            }), new SynchronizationContext());

            print("info", "Starting...");
            net.Start();
            print("info", "Client Running");
        }

        public static void localconnect()
        {
            print("info", "Messaging local server");
            connect(NetConfig.NETWORK_LOCALHOST, NetConfig.NETWORK_PORT);
        }

        public static void connect(string ip, int port)
        {
            print("info", "Attempting to connect...");
            var message = Client.net.CreateMessage();
            message.Write("onion");
            net.Connect(ip, port, message);

            if (waithandle == null)
            {
                waithandle = new ManualResetEvent(false);
                waithandle.WaitOne();
            }
            else
                waithandle.WaitOne();
        }

        public static void disconnect()
        {
            net.Disconnect("see yahh! ;)");
        }

        public static void beginstonevox()
        {
            print("info", "StoneVox starting...");
            try
            {
                //window = new GLWindow(640, 480, new GraphicsMode(new ColorFormat(32), 8, 0, 4));
            }
            catch
            {
                //window = new GLWindow(640, 480, new GraphicsMode(new ColorFormat(32), 8, 0, 0));
            }
            window.Context.SwapInterval = 0;

#if DEBUG
            window.Run_NoErrorCatching(120);
#else
            window.Run_WithErrorCatching(120);
#endif

        }

        public static void update()
        {
            //NetIncomingMessage message = net.ReadMessage();

            //if (message != null)
            //    messageRecieved(message);

            while (OpenGLContextThread.Count > 0)
            {
                OpenGLContextThread[0].Invoke();
                OpenGLContextThread.RemoveAt(0);
            }
        }

        private static void messageRecieved(NetIncomingMessage message)
        {
            switch (message.MessageType)
            {
                case NetIncomingMessageType.DiscoveryResponse:
                    print("info", "Got response from server.");
                    net.Connect(message.SenderEndPoint);
                    print("info", "Attempting to connect to server... " + message.SenderEndPoint.ToString());
                    break;
                case NetIncomingMessageType.DebugMessage:
                    print("info", "Debug: " + message.ReadString());
                    break;
                case NetIncomingMessageType.StatusChanged:
                    switch ((NetConnectionStatus)message.ReadByte())
                    {
                        case NetConnectionStatus.Connected:
                            Client.print("info", "Joined server.");
                            waithandle.Set();
                            break;
                        case NetConnectionStatus.Disconnected:
                            string reason = message.ReadString();
                            Client.print("info", "Disconnected...");
                            waithandle.Set();
                            break;
                    }
                    break;
                case NetIncomingMessageType.Data:
                    PacketHandler.handle(message, NetEndpoint.CLIENT);
                    break;
            }
            net.Recycle(message);
        }

        public static void print(string type, string text, int indent = 0)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("(CLIENT) ");

            if (indent > 0)
            {
                for (int i = 0; i < indent; i++)
                    text = "         " + text;
            }

            switch (type)
            {
                case "info":
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write(text + '\r' + '\n');
                    break;
                case "debug":
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write(text + '\r' + '\n');
                    break;
                case "debug_data":
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write(text + '\r' + '\n');
                    break;
                case "error":
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Write(text + '\r' + '\n');
                    break;
            }

            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}