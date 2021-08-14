namespace Exiled.Network.API.Packets
{
    public class PlayerInteractPacket
    {
        public string UserID { get; set; }
        public byte Type { get; set; }
        public byte[] Data { get; set; }
    }
}
