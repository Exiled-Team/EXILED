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
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using InventorySystem.Items.Usables.Scp244;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp244DeployablePickup"/> to add missing event handler to the <see cref="Scp244DeployablePickup"/>.
    /// </summary>
    [HarmonyPatch(typeof(Scp244DeployablePickup), nameof(Scp244DeployablePickup.UpdateRange))]
    internal static class UpdateScp244
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label normalProcessing = generator.DefineLabel();

            LocalBuilder resultOfDotCheck = generator.DeclareLocal(typeof(int));

            int offset = 1;
            int index = newInstructions.FindIndex(instruction => instruction.LoadsField(Field(typeof(Scp244DeployablePickup), nameof(Scp244DeployablePickup._activationDot)))) + offset;

            // Remove branching on dot result.
            newInstructions.RemoveAt(index);

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Clt),
                new(OpCodes.Stloc, resultOfDotCheck.LocalIndex),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldloc, resultOfDotCheck.LocalIndex),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(OpeningScp244EventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Call, Method(typeof(Handlers.Scp244), nameof(Handlers.Scp244.OnOpeningScp244))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(OpeningScp244EventArgs), nameof(OpeningScp244EventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, normalProcessing),
            });

            int continueOffset = -1;
            int continueIndex = newInstructions.FindLastIndex(instruction => instruction.Calls(PropertyGetter(typeof(Scp244DeployablePickup), nameof(Scp244DeployablePickup.State)))) + continueOffset;

            // Jumping over original NW logic.
            newInstructions[continueIndex].WithLabels(normalProcessing);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
