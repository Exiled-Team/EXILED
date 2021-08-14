namespace Exiled.Network.API.Packets
{
    public class ConsoleResponsePacket
    {
        public bool isRemoteAdmin { get; set; }
        public string Command { get; set; }
        public string Response { get; set; }
    }
}
