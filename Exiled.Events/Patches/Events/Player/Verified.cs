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

    [HarmonyPatch(typeof(ServerRoles), nameof(ServerRoles.CallCmdServerSignatureComplete))]
    internal static class Verified
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var targetMethod = AccessTools.Method(typeof(ServerRoles), nameof(ServerRoles.RefreshPermissions));
            var did = false;

            var ienumerator = instructions.GetEnumerator();
            while (ienumerator.MoveNext())
            {
                var i = ienumerator.Current;
                CodeInstruction i2 = ienumerator.MoveNext() ? ienumerator.Current : null;

                if (!did
                    && i.opcode == OpCodes.Ldc_I4_0
                    && i2 != null && i2.opcode == OpCodes.Call && (MethodInfo)i2.operand == targetMethod)
                {
                    did = true;

                    // Think I wanna have a deal with IL?
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Verified), nameof(CallEvent)));
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                }

                yield return i;
                if (i2 != null)
                    yield return i2;
            }
        }

        private static void CallEvent(ServerRoles instance)
        {
            try
            {
                var player = PlayerAPI.Get(instance.gameObject);

                // Means the player connected before WaitingForPlayers event is fired
                // Let's call Joined event, since it wasn't called, to avoid breaking the logic of the order of event calls
                // Blame NorthWood
                if (player == null)
                    Joined.CallEvent(instance._hub, out player);

                player.IsVerified = true;

                Log.SendRaw($"Player {player.Nickname} ({player.UserId}) ({player.Id}) connected with the IP: {player.IPAddress}", ConsoleColor.Green);

                PlayerEvents.OnVerified(new VerifiedEventArgs(player));
            }
            catch (Exception ex)
            {
                Log.Error($"{typeof(Verified).FullName}.{nameof(CallEvent)}:\n{ex}");
            }
        }
    }
}
