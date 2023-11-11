// -----------------------------------------------------------------------
// <copyright file="StartingRecall.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp049
{
	using System.Collections.Generic;
	using System.Reflection.Emit;

	using API.Features;
	using API.Features.Pools;
	using Exiled.Events.Attributes;
	using Exiled.Events.EventArgs.Scp049;

	using HarmonyLib;

	using PlayerRoles.PlayableScps.Scp049;

	using static HarmonyLib.AccessTools;

	/// <summary>
	///		Patches <see cref="Scp049ResurrectAbility.ServerValidateBegin" />.
	///		Adds the <see cref="Handlers.Scp049.StartingRecall" /> event.
	/// </summary>
	[EventPatch(typeof(Handlers.Scp049), nameof(Handlers.Scp049.StartingRecall))]
    [HarmonyPatch(typeof(Scp049ResurrectAbility), "ServerValidateBegin")]
    internal static class Scp049ResurrectAbilityPatch
    {
        static bool Prefix(ref byte __result, Scp049ResurrectAbility __instance, BasicRagdoll ragdoll)
        {

            Player player = Player.Get(__instance.Owner);

            Ragdoll doll = Ragdoll.Get(ragdoll);

            StartingRecallEventArgsMod ev = new(player, doll, true);

            LimitZombie.ResurrectHandler.OnResurrect(ev);

            bool isAllowed = ev.IsAllowed;

            if (!isAllowed)
            {
                __result = 1;
                return false;
            }

            ResurrectError resurrectError = __instance.CheckBeginConditions(ragdoll);

            if (resurrectError != 0)
            {
                __result = (byte)resurrectError;
                return false;
            }

            if (!__instance.ServerValidateAny())
            {
                __result = 1;
                return false;
            }

            return true;
        }
    }
}