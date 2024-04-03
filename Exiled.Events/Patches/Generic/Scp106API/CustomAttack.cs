// -----------------------------------------------------------------------
// <copyright file="CustomAttack.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic.Scp106API
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Core.Generic.Pools;
    using Exiled.API.Features;
    using HarmonyLib;
    using PlayerRoles.PlayableScps.Scp106;
    using PlayerRoles.Subroutines;

    using static HarmonyLib.AccessTools;

    using BaseScp106Role = PlayerRoles.PlayableScps.Scp106.Scp106Role;
    using Scp106Role = API.Features.Roles.Scp106Role;

    /// <summary>
    /// Patches <see cref="Scp106Attack.ServerShoot"/>.
    /// Adds the <see cref="Scp106Role.AttackDamage" />, <see cref="Scp106Role.CorrodingTime" />,  <see cref="Scp106Role.VigorCaptureReward" />property.
    /// </summary>
    // [HarmonyPatch(typeof(Scp106Attack), nameof(Scp106Attack.ServerShoot))]
    internal class CustomAttack
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder scp049Role = generator.DeclareLocal(typeof(Scp106Role));

            // replace "new ScpDamageHandler(base.Owner, 30f, DeathTranslations.PocketDecay);"
            // with
            // Scp106Role scp106Role = Player.Get(this.Owner).Role.As<Scp106Role>()
            // "new ScpDamageHandler(base.Owner, scp106Role.AttackDamage, DeathTranslations.PocketDecay);"
            int offset = 0;
            int index = newInstructions.FindIndex(instruction => instruction.operand == (object)Scp106Attack.AttackDamage) + offset;
            newInstructions.RemoveAt(index);

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // Player.Get(base.Owner).Role
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, PropertyGetter(typeof(StandardSubroutine<BaseScp106Role>), nameof(StandardSubroutine<BaseScp106Role>.Owner))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.Role))),

                    // (Player.Get(base.Owner).Role as Scp106Role).AttackDamage
                    new(OpCodes.Isinst, typeof(Scp106Role)),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Scp106Role), nameof(Scp106Role.AttackDamage))),
                });

            // replace "base.Vigor.VigorAmount += 0.3f;"
            // with
            // Scp106Role scp106Role = Player.Get(this.Owner).Role.As<Scp106Role>()
            // "base.Vigor.VigorAmount += scp106Role.VigorCaptureReward;"
            offset = 0;
            index = newInstructions.FindIndex(instruction => instruction.operand == (object)Scp106Attack.VigorCaptureReward) + offset;
            newInstructions.RemoveAt(index);

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // Player.Get(base.Owner).Role
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, PropertyGetter(typeof(StandardSubroutine<BaseScp106Role>), nameof(StandardSubroutine<BaseScp106Role>.Owner))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.Role))),

                    // (Player.Get(base.Owner).Role as Scp106Role).AttackDamage
                    new(OpCodes.Isinst, typeof(Scp106Role)),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Scp106Role), nameof(Scp106Role.VigorCaptureReward))),
                });

            // replace "base.Vigor.VigorAmount += 0.3f;"
            // with
            // Scp106Role scp106Role = Player.Get(this.Owner).Role.As<Scp106Role>()
            // "base.Vigor.VigorAmount += scp106Role.VigorCaptureReward;"
            offset = 0;
            index = newInstructions.FindIndex(instruction => instruction.operand == (object)Scp106Attack.VigorCaptureReward) + offset;
            newInstructions.RemoveAt(index);

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // Player.Get(base.Owner).Role
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, PropertyGetter(typeof(StandardSubroutine<BaseScp106Role>), nameof(StandardSubroutine<BaseScp106Role>.Owner))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.Role))),

                    // (Player.Get(base.Owner).Role as Scp106Role).AttackDamage
                    new(OpCodes.Isinst, typeof(Scp106Role)),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Scp106Role), nameof(Scp106Role.VigorCaptureReward))),
                });

            // replace "playerEffectsController.EnableEffect<Corroding>(20f, false);"
            // with
            // Scp106Role scp106Role = Player.Get(this.Owner).Role.As<Scp106Role>()
            // "playerEffectsController.EnableEffect<Corroding>(scp106Role.CorrodingTime, false);"
            offset = 0;
            index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldc_R4) + offset;
            newInstructions.RemoveAt(index);

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // Player.Get(base.Owner).Role
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, PropertyGetter(typeof(StandardSubroutine<BaseScp106Role>), nameof(StandardSubroutine<BaseScp106Role>.Owner))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.Role))),

                    // (Player.Get(base.Owner).Role as Scp106Role).CorrodingTime
                    new(OpCodes.Isinst, typeof(Scp106Role)),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Scp106Role), nameof(Scp106Role.CorrodingTime))),
                });
            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
