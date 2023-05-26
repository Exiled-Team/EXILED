// -----------------------------------------------------------------------
// <copyright file="Lunge.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp939
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Pools;
    using Exiled.Events.EventArgs.Scp939;
    using HarmonyLib;
    using PlayerRoles;
    using PlayerRoles.PlayableScps.Scp939;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="Scp939LungeAbility.ClientSendHit(HumanRole)"/>
    ///     to add the <see cref="Handlers.Scp939.Lunged"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp939LungeAbility), nameof(Scp939LungeAbility.ClientSendHit))]
    internal static class Lunge
    {
        private static void Postfix(Scp939LungeAbility __instance, HumanRole human)
        {
            LungedEventArgs ev = new(__instance.Owner, human.TryGetOwner(out var hub) ? hub : null);
            Handlers.Scp939.OnLunged(ev);
        }
    }
}