namespace Exiled.Network.API.Packets
{
    public class ExecuteCommandPacket
    {
        public string UserID { get; set; }
        public string AddonID { get; set; }
        public string CommandName { get; set; }
        public string[] Arguments { get; set; }
    }
}
