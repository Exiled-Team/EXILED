// -----------------------------------------------------------------------
// <copyright file="BreakingScp2176.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
#pragma warning disable SA1118 // The parameter spans multiple lines

    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using InventorySystem.Items.ThrowableProjectiles;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp2176Projectile.ServerShatter"/>.
    /// Supplements the <see cref="Handlers.Map.ExplodingGrenade"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp2176Projectile), nameof(Scp2176Projectile.ServerShatter))]
    internal static class BreakingScp2176
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            // The return label
            Label retLabel = generator.DefineLabel();

            newInstructions.InsertRange(0, new[]
            {
                // new ExplodingGrenadeEventArgs(Player.Get(PreviousOwner.Hub), this, Array.Empty<Collider>());
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldflda, Field(typeof(Scp2176Projectile), nameof(Scp2176Projectile.PreviousOwner))),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Footprinting.Footprint), nameof(Footprinting.Footprint.Hub))),
                new CodeInstruction(OpCodes.Call, Method(typeof(API.Features.Player), nameof(Exiled.API.Features.Player.Get), new[] { typeof(ReferenceHub) })),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Newarr, typeof(UnityEngine.Collider)),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(ExplodingGrenadeEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),

                // Handlers.Map.OnExplodingGrenade(ev);
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Map), nameof(Handlers.Map.OnExplodingGrenade))),

                // if(!ev.IsAllowed) return;
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ExplodingGrenadeEventArgs), nameof(ExplodingGrenadeEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse_S, retLabel),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(retLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
