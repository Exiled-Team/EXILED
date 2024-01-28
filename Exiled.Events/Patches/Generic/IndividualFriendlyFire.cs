// -----------------------------------------------------------------------
// <copyright file="IndividualFriendlyFire.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
#pragma warning disable SA1402
    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Core.Generic.Pools;

    using Footprinting;

    using HarmonyLib;

    using PlayerRoles;

    using PlayerStatsSystem;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Checks friendly fire rules.
    /// </summary>
    public static class IndividualFriendlyFire
    {
        /// <summary>
        /// Checks if there can be damage between two players, according to the FF rules.
        /// </summary>
        /// <param name="attackerHub">The person attacking.</param>
        /// <param name="victimHub">The person being attacked.</param>
        /// <returns>True if the attacker can damage the victim.</returns>
        /// <remarks>Use <see cref="CheckFriendlyFirePlayer(Footprint, ReferenceHub)"/> instead of this if the damage is not done instantly.</remarks>
        public static bool CheckFriendlyFirePlayer(ReferenceHub attackerHub, ReferenceHub victimHub) => CheckFriendlyFirePlayer(new Footprint(attackerHub), victimHub);

        /// <summary>
        /// Checks if there can be damage between two players, according to the FF rules.
        /// </summary>
        /// <param name="attackerFootprint">The person attacking.</param>
        /// <param name="victimHub">The person being attacked.</param>
        /// <returns>True if the attacker can damage the victim.</returns>
        public static bool CheckFriendlyFirePlayer(Footprint attackerFootprint, ReferenceHub victimHub) => CheckFriendlyFirePlayerRules(attackerFootprint, victimHub, out _);

        /// <summary>
        /// Checks if there can be damage between two players, according to the FF rules.
        /// </summary>
        /// <param name="attackerHub">The person attacking.</param>
        /// <param name="victimHub">The person being attacked.</param>
        /// <returns>True if the attacker can damage the victim.</returns>
        /// <remarks>Use <see cref="CheckFriendlyFirePlayerHitbox(Footprint, ReferenceHub)"/> instead of this if the damage is not done instantly.</remarks>
        public static bool CheckFriendlyFirePlayerHitbox(ReferenceHub attackerHub, ReferenceHub victimHub) => CheckFriendlyFirePlayerHitbox(new Footprint(attackerHub), victimHub);

        /// <summary>
        /// Checks if there can be damage between two players, according to the FF rules.
        /// </summary>
        /// <param name="attackerFootprint">The person attacking.</param>
        /// <param name="victimHub">The person being attacked.</param>
        /// <returns>True if the attacker can damage the victim.</returns>
        public static bool CheckFriendlyFirePlayerHitbox(Footprint attackerFootprint, ReferenceHub victimHub) => Server.FriendlyFire || CheckFriendlyFirePlayerRules(attackerFootprint, victimHub, out _);

        /// <summary>
        /// Checks if there can be damage between two players, according to the FF rules.
        /// </summary>
        /// <param name="attackerHub">The person attacking.</param>
        /// <param name="victimHub">The person being attacked.</param>
        /// <param name="ffMultiplier"> FF multiplier. </param>
        /// <returns> True if the attacker can damage the victim.</returns>
        /// <remarks> Friendly fire multiplier is also provided back if needed. </remarks>
        /// <remarks>Use <see cref="CheckFriendlyFirePlayerRules(Footprint, ReferenceHub, out float)"/> instead of this if the damage is not done instantly.</remarks>
        public static bool CheckFriendlyFirePlayerRules(ReferenceHub attackerHub, ReferenceHub victimHub, out float ffMultiplier) => CheckFriendlyFirePlayerRules(new Footprint(attackerHub), victimHub, out ffMultiplier);

        /// <summary>
        /// Checks if there can be damage between two players, according to the FF rules.
        /// </summary>
        /// <param name="attackerFootprint">The person attacking.</param>
        /// <param name="victimHub">The person being attacked.</param>
        /// <param name="ffMultiplier"> FF multiplier. </param>
        /// <returns> True if the attacker can damage the victim.</returns>
        /// <remarks> Friendly fire multiplier is also provided back if needed. </remarks>
        public static bool CheckFriendlyFirePlayerRules(Footprint attackerFootprint, ReferenceHub victimHub, out float ffMultiplier)
        {
            ffMultiplier = 1f;

            // Return false, no custom friendly fire allowed, default to NW logic for FF. No point in processing if FF is enabled across the board.
            if (Server.FriendlyFire)
                return HitboxIdentity.CheckFriendlyFire(attackerFootprint.Role, victimHub.roleManager.CurrentRole.RoleTypeId);

            // always allow damage from Server.Host
            if (attackerFootprint.Hub == Server.Host.ReferenceHub)
                return true;

            // Only check friendlyFire if the FootPrint hasn't changed (Fix for Grenade not dealing damage because it's from a dead player)
            // TODO rework FriendlyFireRule to make it compatible with Footprint
            if (!attackerFootprint.SameLife(new(attackerFootprint.Hub)))
                return HitboxIdentity.CheckFriendlyFire(attackerFootprint.Role, victimHub.roleManager.CurrentRole.RoleTypeId);

            if (attackerFootprint.Hub is null || victimHub is null)
            {
                Log.Debug($"CheckFriendlyFirePlayerRules, Attacker hub null: {attackerFootprint.Hub is null}, Victim hub null: {victimHub is null}");
                return true;
            }

            try
            {
                Player attacker = Player.Get(attackerFootprint.Hub);
                Player victim = Player.Get(victimHub);
                if (attacker is null || victim is null)
                {
                    Log.Debug($"CheckFriendlyFirePlayerRules, Attacker null: {attacker is null}, Victim null: {victim is null}");
                    return true;
                }

                if (attacker == victim)
                {
                    Log.Debug("CheckFriendlyFirePlayerRules, Attacker player was equal to Victim, likely suicide");
                    return true;
                }

                Log.Debug($"CheckFriendlyFirePlayerRules, Attacker role {attacker.Role} and Victim {victim.Role}");

                if (!string.IsNullOrEmpty(victim.UniqueRole))
                {
                    // If 035 is being shot, then we need to check if we are an 035, then check if the attacker is allowed to attack us
                    if (victim.CustomRoleFriendlyFireMultiplier.Count > 0)
                    {
                        if (victim.CustomRoleFriendlyFireMultiplier.TryGetValue(victim.UniqueRole, out Dictionary<RoleTypeId, float> pairedData))
                        {
                            if (pairedData.ContainsKey(attacker.Role))
                            {
                                ffMultiplier = pairedData[attacker.Role];
                                return true;
                            }
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(attacker.UniqueRole))
                {
                    // If 035 is attacking, whether to allow or disallow based on victim role.
                    if (attacker.CustomRoleFriendlyFireMultiplier.Count > 0)
                    {
                        if (attacker.CustomRoleFriendlyFireMultiplier.TryGetValue(attacker.UniqueRole, out Dictionary<RoleTypeId, float> pairedData))
                        {
                            if (pairedData.ContainsKey(victim.Role))
                            {
                                ffMultiplier = pairedData[victim.Role];
                                return true;
                            }
                        }
                    }
                }

                // If we're SCP then we need to check if we can attack other SCP, or D-Class, etc. This is default FF logic without unique roles.
                if (attacker.FriendlyFireMultiplier.Count > 0)
                {
                    if (attacker.FriendlyFireMultiplier.TryGetValue(victim.Role, out float ffMulti))
                    {
                        ffMultiplier = ffMulti;
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"CheckFriendlyFirePlayerRules failed to handle friendly fire because: {ex}");
            }

            return HitboxIdentity.CheckFriendlyFire(attackerFootprint.Role, victimHub.roleManager.CurrentRole.RoleTypeId);
        }
    }

    /// <summary>
    /// Patches <see cref="HitboxIdentity.CheckFriendlyFire(ReferenceHub, ReferenceHub, bool)"/>.
    /// </summary>
    [HarmonyPatch(typeof(HitboxIdentity), nameof(HitboxIdentity.CheckFriendlyFire), typeof(ReferenceHub), typeof(ReferenceHub), typeof(bool))]
    internal static class HitboxIdentityCheckFriendlyFire
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label jmp = generator.DefineLabel();

            // CheckFriendlyFirePlayer(this.PreviousOwner.Hub, referenceHub)
            newInstructions.InsertRange(
                0,
                new CodeInstruction[]
                {
                    // CheckFriendlyFirePlayerHitbox(attacker, victim);
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Call, Method(typeof(IndividualFriendlyFire), nameof(IndividualFriendlyFire.CheckFriendlyFirePlayerHitbox), new Type[] { typeof(ReferenceHub), typeof(ReferenceHub) })),

                    // goto base game logic if false
                    new(OpCodes.Brfalse_S, jmp),

                    // Return true
                    new(OpCodes.Ldc_I4_1),
                    new(OpCodes.Ret),

                    // jmp
                    new CodeInstruction(OpCodes.Nop).WithLabels(jmp),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }

    /// <summary>
    /// Patches <see cref="AttackerDamageHandler.ProcessDamage(ReferenceHub)"/> to allow or disallow friendly fire.
    /// </summary>
    [HarmonyPatch(typeof(AttackerDamageHandler), nameof(AttackerDamageHandler.ProcessDamage))]
    internal static class ProcessDamagePatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int offset = -1;
            int index = newInstructions.FindLastIndex(
                instruction => instruction.Calls(PropertyGetter(typeof(AttackerDamageHandler), nameof(AttackerDamageHandler.Attacker)))) + offset;

            LocalBuilder ffMulti = generator.DeclareLocal(typeof(float));

            Label uniqueFFMulti = generator.DefineLabel();
            Label normalProcessing = generator.DefineLabel();

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // Load Attacker (this.Attacker)
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(AttackerDamageHandler), nameof(AttackerDamageHandler.Attacker))),

                    // Load Target (ply)
                    new(OpCodes.Ldarg_1),

                    // Set default FF to 1.
                    new(OpCodes.Ldc_I4_1),

                    // ffMulti
                    new(OpCodes.Stloc, ffMulti.LocalIndex),
                    new(OpCodes.Ldloca, ffMulti.LocalIndex),

                    // Pass over Player hubs, and FF multiplier.
                    // CheckFriendlyFirePlayerRules(this.Attacker, ply, ffMulti)
                    new(OpCodes.Call, Method(typeof(IndividualFriendlyFire), nameof(IndividualFriendlyFire.CheckFriendlyFirePlayerRules), new[] { typeof(Footprint), typeof(ReferenceHub), typeof(float).MakeByRefType() })),

                    // If we have rules, we branch to custom logic, otherwise, default to NW logic.
                    new(OpCodes.Brtrue_S, uniqueFFMulti),
                });

            int ffMultiplierIndexOffset = 0;

            // int ffMultiplierIndex = newInstructions.FindLast(index, instruction => instruction.LoadsField(Field(typeof(AttackerDamageHandler), nameof(AttackerDamageHandler._ffMultiplier)))) + ffMultiplierIndexOffset;
            int ffMultiplierIndex = newInstructions.FindLastIndex(instruction => instruction.Calls(Method(typeof(StandardDamageHandler), nameof(StandardDamageHandler.ProcessDamage)))) + ffMultiplierIndexOffset;

            newInstructions[ffMultiplierIndex].WithLabels(normalProcessing);

            // int ffMultiplierIndex = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ret) + ffMultiplierIndexOffset;
            newInstructions.InsertRange(
                ffMultiplierIndex,
                new CodeInstruction[]
                {
                    // Do not run our custom logic, skip over.
                    new(OpCodes.Br, normalProcessing),

                    // AttackerDamageHandler.Damage = AttackerDamageHandler.Damage * ffMulti
                    new CodeInstruction(OpCodes.Ldarg_0).WithLabels(uniqueFFMulti),
                    new(OpCodes.Ldloc, ffMulti.LocalIndex),
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(AttackerDamageHandler), nameof(AttackerDamageHandler.Damage))),
                    new(OpCodes.Mul),
                    new(OpCodes.Callvirt, PropertySetter(typeof(AttackerDamageHandler), nameof(AttackerDamageHandler.Damage))),

                    // Game code, the two instructions before ProcessDamage
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldarg_1),

                    // Next line is ProcessDamage, which uses AttackerDamageHandler information.
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}