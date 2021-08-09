using Lidgren.Network;

namespace stonevox
{
    public class Packet_MatrixRename : Packet
    {
        public override PacketID ID
        {
            get { return PacketID.MATRIX_RENAME; }
        }

        public override void onclientrecieve(NetIncomingMessage message)
        {
            int matrixid = message.ReadInt32();
            string name = message.ReadString();

            base.onclientrecieve(message);
        }

        public override void onserverrecieve(NetIncomingMessage message)
        {
            Packet_MatrixRename packet = PacketWriter.write<Packet_MatrixRename>(NetEndpoint.SERVER);
            packet.outgoingmessage.Write(message.ReadInt32());
            packet.outgoingmessage.Write(message.ReadString());
            packet.send();

            base.onserverrecieve(message);
        }

        public void write(int matrixID, string newname)
        {
            outgoingmessage.Write(matrixID);
            outgoingmessage.Write(newname);
        }
    }
}