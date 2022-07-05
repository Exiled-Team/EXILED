// -----------------------------------------------------------------------
// <copyright file="IndividualFriendlyFire.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
#pragma warning disable SA1402
#pragma warning disable SA1313
    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;

    using Footprinting;

    using HarmonyLib;

    using InventorySystem.Items.ThrowableProjectiles;

    using NorthwoodLib.Pools;

    using PlayerStatsSystem;

    using UnityEngine;

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
        public static bool CheckFriendlyFirePlayer(ReferenceHub attackerHub, ReferenceHub victimHub)
        {
            return CheckFriendlyFirePlayerRules(attackerHub, victimHub, out _);
        }

        /// <summary>
        /// Checks if there can be damage between two players, according to the FF rules.
        /// </summary>
        /// <param name="attackerHub">The person attacking.</param>
        /// <param name="victimHub">The person being attacked.</param>
        /// <returns>True if the attacker can damage the victim.</returns>
        public static bool CheckFriendlyFirePlayerHitbox(ReferenceHub attackerHub, ReferenceHub victimHub)
        {
            return Server.FriendlyFire || CheckFriendlyFirePlayerRules(attackerHub, victimHub, out _);
        }

        /// <summary>
        /// Checks if there can be damage between two players, according to the FF rules.
        /// </summary>
        /// <param name="attackerHub">The person attacking.</param>
        /// <param name="victimHub">The person being attacked.</param>
        /// <param name="ffMultiplier"> FF multiplier. </param>
        /// <returns> True if the attacker can damage the victim.</returns>
        /// <remarks> Friendly fire multiplier is also provided back if needed. </remarks>
        public static bool CheckFriendlyFirePlayerRules(ReferenceHub attackerHub, ReferenceHub victimHub, out float ffMultiplier)
        {
            ffMultiplier = 1f;

            // Return false, no custom friendly fire allowed, default to NW logic for FF. No point in processing if FF is enabled across the board.
            if (Server.FriendlyFire)
                return false;

            if (attackerHub is null || victimHub is null)
            {
                Log.Debug($"CheckFriendlyFirePlayerRules, Attacker hub null: {attackerHub is null}, Victim hub null: {victimHub is null}", Loader.Loader.ShouldDebugBeShown);
                return true;
            }

            try
            {
                Player attacker = Player.Get(attackerHub);
                Player victim = Player.Get(victimHub);
                if (attacker is null || victim is null)
                {
                    Log.Debug($"CheckFriendlyFirePlayerRules, Attacker null: {attacker is null}, Victim null: {victim is null}", Loader.Loader.ShouldDebugBeShown);
                    return true;
                }

                if (attacker == victim)
                {
                    Log.Debug("CheckFriendlyFirePlayerRules, Attacker player was equal to Victim, likely suicide", Loader.Loader.ShouldDebugBeShown);
                    return true;
                }

                Log.Debug($"CheckFriendlyFirePlayerRules, Attacker role {attacker.Role}, \"{attacker.UniqueRole}\" and Victim {victim.Role}, \"{victim.UniqueRole}\"", Loader.Loader.ShouldDebugBeShown);

                if(attacker.AlwaysDealsFriendlyFireDamage)
                {
                    ffMultiplier = attacker.UniversalFriendlyFireMultiplier;
                    return true;
                }

                if (!string.IsNullOrEmpty(victim.UniqueRole))
                {
                    // If 035 is being shot, then we need to check if we are an 035, then check if the attacker is allowed to attack us
                    if (victim.CustomRoleFriendlyFireMultiplier.Count > 0)
                    {
                        if (victim.CustomRoleFriendlyFireMultiplier.TryGetValue(victim.UniqueRole, out Dictionary<RoleType, float> pairedData))
                        {
                            if (pairedData.ContainsKey(attacker.Role))
                            {
                                ffMultiplier = pairedData[attacker.Role];
                                return true;
                            }
                        }
                    }

                    if (victim.CustomRoleToCustomRoleFriendlyFireMultiplier.Count > 0)
                    {
                        if (victim.CustomRoleToCustomRoleFriendlyFireMultiplier.TryGetValue(victim.UniqueRole, out Dictionary<string, float> pairedData))
                        {
                            if (pairedData.ContainsKey(attacker.UniqueRole))
                            {
                                ffMultiplier = pairedData[attacker.UniqueRole];
                                return true;
                            }
                        }
                    }
                }
                else if(!string.IsNullOrEmpty(attacker.UniqueRole))
                {
                    // If 035 is attacking, whether to allow or disallow based on victim role.
                    if (attacker.CustomRoleFriendlyFireMultiplier.Count > 0)
                    {
                        if (attacker.CustomRoleFriendlyFireMultiplier.TryGetValue(attacker.UniqueRole, out Dictionary<RoleType, float> pairedData))
                        {
                            if (pairedData.ContainsKey(victim.Role))
                            {
                                ffMultiplier = pairedData[victim.Role];
                                return true;
                            }
                        }
                    }

                    if (attacker.CustomRoleToCustomRoleFriendlyFireMultiplier.Count > 0)
                    {
                        if (attacker.CustomRoleToCustomRoleFriendlyFireMultiplier.TryGetValue(attacker.UniqueRole, out Dictionary<string, float> pairedData))
                        {
                            if (pairedData.ContainsKey(victim.UniqueRole))
                            {
                                ffMultiplier = pairedData[victim.UniqueRole];
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
                Log.Debug($"CheckFriendlyFirePlayerRules failed to handle friendly fire because: {ex}", Loader.Loader.ShouldDebugBeShown);
            }

            return false;
        }
    }

    /// <summary>
    /// Patches <see cref="HitboxIdentity.CheckFriendlyFire(ReferenceHub, ReferenceHub, bool)"/>.
    /// </summary>
    [HarmonyPatch(typeof(HitboxIdentity), nameof(HitboxIdentity.CheckFriendlyFire), typeof(ReferenceHub), typeof(ReferenceHub), typeof(bool))]
    internal static class HitboxIdentityCheckFriendlyFire
    {
        private static bool Prefix(ReferenceHub attacker, ReferenceHub victim, ref bool __result)
        {
            try
            {
                bool currentResult = IndividualFriendlyFire.CheckFriendlyFirePlayerHitbox(attacker, victim);
                if(!currentResult)
                {
                    return true;
                }

                __result = currentResult;
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"{e}");
                return true;
            }
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
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            int offset = -1;
            int index = newInstructions.FindLastIndex(instruction => instruction.Calls(PropertyGetter(typeof(AttackerDamageHandler), nameof(AttackerDamageHandler.Attacker)))) + offset;
            LocalBuilder ffMulti = generator.DeclareLocal(typeof(float));
            Label uniqueFFMulti = generator.DefineLabel();
            Label normalProcessing = generator.DefineLabel();

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),

                new(OpCodes.Callvirt, PropertyGetter(typeof(AttackerDamageHandler), nameof(AttackerDamageHandler.Attacker))),

                // Load Attacker
                new(OpCodes.Ldfld, Field(typeof(Footprint), nameof(Footprint.Hub))),

                // Load Target
                new(OpCodes.Ldarg_1),

                // Set default FF to 1.
                new(OpCodes.Ldc_I4_1),

                new(OpCodes.Stloc, ffMulti.LocalIndex),

                new(OpCodes.Ldloca, ffMulti.LocalIndex),

                // Pass over Player hubs, and FF multiplier.
                new(OpCodes.Call, Method(typeof(IndividualFriendlyFire), nameof(IndividualFriendlyFire.CheckFriendlyFirePlayerRules), new[] { typeof(ReferenceHub), typeof(ReferenceHub), typeof(float).MakeByRefType() })),

                // If we have rules, we branch to custom logic, otherwise, default to NW logic.
                new (OpCodes.Brtrue_S, uniqueFFMulti),
            });

            int ffMultiplierIndexOffset = 0;

            // int ffMultiplierIndex = newInstructions.FindLast(index, instruction => instruction.LoadsField(Field(typeof(AttackerDamageHandler), nameof(AttackerDamageHandler._ffMultiplier)))) + ffMultiplierIndexOffset;
            int ffMultiplierIndex = newInstructions.FindLastIndex(instruction => instruction.Calls(Method(typeof(StandardDamageHandler), nameof(StandardDamageHandler.ProcessDamage)))) + ffMultiplierIndexOffset;

            newInstructions[ffMultiplierIndex].WithLabels(normalProcessing);

            // int ffMultiplierIndex = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ret) + ffMultiplierIndexOffset;
            newInstructions.InsertRange(ffMultiplierIndex, new CodeInstruction[]
            {
                // Do not run our custom logic, skip over.
                new (OpCodes.Br, normalProcessing),

                // AttackerDamageHandler.Damage = AttackerDamageHandler.Damage * ffMulti
                new CodeInstruction(OpCodes.Ldarg_0).WithLabels(uniqueFFMulti),
                new (OpCodes.Ldloc, ffMulti.LocalIndex),
                new (OpCodes.Ldarg_0),
                new (OpCodes.Callvirt, PropertyGetter(typeof(AttackerDamageHandler), nameof(AttackerDamageHandler.Damage))),
                new (OpCodes.Mul),
                new (OpCodes.Callvirt, PropertySetter(typeof(AttackerDamageHandler), nameof(AttackerDamageHandler.Damage))),
                new (OpCodes.Ldarg_0),
                new (OpCodes.Ldarg_1),

                // Next line is ProcessDamage, which uses AttackerDamageHandler information.
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }

    /// <summary>
    /// Patches <see cref="FlashbangGrenade.PlayExplosionEffects()"/>.
    /// </summary>
    [HarmonyPatch(typeof(FlashbangGrenade), nameof(FlashbangGrenade.PlayExplosionEffects))]
    internal static class FlashbangGrenadePlayExplosionEffectsPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            const int offset = 1;
            const int instructionsToRemove = 9;
            int index = newInstructions.FindLastIndex(code => code.opcode == OpCodes.Brtrue_S) + offset;

            newInstructions.RemoveRange(index, instructionsToRemove);

            // HitboxIdentity.CheckFriendlyFire(ReferenceHub, ReferenceHub, false)
            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // this.PreviousOwner.Hub
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldflda, Field(typeof(FlashbangGrenade), nameof(FlashbangGrenade.PreviousOwner))),
                new(OpCodes.Ldfld, Field(typeof(Footprint), nameof(Footprint.Hub))),

                // KeyValuePair<GameObject, ReferenceHub>.Value (target ReferenceHub)
                new(OpCodes.Ldloca_S, 2),
                new(OpCodes.Call, PropertyGetter(typeof(KeyValuePair<GameObject, ReferenceHub>), nameof(KeyValuePair<GameObject, ReferenceHub>.Value))),

                new(OpCodes.Call, Method(typeof(IndividualFriendlyFire), nameof(IndividualFriendlyFire.CheckFriendlyFirePlayer))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
