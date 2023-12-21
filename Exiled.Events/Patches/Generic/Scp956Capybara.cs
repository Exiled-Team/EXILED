// -----------------------------------------------------------------------
// <copyright file="Scp956Capybara.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using Exiled.API.Features;
    using HarmonyLib;

#pragma warning disable SA1313

    /// <summary>
    /// Patches <see cref="Scp956Pinata.UpdateAi"/>
    /// to implement better pinata capybara.
    /// </summary>
    [HarmonyPatch(typeof(Scp956Pinata), nameof(Scp956Pinata.UpdateAi))]
    internal class Scp956Capybara
    {
        private static void Postfix(Scp956Pinata __instance)
        {
            if (Exiled.Events.Events.Instance.Config.Is956Capybara || Scp956.IsCapybara)
                __instance.Network_carpincho = 69;
        }
    }
}