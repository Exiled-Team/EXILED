﻿namespace Exiled.API.Extensions
{
    using System.Collections.Generic;
    using Exiled.API.Enums;

    /// <summary>
    /// A set of extensions for <see cref="RoomType"/>.
    /// </summary>
    public static class RoomExtensions
    {
        /// <summary>
        /// The list of SCP rooms.
        /// </summary>
        private static readonly List<RoomType> ScpRooms = new()
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

        /// <summary>
        /// Checks if a <see cref="RoomType">room type</see> is a gate.
        /// </summary>
        /// <param name="room">The room to be checked.</param>
        /// <returns>Returns whether the <see cref="RoomType"/> is a gate or not.</returns>
        public static bool IsGate(this RoomType room) => room is RoomType.EzGateA or RoomType.EzGateB;

        /// <summary>
        /// Checks if a <see cref="RoomType">room type</see> is a checkpoint.
        /// </summary>
        /// <param name="room">The room to be checked.</param>
        /// <returns>Returns whether the <see cref="RoomType"/> is a checkpoint or not.</returns>
        public static bool IsCheckpoint(this RoomType room) => room is RoomType.LczChkpA
            or RoomType.LczChkpB or RoomType.HczEzCheckpoint
            or RoomType.HczChkpA or RoomType.HczChkpB;

        /// <summary>
        /// Checks if a <see cref="RoomType">room type</see> is a Lcz checkpoint.
        /// </summary>
        /// <param name="room">The room to be checked.</param>
        /// <returns>Returns whether the <see cref="RoomType"/> is a Lcz checkpoint or not.</returns>
        public static bool IsLczCheckpoint(this RoomType room) => room is RoomType.LczChkpA or RoomType.LczChkpB;

        /// <summary>
        /// Checks if a <see cref="RoomType">room type</see> is a Hcz checkpoint.
        /// </summary>
        /// <param name="room">The room to be checked.</param>
        /// <returns>Returns whether the <see cref="RoomType"/> is a Hcz checkpoint or not.</returns>
        public static bool IsHczCheckpoint(this RoomType room) => room is RoomType.HczChkpA or RoomType.HczChkpB;

        /// <summary>
        ///  Checks if a <see cref="RoomType">room type</see> contains any SCP.
        /// </summary>
        /// <param name="room">The room to be checked.</param>
        /// <returns>Returns whether the <see cref="RoomType"/> contains any SCP or not.</returns>
        public static bool ContainsScp(this RoomType room) => ScpRooms.Contains(room);
    }
}
