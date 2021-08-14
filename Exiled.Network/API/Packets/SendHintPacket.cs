namespace Exiled.Network.API.Packets
{
    public class SendHintPacket
    {
        public bool isAdminOnly { get; set; }
        public string Message { get; set; }
        public float Duration { get; set; }
    }
}
