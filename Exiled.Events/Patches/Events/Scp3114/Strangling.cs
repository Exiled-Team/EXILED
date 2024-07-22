// -----------------------------------------------------------------------
// <copyright file="Strangling.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp3114
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Core.Generic.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp3114;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using PlayerRoles.PlayableScps.Scp3114;

    using static HarmonyLib.AccessTools;

    using Player = API.Features.Player;

    /// <summary>
    /// Patches <see cref="Scp3114Strangle.ProcessAttackRequest"/> to add the <see cref="Scp3114.Strangling"/> event.
    /// </summary>
    [EventPatch(typeof(Scp3114), nameof(Scp3114.Strangling))]
    [HarmonyPatch(typeof(Scp3114Strangle), nameof(Scp3114Strangle.ProcessAttackRequest))]
    internal static class Strangling
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label retLabel = generator.DefineLabel();
            Label jumpLabel = generator.DefineLabel();

            int index = newInstructions.FindIndex(i => i.LoadsField(Field(typeof(ReferenceHub), nameof(ReferenceHub.playerEffectsController)))) - 1;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // Scp3114Strangle::Owner
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Scp3114Strangle), nameof(Scp3114Strangle.Owner))),

                // Player::Get(Scp3114Strangle::Owner)
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // hub
                new(OpCodes.Ldloc_0),

                // Player::Get(hub)
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // true
                new(OpCodes.Ldc_I4_1),

                // StranglingEventArgs args = new(player, hub);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(StranglingEventArgs))[0]),
                new(OpCodes.Dup),

                // Scp3114::OnStrangling(args);
                new(OpCodes.Call, Method(typeof(Scp3114), nameof(Scp3114.OnStrangling))),

                // if (args.IsAllowed) goto jumpLabel
                new(OpCodes.Callvirt, PropertyGetter(typeof(StranglingEventArgs), nameof(StranglingEventArgs.IsAllowed))),
                new(OpCodes.Brtrue_S, jumpLabel),

                // return strangleTarget = null
                new(OpCodes.Ldloca_S, 4),
                new(OpCodes.Initobj, typeof(Scp3114Strangle.StrangleTarget?)),
                new(OpCodes.Ldloc_S, 4),
                new(OpCodes.Stloc_S, 4),
                new(OpCodes.Leave, retLabel),

                // jumpLabel
                new CodeInstruction(OpCodes.Nop).WithLabels(jumpLabel),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(retLabel);

            foreach (CodeInstruction instruction in newInstructions)
                yield return instruction;

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}