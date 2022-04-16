// -----------------------------------------------------------------------
// <copyright file="Structures.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Prefabs
{
    using System;
    using Exiled.API.Enums;
    using Mirror;
    using UnityEngine;

    /// <summary>
    /// A helper class for structure prefabs.
    /// </summary>
    public static class Structures
    {
        private static GameObject workstation;
        private static GameObject largeGunLocker;
        private static GameObject rifleRack;
        private static GameObject miscLocker;
        private static GameObject regularMedkit;
        private static GameObject adrenalineMedkit;
        private static GameObject generator;
        private static GameObject scp018Pedestal;
        private static GameObject scp207Pedestal;
        private static GameObject scp244Pedestal;
        private static GameObject scp268Pedestal;
        private static GameObject scp500Pedestal;
        private static GameObject scp1853Pedestal;
        private static GameObject scp2176Pedestal;

        /// <summary>
        /// Gets the WorkStation structure.
        /// </summary>
        public static GameObject WorkStation => workstation ??= NetworkClient.prefabs[Guid.Parse("ad8a455f-062d-dea4-5b47-ac9217d4c58b")];

        /// <summary>
        /// Gets the LargeGunLocker structure.
        /// </summary>
        public static GameObject LargeGunLocker => largeGunLocker ??= NetworkClient.prefabs[Guid.Parse("5ad5dc6d-7bc5-3154-8b1a-3598b96e0d5b")];

        /// <summary>
        /// Gets the RifleRack structure.
        /// </summary>
        public static GameObject RifleRack => rifleRack ??= NetworkClient.prefabs[Guid.Parse("850f84ad-e273-1824-8885-11ae5e01e2f4")];

        /// <summary>
        /// Gets the MiscLocker structure.
        /// </summary>
        public static GameObject MiscLocker => miscLocker ??= NetworkClient.prefabs[Guid.Parse("d54bead1-286f-3004-facd-74482a872ad8")];

        /// <summary>
        /// Gets the Regular Medkit structure.
        /// </summary>
        public static GameObject RegularMedkit => regularMedkit ??= NetworkClient.prefabs[Guid.Parse("5b227bd2-1ed2-8fc4-2aa1-4856d7cb7472")];

        /// <summary>
        /// Gets the Adrenaline Medkit structure.
        /// </summary>
        public static GameObject AdrenalineMedkit => adrenalineMedkit ??= NetworkClient.prefabs[Guid.Parse("db602577-8d4f-97b4-890b-8c893bfcd553")];

        /// <summary>
        /// Gets the Generator structure.
        /// </summary>
        public static GameObject Generator079 => generator ??= NetworkClient.prefabs[Guid.Parse("daf3ccde-4392-c0e4-882d-b7002185c6b8")];

        /// <summary>
        /// Gets the SCP-018 Pedestal structure.
        /// </summary>
        public static GameObject Scp018Pedestal => scp018Pedestal ??= NetworkClient.prefabs[Guid.Parse("a149d3eb-11bd-de24-f9dd-57187f5771ef")];

        /// <summary>
        /// Gets the SCP-207 Pedestal structure.
        /// </summary>
        public static GameObject Scp207Pedestal => scp207Pedestal ??= NetworkClient.prefabs[Guid.Parse("17054030-9461-d104-5b92-9456c9eb0ab7")];

        /// <summary>
        /// Gets the SCP-244 Pedestal structure.
        /// </summary>
        public static GameObject Scp244Pedestal => scp244Pedestal ??= NetworkClient.prefabs[Guid.Parse("fa602fdc-724c-d2a4-8b8c-1fb314b82746")];

        /// <summary>
        /// Gets the SCP-268 Pedestal structure.
        /// </summary>
        public static GameObject Scp268Pedestal => scp268Pedestal ??= NetworkClient.prefabs[Guid.Parse("68f13209-e652-6024-2b89-0f75fb88a998")];

        /// <summary>
        /// Gets the SCP-500 Pedestal structure.
        /// </summary>
        public static GameObject Scp500Pedestal => scp500Pedestal ??= NetworkClient.prefabs[Guid.Parse("f4149b66-c503-87a4-0b93-aabfe7c352da")];

        /// <summary>
        /// Gets the SCP-1853 Pedestal structure.
        /// </summary>
        public static GameObject Scp1853Pedestal => scp1853Pedestal ??= NetworkClient.prefabs[Guid.Parse("4f36c701-ea0c-9064-2a58-2c89240e51ba")];

        /// <summary>
        /// Gets the SCP-2176 Pedestal structure.
        /// </summary>
        public static GameObject Scp2176Pedestal => scp2176Pedestal ??= NetworkClient.prefabs[Guid.Parse("fff1c10c-a719-bea4-d95c-3e262ed03ab2")];

        /// <summary>
        /// Gets the Prefab of a <see cref="StructureType"/>.
        /// </summary>
        /// <param name="structureType">The structure type of the prefab.</param>
        /// <returns>The prefab of the desired structure type.</returns>
        public static GameObject GetPrefab(this StructureType structureType)
        {
            switch (structureType)
            {
                case StructureType.WorkStation:
                    return WorkStation;
                case StructureType.LargeGunLocker:
                    return LargeGunLocker;
                case StructureType.RifleRack:
                    return RifleRack;
                case StructureType.MiscLocker:
                    return MiscLocker;
                case StructureType.Generator:
                    return Generator079;
                case StructureType.RegularMedkit:
                    return RegularMedkit;
                case StructureType.AdrenalineMedkit:
                    return AdrenalineMedkit;
                case StructureType.Scp018Pedestal:
                    return Scp018Pedestal;
                case StructureType.Scp207Pedestal:
                    return Scp207Pedestal;
                case StructureType.Scp244Pedestal:
                    return Scp244Pedestal;
                case StructureType.Scp268Pedestal:
                    return Scp268Pedestal;
                case StructureType.Scp500Pedestal:
                    return Scp500Pedestal;
                case StructureType.Scp1853Pedestal:
                    return Scp1853Pedestal;
                case StructureType.Scp2176Pedestal:
                    return Scp2176Pedestal;
                default:
                    throw new ArgumentOutOfRangeException(nameof(structureType), structureType, null);
            }
        }
    }
}
