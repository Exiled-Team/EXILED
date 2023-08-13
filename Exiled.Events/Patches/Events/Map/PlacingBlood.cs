// -----------------------------------------------------------------------
// <copyright file="PlacingBlood.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Pools;

    using Exiled.Events.EventArgs.Map;

    using HarmonyLib;

    using InventorySystem.Items.Firearms.Modules;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    using ExiledEvents = Exiled.Events.Events;

    /// <summary>
    /// Patches <see cref="StandardHitregBase.PlaceBloodDecal(Ray, RaycastHit, IDestructible)"/>.
    /// Adds the <see cref="PlacingBlood"/> event.
    /// </summary>
    [HarmonyPatch(typeof(StandardHitregBase), nameof(StandardHitregBase.PlaceBloodDecal))]
    internal static class PlacingBlood
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(PlacingBloodEventArgs));

            // if (!ExiledEvents.Instance.Config.CanSpawnBlood)
            //     return;
            //
            // PlacingBloodEventArgs ev = new(Player.Get(this.Hub), Player.Get(target.NetworkId), hit, true);
            //
            // Handlers.Map.OnPlacingBlood(ev);
            //
            // if (!ev.IsAllowed)
            //     return;
            //
            // hit.point = ev.Position;
            newInstructions.InsertRange(
                0,
                new[]
                {
                    // if (!ExiledEvents.Instance.Config.CanSpawnBlood)
                    //     return;
                    new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(ExiledEvents), nameof(ExiledEvents.Instance))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ExiledEvents), nameof(ExiledEvents.Config))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Config), nameof(Config.CanSpawnBlood))),
                    new(OpCodes.Brfalse_S, returnLabel),

                    // Player.Get(this.Hub)
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(StandardHitregBase), nameof(StandardHitregBase.Hub))),
                    new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),

                    // Player.Get(target.NetworkId)
                    new(OpCodes.Ldarg_3),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(IDestructible), nameof(IDestructible.NetworkId))),
                    new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(uint) })),

                    // hit
                    new(OpCodes.Ldarg_2),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // PlacingBloodEventArgs ev = new(Player, Player, RaycastHit, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(PlacingBloodEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Handlers.Map.OnPlacingBlood(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Map), nameof(Handlers.Map.OnPlacingBlood))),

                    // if (!ev.isAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(PlacingBloodEventArgs), nameof(PlacingBloodEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),

                    // hit.info = ev.Position
                    new(OpCodes.Ldarga_S, 2),
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(PlacingBloodEventArgs), nameof(PlacingBloodEventArgs.Position))),
                    new(OpCodes.Callvirt, PropertySetter(typeof(RaycastHit), nameof(RaycastHit.point))),
                });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}