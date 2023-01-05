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
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using InventorySystem.Items.Firearms;
    using InventorySystem.Items.Firearms.Modules;

    using NorthwoodLib.Pools;
    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="SingleBulletHitreg.ServerProcessRaycastHit(Ray, RaycastHit)" />.
    ///     Adds the <see cref="Handlers.Player.Shooting" /> and <see cref="Handlers.Player.Shot" /> events.
    /// </summary>
    [HarmonyPatch(typeof(SingleBulletHitreg), nameof(SingleBulletHitreg.ServerProcessRaycastHit))]
    internal static class Shot
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label returnLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(ShotEventArgs));

            const int offset = 2;
            int index = newInstructions.FindLastIndex(
                instruction => instruction.Calls(Method(typeof(FirearmBaseStats), nameof(FirearmBaseStats.DamageAtDistance)))) + offset;

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // Player.Get(this.Hub)
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(StandardHitregBase), nameof(StandardHitregBase.Hub))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // hit
                    new(OpCodes.Ldarg_2),

                    // destructible
                    new(OpCodes.Ldloc_0),

                    // damage
                    new(OpCodes.Ldloc_1),

                    // ShotEventArgs ev = new(Player, RaycastHit, IDestructible, float)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ShotEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc, ev.LocalIndex),

                    // Handlers.Player.OnShot(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnShot))),

                    // if (!ev.CanHurt)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ShotEventArgs), nameof(ShotEventArgs.CanHurt))),
                    new(OpCodes.Brfalse, returnLabel),

                    // damage = ev.Damage
                    new(OpCodes.Ldloc, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ShotEventArgs), nameof(ShotEventArgs.Damage))),
                    new(OpCodes.Stloc_1),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        /// <summary>
        ///     Patches <see cref="BuckshotHitreg.ShootPellet" />.
        ///     Adds the <see cref="Handlers.Player.Shooting" /> and <see cref="Handlers.Player.Shot" /> events.
        /// </summary>
        [HarmonyPatch(typeof(BuckshotHitreg), nameof(BuckshotHitreg.ShootPellet))]
        internal static class ShotPellets
        {
            private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
            {
                List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

                Label returnLabel = generator.DefineLabel();

                LocalBuilder ev = generator.DeclareLocal(typeof(ShotEventArgs));

                const int offset = 0;
                int index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldsfld) + offset;

                newInstructions.InsertRange(
                    index,
                    new[]
                    {
                        // Player player = Player.Get(this.Hub)
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new(OpCodes.Callvirt, PropertyGetter(typeof(BuckshotHitreg), nameof(BuckshotHitreg.Hub))),
                        new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                        // hit
                        new(OpCodes.Ldloc_2),

                        // destructible
                        new(OpCodes.Ldloc_3),

                        // damage
                        new(OpCodes.Ldloc, 4),

                        // ShotEventArgs ev = new(Player, RaycastHit, IDestructible, float)
                        new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ShotEventArgs))[0]),
                        new(OpCodes.Dup),
                        new(OpCodes.Dup),
                        new(OpCodes.Stloc, ev.LocalIndex),

                        // Handlers.Player.OnShot(ev)
                        new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnShot))),

                        // if (!ev.CanHurt)
                        //    return;
                        new(OpCodes.Callvirt, PropertyGetter(typeof(ShotEventArgs), nameof(ShotEventArgs.CanHurt))),
                        new(OpCodes.Brfalse, returnLabel),

                        // damage = ev.Damage
                        new(OpCodes.Ldloc, ev.LocalIndex),
                        new(OpCodes.Callvirt, PropertyGetter(typeof(ShotEventArgs), nameof(ShotEventArgs.Damage))),
                        new(OpCodes.Stloc, 4),
                    });

                newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

                for (int z = 0; z < newInstructions.Count; z++)
                    yield return newInstructions[z];

                ListPool<CodeInstruction>.Shared.Return(newInstructions);
            }
        }
    }
}