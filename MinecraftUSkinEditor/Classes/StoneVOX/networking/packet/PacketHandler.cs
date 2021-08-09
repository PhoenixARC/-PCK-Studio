using Lidgren.Network;
using System.Collections.Generic;

namespace stonevox
{
    public static class PacketHandler
    {
        public static Dictionary<PacketID, Packet> handlers = new Dictionary<PacketID, Packet>();

        static PacketHandler()
        {
            handlers.Add(PacketID.MATRIX_RENAME, new Packet_MatrixRename());
            handlers.Add(PacketID.CHAT, new Packet_Chat());
            handlers.Add(PacketID.QB_IMPORTED, new Packet_QbImported());
        }

        public static void handle(NetIncomingMessage message, NetEndpoint endpoint)
        {
            PacketID id = (PacketID)message.ReadInt32();

            switch (endpoint)
            {
                case NetEndpoint.NONE:
                    break;
                case NetEndpoint.CLIENT:
                    handlers[id].onclientrecieve(message);
                    break;
                case NetEndpoint.SERVER:
                    handlers[id].onserverrecieve(message);
                    break;
            }
        }
    }
}