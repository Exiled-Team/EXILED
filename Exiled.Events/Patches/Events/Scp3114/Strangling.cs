// -----------------------------------------------------------------------
// <copyright file="Strangling.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp3114
{
#pragma warning disable SA1402 // File may only contain a single type

    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Attributes;
    using Exiled.API.Features.Pools;
    using Exiled.Events.EventArgs.Scp3114;
    using Handlers;
    using HarmonyLib;
    using PlayerRoles.PlayableScps.Scp3114;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="Scp3114Strangle.ServerProcessCmd" />.
    ///     Adds the <see cref="Handlers.Scp3114.StranglingFinished" /> and <see cref="Handlers.Scp3114.StranglingFinished" /> event.
    /// </summary>
    [EventPatch(typeof(Scp3114), nameof(Scp3114.Strangling))]
    [HarmonyPatch(typeof(Scp3114Strangle), nameof(Scp3114Strangle.ServerProcessCmd))]
    internal sealed class Strangling
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);
            Label skipLabel = generator.DefineLabel();
            int index = newInstructions.FindIndex(x => x.opcode == OpCodes.Ldfld && (FieldInfo)x.operand == Field(typeof(Scp3114Strangle), nameof(Scp3114Strangle.ServerOnBegin))) - 1;
            var injectedInstructions = new CodeInstruction[]
            {
                // ev = new StranglingPlayerArgs(this, syncTarget (new one), 1)
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldloca_S, 0), // syncTarget - NOT SyncTarget
                new(OpCodes.Call, PropertyGetter(typeof(Scp3114Strangle.StrangleTarget?), nameof(Nullable<Scp3114Strangle.StrangleTarget>.Value))),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(StranglingEventArgs))[0]),
                new(OpCodes.Dup),

                // if(!ev.IsAllowed)
                new(OpCodes.Call, Method(typeof(Scp3114), nameof(Scp3114.OnStrangling))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(StranglingEventArgs), nameof(StranglingEventArgs.IsAllowed))),

                // return
                new(OpCodes.Brtrue_S, skipLabel),
                new(OpCodes.Ret),
            };

            newInstructions.InsertRange(index - 1, injectedInstructions);
            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }

    /// <summary>
    ///     Patches <see cref="Scp3114Strangle.ServerProcessCmd" />.
    ///     Adds the <see cref="Handlers.Scp3114.StranglingFinished" /> and <see cref="Handlers.Scp3114.StranglingFinished" /> event.
    /// </summary>
    [EventPatch(typeof(Scp3114), nameof(Scp3114.StranglingFinished))]
    [HarmonyPatch(typeof(Scp3114Strangle), nameof(Scp3114Strangle.ServerProcessCmd))]
    internal sealed class StranglingFinishedTranspiler
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);
            LocalBuilder ev = generator.DeclareLocal(typeof(StranglingFinishedEventArgs));
            int index = newInstructions.FindIndex(x => x.opcode == OpCodes.Ldfld && (FieldInfo)x.operand == Field(typeof(Scp3114Strangle), nameof(Scp3114Strangle.Cooldown)));
            var injectedInstructions = new CodeInstruction[]
            {
                // ev = new StrangleFinishedPlayerArgs(this, SyncTarget (old one), Cooldown)
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, PropertyGetter(typeof(Scp3114Strangle), nameof(Scp3114Strangle.SyncTarget))),
                new(OpCodes.Stloc_2),
                new(OpCodes.Ldloca_S, 2),
                new(OpCodes.Call, PropertyGetter(typeof(Scp3114Strangle.StrangleTarget?), nameof(Nullable<Scp3114Strangle.StrangleTarget>.Value))),

                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(Scp3114Strangle), nameof(Scp3114Strangle._postReleaseCooldown))),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(StranglingFinishedEventArgs))[0]),
                new(OpCodes.Stloc_S, ev.LocalIndex),

                // Events.OnStrangleFinished.SafeInvoke
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Call, Method(typeof(Scp3114), nameof(Scp3114.OnStranglingFinished))),

                // Scp3114Strangle.Cooldown.Trigger(ev.Cooldown)
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(Scp3114Strangle), nameof(Scp3114Strangle.Cooldown))),
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(StranglingFinishedEventArgs), nameof(StranglingFinishedEventArgs.StrangleCooldown))),
            };
            newInstructions.InsertRange(index - 1, injectedInstructions);
            newInstructions.RemoveRange(index + injectedInstructions.Length, 3);
            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}