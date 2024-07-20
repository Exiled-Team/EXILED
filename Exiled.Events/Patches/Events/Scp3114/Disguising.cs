// -----------------------------------------------------------------------
// <copyright file="Disguising.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp3114
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.API.Features.Core.Generic.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp3114;
    using Exiled.Events.Handlers;
    using HarmonyLib;
    using PlayerRoles.PlayableScps.Scp3114;
    using PlayerRoles.Ragdolls;

    using static HarmonyLib.AccessTools;

    using Player = Exiled.API.Features.Player;

    /// <summary>
    /// Patches <see cref="Scp3114Disguise.ServerComplete()" />.
    /// Adds the <see cref="Scp3114.Disguising" /> and <see cref="Scp3114.Disguised" /> events.
    /// </summary>
    [EventPatch(typeof(Scp3114), nameof(Scp3114.Disguising))]
    [EventPatch(typeof(Scp3114), nameof(Scp3114.Disguised))]
    [HarmonyPatch(typeof(Scp3114Disguise), nameof(Scp3114Disguise.ServerComplete))]
    internal static class Disguising
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label retLabel = generator.DefineLabel();
            LocalBuilder player = generator.DeclareLocal(typeof(Player));
            LocalBuilder ragdoll = generator.DeclareLocal(typeof(Ragdoll));

            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldarg_0);

            // Removes Scp3114Disguise::CastRole::Disguised = true
            newInstructions.RemoveRange(index, 4);

            const int offset = -3;
            index = newInstructions.FindIndex(instruction => instruction.Calls(Method(typeof(Scp3114RagdollToBonesConverter), nameof(Scp3114RagdollToBonesConverter.ServerConvertNew)))) + offset;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // Scp3114Disguise::Owner
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Scp3114Disguise), nameof(Scp3114Disguise.Owner))),

                // Player::Get(Scp3114Disguise::Owner)
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Stloc_S, player.LocalIndex),

                // curRagdoll
                new(OpCodes.Ldloc_0),

                // Ragdoll::Get(curRagdoll)
                new(OpCodes.Call, Method(typeof(Ragdoll), nameof(Ragdoll.Get), new[] { typeof(DynamicRagdoll) })),
                new(OpCodes.Stloc_S, ragdoll.LocalIndex),

                // HandleEvent(Scp3114Disguise, player, ragdoll)
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldloc_S, player.LocalIndex),
                new(OpCodes.Ldloc_S, ragdoll.LocalIndex),
                new(OpCodes.Call, Method(typeof(Disguising), nameof(HandleEvent))),
                new (OpCodes.Brtrue_S, retLabel),
            });

            // Call DisguisedEventArgs
            newInstructions.InsertRange(newInstructions.Count - 1, new CodeInstruction[]
            {
                new(OpCodes.Ldloc_S, player.LocalIndex),
                new(OpCodes.Ldloc_S, ragdoll.LocalIndex),

                // DisguisedEventArgs args = new(player, ragdoll)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(DisguisedEventArgs))[0]),

                // Scp3114::OnDisguised(args)
                new(OpCodes.Call, Method(typeof(Scp3114), nameof(Scp3114.OnDisguised))),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(retLabel);

            foreach (CodeInstruction codeInstruction in newInstructions)
                yield return codeInstruction;

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private static bool HandleEvent(Scp3114Disguise disguise, Player player, Ragdoll ragdoll)
        {
            DisguisingEventArgs args = new(player, ragdoll, true);
            Scp3114.OnDisguising(args);

            if (!args.IsAllowed)
                return true;

            disguise.CastRole.Disguised = true;
            return false;
        }
    }
}
