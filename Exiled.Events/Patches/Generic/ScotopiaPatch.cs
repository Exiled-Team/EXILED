// -----------------------------------------------------------------------
// <copyright file="ScotopiaPatch.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    using Player = Exiled.API.Features.Player;

    /// <summary>
    /// Patches <see cref="LocalCurrentRoomEffects.FixedUpdate"/>.
    /// </summary>
    [HarmonyPatch(typeof(LocalCurrentRoomEffects), nameof(LocalCurrentRoomEffects.FixedUpdate))]
    internal static class ScotopiaPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            int index = newInstructions.Count - 2;

            LocalBuilder player = generator.DeclareLocal(typeof(Player));
            LocalBuilder value = generator.DeclareLocal(typeof(bool?));
            LocalBuilder curVal = generator.DeclareLocal(typeof(bool));

            Label cdc = generator.DefineLabel();
            Label jcc = generator.DefineLabel();

            newInstructions[index].labels.Add(cdc);

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(LocalCurrentRoomEffects), nameof(LocalCurrentRoomEffects._hub))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, player.LocalIndex),
                    new(OpCodes.Brfalse_S, cdc),
                    new(OpCodes.Ldloc_S, player.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.HasScotopia))),
                    new(OpCodes.Stloc_S, value.LocalIndex),
                    new(OpCodes.Ldloca_S, value.LocalIndex),
                    new(OpCodes.Callvirt, Method(typeof(bool?), "get_HasValue")),
                    new(OpCodes.Brfalse_S, cdc),
                    new(OpCodes.Pop),
                    new(OpCodes.Ldloca_S, value.LocalIndex),
                    new(OpCodes.Callvirt, Method(typeof(bool?), "get_Value")),
                    new(OpCodes.Stloc_S, curVal.LocalIndex),
                    new(OpCodes.Ldloc_S, player.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.IsScp))),
                    new(OpCodes.Brtrue_S, jcc),
                    new(OpCodes.Ldloc_S, curVal.LocalIndex),
                    new(OpCodes.Ldc_I4_0),
                    new(OpCodes.Ceq),
                    new(OpCodes.Br_S, cdc),
                    new CodeInstruction(OpCodes.Ldloc_S, curVal.LocalIndex).WithLabels(jcc),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}