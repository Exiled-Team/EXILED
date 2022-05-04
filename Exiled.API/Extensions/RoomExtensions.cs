namespace Exiled.API.Extensions
{
    using System.Collections.Generic;
    using Exiled.API.Enums;

    /// <summary>
    /// A set of extensions for <see cref="RoomType"/>.
    /// </summary>
    public static class RoomExtensions
    {
        /// <summary>
        /// Checks if a <see cref="RoomType">room type</see> is a gate.
        /// </summary>
        /// <param name="room">The room to be checked.</param>
        /// <returns>Returns whether the <see cref="RoomType"/> is a gate or not.</returns>
        public static bool IsGate(this RoomType room)
        {
            return room is RoomType.EzGateA or RoomType.EzGateB;
        }

        /// <summary>
        /// Checks if a <see cref="RoomType">room type</see> is a checkpoint.
        /// </summary>
        /// <param name="room">The room to be checked.</param>
        /// <returns>Returns whether the <see cref="RoomType"/> is an checkpoint or not.</returns>
        public static bool IsCheckpoint(this RoomType room)
        {
            return room is RoomType.LczChkpA
                or RoomType.LczChkpB or RoomType.HczEzCheckpoint
                or RoomType.HczChkpA or RoomType.HczChkpB;
        }

        /// <summary>
        ///  Checks if a <see cref="RoomType">room type</see> is a SCP room.
        /// </summary>
        /// <param name="room">The room to be checked.</param>
        /// <returns>Returns whether the <see cref="RoomType"/> is a SCP room or not.</returns>
        public static bool IsScpRoom(this RoomType room)
        {
            List<RoomType> scpRooms = new()
            {
                RoomType.Lcz173,
                RoomType.Lcz330,
                RoomType.Lcz914,
                RoomType.Hcz049,
                RoomType.Hcz079,
                RoomType.Hcz096,
                RoomType.Hcz106,
                RoomType.Hcz939,
            };

            return scpRooms.Contains(room);
        }
    }
}
