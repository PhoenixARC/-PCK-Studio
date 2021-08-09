using Lidgren.Network;

namespace stonevox
{
    public interface IPacket
    {
        PacketID ID { get; }
        void onclientrecieve(NetIncomingMessage message);
        void onserverrecieve(NetIncomingMessage message);
    }
}