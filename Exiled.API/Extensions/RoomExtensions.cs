// -----------------------------------------------------------------------
// <copyright file="RoomExtensions.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Extensions
{
    using Exiled.API.Enums;
    using MapGeneration;

    /// <summary>
    /// A set of extensions for <see cref="RoomType"/> and <see cref="ZoneType"/>.
    /// </summary>
    public static class RoomExtensions
    {
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
        public static bool IsCheckpoint(this RoomType room) => room is RoomType.LczCheckpointA
            or RoomType.LczCheckpointB or RoomType.HczEzCheckpointA or RoomType.HczEzCheckpointB;

        /// <summary>
        /// Checks if a <see cref="RoomType">room type</see> is a Lcz checkpoint.
        /// </summary>
        /// <param name="room">The room to be checked.</param>
        /// <returns>Returns whether the <see cref="RoomType"/> is a Lcz checkpoint or not.</returns>
        public static bool IsLczCheckpoint(this RoomType room) => room is RoomType.LczCheckpointA or RoomType.LczCheckpointB;

        /// <summary>
        /// Checks if a <see cref="RoomType">room type</see> is a Hcz checkpoint.
        /// </summary>
        /// <param name="room">The room to be checked.</param>
        /// <returns>Returns whether the <see cref="RoomType"/> is a Hcz checkpoint or not.</returns>
        public static bool IsHczCheckpoint(this RoomType room) => room is RoomType.HczEzCheckpointA or RoomType.HczEzCheckpointB;

        /// <summary>
        ///  Checks if a <see cref="RoomType">room type</see> contains any SCP.
        /// </summary>
        /// <param name="room">The room to be checked.</param>
        /// <returns>Returns whether the <see cref="RoomType"/> contains any SCP or not.</returns>
        public static bool IsScp(this RoomType room)
            => room is RoomType.Lcz173 or RoomType.Lcz330 or RoomType.Lcz914 or RoomType.Hcz049 or RoomType.Hcz079 or
                RoomType.Hcz096 or RoomType.Hcz106 or RoomType.Hcz939;

        /// <summary>
        /// Converts the provided <see cref="FacilityZone"/> into the corresponding <see cref="ZoneType"/>.
        /// </summary>
        /// <param name="facility">The <see cref="FacilityZone"/> to convert.</param>
        /// <returns>ZoneType.</returns>
        public static ZoneType GetZone(this FacilityZone facility) => facility switch
        {
            FacilityZone.HeavyContainment => ZoneType.HeavyContainment,
            FacilityZone.LightContainment => ZoneType.LightContainment,
            FacilityZone.Entrance => ZoneType.Entrance,
            FacilityZone.Surface => ZoneType.Surface,
            FacilityZone.Other => ZoneType.Other,
            _ => ZoneType.Unspecified,
        };

        /// <summary>
        /// Converts the provided <see cref="ZoneType"/> into the corresponding <see cref="FacilityZone"/>.
        /// </summary>
        /// <param name="facility">The <see cref="ZoneType"/> to convert.</param>
        /// <returns>FacilityZone.</returns>
        public static FacilityZone GetZone(this ZoneType facility) => facility switch
        {
            ZoneType.HeavyContainment => FacilityZone.HeavyContainment,
            ZoneType.LightContainment => FacilityZone.LightContainment,
            ZoneType.Entrance => FacilityZone.Entrance,
            ZoneType.Surface => FacilityZone.Surface,
            ZoneType.Other => FacilityZone.Other,
            _ => FacilityZone.Other,
        };
    }
}