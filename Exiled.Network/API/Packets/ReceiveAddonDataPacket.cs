namespace Exiled.Network.API.Packets
{
    public class ReceiveAddonDataPacket
    {
        public string AddonID { get; set; }
        public byte[] Data { get; set; }
    }
}
