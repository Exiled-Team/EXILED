// -----------------------------------------------------------------------
// <copyright file="LiftList.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using API.Features;

    using HarmonyLib;
    using Interactables.Interobjects;

    /// <summary>
    /// Patches <see cref="ElevatorManager.SpawnChamber"/>.
    /// </summary>
    [HarmonyPatch(typeof(ElevatorManager), nameof(ElevatorManager.SpawnAllChambers))]
    internal class LiftList
    {
        private static void Postfix()
        {
            Lift.ElevatorChamberToLift.Clear();

            foreach (ElevatorChamber lift in ElevatorChamber.AllChambers)
            {
                Lift.Get(lift);
            }
        }
    }
}