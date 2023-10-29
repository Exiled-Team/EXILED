// -----------------------------------------------------------------------
// <copyright file="MaxHealthFix.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
#pragma warning disable SA1649 // File name should match first type name
#pragma warning disable SA1402 // File may only contain a single type
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.API.Features.Pools;

    using HarmonyLib;

    using MapGeneration.Distributors;

    using PlayerRoles;
    using PlayerRoles.PlayableScps;
    using PlayerRoles.PlayableScps.Scp049.Zombies;

    using PlayerStatsSystem;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Fix for <see cref="Player.MaxHealth"/> for <see cref="HumanRole"/>.
    /// </summary>
    [HarmonyPatch(typeof(HumanRole), nameof(HumanRole.MaxHealth), MethodType.Getter)]
    internal class MaxHealthFixHuman
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder player = generator.DeclareLocal(typeof(Player));
            LocalBuilder value = generator.DeclareLocal(typeof(float));

            Label @default = generator.DefineLabel();
            Label end = generator.DefineLabel();

            int offset = 0;
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Ldc_R4) + offset;

            newInstructions.RemoveAt(index);

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(HumanRole), nameof(HumanRole._lastOwner))),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, player.LocalIndex),
                new(OpCodes.Brfalse_S, @default),

                new(OpCodes.Ldloc_S, player),
                new(OpCodes.Ldfld, Field(typeof(Player), nameof(Player.OverrideMaxHealth))),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, value.LocalIndex),
                new(OpCodes.Ldc_R4, 0.0f),
                new(OpCodes.Beq_S, @default),

                new(OpCodes.Ldloc_S, value.LocalIndex),
                new(OpCodes.Br_S, end),

                new CodeInstruction(OpCodes.Ldc_R4, 100f).WithLabels(@default),

                new CodeInstruction(OpCodes.Nop).WithLabels(end),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }

    /// <summary>
    /// Fix for <see cref="Player.MaxHealth"/> for <see cref="HumanRole"/>.
    /// </summary>
    [HarmonyPatch(typeof(FpcStandardScp), nameof(FpcStandardScp.MaxHealth), MethodType.Getter)]
    internal class MaxHealthFixScp
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder player = generator.DeclareLocal(typeof(Player));
            LocalBuilder value = generator.DeclareLocal(typeof(float));

            Label end = generator.DefineLabel();

            newInstructions.InsertRange(0, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(FpcStandardScp), nameof(FpcStandardScp._lastOwner))),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, player.LocalIndex),
                new(OpCodes.Brfalse_S, end),

                new(OpCodes.Ldloc_S, player),
                new(OpCodes.Ldfld, Field(typeof(Player), nameof(Player.OverrideMaxHealth))),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, value.LocalIndex),
                new(OpCodes.Ldc_R4, 0.0f),
                new(OpCodes.Beq_S, end),

                new(OpCodes.Ldloc_S, value.LocalIndex),
                new(OpCodes.Ret),

                new CodeInstruction(OpCodes.Nop).WithLabels(end),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }

    /// <summary>
    /// Fix for <see cref="Player.MaxHealth"/> for <see cref="ZombieRole"/>.
    /// </summary>
    [HarmonyPatch(typeof(ZombieRole), nameof(ZombieRole.MaxHealth), MethodType.Getter)]
    internal class MaxHealthFixZombie
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder player = generator.DeclareLocal(typeof(Player));
            LocalBuilder value = generator.DeclareLocal(typeof(float));

            Label end = generator.DefineLabel();

            newInstructions.InsertRange(0, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(ZombieRole), nameof(ZombieRole._lastOwner))),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, player.LocalIndex),
                new(OpCodes.Brfalse_S, end),

                new(OpCodes.Ldloc_S, player),
                new(OpCodes.Ldfld, Field(typeof(Player), nameof(Player.OverrideMaxHealth))),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, value.LocalIndex),
                new(OpCodes.Ldc_R4, 0.0f),
                new(OpCodes.Beq_S, end),

                new(OpCodes.Ldloc_S, value.LocalIndex),
                new(OpCodes.Ret),

                new CodeInstruction(OpCodes.Nop).WithLabels(end),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }

    /// <summary>
    /// Fix <see cref="Player.MaxHealth"/> reseting.
    /// </summary>
    [HarmonyPatch(typeof(HealthStat), nameof(HealthStat.ClassChanged))]
    internal class MaxHealthFixReset
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder player = generator.DeclareLocal(typeof(Player));
            LocalBuilder value = generator.DeclareLocal(typeof(float));

            Label end = generator.DefineLabel();

            newInstructions.InsertRange(0, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(HealthStat), nameof(HealthStat.Hub))),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, player.LocalIndex),
                new(OpCodes.Brfalse_S, end),

                new(OpCodes.Ldloc_S, player),
                new(OpCodes.Ldc_R4, 0.0f),
                new(OpCodes.Stfld, Field(typeof(Player), nameof(Player.OverrideMaxHealth))),

                new CodeInstruction(OpCodes.Nop).WithLabels(end),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}