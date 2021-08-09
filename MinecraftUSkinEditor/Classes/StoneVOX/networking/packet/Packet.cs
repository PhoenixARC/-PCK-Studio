using Lidgren.Network;

namespace stonevox
{
    public abstract class Packet : IPacket
    {
        public abstract PacketID ID { get; }

        public NetOutgoingMessage outgoingmessage;
        public NetEndpoint endpoint;

        public void writedefault()
        {
            outgoingmessage.Write((int)ID);
        }

        public void send()
        {
            switch (endpoint)
            {
                case NetEndpoint.NONE:
                    break;
                case NetEndpoint.CLIENT:
                    Client.net.SendMessage(outgoingmessage, NetDeliveryMethod.ReliableOrdered);
                    break;
                case NetEndpoint.SERVER:
                    Server.net.SendToAll(outgoingmessage, NetDeliveryMethod.ReliableOrdered);
                    break;
            }
        }

        public void send(NetConnection exclude)
        {
            switch (endpoint)
            {
                case NetEndpoint.NONE:
                    break;
                case NetEndpoint.CLIENT:
                    Client.net.SendMessage(outgoingmessage, NetDeliveryMethod.ReliableOrdered);
                    break;
                case NetEndpoint.SERVER:
                    foreach (var c in Server.net.Connections)
                    {
                        if (c.RemoteUniqueIdentifier != exclude.RemoteUniqueIdentifier)
                        {
                            Server.net.SendMessage(outgoingmessage, c, NetDeliveryMethod.ReliableOrdered);
                            Server.print("info", "Send qbimport packet to client");
                        }
                    }
                    break;
            }
        }

        public virtual void onclientrecieve(NetIncomingMessage message)
        {
        }

        public virtual void onserverrecieve(NetIncomingMessage message)
        {
        }
    }
}