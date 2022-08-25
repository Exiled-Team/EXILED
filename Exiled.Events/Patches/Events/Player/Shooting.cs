// -----------------------------------------------------------------------
// <copyright file="Shooting.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using InventorySystem.Items.Firearms.BasicMessages;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="FirearmBasicMessagesHandler.ServerShotReceived" />.
    ///     Adds the <see cref="Handlers.Player.Shooting" /> and <see cref="Handlers.Player.Shot" /> events.
    /// </summary>
    [HarmonyPatch(typeof(FirearmBasicMessagesHandler), nameof(FirearmBasicMessagesHandler.ServerShotReceived))]
    internal static class Shooting
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            int offset = -6;
            int index = newInstructions.Count + offset;

            LocalBuilder ev = generator.DeclareLocal(typeof(ShootingEventArgs));
            LocalBuilder cmp = generator.DeclareLocal(typeof(bool));
            LocalBuilder firearm = generator.DeclareLocal(typeof(Item));

            Label returnLabelModAmmo = generator.DefineLabel();
            Label jcc = generator.DefineLabel();
            Label returnLabel = generator.DefineLabel();

            newInstructions.InsertRange(index, new[]
            {
                new(OpCodes.Pop),
                new(OpCodes.Ldloc_0),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ShootingEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc, ev.LocalIndex),
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnShooting))),

                new(OpCodes.Callvirt, PropertyGetter(typeof(ShootingEventArgs), nameof(ShootingEventArgs.IsAllowed))),
                new(OpCodes.Stloc_S, cmp.LocalIndex),

                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ShootingEventArgs), nameof(ShootingEventArgs.Player))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.CurrentItem))),
                new(OpCodes.Stloc_S, firearm.LocalIndex),

                new(OpCodes.Ldloc_S, cmp.LocalIndex),
                new(OpCodes.Brtrue_S, jcc),
                new(OpCodes.Ldloc_S, firearm.LocalIndex),
                new(OpCodes.Brfalse_S, returnLabel),
                new(OpCodes.Br_S, returnLabelModAmmo),

                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex).WithLabels(jcc),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ShootingEventArgs), nameof(ShootingEventArgs.ShotMessage))),
                new(OpCodes.Starg, 1),
                new(OpCodes.Ldloc_1),
            });

            newInstructions.InsertRange(newInstructions.Count - 1, new[]
            {
                new CodeInstruction(OpCodes.Ldloc_S, cmp.LocalIndex).WithLabels(returnLabelModAmmo),
                new(OpCodes.Brtrue_S, returnLabel),
                new(OpCodes.Ldloc_S, firearm.LocalIndex),
                new(OpCodes.Dup),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Firearm), nameof(Firearm.Ammo))),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Add),
                new(OpCodes.Callvirt, PropertySetter(typeof(Firearm), nameof(Firearm.Ammo))),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
