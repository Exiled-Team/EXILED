// -----------------------------------------------------------------------
// <copyright file="Scp244Spawning.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Core.Generic.Pools;
    using Exiled.API.Features.Pickups;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Map;
    using MapGeneration;
    using UnityEngine;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
    using HarmonyLib;
    using InventorySystem.Items.Pickups;
    using InventorySystem.Items.Usables.Scp244;
    using Mirror;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp244Spawner.SpawnScp244" />.
    /// Adds the <see cref="Handlers.Map.Scp244Spawning" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Map), nameof(Handlers.Map.Scp244Spawning))]
    [HarmonyPatch(typeof(Scp244Spawner), nameof(Scp244Spawner.SpawnScp244))]
    internal static class Scp244Spawning
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label continueLabel = generator.DefineLabel();

            LocalBuilder pickup = generator.DeclareLocal(typeof(ItemPickupBase));

            int index = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Dup);

            newInstructions.RemoveRange(index, 1);

            int offset = 1;
            index = newInstructions.FindIndex(i => i.opcode == OpCodes.Dup) + offset;
            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, pickup.LocalIndex),
            });

            offset = -2;
            index = newInstructions.FindIndex(i => i.Calls(Method(typeof(NetworkServer), nameof(NetworkServer.Spawn), new[] { typeof(GameObject), typeof(NetworkConnection) }))) + offset;

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldsfld, Field(typeof(Scp244Spawner), nameof(Scp244Spawner.CompatibleRooms))).MoveLabelsFrom(newInstructions[index]),
                new(OpCodes.Ldloc_0),
                new(OpCodes.Callvirt, Method(typeof(List<RoomIdentifier>), "get_Item")),

                new(OpCodes.Ldloc_S, pickup.LocalIndex),
                new(OpCodes.Call, Method(typeof(Pickup), nameof(Pickup.Get), new[] { typeof(ItemPickupBase) })),

                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(Scp244SpawningEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Call, Method(typeof(Handlers.Map), nameof(Handlers.Map.OnScp244Spawning))),

                new(OpCodes.Call, PropertyGetter(typeof(Scp244SpawningEventArgs), nameof(Scp244SpawningEventArgs.IsAllowed))),
                new(OpCodes.Brtrue_S, continueLabel),

                new(OpCodes.Ldloc_S, pickup.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ItemPickupBase), nameof(ItemPickupBase.gameObject))),
                new(OpCodes.Call, Method(typeof(NetworkServer), nameof(NetworkServer.Destroy))),
                new(OpCodes.Ret),

                new CodeInstruction(OpCodes.Nop).WithLabels(continueLabel),
                new(OpCodes.Ldloc_S, pickup.LocalIndex),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}