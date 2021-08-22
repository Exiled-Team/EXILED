// -----------------------------------------------------------------------
// <copyright file="IndividualFriendlyFire.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
#pragma warning disable SA1118
#pragma warning disable SA1313
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="HitboxIdentity.CheckFriendlyFire(ReferenceHub, ReferenceHub, bool)"/>.
    /// </summary>
    [HarmonyPatch(typeof(HitboxIdentity), nameof(HitboxIdentity.CheckFriendlyFire), new[] { typeof(ReferenceHub), typeof(ReferenceHub), typeof(bool) })]
    internal static class IndividualFriendlyFire
    {
        private static void Prefix(HitboxIdentity __instance, ReferenceHub attacker, ReferenceHub victim, bool ignoreConfig = false)
        {
            Player player = Player.Get(attacker);
            if (player != null && player.IsFriendlyFireEnabled)
                ignoreConfig = true;
        }
    }
}
