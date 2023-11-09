// -----------------------------------------------------------------------
// <copyright file="Strangling.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp3114
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Attributes;
    using Exiled.API.Features.Pools;
    using Exiled.Events.EventArgs.Scp3114;
    using Handlers;
    using HarmonyLib;
    using PlayerRoles.PlayableScps.Scp079;
    using PlayerRoles.PlayableScps.Scp3114;
    using PlayerRoles.PlayableScps.Subroutines;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="Scp3114Strangle.ServerProcessCmd" />.
    ///     Adds the <see cref="Scp3114.Strangling" /> and <see cref="Scp3114.StranglingFinished" /> event.
    /// </summary>
    [EventPatch(typeof(Scp3114), nameof(Scp3114.Strangling))]
    [EventPatch(typeof(Scp3114), nameof(Scp3114.StranglingFinished))]
    [HarmonyPatch(typeof(Scp3114Strangle), nameof(Scp3114Strangle.ServerProcessCmd))]
    internal class Strangling
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder evStranglingFinishedEventArgs = generator.DeclareLocal(typeof(StranglingFinishedEventArgs));

            Label returnLabel = generator.DefineLabel();
            int offset = -1;
            int index = newInstructions.FindIndex(x => x.opcode == OpCodes.Ldfld && (FieldInfo)x.operand == Field(typeof(Scp3114Strangle), nameof(Scp3114Strangle.ServerOnBegin))) - 1;
            var injectedInstructions = new CodeInstruction[]
            {
                // this.Owner
                new CodeInstruction(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ScpStandardSubroutine<Scp3114Role>), nameof(ScpStandardSubroutine<Scp3114Role>.Owner))),

                // StrangleTarget?
                new(OpCodes.Stloc_2),

                // true
                new(OpCodes.Ldc_I4_1),

                // evStranglingEventArgs = new StranglingPlayerArgs(ReferenceHub, StrangleTarget?, bool)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(StranglingEventArgs))[0]),
                new(OpCodes.Dup),

                // if(!evStranglingEventArgs.IsAllowed)
                //   goto returnLabel;
                new(OpCodes.Call, Method(typeof(Scp3114), nameof(Scp3114.OnStrangling))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(StranglingEventArgs), nameof(StranglingEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, returnLabel),
            };

            newInstructions.InsertRange(index - 1, injectedInstructions);

            offset = -1;
            index = newInstructions.FindIndex(x => x.opcode == OpCodes.Ldfld && (FieldInfo)x.operand == Field(typeof(Scp3114Strangle), nameof(Scp3114Strangle.Cooldown))) + offset;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // this.Owner
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ScpStandardSubroutine<Scp3114Role>), nameof(ScpStandardSubroutine<Scp3114Role>.Owner))),

                // this.SyncTarget
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, PropertyGetter(typeof(Scp3114Strangle), nameof(Scp3114Strangle.SyncTarget))),

                // this._postReleaseCooldown
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(Scp3114Strangle), nameof(Scp3114Strangle._postReleaseCooldown))),

                // evStranglingFinishedEventArgs = new StrangleFinishedPlayerArgs(ReferenceHub, StrangleTarget?, Cooldown)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(StranglingFinishedEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, evStranglingFinishedEventArgs.LocalIndex),

                // Scp3114.OnStranglingFinished(evStranglingFinishedEventArgs);
                new(OpCodes.Call, Method(typeof(Scp3114), nameof(Scp3114.OnStranglingFinished))),
            });

            // replace "this.Cooldown.Trigger((double)this._postReleaseCooldown);"
            // with
            // "this.Cooldown.Trigger(evStranglingFinishedEventArgs.StrangleCooldown);"
            offset = -1;
            index = newInstructions.FindLastIndex(instruction => instruction.operand == (object)Field(typeof(Scp3114Strangle), nameof(Scp3114Strangle._postReleaseCooldown))) + offset;
            newInstructions.RemoveRange(index, 3);

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // evStranglingFinishedEventArgs.StrangleCooldown
                    new(OpCodes.Ldloc_S, evStranglingFinishedEventArgs.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(StranglingFinishedEventArgs), nameof(StranglingFinishedEventArgs.StrangleCooldown))),
                });

            // returnLabel will trigger "base.ServerSendRpc(true);"
            offset = -2;
            index = newInstructions.FindLastIndex(x => x.operand == (object)Method(typeof(ScpSubroutineBase), nameof(ScpSubroutineBase.ServerSendRpc))) + offset;
            newInstructions[index].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}