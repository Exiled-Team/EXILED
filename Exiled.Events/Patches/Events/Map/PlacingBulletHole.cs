// -----------------------------------------------------------------------
// <copyright file="PlacingBulletHole.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.Attributes;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using InventorySystem.Items.Firearms.Modules;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    using Player = Exiled.API.Features.Player;

    /// <summary>
    ///     Patches <see cref="StandardHitregBase.PlaceBulletholeDecal" />.
    ///     Adds the <see cref="Map.PlacingBulletHole" /> event.
    /// </summary>
    [EventPatch(typeof(Map), nameof(Map.PlacingBulletHole))]
    [HarmonyPatch(typeof(StandardHitregBase), nameof(StandardHitregBase.PlaceBulletholeDecal))]
    internal static class PlacingBulletHole
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);
            const int offset = 0;
            int index = 0 + offset;
            Label returnLabel = generator.DefineLabel();
            LocalBuilder ev = generator.DeclareLocal(typeof(EventArgs.Map.PlacingBulletHole));

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(SingleBulletHitreg), nameof(SingleBulletHitreg.Hub))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                    new(OpCodes.Ldarg_2),
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(EventArgs.Map.PlacingBulletHole))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc, ev.LocalIndex),
                    new(OpCodes.Call, Method(typeof(Map), nameof(Map.OnPlacingBulletHole))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(EventArgs.Map.PlacingBulletHole), nameof(EventArgs.Map.PlacingBulletHole.IsAllowed))),
                    new(OpCodes.Brfalse, returnLabel),
                });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}