// -----------------------------------------------------------------------
// <copyright file="Verified.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.API.Features;

    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using PlayerAPI = Exiled.API.Features.Player;
    using PlayerEvents = Exiled.Events.Handlers.Player;

#pragma warning disable SA1600 // Elements should be documented

    [HarmonyPatch(typeof(ServerRoles), nameof(ServerRoles.CmdServerSignatureComplete))]
    internal static class Verified
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var targetMethod = AccessTools.Method(typeof(ServerRoles), nameof(ServerRoles.RefreshPermissions));
            var did = false;

            foreach (var i in instructions)
            {
                if (!did && i.opcode == OpCodes.Call && (MethodInfo)i.operand == targetMethod)
                {
                    did = true;

                    // Think I wanna have a deal with IL?
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Verified), nameof(CallEvent)));
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                }

                yield return i;
            }
        }

        private static void CallEvent(ServerRoles instance)
        {
            try
            {
                var player = PlayerAPI.Get(instance.gameObject);
                player.IsVerified = true;

                PlayerEvents.OnVerified(new VerifiedEventArgs(player));
            }
            catch (Exception ex)
            {
                Log.Error($"{typeof(Verified).FullName}.{nameof(CallEvent)}:\n{ex}");
            }
        }
    }
}
