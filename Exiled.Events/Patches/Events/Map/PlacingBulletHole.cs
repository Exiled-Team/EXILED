// -----------------------------------------------------------------------
// <copyright file="PlacingBulletHole.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map {
#pragma warning disable SA1118

    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using InventorySystem.Items.Firearms.Modules;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    using Player = Exiled.API.Features.Player;

    /// <summary>
    /// Patches <see cref="StandardHitregBase.PlaceBulletholeDecal"/>.
    /// Adds the <see cref="Map.PlacingBulletHole"/> event.
    /// </summary>
    [HarmonyPatch(typeof(StandardHitregBase), nameof(StandardHitregBase.PlaceBulletholeDecal))]
    internal static class PlacingBulletHole {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);
            const int offset = 0;
            int index = 0 + offset;
            Label returnLabel = generator.DefineLabel();
            LocalBuilder ev = generator.DeclareLocal(typeof(EventArgs.PlacingBulletHole));

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(SingleBulletHitreg), nameof(SingleBulletHitreg.Hub))),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new CodeInstruction(OpCodes.Ldarg_2),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(EventArgs.PlacingBulletHole))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc, ev.LocalIndex),
                new CodeInstruction(OpCodes.Call, Method(typeof(Map), nameof(Map.OnPlacingBulletHole))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(EventArgs.PlacingBulletHole), nameof(EventArgs.PlacingBulletHole.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse, returnLabel),
            });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        private static Transform GetTransform(RaycastHit hit) => hit.collider.transform;
    }
}
