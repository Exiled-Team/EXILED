// -----------------------------------------------------------------------
// <copyright file="SpawningItemInLocker.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Pickups;
    using Exiled.API.Features.Pools;
    using Exiled.Events.EventArgs.Map;
    using HarmonyLib;
    using InventorySystem.Items.Pickups;
    using MapGeneration.Distributors;
    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="LockerChamber.SpawnItem"/>
    /// to add <see cref="Handlers.Map.SpawningItemInLocker"/> event.
    /// </summary>
    [HarmonyPatch(typeof(LockerChamber), nameof(LockerChamber.SpawnItem))]
    internal class SpawningItemInLocker
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(SpawningItemInLockerEventArgs));

            Label continueLabel = generator.DefineLabel();
            Label initSpawn = generator.DefineLabel();
            Label addToList = generator.DefineLabel();
            Label notDestroy = generator.DefineLabel();

            int offset = 2;
            int index = newInstructions.FindIndex(x => x.Calls(Method(typeof(HashSet<Rigidbody>), nameof(HashSet<Rigidbody>.Add)))) + offset;

            int skipIndex = newInstructions.FindLastIndex(x => x.Is(OpCodes.Ldloc_S, 4));
            newInstructions[skipIndex].labels.Add(initSpawn);

            newInstructions[index + 3].labels.Add(addToList);

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // this
                    new(OpCodes.Ldarg_0),

                    // ipb
                    new(OpCodes.Ldloc_S, 4),

                    // this._spawnOnFirstChamberOpening
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(LockerChamber), nameof(LockerChamber._spawnOnFirstChamberOpening))),

                    // true
                    new(OpCodes.Ldc_I4_0),

                    // SpawningItemInLockerEventArgs ev = new(this, ipb, this._spawnOnFirstChamberOpening, true);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(SpawningItemInLockerEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Handlers.Map.OnSpawningItemInLocker(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Map), nameof(Handlers.Map.OnSpawningItemInLocker))),

                    // if (!ev.IsAllowed)
                    //  return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(SpawningItemInLockerEventArgs), nameof(SpawningItemInLockerEventArgs.IsAllowed))),
                    new(OpCodes.Brtrue_S, continueLabel),

                    new(OpCodes.Ret),

                    // if (ipb.Info.Serial != ev.Pickup.Serial)
                    //      Object.Destroy(ipb);
                    //      ipb = ev.Pickup.Base;
                    new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex).WithLabels(continueLabel),
                    new(OpCodes.Ldloc_S, 4),
                    new(OpCodes.Ldfld, Field(typeof(ItemPickupBase), nameof(ItemPickupBase.Info))),
                    new(OpCodes.Ldfld, Field(typeof(PickupSyncInfo), nameof(PickupSyncInfo.Serial))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(SpawningItemInLockerEventArgs), nameof(SpawningItemInLockerEventArgs.Pickup))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Pickup), nameof(Pickup.Serial))),
                    new(OpCodes.Beq_S, notDestroy),

                    new(OpCodes.Ldloc_S, 4),
                    new(OpCodes.Call, Method(typeof(Object), nameof(Object.Destroy), new[] { typeof(Object) })),

                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(SpawningItemInLockerEventArgs), nameof(SpawningItemInLockerEventArgs.Pickup))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Pickup), nameof(Pickup.Base))),
                    new(OpCodes.Stloc_0),

                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Brtrue_S, initSpawn),

                    new(OpCodes.Br_S, addToList),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}