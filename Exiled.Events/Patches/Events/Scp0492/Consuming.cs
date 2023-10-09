// -----------------------------------------------------------------------
// <copyright file="Consuming.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp0492
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp0492;
    using HarmonyLib;

    using PlayerRoles.PlayableScps.Scp049;
    using PlayerRoles.PlayableScps.Scp049.Zombies;
    using PlayerRoles.PlayableScps.Subroutines;
    using PlayerStatsSystem;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Adds the <see cref="Handlers.Scp0492.ConsumingCorpse" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Scp0492), nameof(Handlers.Scp0492.ConsumingCorpse))]
    [HarmonyPatch(typeof(ZombieConsumeAbility), nameof(ZombieConsumeAbility.ServerProcessCmd))]
    public class Consuming
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(ConsumingCorpseEventArgs));

            int offset = 1;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ret) + offset;

            Label returnLabel = generator.DefineLabel();

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player.Get(base.Owner)
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Call, PropertyGetter(typeof(ScpStandardSubroutine<ZombieRole>), nameof(ScpStandardSubroutine<ZombieRole>.Owner))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // base.CurRagdoll
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new(OpCodes.Call, PropertyGetter(typeof(RagdollAbilityBase<ZombieRole>), nameof(RagdollAbilityBase<ZombieRole>.CurRagdoll))),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // ConsumingCorpseEventArgs ev = new(Player, Ragdoll, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ConsumingCorpseEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Handlers.Scp0492.OnSendingCall(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Scp0492), nameof(Handlers.Scp0492.OnConsumingCorpse))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ConsumingCorpseEventArgs), nameof(ConsumingCorpseEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),
                });

            // replace "base.Owner.playerStats.GetModule<HealthStat>().ServerHeal(100f)" with "base.Owner.playerStats.GetModule<HealthStat>().ServerHeal(ev.ConsumeHeal)"
            offset = -1;
            index = newInstructions.FindIndex(instruction => instruction.operand == (object)Method(typeof(HealthStat), nameof(HealthStat.ServerHeal))) + offset;
            newInstructions.RemoveAt(index);

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // ev.ConsumeHeal
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ConsumingCorpseEventArgs), nameof(ConsumingCorpseEventArgs.ConsumeHeal))),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
