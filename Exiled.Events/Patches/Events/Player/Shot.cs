// -----------------------------------------------------------------------
// <copyright file="Shot.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1402 // File may only contain a single type

    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Core.Generic.Pools;

    using EventArgs.Player;
    using Exiled.Events.Attributes;
    using HarmonyLib;

    using InventorySystem.Items.Firearms;
    using InventorySystem.Items.Firearms.Modules;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    using Item = API.Features.Items.Item;

    /// <summary>
    /// Patches <see cref="SingleBulletHitreg.ServerProcessRaycastHit(Ray, RaycastHit)" />.
    /// Adds the <see cref="Handlers.Player.Shot" /> events.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.Shot))]
    [HarmonyPatch(typeof(SingleBulletHitreg), nameof(SingleBulletHitreg.ServerProcessRaycastHit))]
    internal static class Shot
    {
        /// <summary>
        /// Process shot.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="firearm">The firearm.</param>
        /// <param name="hit">The raycast hit.</param>
        /// <param name="destructible">The destructible.</param>
        /// <param name="damage">The damage.</param>
        /// <returns>If the shot is allowed.</returns>
        internal static bool ProcessShot(ReferenceHub player, Firearm firearm, RaycastHit hit, IDestructible destructible, ref float damage)
        {
            ShotEventArgs shotEvent = new(Player.Get(player), Item.Get(firearm).Cast<API.Features.Items.Firearm>(), hit, destructible, damage);

            Handlers.Player.OnShot(shotEvent);

            if (shotEvent.CanHurt)
                damage = shotEvent.Damage;

            return shotEvent.CanHurt;
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();
            Label jump = generator.DefineLabel();

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

                    // this.Firearm
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(StandardHitregBase), nameof(StandardHitregBase.Firearm))),

                    // hit
                    new(OpCodes.Ldarg_2),

                    // destructible
                    new(OpCodes.Ldloc_0),

                    // damage
                    new(OpCodes.Ldloca_S, 1),

                    new(OpCodes.Call, Method(typeof(Shot), nameof(ProcessShot), new[] { typeof(ReferenceHub), typeof(Firearm), typeof(RaycastHit), typeof(IDestructible), typeof(float).MakeByRefType(), })),

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
                new[]
                {
                    new CodeInstruction(OpCodes.Nop).WithLabels(jump),

                    // this.Hub
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(StandardHitregBase), nameof(StandardHitregBase.Hub))),

                    // this.Firearm
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(StandardHitregBase), nameof(StandardHitregBase.Firearm))),

                    // hit
                    new(OpCodes.Ldarg_2),

                    // destructible
                    new(OpCodes.Ldnull),

                    // damage
                    new(OpCodes.Ldc_R4, 0f),
                    new(OpCodes.Stloc_S, 1),
                    new(OpCodes.Ldloca_S, 1),

                    // Shot.ProcessShot
                    new(OpCodes.Call, Method(typeof(Shot), nameof(ProcessShot), new[] { typeof(ReferenceHub), typeof(Firearm), typeof(RaycastHit), typeof(IDestructible), typeof(float).MakeByRefType(), })),
                    new(OpCodes.Pop),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }

    /// <summary>
    /// Patches the "else" branch of ServerPerformShot raycast attempt to fire OnShot even if the raycast didn't hit anything.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.Shot))]
    [HarmonyPatch(typeof(SingleBulletHitreg), nameof(SingleBulletHitreg.ServerPerformShot))]
    internal static class Miss
    {
        /// <summary>
        /// Method to fire the OnShot event when raycast fails.
        /// </summary>
        private static void ProcessMiss(ReferenceHub player, Firearm firearm, Ray ray)
        {
            RaycastHit hit = new();
            hit.distance = firearm.BaseStats.MaxDistance(); // Assign artificial values to raycast hit
            hit.point = ray.GetPoint(hit.distance);
            hit.normal = -ray.direction;

            ShotEventArgs shotEvent = new(Player.Get(player), Item.Get(firearm) as API.Features.Items.Firearm, hit, null, 0f);

            Handlers.Player.OnShot(shotEvent);
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int firstGetDebugModeIndex = newInstructions.FindIndex(instruction => instruction.Calls(PropertyGetter(typeof(StandardHitregBase), nameof(StandardHitregBase.DebugMode))));

            int secondGetDebugModeIndex = newInstructions.FindIndex( // There are two calls to get_DebugMode, we need the second one
                firstGetDebugModeIndex + 1,
                instruction => instruction.Calls(PropertyGetter(typeof(StandardHitregBase), nameof(StandardHitregBase.DebugMode))));

            /*
            call         bool InventorySystem.Items.Firearms.Modules.StandardHitregBase::get_DebugMode()
            [] <----  we insert instructions here so they fire only in else branch
            brfalse.s    IL_00c1
            */
            int falseReturnIndex = secondGetDebugModeIndex + 1;

            newInstructions.InsertRange(falseReturnIndex, new CodeInstruction[]
            {
                // this.Hub
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(StandardHitregBase), nameof(StandardHitregBase.Hub))),

                // this.Firearm
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(StandardHitregBase), nameof(StandardHitregBase.Firearm))),

                // ray
                new(OpCodes.Ldarg_1),

                // ProcessMiss
                new(OpCodes.Call, Method(typeof(Miss), nameof(ProcessMiss), new[] { typeof(ReferenceHub), typeof(Firearm), typeof(Ray) })),
            });

            for (int i = 0; i < newInstructions.Count; i++)
                yield return newInstructions[i];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }

    /// <summary>
    /// Patches <see cref="BuckshotHitreg.ShootPellet" />.
    /// Adds the <see cref="Handlers.Player.Shot" /> events.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.Shot))]
    [HarmonyPatch(typeof(BuckshotHitreg), nameof(BuckshotHitreg.ShootPellet))]
    internal static class ShotBuckshot
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
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
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(BuckshotHitreg), nameof(BuckshotHitreg.Hub))),

                    // this.Firearm
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(BuckshotHitreg), nameof(BuckshotHitreg.Firearm))),

                    // hit
                    new(OpCodes.Ldloc_2),

                    // destructible
                    new(OpCodes.Ldloc_3),

                    // damage
                    new(OpCodes.Ldc_R4, 0f),
                    new(OpCodes.Stloc_S, 4),
                    new(OpCodes.Ldloca_S, 4),

                    new(OpCodes.Call, Method(typeof(Shot), nameof(Shot.ProcessShot), new[] { typeof(ReferenceHub), typeof(Firearm), typeof(RaycastHit), typeof(IDestructible), typeof(float).MakeByRefType(), })),

                    // if (!ev.CanHurt)
                    //    return;
                    new(OpCodes.Brfalse_S, returnLabel),
                });

            offset = 0;
            index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldsfld) + offset;

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // this.Hub
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(BuckshotHitreg), nameof(BuckshotHitreg.Hub))),

                    // this.Firearm
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(BuckshotHitreg), nameof(BuckshotHitreg.Firearm))),

                    // hit
                    new(OpCodes.Ldloc_2),

                    // destructible
                    new(OpCodes.Ldloc_3),

                    // damage
                    new(OpCodes.Ldloca_S, 4),

                    new(OpCodes.Call, Method(typeof(Shot), nameof(Shot.ProcessShot), new[] { typeof(ReferenceHub), typeof(Firearm), typeof(RaycastHit), typeof(IDestructible), typeof(float).MakeByRefType(), })),

                    // if (!ev.CanHurt)
                    //    return;
                    new(OpCodes.Brfalse_S, returnLabel),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }

    /// <summary>
    /// Patches <see cref="DisruptorHitreg.ServerPerformShot" />.
    /// Adds the <see cref="Handlers.Player.Shot" /> events.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.Shot))]
    [HarmonyPatch(typeof(DisruptorHitreg), nameof(DisruptorHitreg.ServerPerformShot))]
    internal static class ShotDisruptor
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();

            int offset = -3;
            int index = newInstructions.FindIndex(instruction => instruction.Calls(Method(typeof(DisruptorHitreg), nameof(DisruptorHitreg.CreateExplosion)))) + offset;

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // this.Hub
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(DisruptorHitreg), nameof(DisruptorHitreg.Hub))),

                    // this.Firearm
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(DisruptorHitreg), nameof(DisruptorHitreg.Firearm))),

                    // hit
                    new(OpCodes.Ldloc_S, 7),

                    // destructible
                    new(OpCodes.Ldloc_S, 8),

                    // damage
                    new(OpCodes.Ldc_R4, 0f),
                    new(OpCodes.Stloc_S, 9),
                    new(OpCodes.Ldloca_S, 9),

                    new(OpCodes.Call, Method(typeof(Shot), nameof(Shot.ProcessShot), new[] { typeof(ReferenceHub), typeof(Firearm), typeof(RaycastHit), typeof(IDestructible), typeof(float).MakeByRefType(), })),

                    // if (!ev.CanHurt)
                    //    return;
                    new(OpCodes.Brfalse_S, returnLabel),
                });

            offset = -5;
            index = newInstructions.FindLastIndex(instruction => instruction.Calls(Method(typeof(IDestructible), nameof(IDestructible.Damage)))) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // this.Hub
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(DisruptorHitreg), nameof(DisruptorHitreg.Hub))),

                    // this.Firearm
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(DisruptorHitreg), nameof(DisruptorHitreg.Firearm))),

                    // hit
                    new(OpCodes.Ldloc_S, 7),

                    // destructible
                    new(OpCodes.Ldloc_S, 8),

                    // damage
                    new(OpCodes.Ldloca_S, 9),

                    new(OpCodes.Call, Method(typeof(Shot), nameof(Shot.ProcessShot), new[] { typeof(ReferenceHub), typeof(Firearm), typeof(RaycastHit), typeof(IDestructible), typeof(float).MakeByRefType(), })),

                    // if (!ev.CanHurt)
                    //    return;
                    new(OpCodes.Brfalse_S, returnLabel),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
