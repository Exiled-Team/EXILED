// -----------------------------------------------------------------------
// <copyright file="StartingRecall.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------
namespace Exiled.Events.Patches.Events.Scp049
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp049;
    using HarmonyLib;
    using PlayerRoles.PlayableScps.Scp049;
    using PlayerRoles.Ragdolls;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp049ResurrectAbility.ServerValidateBegin" />.
    /// Adds the <see cref="Handlers.Scp049.StartingRecall" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Scp049), nameof(Handlers.Scp049.StartingRecall))]
    [HarmonyPatch(typeof(Scp049ResurrectAbility), nameof(Scp049ResurrectAbility.ServerValidateBegin))]
    internal static class StartingRecall
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label continueLabel = generator.DefineLabel();
            Label retLabel = generator.DefineLabel();

            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                // Player player = Player.Get(this.Owner);
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Scp049ResurrectAbility), nameof(Scp049ResurrectAbility.Owner))),
                new(OpCodes.Call, Method(typeof(Exiled.API.Features.Player), nameof(Exiled.API.Features.Player.Get), new[] { typeof(ReferenceHub) })),

                // Ragdoll doll = Ragdoll.Get(ragdoll);
                new(OpCodes.Ldarg_1),
                new(OpCodes.Call, Method(typeof(Ragdoll), nameof(Ragdoll.Get), new[] { typeof(BasicRagdoll) })),

                // true
                new(OpCodes.Ldc_I4_1),

                // StartingRecallEventArgs ev = new StartingRecallEventArgs(player, doll, true);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(StartingRecallEventArgs))[0]),
                new(OpCodes.Dup),

                // Handlers.Scp049.OnStartingRecall(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Scp049), nameof(Handlers.Scp049.OnStartingRecall))),

                // if (!ev.IsAllowed)
                //      return 1;
                // else
                //      continue;
                new(OpCodes.Callvirt, PropertyGetter(typeof(StartingRecallEventArgs), nameof(StartingRecallEventArgs.IsAllowed))),
                new(OpCodes.Brtrue_S, continueLabel),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Ret),
                new CodeInstruction(OpCodes.Nop).WithLabels(continueLabel),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(retLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
