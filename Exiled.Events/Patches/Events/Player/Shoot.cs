// -----------------------------------------------------------------------
// <copyright file="Shoot.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1313
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using CustomPlayerEffects;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

#pragma warning disable SA1512 // Single-line comments should not be followed by blank line
#pragma warning disable SA1005 // Single line comments should begin with single space
#pragma warning disable SA1515 // Single-line comment should be preceded by blank line

    /// <summary>
    /// Patches <see cref="WeaponManager.CallCmdShoot(GameObject, HitBoxType, Vector3, Vector3, Vector3)"/>.
    /// Adds the <see cref="Handlers.Player.Shooting"/> and <see cref="Handlers.Player.Shot"/> events.
    /// </summary>
    [HarmonyPatch(typeof(WeaponManager), nameof(WeaponManager.CallCmdShoot))]
    internal static class Shoot
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            const byte DAMAGE_LOCAL_INDEX = 13;

            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            // search for "this._hub" before the only "PlayerEffect.ServerDisable()"
            int offset = -17;
            int index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Callvirt && (MethodInfo)instruction.operand == Method(typeof(PlayerEffect), nameof(PlayerEffect.ServerDisable))) + offset;

            LocalBuilder shoootingEv = generator.DeclareLocal(typeof(ShootingEventArgs));
            LocalBuilder shotEv = generator.DeclareLocal(typeof(ShotEventArgs));

            Label returnLabel = generator.DefineLabel();

            newInstructions.InsertRange(index, new[]
            {
                // Player.Get(this.gameObject)
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(WeaponManager), nameof(WeaponManager._hub))),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // GameObject target
                new CodeInstruction(OpCodes.Ldarg_1),

                // Vector3 targetPos
                new CodeInstruction(OpCodes.Ldarg_S, 5),

                // true
                new CodeInstruction(OpCodes.Ldc_I4_1),

                // var ev = new ShootingEventArgs(...)
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(ShootingEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc_S, shoootingEv.LocalIndex),

                // Handlers.Server.OnShooting(ev)
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnShooting))),

                // if (!ev.IsAllowed)
                //    return;
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ShootingEventArgs), nameof(ShootingEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse, returnLabel),
            });

            // Search for the last "stloc.S <DAMAGE_LOCAL_INDEX>" call before the only "PlayerStats.HurtPlayer()"
            offset = 1;
            index = newInstructions.FindLastIndex(instruction =>
                instruction.opcode == OpCodes.Stloc_S &&
                ((LocalBuilder)instruction.operand).LocalIndex == DAMAGE_LOCAL_INDEX) + offset;

            newInstructions.InsertRange(index, new[]
            {
                // Player.Get(this.gameObject)
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(WeaponManager), nameof(WeaponManager._hub))),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // GameObject target
                new CodeInstruction(OpCodes.Ldarg_1),

                // HitboxType hitboxType
                new CodeInstruction(OpCodes.Ldarg_2),

                // float num3
                new CodeInstruction(OpCodes.Ldloc_S, 12),
                // float num4
                new CodeInstruction(OpCodes.Ldloc_S, DAMAGE_LOCAL_INDEX),

                // true
                new CodeInstruction(OpCodes.Ldc_I4_1),

                // var ev = new ShotEventArgs(...)
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(ShotEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc_S, shotEv.LocalIndex),

                // Handlers.Player.OnShot(ev)
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnShot))),

                // if (!ev.CanHurt)
                //    return;
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ShotEventArgs), nameof(ShotEventArgs.CanHurt))),
                new CodeInstruction(OpCodes.Brfalse, returnLabel),

                // num4 = ev.Damage
                new CodeInstruction(OpCodes.Ldloc_S, shotEv.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ShotEventArgs), nameof(ShotEventArgs.Damage))),
                new CodeInstruction(OpCodes.Stloc_S, DAMAGE_LOCAL_INDEX),
            });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
