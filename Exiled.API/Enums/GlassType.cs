// -----------------------------------------------------------------------
// <copyright file="GlassType.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    using Features;

    /// <summary>
    /// Unique identifier for the different types of Window.
    /// </summary>
    /// <seealso cref="Window.Type"/>
    public enum GlassType
    {
        /// <summary>
        /// Represents an unknown window.
        /// </summary>
        Unknown,

        /// <summary>
        /// Represents all the windows in <see cref="RoomType.LczGlassBox"/>.
        /// </summary>
        GR18,

        /// <summary>
        /// Represents the window in <see cref="RoomType.Hcz049"/>.
        /// </summary>
        Scp049,

        /// <summary>
        /// Represents the windows in <see cref="RoomType.HczHid"/>.
        /// </summary>
        MicroHid,

        /// <summary>
        /// Represents the window in <see cref="RoomType.Hcz079"/>.
        /// </summary>
        Scp079,

        /// <summary>
        /// Represents the <see cref="Recontainer.ActivatorWindow"/>.
        /// </summary>
        Scp079Trigger,

        /// <summary>
        /// Represents the window in <see cref="RoomType.Lcz330"/>.
        /// </summary>
        Scp330,

        /// <summary>
        /// Represents all the windows in <see cref="RoomType.LczPlants"/>.
        /// </summary>
        Plants,

        /// <summary>
        /// Represents all the windows in <see cref="RoomType.HczEzCheckpointA"/>.
        /// </summary>
        HczEzCheckpointA,

        /// <summary>
        /// Represents all the windows in <see cref="RoomType.HczEzCheckpointB"/>.
        /// </summary>
        HczEzCheckpointB,

        /// <summary>
        /// Represents the window in <see cref="RoomType.HczTestRoom"/>.
        /// </summary>
        TestRoom,
    }
}