// -----------------------------------------------------------------------
// <copyright file="SendTargetRpcInternalPatch.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Dummies
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using HarmonyLib;

    using Mirror;

    using NorthwoodLib.Pools;

    using RemoteAdmin;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="NetworkBehaviour.SendTargetRPCInternal"/> to prevent network garbage with dummies.
    /// </summary>
    [HarmonyPatch(typeof(NetworkBehaviour), nameof(NetworkBehaviour.SendTargetRPCInternal))]
    internal static class SendTargetRpcInternalPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label ret = generator.DefineLabel();

            newInstructions.InsertRange(0, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(NetworkBehaviour), nameof(NetworkBehaviour.netIdentity))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(NetworkIdentity), nameof(NetworkIdentity.netId))),
                new CodeInstruction(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(uint) })),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(API.Features.Player), nameof(API.Features.Player.IsDummy))),
                new CodeInstruction(OpCodes.Brtrue_S, ret),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
