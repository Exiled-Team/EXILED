// -----------------------------------------------------------------------
// <copyright file="Shot.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Pools;

    using EventArgs.Player;

    using HarmonyLib;

    using InventorySystem.Items.Firearms;
    using InventorySystem.Items.Firearms.Modules;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="SingleBulletHitreg.ServerProcessRaycastHit(Ray, RaycastHit)" />.
    ///     Adds the <see cref="Handlers.Player.Shooting" /> and <see cref="Handlers.Player.Shot" /> events.
    /// </summary>
    [HarmonyPatch]
    internal static class Shot
    {
        [HarmonyPatch(typeof(SingleBulletHitreg), nameof(SingleBulletHitreg.ServerProcessRaycastHit))]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> ShotBullet(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();
            Label jump = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(ShotEventArgs));

            int offset = 2;
            int index = newInstructions.FindLastIndex(
                instruction => instruction.Calls(Method(typeof(FirearmBaseStats), nameof(FirearmBaseStats.DamageAtDistance)))) + offset;

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // this.Hub
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(StandardHitregBase), nameof(StandardHitregBase.Hub))),

                    // hit
                    new(OpCodes.Ldarg_2),

                    // destructible
                    new(OpCodes.Ldloc_0),

                    // damage
                    new(OpCodes.Ldloca_S, 1),

                    new(OpCodes.Call, Method(typeof(Shot), nameof(ProcessShot), new[] { typeof(ReferenceHub), typeof(RaycastHit), typeof(IDestructible), typeof(float).MakeByRefType(), })),

                    // if (!ev.CanHurt)
                    //    return;
                    new(OpCodes.Brfalse_S, returnLabel),
                });

            offset = -3;
            index = newInstructions.FindLastIndex(
                instruction => instruction.Calls(Method(typeof(StandardHitregBase), nameof(StandardHitregBase.PlaceBulletholeDecal)))) + offset;

            // replace the original goto label
            newInstructions.FindAll(instruction => instruction.opcode == OpCodes.Brfalse).ForEach(instruction => instruction.operand = jump);

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    new CodeInstruction(OpCodes.Nop).WithLabels(jump),

                    // this.Hub
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(StandardHitregBase), nameof(StandardHitregBase.Hub))),

                    // hit
                    new(OpCodes.Ldarg_2),

                    // destructible
                    new(OpCodes.Ldnull),

                    // damage
                    new(OpCodes.Ldc_R4, 0f),
                    new(OpCodes.Stloc_S, 1),
                    new(OpCodes.Ldloca_S, 1),

                    // Shot.ProcessShot
                    new(OpCodes.Call, Method(typeof(Shot), nameof(ProcessShot), new[] { typeof(ReferenceHub), typeof(RaycastHit), typeof(IDestructible), typeof(float).MakeByRefType(), })),
                    new(OpCodes.Pop),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        /// <summary>
        ///     Patches <see cref="BuckshotHitreg.ShootPellet" />.
        ///     Adds the <see cref="Handlers.Player.Shooting" /> and <see cref="Handlers.Player.Shot" /> events.
        /// </summary>
        [HarmonyPatch(typeof(BuckshotHitreg), nameof(BuckshotHitreg.ShootPellet))]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> ShotPellet(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();

            int offset = -3;
            int index = newInstructions.FindIndex(instruction => instruction.Calls(Method(typeof(StandardHitregBase), nameof(StandardHitregBase.PlaceBulletholeDecal)))) + offset;

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // this.Hub
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(BuckshotHitreg), nameof(BuckshotHitreg.Hub))),

                    // hit
                    new(OpCodes.Ldloc_2),

                    // destructible
                    new(OpCodes.Ldloc_3),

                    // damage
                    new(OpCodes.Ldc_R4, 0f),
                    new(OpCodes.Stloc_S, 4),
                    new(OpCodes.Ldloca_S, 4),

                    new(OpCodes.Call, Method(typeof(Shot), nameof(ProcessShot), new[] { typeof(ReferenceHub), typeof(RaycastHit), typeof(IDestructible), typeof(float).MakeByRefType(), })),

                    // if (!ev.CanHurt)
                    //    return;
                    new(OpCodes.Brfalse_S, returnLabel),
                });

            offset = 0;
            index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldsfld) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // this.Hub
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(BuckshotHitreg), nameof(BuckshotHitreg.Hub))),

                    // hit
                    new(OpCodes.Ldloc_2),

                    // destructible
                    new(OpCodes.Ldloc_3),

                    // damage
                    new(OpCodes.Ldloca_S, 4),

                    new(OpCodes.Call, Method(typeof(Shot), nameof(ProcessShot), new[] { typeof(ReferenceHub), typeof(RaycastHit), typeof(IDestructible), typeof(float).MakeByRefType(), })),

                    // if (!ev.CanHurt)
                    //    return;
                    new(OpCodes.Brfalse_S, returnLabel),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private static bool ProcessShot(ReferenceHub player, RaycastHit hit, IDestructible destructible, ref float damage)
        {
            ShotEventArgs shotEvent = new ShotEventArgs(Player.Get(player), hit, destructible, damage);

            Handlers.Player.OnShot(shotEvent);

            if (shotEvent.CanHurt)
                damage = shotEvent.Damage;

            return shotEvent.CanHurt;
        }
    }
}