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
    ///     Patches <see cref="Scp049ResurrectAbility.ServerValidateBegin" />.
    ///     Adds the <see cref="Handlers.Scp049.StartingRecall" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Scp049), nameof(Handlers.Scp049.StartingRecall))]
    [HarmonyPatch(typeof(Scp049ResurrectAbility), nameof(Scp049ResurrectAbility.ServerValidateBegin))]
    internal static class StartingRecall
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label retLabel = generator.DefineLabel();

            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                // Player target = Player.Get(ragdoll.Info.OwnerHub);
                new(OpCodes.Ldarg_1),
                new(OpCodes.Ldfld, Field(typeof(BasicRagdoll), nameof(BasicRagdoll.Info))),
                new(OpCodes.Ldfld, Field(typeof(RagdollData), nameof(RagdollData.OwnerHub))),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // Player player = Player.Get(this.Owner);
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Scp049ResurrectAbility), nameof(Scp049ResurrectAbility.Owner))),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // Ragdoll doll = Rangdoll.Get(ragdoll);
                new(OpCodes.Ldarg_1),
                new(OpCodes.Call, Method(typeof(Ragdoll), nameof(Ragdoll.Get), new[] { typeof(BasicRagdoll) })),

                // true
                new(OpCodes.Ldc_I4_1),

                // StartingRecallEventArgs ev = new(target, player, doll, true);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(StartingRecallEventArgs))[0]),
                new(OpCodes.Dup),

                // Handlers.Scp049.OnStartingRecall(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Scp049), nameof(Handlers.Scp049.OnStartingRecall))),

                // if (!ev.IsAllowed)
                //      return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(StartingRecallEventArgs), nameof(StartingRecallEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, retLabel),
            });
            newInstructions[newInstructions.Count - 1].WithLabels(retLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
