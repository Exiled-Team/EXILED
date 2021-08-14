namespace Exiled.Network.API.Packets
{
    public class ExecuteConsoleCommandPacket
    {
        public string AddonID { get; set; }
        public string Command { get; set; }
    }
}
