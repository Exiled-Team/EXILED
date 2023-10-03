// -----------------------------------------------------------------------
// <copyright file="NpcPlayerCountFix.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;

    using Authenticator;

    using Exiled.API.Features;
    using Exiled.API.Features.Pools;

    using HarmonyLib;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="AuthenticatorQuery.SendData"/> to remove the NPC player count from the server list data.
    /// </summary>
    [HarmonyPatch(typeof(AuthenticatorQuery), nameof(AuthenticatorQuery.SendData))]
    internal static class NpcPlayerCountFix
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder list = generator.DeclareLocal(typeof(List<string>));
            LocalBuilder s = generator.DeclareLocal(typeof(string));
            LocalBuilder ss = generator.DeclareLocal(typeof(string));
            LocalBuilder v2 = generator.DeclareLocal(typeof(IEnumerator<string>));
            LocalBuilder v4 = generator.DeclareLocal(typeof(int));
            Label entry = generator.DefineLabel();
            Label process = generator.DefineLabel();
            Label skip = generator.DefineLabel();
            Label skip2 = generator.DefineLabel();
            Label end = generator.DefineLabel();

            newInstructions.InsertRange(0, new[]
            {
                new(OpCodes.Call, PropertyGetter(typeof(ListPool<string>), nameof(ListPool<string>.Pool))),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, Method(typeof(ListPool<string>), nameof(ListPool<string>.Get), new[] { typeof(IEnumerable<string>) })),
                new(OpCodes.Stloc, list),
                new(OpCodes.Ldloc, list),
                new(OpCodes.Ldnull),
                new(OpCodes.Stloc, s),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, Method(typeof(IEnumerable<string>), nameof(IEnumerable<string>.GetEnumerator))),
                new(OpCodes.Stloc, v2),
                new(OpCodes.Br_S, entry),
                new CodeInstruction(OpCodes.Ldloc, v2).WithLabels(process),
                new(OpCodes.Callvirt, PropertyGetter(typeof(IEnumerator<string>), nameof(IEnumerator<string>.Current))),
                new(OpCodes.Stloc, ss),
                new(OpCodes.Ldloc, ss),
                new(OpCodes.Ldstr, "players="),
                new(OpCodes.Callvirt, Method(typeof(string), nameof(string.StartsWith), new[] { typeof(string) })),
                new(OpCodes.Brfalse_S, entry),
                new(OpCodes.Ldloc, ss),
                new(OpCodes.Stloc, s),
                new CodeInstruction(OpCodes.Ldloc, v2).WithLabels(entry),
                new(OpCodes.Callvirt, Method(typeof(IEnumerator), nameof(IEnumerator.MoveNext))),
                new(OpCodes.Brtrue_S, process),
                new CodeInstruction(OpCodes.Ldloc, v2),
                new(OpCodes.Brfalse_S, skip),
                new(OpCodes.Ldloc, v2),
                new(OpCodes.Callvirt, Method(typeof(IDisposable), nameof(IDisposable.Dispose))),
                new CodeInstruction(OpCodes.Ldloc, s).WithLabels(skip, end),
                new(OpCodes.Brfalse_S, skip2),
                new(OpCodes.Ldloc, list),
                new(OpCodes.Dup),
                new(OpCodes.Ldloc, s),
                new(OpCodes.Callvirt, Method(typeof(List<string>), nameof(List<string>.IndexOf), new[] { typeof(string) })),
                new(OpCodes.Ldstr, "players="),
                new(OpCodes.Ldsfld, Field(typeof(ServerConsole), nameof(ServerConsole._playersAmount))),
                new(OpCodes.Call, PropertyGetter(typeof(Npc), nameof(Npc.List))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(List<Npc>), nameof(List<Npc>.Count))),
                new(OpCodes.Sub),
                new(OpCodes.Stloc, v4),
                new(OpCodes.Ldloca_S, v4),
                new(OpCodes.Call, Method(typeof(int), nameof(int.ToString))),
                new(OpCodes.Ldstr, "/"),
                new(OpCodes.Ldsflda, Field(typeof(CustomNetworkManager), nameof(CustomNetworkManager.slots))),
                new(OpCodes.Call, Method(typeof(int), nameof(int.ToString))),
                new(OpCodes.Call, Method(typeof(string), nameof(string.Concat), new[] { typeof(string), typeof(string), typeof(string), typeof(string) })),
                new(OpCodes.Callvirt, Method(typeof(List<string>), "set_Item")),
                new CodeInstruction(OpCodes.Nop).WithLabels(skip2),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}