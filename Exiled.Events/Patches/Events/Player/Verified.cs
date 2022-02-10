// -----------------------------------------------------------------------
// <copyright file="Verified.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1600
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs;
    using Exiled.Events.Utils;

    using HarmonyLib;

    using PlayerAPI = Exiled.API.Features.Player;
    using PlayerEvents = Exiled.Events.Handlers.Player;

    [HarmonyPatch(typeof(ServerRoles), nameof(ServerRoles.UserCode_CmdServerSignatureComplete))]
    internal static class Verified
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            MethodInfo targetMethod = AccessTools.Method(typeof(ServerRoles), nameof(ServerRoles.RefreshPermissions));
            bool did = false;

            using (NextEnumerator<CodeInstruction> nextEnumerator = new NextEnumerator<CodeInstruction>(instructions.GetEnumerator()))
            {
                while (nextEnumerator.MoveNext())
                {
                    if (!did
                        && nextEnumerator.Current.opcode == OpCodes.Ldc_I4_0
                        && nextEnumerator.NextCurrent != null && nextEnumerator.NextCurrent.opcode == OpCodes.Call && (MethodInfo)nextEnumerator.NextCurrent.operand == targetMethod)
                    {
                        did = true;

                        // Think I wanna have a deal with IL?
                        yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Verified), nameof(CallEvent)));
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                    }

                    yield return nextEnumerator.Current;
                    if (nextEnumerator.NextCurrent != null)
                        yield return nextEnumerator.NextCurrent;
                }
            }
        }

        private static void CallEvent(ServerRoles instance)
        {
            try
            {
                PlayerAPI.UnverifiedPlayers.TryGetValue(instance._hub, out PlayerAPI player);

                // Means the player connected before WaitingForPlayers event is fired
                // Let's call Joined event, since it wasn't called, to avoid breaking the logic of the order of event calls
                // Blame NorthWood
                if (player == null)
                    Joined.CallEvent(instance._hub, out player);

#if DEBUG
                Log.Debug($"{player.Nickname} has verified!", true);
#endif
                PlayerAPI.Dictionary.Add(instance._hub.gameObject, player);
                player.IsVerified = true;
                player.RawUserId = player.UserId.GetRawUserId();

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
