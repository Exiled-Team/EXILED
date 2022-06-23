// -----------------------------------------------------------------------
// <copyright file="AntiCheatSpeedFix.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Fixes position desync when player speed changed.
    /// Adds <see cref="Player.RunningSpeed"/> and <see cref="Player.WalkingSpeed"/> implementation.
    /// </summary>
    [HarmonyPatch(typeof(FirstPersonController), nameof(FirstPersonController.GetSpeed))]
    internal static class AntiCheatSpeedFix
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            const int offset = 2;
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Ldloc_0) + offset;

            Label maxSpeed = generator.DefineLabel();
            Label setSpeed = generator.DefineLabel();

            // Remove base-game stamina speed setter
            newInstructions.RemoveRange(index, 14);

            newInstructions.InsertRange(index, new[]
            {
                // &speed
                new(OpCodes.Ldarg_1),

                // speed
                new(OpCodes.Ldarg_1),
                new(OpCodes.Ldind_R4),

                // Player player = Player.Get(_hub)
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(FirstPersonController), nameof(FirstPersonController._hub))),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // staminaController.AllowMaxSpeed ? player.RunningSpeed : player.WalkSpeed
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(FirstPersonController), nameof(FirstPersonController.staminaController))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Stamina), nameof(Stamina.AllowMaxSpeed))),
                new(OpCodes.Brtrue_S, maxSpeed),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.WalkingSpeed))),
                new(OpCodes.Br_S, setSpeed),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.RunningSpeed))).WithLabels(maxSpeed),

                // speed *= (speed value above)
                new CodeInstruction(OpCodes.Mul).WithLabels(setSpeed),
                new(OpCodes.Stind_R4),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
