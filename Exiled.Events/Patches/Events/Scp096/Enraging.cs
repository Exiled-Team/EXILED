// -----------------------------------------------------------------------
// <copyright file="Enraging.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp096
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp096;

    using HarmonyLib;

    using PlayerRoles.PlayableScps.Scp096;
    using PlayerRoles.PlayableScps.Subroutines;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="Scp096RageManager.ServerEnrage(float)" />.
    ///     Adds the <see cref="Handlers.Scp096.Enraging" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Scp096), nameof(Handlers.Scp096.Enraging))]
    [HarmonyPatch(typeof(Scp096RageManager), nameof(Scp096RageManager.ServerEnrage))]
    internal static class Enraging
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(EnragingEventArgs));

            const int offset = 0;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldarg_0) + offset;

            // EnragingEventArgs ev = new EnragingEventArgs(this, Player, initialDuration, true);
            //
            // Handlers.Scp096.OnEnraging(ev);
            //
            // if (!ev.IsAllowed)
            //     return;
            newInstructions.InsertRange(
                index,
                new[]
                {
                    // base.ScpRole
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Call, PropertyGetter(typeof(ScpStandardSubroutine<Scp096Role>), nameof(ScpStandardSubroutine<Scp096Role>.ScpRole))),

                    // Player.Get(base.Owner)
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, PropertyGetter(typeof(ScpStandardSubroutine<Scp096Role>), nameof(ScpStandardSubroutine<Scp096Role>.Owner))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // initialDuration
                    new(OpCodes.Ldarg_1),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // EnragingEventArgs ev = new(Scp096Role, Player, float, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(EnragingEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Handlers.Scp096.OnEnraging(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Scp096), nameof(Handlers.Scp096.OnEnraging))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(EnragingEventArgs), nameof(EnragingEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),

                    // initialDuration = ev.Initialduration
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(EnragingEventArgs), nameof(EnragingEventArgs.InitialDuration))),
                    new(OpCodes.Starg_S, 1),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}