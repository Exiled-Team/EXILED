// -----------------------------------------------------------------------
// <copyright file="LiftList.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using System.Collections.Generic;

    using API.Features;

    using HarmonyLib;
    using Interactables.Interobjects;

    /// <summary>
    /// Patches <see cref="ElevatorManager.RefreshChambers"/>.
    /// </summary>
    [HarmonyPatch(typeof(ElevatorManager), nameof(ElevatorManager.RefreshChambers))]
    internal class LiftList
    {
        private static void Postfix()
        {
            Lift.ElevatorChamberToLift.Clear();

            foreach (KeyValuePair<ElevatorManager.ElevatorGroup, ElevatorChamber> lift in ElevatorManager.SpawnedChambers)
            {
                Lift.Get(lift.Value);
            }
        }
    }
}