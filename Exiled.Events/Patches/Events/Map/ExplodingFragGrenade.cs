// -----------------------------------------------------------------------
// <copyright file="ExplodingFragGrenade.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using Footprinting;

    using HarmonyLib;

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
                if (collider.TryGetComponent(out IDestructible dest) &&
                    ReferenceHub.TryGetHubNetID(dest.NetworkId, out ReferenceHub hub) &&
                    Player.Get(hub) is Player player && !ev.TargetsToAffect.Contains(player))
                {
                    colliders.Add(collider);
                }
                else
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
            LocalBuilder ev = generator.DeclareLocal(typeof(ExplodingGrenadeEventArgs));

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // Player.Get(attacker);
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(Footprint), nameof(Footprint.Hub))),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // this
                new(OpCodes.Ldarg_2),

                // Collider[]
                new(OpCodes.Ldloc_3),

                new(OpCodes.Newobj, DeclaredConstructor(typeof(ExplodingGrenadeEventArgs), new[] { typeof(Player), typeof(EffectGrenade), typeof(Collider[]) })),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc, ev.LocalIndex),
                new(OpCodes.Call, Method(typeof(Handlers.Map), nameof(Handlers.Map.OnExplodingGrenade))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ExplodingGrenadeEventArgs), nameof(ExplodingGrenadeEventArgs.IsAllowed))),
                new(OpCodes.Brfalse, returnLabel),
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
