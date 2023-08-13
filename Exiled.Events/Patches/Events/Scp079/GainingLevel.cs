// -----------------------------------------------------------------------
// <copyright file="GainingLevel.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp079
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Pools;

    using Exiled.Events.EventArgs.Scp079;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using PlayerRoles.PlayableScps.Scp079;
    using PlayerRoles.PlayableScps.Subroutines;

    using static HarmonyLib.AccessTools;

    using Player = API.Features.Player;

    /// <summary>
    ///     Patches <see cref="Scp079TierManager.AccessTierIndex" />.
    ///     Adds the <see cref="Scp079.GainingLevel" /> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp079TierManager), nameof(Scp079TierManager.AccessTierIndex), MethodType.Setter)]
    internal static class GainingLevel
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            const int offset = 1;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ret) + offset;

            Label returnLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(GainingLevelEventArgs));

            // GainingLevelEventArgs ev = new GainingLevelEventArgs(Player.Get(base.Owner), value + 1, true);
            //
            // Handlers.Scp079.OnGainingLevel(ev);
            //
            // newLvl = ev.NewLevel;
            //
            // if (!ev.IsAllowed)
            //     return;
            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player.Get(base.Owner)
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Call, PropertyGetter(typeof(ScpStandardSubroutine<Scp079Role>), nameof(ScpStandardSubroutine<Scp079Role>.Owner))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // value + 1
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Ldc_I4_1),
                    new(OpCodes.Add),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // GainingLevelEventArgs ev = new(Player, int, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(GainingLevelEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Scp079.OnGainingLevel(ev)
                    new(OpCodes.Call, Method(typeof(Scp079), nameof(Scp079.OnGainingLevel))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(GainingLevelEventArgs), nameof(GainingLevelEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),

                    // value = ev.NewLevel - 1
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(GainingLevelEventArgs), nameof(GainingLevelEventArgs.NewLevel))),
                    new(OpCodes.Ldc_I4_1),
                    new(OpCodes.Sub),
                    new(OpCodes.Starg_S, 1),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}