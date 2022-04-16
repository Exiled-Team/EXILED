// -----------------------------------------------------------------------
// <copyright file="StructureType.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    /// <summary>
    /// All of the spawnable structures.
    /// </summary>
    public enum StructureType
    {
        /// <summary>
        /// The Work Station structure.
        /// </summary>
        WorkStation,

        /// <summary>
        /// The Large Gun Locker structure.
        /// </summary>
        LargeGunLocker,

        /// <summary>
        /// The Rifle Rack structure.
        /// </summary>
        RifleRack,

        /// <summary>
        /// The Misc Locker structure.
        /// </summary>
        MiscLocker,

        /// <summary>
        /// The 079 Generator structure.
        /// <remarks>
        /// When you spawn this structure, it is automatically added to the generators list.
        /// </remarks>
        /// </summary>
        Generator,

        /// <summary>
        /// The Regular Medkit structure.
        /// </summary>
        RegularMedkit,

        /// <summary>
        /// The Adrenaline Medkit structure.
        /// </summary>
        AdrenalineMedkit,

        /// <summary>
        /// The SCP Pedestal of the SCP-018.
        /// </summary>
        Scp018Pedestal,

        /// <summary>
        /// The SCP Pedestal of the SCP-207.
        /// </summary>
        Scp207Pedestal,

        /// <summary>
        /// The SCP Pedestal of the SCP-244.
        /// </summary>
        Scp244Pedestal,

        /// <summary>
        /// The SCP Pedestal of the SCP-268.
        /// </summary>
        Scp268Pedestal,

        /// <summary>
        /// The SCP Pedestal of the SCP-500.
        /// </summary>
        Scp500Pedestal,

        /// <summary>
        /// The SCP Pedestal of the SCP-1853.
        /// </summary>
        Scp1853Pedestal,

        /// <summary>
        /// The SCP Pedestal of the SCP-2176.
        /// </summary>
        Scp2176Pedestal,
    }
}
