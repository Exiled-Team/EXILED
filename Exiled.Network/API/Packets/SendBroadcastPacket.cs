namespace Exiled.Network.API.Packets
{
    public class SendBroadcastPacket
    {
        public bool isAdminOnly { get; set; }
        public string Message { get; set; }
        public ushort Duration { get; set; }
    }
}
