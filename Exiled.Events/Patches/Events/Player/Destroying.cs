// -----------------------------------------------------------------------
// <copyright file="Destroying.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1118
#pragma warning disable SA1600
    using System.Collections.Generic;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;

    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    [HarmonyPatch(typeof(ReferenceHub), nameof(ReferenceHub.OnDestroy))]
    internal static class Destroying
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            LocalBuilder player = generator.DeclareLocal(typeof(API.Features.Player));

            Label cdcLabel = generator.DefineLabel();

            newInstructions[0].labels.Add(cdcLabel);

            newInstructions.InsertRange(0, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc_S, player.LocalIndex),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(API.Features.Server), nameof(API.Features.Server.Host))),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Brtrue_S, cdcLabel),
                new CodeInstruction(OpCodes.Ldloc_S, player.LocalIndex),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(DestroyingEventArgs))[0]),
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnDestroying))),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(API.Features.Player), nameof(API.Features.Player.Dictionary))),
                new CodeInstruction(OpCodes.Ldloc_S, player.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(API.Features.Player), nameof(API.Features.Player.GameObject))),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(Dictionary<UnityEngine.GameObject, API.Features.Player>), nameof(Dictionary<UnityEngine.GameObject, API.Features.Player>.Remove), new[] { typeof(UnityEngine.GameObject) })),
                new CodeInstruction(OpCodes.Pop),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(API.Features.Player), nameof(API.Features.Player.UnverifiedPlayers))),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(ConditionalWeakTable<ReferenceHub, API.Features.Player>), nameof(ConditionalWeakTable<ReferenceHub, API.Features.Player>.Remove), new[] { typeof(ReferenceHub) })),
                new CodeInstruction(OpCodes.Pop),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(API.Features.Player), nameof(API.Features.Player.IdsCache))),
                new CodeInstruction(OpCodes.Ldloc_S, player.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(API.Features.Player), nameof(API.Features.Player.Id))),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(Dictionary<int, API.Features.Player>), nameof(Dictionary<int, API.Features.Player>.Remove), new[] { typeof(int) })),
                new CodeInstruction(OpCodes.Pop),
                new CodeInstruction(OpCodes.Ldloc_S, player.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(API.Features.Player), nameof(API.Features.Player.UserId))),
                new CodeInstruction(OpCodes.Brfalse_S, cdcLabel),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(API.Features.Player), nameof(API.Features.Player.UserIdsCache))),
                new CodeInstruction(OpCodes.Ldloc_S, player.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(API.Features.Player), nameof(API.Features.Player.UserId))),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(Dictionary<string, API.Features.Player>), nameof(Dictionary<string, API.Features.Player>.Remove), new[] { typeof(string) })),
                new CodeInstruction(OpCodes.Pop),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
