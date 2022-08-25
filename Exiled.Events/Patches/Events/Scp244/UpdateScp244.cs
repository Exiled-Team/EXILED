// -----------------------------------------------------------------------
// <copyright file="UpdateScp244.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp244
{
#pragma warning disable SA1313

    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs.Scp244;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using InventorySystem.Items.Usables.Scp244;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="Scp244DeployablePickup" /> to add missing event handler to the
    ///     <see cref="Scp244DeployablePickup" />.
    /// </summary>
    [HarmonyPatch(typeof(Scp244DeployablePickup), nameof(Scp244DeployablePickup.UpdateRange))]
    internal static class UpdateScp244
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label retLabel = generator.DefineLabel();

            int offset = 2;
            int index = newInstructions.FindIndex(instruction => instruction.LoadsField(Field(typeof(Scp244DeployablePickup), nameof(Scp244DeployablePickup._activationDot)))) + offset;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(OpeningScp244EventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Call, Method(typeof(Scp244), nameof(Scp244.OnOpeningScp244))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(OpeningScp244EventArgs), nameof(OpeningScp244EventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, retLabel),
            });

            index = newInstructions.FindIndex(i => i.opcode == OpCodes.Callvirt && (MethodInfo)i.operand == Method(typeof(Stopwatch), nameof(Stopwatch.Restart)));

            newInstructions[index].WithLabels(retLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
