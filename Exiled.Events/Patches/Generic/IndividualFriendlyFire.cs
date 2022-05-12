// -----------------------------------------------------------------------
// <copyright file="IndividualFriendlyFire.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
#pragma warning disable SA1118
#pragma warning disable SA1402
#pragma warning disable SA1649
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
        /// <param name="attackerRole">The attackers current role.</param>
        /// <returns>True if the attacker can damage the victim.</returns>
        public static bool CheckFriendlyFirePlayerFriendly(ReferenceHub attackerHub, ReferenceHub victimHub, RoleType attackerRole)
        {
            return CheckFriendlyFirePlayer(attackerHub, victimHub);
        }

        /// <summary>
        /// Checks if there can be damage between two players, according to the FF rules.
        /// </summary>
        /// <param name="attackerHub">The person attacking.</param>
        /// <param name="victimHub">The person being attacked.</param>
        /// <returns>True if the attacker can damage the victim.</returns>
        public static bool CheckFriendlyFirePlayer(ReferenceHub attackerHub, ReferenceHub victimHub)
        {
            if (Server.FriendlyFire)
                return true;

            if (attackerHub is null || victimHub is null)
            {
                return true;
            }

            try
            {
                Player attacker = Player.Get(attackerHub);
                Player victim = Player.Get(victimHub);
                if (attacker is null || victim is null)
                {
                    return true;
                }

                if (attacker == victim){
                    return true;
                }

                if (!victim.UniqueRole.IsEmpty())
                {
                    // If 035 is being shot, then we need to check if we are an 035, then check if the attacker is allowed to attack us
                    if (victim.UniqueFriendlyFireRules.Count > 0)
                    {
                        if (victim.UniqueFriendlyFireRules.TryGetValue(victim.UniqueRole, out Dictionary<RoleType, int> pairedData))
                        {
                            if (pairedData.ContainsKey(attacker.Role))
                            {
                                return true;
                            }
                        }
                    }
                }
                else if(!attacker.UniqueRole.IsEmpty())
                {
                    // If 035 is attacking, whether to allow or disallow.
                    if (attacker.UniqueFriendlyFireRules.Count > 0)
                    {
                        if (attacker.UniqueFriendlyFireRules.TryGetValue(attacker.UniqueRole, out Dictionary<RoleType, int> pairedData))
                        {
                            if (pairedData.ContainsKey(victim.Role))
                            {
                                return true;
                            }
                        }
                    }
                }

                // If we're SCP then we need to check if we can attack other SCP, or D-Class, etc. This is default FF logic without unique roles. 
                if (attacker.FriendlyFireRules.Count > 0)
                {
                    if (attacker.FriendlyFireRules.TryGetValue(victim.Role, out int ffMult))
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Info($"CheckFriendlyFirePlayer failed to handle friendly fire because: {ex}");
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
        private static bool Prefix(ReferenceHub attacker, ReferenceHub victim, bool ignoreConfig, ref bool __result)
        {
            try
            {
                __result = IndividualFriendlyFire.CheckFriendlyFirePlayer(attacker,  victim);

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
    /// Patches <see cref="AttackerDamageHandler.ProcessDamage(ReferenceHub)"/>.
    /// </summary>

    [HarmonyPatch(typeof(AttackerDamageHandler), nameof(AttackerDamageHandler.ProcessDamage))]
    internal static class ProcessDamagePatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            int offset = 1;
            int index = newInstructions.FindLastIndex(instruction => instruction.Calls(PropertyGetter(typeof(AttackerDamageHandler), nameof(AttackerDamageHandler.Attacker)))) + offset;

            newInstructions.RemoveRange(index, 4);

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // Load Attacker
                new(OpCodes.Ldfld, Field(typeof(Footprint), nameof(Footprint.Hub))),

                // Load Target
                new(OpCodes.Ldarg_1),

                // Pass both over.
                new(OpCodes.Call, Method(typeof(IndividualFriendlyFire), nameof(IndividualFriendlyFire.CheckFriendlyFirePlayer), new[] { typeof(ReferenceHub), typeof(ReferenceHub) })),
            });
            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }

    /// <summary>
    /// Patches <see cref="ExplosionGrenade.ExplodeDestructible(IDestructible, Footprint, Vector3, ExplosionGrenade)"/>.
    /// </summary>
    // TODO: Re-do this
    // [HarmonyPatch(typeof(ExplosionGrenade), nameof(ExplosionGrenade.ExplodeDestructible))]
    internal static class ExplosionGrenadeExplodeDestructiblePatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            const int targetIsOwnerIndex = 5;
            const int offset = 6;
            const int instructionsToRemove = 8;

            int index = newInstructions.FindIndex(code => code.opcode == OpCodes.Stloc_S &&
                ((LocalBuilder)code.operand).LocalIndex == targetIsOwnerIndex) + offset;

            newInstructions.RemoveRange(index, instructionsToRemove);

            // HitboxIdentity.CheckFriendlyFire(ReferenceHub, ReferenceHub, false)
            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // this.PreviousOwner.Hub
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldflda, Field(typeof(ExplosionGrenade), nameof(ExplosionGrenade.PreviousOwner))),
                new(OpCodes.Ldfld, Field(typeof(Footprint), nameof(Footprint.Hub))),

                // targetReferenceHub
                new(OpCodes.Ldloc_3),

                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldflda, Field(typeof(ExplosionGrenade), nameof(ExplosionGrenade.PreviousOwner))),
                new(OpCodes.Ldfld, Field(typeof(Footprint), nameof(Footprint.Role))),
                new(OpCodes.Call, Method(typeof(IndividualFriendlyFire), nameof(IndividualFriendlyFire.CheckFriendlyFirePlayerFriendly))),
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

    /// <summary>
    /// Patches <see cref="Scp018Projectile.DetectPlayers()"/>.
    /// </summary>
    // TODO: Re-do this
    // [HarmonyPatch(typeof(Scp018Projectile), nameof(Scp018Projectile.DetectPlayers))]
    internal static class Scp018ProjectileDetectPlayersPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            const int referenceHubIndex = 1;
            const int offset = 5;
            const int instructionsToRemove = 7;

            int index = newInstructions.FindLastIndex(code => code.opcode == OpCodes.Ldloca_S &&
                ((LocalBuilder)code.operand).LocalIndex == referenceHubIndex) + offset;

            newInstructions.RemoveRange(index, instructionsToRemove);

            // HitboxIdentity.CheckFriendlyFire(ReferenceHub, ReferenceHub, false)
            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // Scp018Projectile.PreviousOwner.Hub
                new(OpCodes.Ldflda, Field(typeof(Scp018Projectile), nameof(Scp018Projectile.PreviousOwner))),
                new(OpCodes.Ldfld, Field(typeof(Footprint), nameof(Footprint.Hub))),

                // targetReferenceHub
                new(OpCodes.Ldloc_1),

                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldflda, Field(typeof(Scp018Projectile), nameof(Scp018Projectile.PreviousOwner))),
                new(OpCodes.Ldfld, Field(typeof(Footprint), nameof(Footprint.Role))),
                new(OpCodes.Call, Method(typeof(IndividualFriendlyFire), nameof(IndividualFriendlyFire.CheckFriendlyFirePlayerFriendly))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
