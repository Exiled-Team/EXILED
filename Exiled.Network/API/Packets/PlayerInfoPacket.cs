namespace Exiled.Network.API.Packets
{
    using LiteNetLib.Utils;

    public struct PlayerInfoPacket : INetSerializable
    {
        public string UserID;

        public void Deserialize(NetDataReader reader)
        {
            UserID = reader.GetString();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(UserID);
        }
    }
}
