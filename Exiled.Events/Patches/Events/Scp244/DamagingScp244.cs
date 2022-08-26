// -----------------------------------------------------------------------
// <copyright file="DamagingScp244.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp244
{
#pragma warning disable SA1313

    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.DamageHandlers;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp244;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using InventorySystem.Items.Usables.Scp244;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="Scp244DeployablePickup.Damage" /> to add missing logic to the
    ///     <see cref="Scp244DeployablePickup" />.
    /// </summary>
    [EventPatch(typeof(Handlers.Scp244), nameof(Handlers.Scp244.DamagingScp244))]
    [HarmonyPatch(typeof(Scp244DeployablePickup), nameof(Scp244DeployablePickup.Damage))]
    internal static class DamagingScp244
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label returnFalse = generator.DefineLabel();

            Label continueProcessing = generator.DefineLabel();

            LocalBuilder eventHandler = generator.DeclareLocal(typeof(DamagingScp244EventArgs));

            // Remove grenade damage check, let event handler do it.
            newInstructions.RemoveRange(0, 5);

            int insertOffset = 5;

            int index = newInstructions.FindIndex(instruction => instruction.Calls(PropertyGetter(typeof(Scp244DeployablePickup), nameof(Scp244DeployablePickup.ModelDestroyed)))) + insertOffset;

            newInstructions.RemoveRange(index, 3);

            // Insert event handler at start of function to determine whether to allow function to run or not.
            newInstructions.InsertRange(
                index,
                new[]
                {
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Scp244DeployablePickup), nameof(Scp244DeployablePickup.State))),
                    new(OpCodes.Ldc_I4_2),
                    new(OpCodes.Beq_S, returnFalse),
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Ldarg_2),
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(DamagingScp244EventArgs))[0]),
                    new(OpCodes.Stloc, eventHandler.LocalIndex),
                    new(OpCodes.Ldloc, eventHandler.LocalIndex),
                    new(OpCodes.Call, Method(typeof(Scp244), nameof(Scp244.OnDamagingScp244))),
                    new(OpCodes.Ldloc, eventHandler.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(DamagingScp244EventArgs), nameof(DamagingScp244EventArgs.IsAllowed))),
                    new(OpCodes.Brtrue_S, continueProcessing),
                    new CodeInstruction(OpCodes.Ldc_I4_0).WithLabels(returnFalse),
                    new(OpCodes.Ret),
                    new CodeInstruction(OpCodes.Ldarg_0).WithLabels(continueProcessing),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(Scp244DeployablePickup), nameof(Scp244DeployablePickup._health))),
                    new(OpCodes.Ldloc, eventHandler.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(DamagingScp244EventArgs), nameof(DamagingScp244EventArgs.DamageHandler))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(DamageHandler), nameof(DamageHandler.Damage))),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}