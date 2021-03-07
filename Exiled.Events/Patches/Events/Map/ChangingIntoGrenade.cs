// -----------------------------------------------------------------------
// <copyright file="ChangingIntoGrenade.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using Grenades;

    using HarmonyLib;

    using Mirror;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="FragGrenade.ChangeIntoGrenade"/>.
    /// Adds the <see cref="Handlers.Map.ChangingIntoGrenade"/> event.
    /// </summary>
    [HarmonyPatch(typeof(FragGrenade), nameof(FragGrenade.ChangeIntoGrenade))]
    internal static class ChangingIntoGrenade
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            // The index offset
            int offset = 2;

            // Find the last return false call.
            int index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldc_I4_0) + offset;

            // Generate a continue lable.
            Label continueLabel = generator.DefineLabel();

            // Declare ServerChangingGrenadeEventArgs, to be able to store it's instance with "stloc.s".
            LocalBuilder ev = generator.DeclareLocal(typeof(ChangingIntoGrenadeEventArgs));

            newInstructions.InsertRange(index, new[]
            {
                // (Pickup item)
                new CodeInstruction(OpCodes.Ldarg_1).MoveLabelsFrom(newInstructions[index]),

                // var ev = new ServerChangingGrenadeEventArgs(pickup)
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangingIntoGrenadeEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc_S, ev.LocalIndex),

                // Handlers.Map.OnServerChangingGrenade(ev)
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Map), nameof(Handlers.Map.OnServerChangingGrenade))),

                // if (!ev.IsAllowed)
                //     return;
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ChangingIntoGrenadeEventArgs), nameof(ChangingIntoGrenadeEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brtrue_S, continueLabel),
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Ret),

                // item.ItemId = ev.Type
                new CodeInstruction(OpCodes.Ldarg_1).WithLabels(continueLabel),
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(ChangingIntoGrenadeEventArgs), nameof(ChangingIntoGrenadeEventArgs.Type))),
                new CodeInstruction(OpCodes.Call, PropertySetter(typeof(Pickup), nameof(Pickup.ItemId))),
            });

            // Find the last method call
            index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Callvirt && (MethodInfo)instruction.operand == Method(typeof(Grenade), nameof(Grenade.InitData), new[] { typeof(FragGrenade), typeof(Pickup) }));

            newInstructions.InsertRange(index, new[]
            {
                // grenade.NetworkFuseTime = NetworkTime.Time + ev.FuseTime
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ChangingIntoGrenadeEventArgs), nameof(ChangingIntoGrenadeEventArgs.FuseTime))),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(NetworkTime), nameof(NetworkTime.time))),
                new CodeInstruction(OpCodes.Add),
                new CodeInstruction(OpCodes.Callvirt, PropertySetter(typeof(Grenades.Grenade), nameof(Grenades.Grenade.NetworkfuseTime))),
            });

            foreach (CodeInstruction instruction in newInstructions)
                yield return instruction;

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
