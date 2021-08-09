namespace stonevox
{
    public static class PacketWriter
    {
        public static T write<T>(NetEndpoint endpoint) where T : Packet, new()
        {
            T t = new T();
            t.endpoint = endpoint;

            switch (endpoint)
            {
                case NetEndpoint.NONE:
                    break;
                case NetEndpoint.CLIENT:
                    t.outgoingmessage = Client.net.CreateMessage();
                    break;
                case NetEndpoint.SERVER:
                    t.outgoingmessage = Server.net.CreateMessage();
                    break;
            }

            t.writedefault();

            return t;
        }
    }
}