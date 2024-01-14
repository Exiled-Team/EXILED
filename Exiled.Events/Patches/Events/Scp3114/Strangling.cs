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

    using Exiled.API.Features.Pools;
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
    internal class Strangling
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instruction, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instruction);
            Label retLabel = generator.DefineLabel();
            LocalBuilder ev = generator.DeclareLocal(typeof(StranglingEventArgs));

            const int offset = 0;
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Ldloc_3) + offset;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Scp3114Strangle), nameof(Scp3114Strangle.Owner))),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Ldloc_3),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(StranglingEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Call, Method(typeof(Handlers.Scp3114), nameof(Handlers.Scp3114.OnStrangling))),
                new(OpCodes.Stloc, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(StranglingEventArgs), nameof(StranglingEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, retLabel),
                new(OpCodes.Ldloc, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(StranglingEventArgs), nameof(StranglingEventArgs.StrangleTarget))),
                new(OpCodes.Stloc_3),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(retLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}