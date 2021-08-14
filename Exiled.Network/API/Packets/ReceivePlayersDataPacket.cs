namespace Exiled.Network.API.Packets
{
    using System.Collections.Generic;

    public class ReceivePlayersDataPacket
    {
        public List<PlayerInfoPacket> Players { get; set; }
    }
}
