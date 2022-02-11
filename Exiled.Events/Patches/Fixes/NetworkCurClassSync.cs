// -----------------------------------------------------------------------
// <copyright file="NetworkCurClassSync.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Fixes desync which causes NRE between base <see cref="CharacterClassManager.NetworkCurClass"/> and <see cref="API.Features.Player.Role"/> via
    /// <see cref="CharacterClassManager.SetClassIDAdv(RoleType, bool, CharacterClassManager.SpawnReason, bool)"/>.
    /// </summary>
    [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.SetClassIDAdv))]
    public static class NetworkCurClassSync
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            LocalBuilder player = generator.DeclareLocal(typeof(API.Features.Player));

            int index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldarg_S);

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(CharacterClassManager), nameof(CharacterClassManager._hub))),
                new CodeInstruction(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),
                new CodeInstruction(OpCodes.Stloc_S, player.LocalIndex),
                new CodeInstruction(OpCodes.Ldloc_S, player.LocalIndex),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldloc_S, player.LocalIndex),
                new CodeInstruction(OpCodes.Call, Method(typeof(API.Features.Roles.Role), nameof(API.Features.Roles.Role.Create))),
                new CodeInstruction(OpCodes.Callvirt, PropertySetter(typeof(API.Features.Player), nameof(API.Features.Player.Role))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
