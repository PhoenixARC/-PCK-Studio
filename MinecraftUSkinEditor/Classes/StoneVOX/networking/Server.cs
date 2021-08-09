using Lidgren.Network;
using System;
using System.Threading;

namespace stonevox
{
    public static class Server
    {
        public static NetServer net;
        public static NetPeerConfiguration config = new NetPeerConfiguration(NetConfig.NETWORK_IDENTIFIER);

        private static NetIncomingMessage message;

        public static void defaultConfigure()
        {
            print("info", "Configured with default values");
            config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            config.MaximumConnections = 32;
            config.Port = NetConfig.NETWORK_PORT;
        }

        public static void start()
        {
            net = new NetServer(config);

            net.RegisterReceivedCallback(new SendOrPostCallback((e) =>
            {
                message = ((NetServer)e).ReadMessage();
                messageRecieved(message);
            }), new SynchronizationContext());

            print("info", "Starting...");
            net.Start();
            print("info", "Server running");
        }

        private static void messageRecieved(NetIncomingMessage message)
        {
            switch (message.MessageType)
            {
                case NetIncomingMessageType.DiscoveryRequest:
                    print("info", "Got discoverz request from client");
                    NetOutgoingMessage response = net.CreateMessage();
                    response.Write((byte)1);
                    net.SendDiscoveryResponse(response, message.SenderEndPoint);
                    break;
                case NetIncomingMessageType.ConnectionApproval:

                    string onion = message.ReadString();
                    string name = message.ReadString();

                    if (!string.IsNullOrEmpty(name))
                    {
                        print("info", string.Format("Connection to {0} accepted", name));
                    }
                    else
                        print("info", "Connection accepted");

                    if (onion == "onion")
                        message.SenderConnection.Approve();
                    else
                        message.SenderConnection.Deny();
                    break;
                case NetIncomingMessageType.DebugMessage:
                    print("info", "Debug: " + message.ReadString());
                    break;
                case NetIncomingMessageType.StatusChanged:
                    switch ((NetConnectionStatus)message.ReadByte())
                    {
                        case NetConnectionStatus.Connected:
                            break;
                        case NetConnectionStatus.Disconnected:
                            string reason = message.ReadString();
                            Server.print("info", "Client disconnected");
                            break;
                    }
                    break;
                case NetIncomingMessageType.Data:
                    PacketHandler.handle(message, NetEndpoint.SERVER);
                    break;
            }
            net.Recycle(message);
        }

        public static void print(string Type, string text)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("(SERVER) ");

            switch (Type)
            {
                case "info":
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(text + '\r' + '\n');
                    break;
            }
        }
    }
}