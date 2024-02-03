// -----------------------------------------------------------------------
// <copyright file="DamagingScp244.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp244
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.DamageHandlers;
    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp244;

    using Handlers;

    using HarmonyLib;

    using InventorySystem.Items.Usables.Scp244;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp244DeployablePickup.Damage" /> to add missing logic to the
    /// <see cref="Scp244.DamagingScp244" />.
    /// </summary>
    [EventPatch(typeof(Scp244), nameof(Scp244.DamagingScp244))]
    [HarmonyPatch(typeof(Scp244DeployablePickup), nameof(Scp244DeployablePickup.Damage))]
    internal static class DamagingScp244
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnFalse = generator.DefineLabel();
            Label continueProcessing = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(DamagingScp244EventArgs));

            // Remove grenade damage check, let event handler do it.
            newInstructions.RemoveRange(0, 5);

            int insertOffset = 5;
            int index = newInstructions.FindIndex(
                instruction => instruction.Calls(PropertyGetter(typeof(Scp244DeployablePickup), nameof(Scp244DeployablePickup.ModelDestroyed)))) + insertOffset;

            newInstructions.RemoveRange(index, 3);

            // Insert event handler at start of function to determine whether to allow function to run or not.
            newInstructions.InsertRange(
                index,
                new[]
                {
                    // if (this.State == 2) --> "this" is taken from the previous instruction
                    //    return false;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Scp244DeployablePickup), nameof(Scp244DeployablePickup.State))),
                    new(OpCodes.Ldc_I4_2),
                    new(OpCodes.Beq_S, returnFalse),

                    // this
                    new(OpCodes.Ldarg_0),

                    // damage
                    new(OpCodes.Ldarg_1),

                    // handler
                    new(OpCodes.Ldarg_2),

                    // DamagingScp244EventArgs ev = new(Scp244DeployablePickup, float, DamageHandlerBase)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(DamagingScp244EventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc, ev.LocalIndex),

                    // Scp244.OnDamagingScp244(ev)
                    new(OpCodes.Call, Method(typeof(Scp244), nameof(Scp244.OnDamagingScp244))),

                    // if (!ev.IsAllowed)
                    //    goto continueProcessing;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(DamagingScp244EventArgs), nameof(DamagingScp244EventArgs.IsAllowed))),
                    new(OpCodes.Brtrue_S, continueProcessing),

                    // return false;
                    new CodeInstruction(OpCodes.Ldc_I4_0).WithLabels(returnFalse),
                    new(OpCodes.Ret),

                    // reload "this"
                    new CodeInstruction(OpCodes.Ldarg_0).WithLabels(continueProcessing),

                    // this._health
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(Scp244DeployablePickup), nameof(Scp244DeployablePickup._health))),

                    // ev.DamageHandler.Damage
                    new(OpCodes.Ldloc, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(DamagingScp244EventArgs), nameof(DamagingScp244EventArgs.Handler))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(DamageHandler), nameof(DamageHandler.Damage))),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}