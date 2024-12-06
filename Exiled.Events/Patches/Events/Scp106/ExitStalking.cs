// -----------------------------------------------------------------------
// <copyright file="ExitStalking.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp106
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Core.Generic.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp106;
    using HarmonyLib;

    using PlayerRoles.PlayableScps.Scp106;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp106StalkAbility.UpdateServerside"/>.
    /// To add the <see cref="Handlers.Scp106.ExitStalking"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Scp106), nameof(Handlers.Scp106.ExitStalking))]
    [HarmonyPatch(typeof(Scp106StalkAbility), nameof(Scp106StalkAbility.UpdateServerside))]
    public class ExitStalking
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(ExitStalkingEventArgs));

            Label returnLabel = generator.DefineLabel();

            const int offset = -2;
            int index = newInstructions.FindIndex(instruction => instruction.OperandIs(Method(typeof(Scp106StalkAbility), nameof(Scp106StalkAbility.ServerSetStalk), new[] { typeof(bool) }))) + offset;
            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player.Get(this.Owner);
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Scp106StalkAbility), nameof(Scp106StalkAbility.Owner))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // ExitStalking ev = new(Player, isAllowed)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ExitStalkingEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Handlers.Scp106.OnExitStalking(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Scp106), nameof(Handlers.Scp106.OnExitStalking))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ExitStalkingEventArgs), nameof(ExitStalkingEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),
                });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            foreach (CodeInstruction instruction in newInstructions)
                yield return instruction;

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}