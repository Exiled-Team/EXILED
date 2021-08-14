namespace Exiled.Network.API.Packets
{
    using System.Collections.Generic;

    public class ReceiveCommandsPacket
    {
        public List<CommandInfoPacket> Commands { get; set; }
    }
}
