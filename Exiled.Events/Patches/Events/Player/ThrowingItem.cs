// -----------------------------------------------------------------------
// <copyright file="ThrowingItem.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs;
    using Exiled.Events.Patches.Events.Map;

    using HarmonyLib;

    using InventorySystem.Items.ThrowableProjectiles;

    using NorthwoodLib.Pools;

    using UnityEngine;
    using UnityEngine.Animations;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="ThrowableNetworkHandler.ServerProcessRequest"/>.
    /// Adds the <see cref="Handlers.Player.ThrowingItem"/> event.
    /// </summary>
    [HarmonyPatch(typeof(ThrowableNetworkHandler), nameof(ThrowableNetworkHandler.ServerProcessRequest))]
    internal static class ThrowingItem
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            int offset = 4;

            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Dup) + offset;

            Label returnLabel = generator.DefineLabel();
            Label doNotCache = generator.DefineLabel();
            Label je = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(ThrowingItemEventArgs));
            LocalBuilder player = generator.DeclareLocal(typeof(Player));

            int moveOffset = -2;

            int moveIndex = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Stloc_2) + moveOffset;

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldloc_0).MoveLabelsFrom(newInstructions[moveIndex]),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Stloc_S, player.LocalIndex),
                new(OpCodes.Call, PropertyGetter(typeof(Exiled.Events.Events), nameof(Exiled.Events.Events.Instance))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Exiled.Events.Events), nameof(Exiled.Events.Events.Config))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Config), nameof(Config.IsGrenadeDamageSuppressedOnQuit))),
                new(OpCodes.Brtrue_S, je),
                new(OpCodes.Call, PropertyGetter(typeof(Server), nameof(Server.FriendlyFire))),
                new(OpCodes.Brtrue_S, doNotCache),
                new CodeInstruction(OpCodes.Ldloc_1).WithLabels(je),
                new(OpCodes.Ldfld, Field(typeof(ThrowableItem), nameof(ThrowableItem.ItemTypeId))),
                new(OpCodes.Ldc_I4_S, (int)ItemType.GrenadeHE),
                new(OpCodes.Ceq),
                new(OpCodes.Brfalse_S, doNotCache),
                new(OpCodes.Call, PropertyGetter(typeof(ExplodingFragGrenade), nameof(ExplodingFragGrenade.GrenadeCacheAccessors))),
                new(OpCodes.Ldloc_1),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ThrowableItem), nameof(ThrowableItem.ItemSerial))),
                new(OpCodes.Callvirt, Method(typeof(Dictionary<ushort, Side>), nameof(Dictionary<ushort, Side>.ContainsKey))),
                new(OpCodes.Brtrue_S, doNotCache),
                new(OpCodes.Call, PropertyGetter(typeof(ExplodingFragGrenade), nameof(ExplodingFragGrenade.GrenadeCacheAccessors))),
                new(OpCodes.Ldloc_1),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ThrowableItem), nameof(ThrowableItem.ItemSerial))),
                new(OpCodes.Ldloc_S, player.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.Role))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(API.Features.Roles.Role), nameof(API.Features.Roles.Role.Side))),
                new(OpCodes.Callvirt, Method(typeof(Dictionary<ushort, Side>), nameof(Dictionary<ushort, Side>.Add))),
                new CodeInstruction(OpCodes.Ldloc_S, player.LocalIndex).WithLabels(doNotCache),
                new(OpCodes.Ldloc_1),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Ldfld, Field(typeof(ThrowableNetworkHandler.ThrowableItemRequestMessage), nameof(ThrowableNetworkHandler.ThrowableItemRequestMessage.Request))),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ThrowingItemEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev.LocalIndex),
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnThrowingItem))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ThrowingItemEventArgs), nameof(ThrowingItemEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, returnLabel),
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ThrowingItemEventArgs), nameof(ThrowingItemEventArgs.Item))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(API.Features.Items.Item), nameof(API.Features.Items.Item.Base))),
                new(OpCodes.Stloc_1),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Ldfld, Field(typeof(ThrowableNetworkHandler.ThrowableItemRequestMessage), nameof(ThrowableNetworkHandler.ThrowableItemRequestMessage.Serial))),
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ThrowingItemEventArgs), nameof(ThrowingItemEventArgs.RequestType))),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Ldfld, Field(typeof(ThrowableNetworkHandler.ThrowableItemRequestMessage), nameof(ThrowableNetworkHandler.ThrowableItemRequestMessage.CameraRotation))),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Ldfld, Field(typeof(ThrowableNetworkHandler.ThrowableItemRequestMessage), nameof(ThrowableNetworkHandler.ThrowableItemRequestMessage.CameraPosition))),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Ldfld, Field(typeof(ThrowableNetworkHandler.ThrowableItemRequestMessage), nameof(ThrowableNetworkHandler.ThrowableItemRequestMessage.PlayerVelocity))),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ThrowableNetworkHandler.ThrowableItemRequestMessage))[0]),
                new(OpCodes.Starg_S, 1),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
