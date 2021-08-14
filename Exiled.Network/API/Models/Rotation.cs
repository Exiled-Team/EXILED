namespace Exiled.Network.API.Models
{
    using LiteNetLib.Utils;

    public struct Rotation : INetSerializable
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
        public float w { get; set; }

        public void Deserialize(NetDataReader reader)
        {
            x = reader.GetFloat();
            y = reader.GetFloat();
            z = reader.GetFloat();
            w = reader.GetFloat();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(x);
            writer.Put(y);
            writer.Put(z);
            writer.Put(w);
        }
    }
}
