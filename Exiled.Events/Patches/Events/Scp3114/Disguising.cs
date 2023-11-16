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

    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp3114;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using PlayerRoles.PlayableScps.Scp3114;
    using PlayerRoles.Ragdolls;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="Scp3114Disguise.ServerComplete()" />.
    ///     Adds the <see cref="Scp3114.Disguising" /> and <see cref="Scp3114.Disguised" /> events.
    /// </summary>
    [EventPatch(typeof(Scp3114), nameof(Scp3114.Disguising))]
    [EventPatch(typeof(Scp3114), nameof(Scp3114.Disguised))]
    [HarmonyPatch(typeof(Scp3114Disguise), nameof(Scp3114Disguise.ServerComplete))]
    internal class Disguising
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label retLabel = generator.DefineLabel();

            LocalBuilder player = generator.DeclareLocal(typeof(API.Features.Player));
            LocalBuilder ragdoll = generator.DeclareLocal(typeof(API.Features.Ragdoll));

            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                // Player.Get(this.Owner);
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Scp3114Disguise), nameof(Scp3114Disguise.Owner))),
                new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, player.LocalIndex),

                // Ragdoll.Get(this.CurRagdoll);
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Scp3114Disguise), nameof(Scp3114Disguise.CurRagdoll))),
                new(OpCodes.Call, Method(typeof(API.Features.Ragdoll), nameof(API.Features.Ragdoll.Get), new[] { typeof(BasicRagdoll) })),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ragdoll.LocalIndex),

                // true
                new(OpCodes.Ldc_I4_1),

                // DisguisingEventArgs ev = new DisguisingEventArgs(Player, Ragdoll, bool)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(DisguisingEventArgs))[0]),
                new(OpCodes.Dup),

                // Handlers.Scp3114.OnDisguising(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Scp3114), nameof(Handlers.Scp3114.OnDisguising))),

                // if (!ev.IsAllowed)
                //      return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(TryUseBodyEventArgs), nameof(TryUseBodyEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, retLabel),
            });

            const int offset = 0;
            int index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ret) + offset;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // player
                new(OpCodes.Ldloc_S, player.LocalIndex),

                // ragdoll
                new(OpCodes.Ldloc_S, ragdoll.LocalIndex),

                // DisguisedEventArgs ev = new DisguisedEventArgs(Player, Ragdoll);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(DisguisedEventArgs))[0]),

                // Handlers.Scp3114.OnDisguised(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Scp3114), nameof(Handlers.Scp3114.OnDisguised))),
            });

            newInstructions[newInstructions.Count - 1].WithLabels(retLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
