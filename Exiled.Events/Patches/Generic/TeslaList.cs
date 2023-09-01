// -----------------------------------------------------------------------
// <copyright file="TeslaList.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using API.Features;

    using HarmonyLib;

    /// <summary>
    /// Patches <see cref="TeslaGateController.Start"/>.
    /// </summary>
    [HarmonyPatch(typeof(TeslaGateController), nameof(TeslaGateController.Start))]
    internal class TeslaList
    {
        private static void Postfix()
        {
            TeslaGate.BaseTeslaGateToTeslaGate.Clear();
        }
    }
}