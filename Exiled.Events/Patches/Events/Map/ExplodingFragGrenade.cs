// -----------------------------------------------------------------------
// <copyright file="ExplodingFragGrenade.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
    using System.Collections.Generic;
    using System.Reflection.Emit;
    using System.Runtime.Serialization.Formatters;

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Roles;
    using Exiled.Events.EventArgs;

    using Footprinting;

    using HarmonyLib;

    using InventorySystem.Items.Pickups;
    using InventorySystem.Items.ThrowableProjectiles;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="ExplosionGrenade.Explode(Footprint, Vector3, ExplosionGrenade)"/>.
    /// Adds the <see cref="Handlers.Map.OnExplodingGrenade"/> event.
    /// </summary>
    [HarmonyPatch(typeof(ExplosionGrenade), nameof(ExplosionGrenade.Explode))]
    internal static class ExplodingFragGrenade
    {
        /// <summary>
        /// Gets all the cached thrown grenades.
        /// </summary>
        internal static Dictionary<ushort, Side> GrenadeCacheAccessors { get; } = new();

        /// <summary>
        /// Trims colliders from the given array.
        /// </summary>
        /// <param name="ev"><inheritdoc cref="ExplodingGrenadeEventArgs"/></param>
        /// <param name="colliderArray">The list of colliders to trim from.</param>
        /// <returns>An array of colliders.</returns>
        public static Collider[] TrimColliders(ExplodingGrenadeEventArgs ev, Collider[] colliderArray)
        {
            List<Collider> colliders = new();
            foreach (Collider collider in colliderArray)
            {
                if (!collider.TryGetComponent(out IDestructible dest) ||
                    !ReferenceHub.TryGetHubNetID(dest.NetworkId, out ReferenceHub hub) ||
                    Player.Get(hub) is not Player player || ev.TargetsToAffect.Contains(player))
                {
                    colliders.Add(collider);
                }
            }

            return colliders.ToArray();
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            int offset = 1;
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Stloc_3) + offset;

            Label returnLabel = generator.DefineLabel();
            Label jne_0x00 = generator.DefineLabel();
            Label jne_0x01 = generator.DefineLabel();
            Label je = generator.DefineLabel();
            Label lpInc = generator.DefineLabel();
            Label lpHd = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(ExplodingGrenadeEventArgs));
            LocalBuilder side = generator.DeclareLocal(typeof(Side));
            LocalBuilder idx = generator.DeclareLocal(typeof(int));

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // Player.Get(attacker.Hub);
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(Footprint), nameof(Footprint.Hub))),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // position
                new(OpCodes.Ldarg_1),

                // grenade
                new(OpCodes.Ldarg_2),

                // Collider[]
                new(OpCodes.Ldloc_3),

                // ExplodingGrenadeEventArgs ev = new(player, position, grenade, colliders);
                // Map.OnExplodingGrenade(ev);
                // if(!ev.IsAllowed)
                //     return;
                new(OpCodes.Newobj, DeclaredConstructor(typeof(ExplodingGrenadeEventArgs), new[] { typeof(Player), typeof(Vector3), typeof(EffectGrenade), typeof(Collider[]) })),
                new(OpCodes.Stloc, ev.LocalIndex),

                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ExplodingGrenadeEventArgs), nameof(ExplodingGrenadeEventArgs.GrenadeType))),
                new(OpCodes.Ldc_I4_S, (int)GrenadeType.FragGrenade),
                new(OpCodes.Ceq),
                new(OpCodes.Brfalse_S, jne_0x01),

                new(OpCodes.Call, PropertyGetter(typeof(Server), nameof(Server.FriendlyFire))),
                new(OpCodes.Brtrue_S, jne_0x00),

                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(ExplodingFragGrenade), nameof(GrenadeCacheAccessors))).WithLabels(je),
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ExplodingGrenadeEventArgs), nameof(ExplodingGrenadeEventArgs.Grenade))),
                new(OpCodes.Ldflda, Field(typeof(EffectGrenade), nameof(EffectGrenade.Info))),
                new(OpCodes.Ldfld, Field(typeof(PickupSyncInfo), nameof(PickupSyncInfo.Serial))),
                new(OpCodes.Ldloca_S, side.LocalIndex),
                new(OpCodes.Callvirt, Method(typeof(Dictionary<ushort, Side>), nameof(Dictionary<ushort, Side>.TryGetValue))),
                new(OpCodes.Brfalse_S, jne_0x01),

                new(OpCodes.Call, PropertyGetter(typeof(ExplodingFragGrenade), nameof(GrenadeCacheAccessors))),
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ExplodingGrenadeEventArgs), nameof(ExplodingGrenadeEventArgs.Grenade))),
                new(OpCodes.Ldflda, Field(typeof(EffectGrenade), nameof(EffectGrenade.Info))),
                new(OpCodes.Ldfld, Field(typeof(PickupSyncInfo), nameof(PickupSyncInfo.Serial))),
                new(OpCodes.Callvirt, Method(typeof(Dictionary<ushort, Side>), nameof(Dictionary<ushort, Side>.Remove), new[] { typeof(ushort) })),
                new(OpCodes.Pop),

                new(OpCodes.Ldc_I4_0),
                new(OpCodes.Stloc_S, idx.LocalIndex),
                new(OpCodes.Br_S, lpInc),

                new CodeInstruction(OpCodes.Nop).WithLabels(lpHd),

                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ExplodingGrenadeEventArgs), nameof(ExplodingGrenadeEventArgs.TargetsToAffect))),
                new(OpCodes.Ldloc_S, idx.LocalIndex),
                new(OpCodes.Callvirt, Method(typeof(List<Player>), "get_Item")),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.Role))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Role), nameof(Role.Side))),
                new(OpCodes.Ldloc_S, side.LocalIndex),
                new(OpCodes.Ceq),
                new(OpCodes.Brfalse_S, lpInc),

                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ExplodingGrenadeEventArgs), nameof(ExplodingGrenadeEventArgs.TargetsToAffect))),
                new(OpCodes.Ldloc_S, idx.LocalIndex),
                new(OpCodes.Callvirt, Method(typeof(List<Player>), nameof(List<Player>.RemoveAt))),

                new CodeInstruction(OpCodes.Ldloc_S, idx.LocalIndex).WithLabels(lpInc),
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ExplodingGrenadeEventArgs), nameof(ExplodingGrenadeEventArgs.TargetsToAffect))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(List<Player>), nameof(List<Player>.Count))),
                new(OpCodes.Clt),
                new(OpCodes.Brtrue_S, lpHd),

                new(OpCodes.Br_S, jne_0x01),

                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(Exiled.Events.Events), nameof(Exiled.Events.Events.Instance))).WithLabels(jne_0x00),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Exiled.Events.Events), nameof(Exiled.Events.Events.Config))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Config), nameof(Config.IsGrenadeDamageSuppressedOnQuit))),
                new(OpCodes.Brtrue_S, je),

                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex).WithLabels(jne_0x01),
                new(OpCodes.Dup),
                new(OpCodes.Call, Method(typeof(Handlers.Map), nameof(Handlers.Map.OnExplodingGrenade))),

                new(OpCodes.Callvirt, PropertyGetter(typeof(ExplodingGrenadeEventArgs), nameof(ExplodingGrenadeEventArgs.IsAllowed))),
                new(OpCodes.Brfalse, returnLabel),

                // colliders = TrimColliders(ev, colliders)
                new(OpCodes.Ldloc, ev.LocalIndex),
                new(OpCodes.Ldloc_3),
                new(OpCodes.Call, Method(typeof(ExplodingFragGrenade), nameof(TrimColliders))),
                new(OpCodes.Stloc_3),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
