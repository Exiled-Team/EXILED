// -----------------------------------------------------------------------
// <copyright file="StartingByCommand.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Warhead
{
#pragma warning disable SA1402

    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Pools;

    using CommandSystem;
    using CommandSystem.Commands.RemoteAdmin.ServerEvent;
    using CommandSystem.Commands.RemoteAdmin.Warhead;

    using API.Features;
    using Exiled.Events.EventArgs.Warhead;

    using HarmonyLib;

    

    using static HarmonyLib.AccessTools;

    using Warhead = Handlers.Warhead;

    /// <summary>
    ///     Contains the instructions we will insert into multiple methods.
    /// </summary>
    internal static class StartingByCommand
    {
        /// <summary>
        ///     Inserts the needed instructions to a <see cref="List{CodeInstruction}" />.
        /// </summary>
        /// <param name="index">The starting index to add instructions at.</param>
        /// <param name="generator">The <see cref="ILGenerator" /> for the method being patched.</param>
        /// <param name="instructions">The <see cref="List{T}" /> of instructions to add to.</param>
        internal static void InsertInstructions(int index, ILGenerator generator, ref List<CodeInstruction> instructions)
        {
            Label cdcLabel = generator.DefineLabel();

            CodeInstruction[] instructionsToInsert =
            {
                // Player.Get(sender)
                new CodeInstruction(OpCodes.Ldarg_2).MoveLabelsFrom(instructions[index]),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ICommandSender) })),

                // true
                new(OpCodes.Ldc_I4_1),

                // StartingEventArgs ev = new(Player, bool)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(StartingEventArgs))[0]),
                new(OpCodes.Dup),

                // Warhead.OnStarting(ev)
                new(OpCodes.Call, Method(typeof(Warhead), nameof(Warhead.OnStarting))),

                // if (!ev.IsAllowed)
                //    goto cdcLabel;
                new(OpCodes.Callvirt, PropertyGetter(typeof(StartingEventArgs), nameof(StartingEventArgs.IsAllowed))),
                new(OpCodes.Brtrue, cdcLabel),

                // response = "Action prevented by a plugin."
                new(OpCodes.Ldarg_3),
                new(OpCodes.Ldstr, "Action prevented by a plugin."),
                new(OpCodes.Stind_Ref),

                // return false
                new(OpCodes.Ldc_I4_0),
                new(OpCodes.Ret),
            };

            instructions.InsertRange(index, instructionsToInsert);

            instructions[index + instructionsToInsert.Length].WithLabels(cdcLabel);
        }
    }

    /// <summary>
    ///     Patches <see cref="DetonateCommand.Execute" /> to add the
    ///     <see cref="Warhead.Starting" /> event when triggered by a command.
    /// </summary>
    [HarmonyPatch(typeof(DetonateCommand), nameof(DetonateCommand.Execute))]
    internal static class StartingByDetonateCommand
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            const int offset = 1;
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Ret) + offset;

            StartingByCommand.InsertInstructions(index, generator, ref newInstructions);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }

    /// <summary>
    ///     Patches <see cref="InstantCommand.Execute" /> to add the
    ///     <see cref="Warhead.Starting" /> event when triggered by a command.
    /// </summary>
    [HarmonyPatch(typeof(InstantCommand), nameof(InstantCommand.Execute))]
    internal static class StartingByInstantCommand
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            const int offset = 1;
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Ret) + offset;

            StartingByCommand.InsertInstructions(index, generator, ref newInstructions);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }

    /// <summary>
    ///     Patches <see cref="DetonationStartCommand.Execute" /> to add the
    ///     <see cref="Warhead.Starting" /> event when triggered by a command.
    /// </summary>
    [HarmonyPatch(typeof(DetonationStartCommand), nameof(DetonationStartCommand.Execute))]
    internal static class StartingByEventDetonateCommand
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            const int offset = 1;
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Ret) + offset;

            StartingByCommand.InsertInstructions(index, generator, ref newInstructions);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }

    /// <summary>
    ///     Patches <see cref="DetonationInstantCommand.Execute" /> to add the
    ///     <see cref="Warhead.Starting" /> event when triggered by a command.
    /// </summary>
    [HarmonyPatch(typeof(DetonationInstantCommand), nameof(DetonationInstantCommand.Execute))]
    internal static class StartingByEventInstantDetonateCommand
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            const int offset = 1;
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Ret) + offset;

            StartingByCommand.InsertInstructions(index, generator, ref newInstructions);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}