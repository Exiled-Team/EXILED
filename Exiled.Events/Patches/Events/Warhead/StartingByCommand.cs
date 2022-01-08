// -----------------------------------------------------------------------
// <copyright file="StartingByCommand.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Warhead
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using CommandSystem;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using RemoteAdmin;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="CommandSystem.Commands.RemoteAdmin.Warhead.DetonateCommand.Execute"/> to add the <see cref="Exiled.Events.Handlers.Warhead.Starting"/> event when triggered by a command.
    /// </summary>
    [HarmonyPatch(typeof(CommandSystem.Commands.RemoteAdmin.Warhead.DetonateCommand), nameof(CommandSystem.Commands.RemoteAdmin.Warhead.DetonateCommand.Execute))]
    internal static class StartingByCommand
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);
            const int offset = 1;
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Ret) + offset;
            Label retLabel = generator.DefineLabel();

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_2).MoveLabelsFrom(newInstructions[index]),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ICommandSender) })),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(StartingEventArgs))),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Warhead), nameof(Handlers.Warhead.OnStarting))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(StartingEventArgs), nameof(StartingEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse, retLabel),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(retLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
