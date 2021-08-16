// -----------------------------------------------------------------------
// <copyright file="Shooting.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1313
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using InventorySystem.Items.Firearms.BasicMessages;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

#pragma warning disable SA1512 // Single-line comments should not be followed by blank line
#pragma warning disable SA1005 // Single line comments should begin with single space
#pragma warning disable SA1515 // Single-line comment should be preceded by blank line

    /// <summary>
    /// Patches <see cref="FirearmBasicMessagesHandler.ServerShotReceived"/>.
    /// Adds the <see cref="Handlers.Player.Shooting"/> and <see cref="Handlers.Player.Shot"/> events.
    /// </summary>
    [HarmonyPatch(typeof(FirearmBasicMessagesHandler), nameof(FirearmBasicMessagesHandler.ServerShotReceived))]
    internal static class Shooting
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            int offset = -4;
            int index = newInstructions.Count + offset;

            LocalBuilder ev = generator.DeclareLocal(typeof(ShootingEventArgs));

            Label returnLabelModAmmo = generator.DefineLabel();
            Label returnLabel = generator.DefineLabel();

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Pop),
                new CodeInstruction(OpCodes.Ldloc_0),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(ShootingEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc, ev.LocalIndex),
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnShooting))),
                new CodeInstruction(OpCodes.Ldloc, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ShootingEventArgs), nameof(ShootingEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse, returnLabelModAmmo),
                new CodeInstruction(OpCodes.Ldloc, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ShootingEventArgs), nameof(ShootingEventArgs.ShotMessage))),
                new CodeInstruction(OpCodes.Starg, 1),
                new CodeInstruction(OpCodes.Ldloc_1),
            });

            newInstructions.InsertRange(newInstructions.Count - 1, new[]
            {
               new CodeInstruction(OpCodes.Ldloc, ev.LocalIndex).WithLabels(returnLabelModAmmo),
               new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ShootingEventArgs), nameof(ShootingEventArgs.IsAllowed))),
               new CodeInstruction(OpCodes.Brtrue, returnLabel),
               new CodeInstruction(OpCodes.Ldloc, ev.LocalIndex),
               new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ShootingEventArgs), nameof(ShootingEventArgs.Shooter))),
               new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.CurrentItem))),
               new CodeInstruction(OpCodes.Dup),
               new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Firearm), nameof(Firearm.Ammo))),
               new CodeInstruction(OpCodes.Ldc_I4_1),
               new CodeInstruction(OpCodes.Add),
               new CodeInstruction(OpCodes.Callvirt, PropertySetter(typeof(Firearm), nameof(Firearm.Ammo))),
            });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
