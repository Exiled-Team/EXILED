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
            return CheckFriendlyFirePlayer(attackerHub, victimHub, out _);
        }

        /// <summary>
        /// Checks if there can be damage between two players, according to the FF rules.
        /// </summary>
        /// <param name="attackerHub">The person attacking.</param>
        /// <param name="victimHub">The person being attacked.</param>
        /// <returns>True if the attacker can damage the victim.</returns>
        public static bool CheckFriendlyFirePlayer(ReferenceHub attackerHub, ReferenceHub victimHub)
        {
            return CheckFriendlyFirePlayer(attackerHub, victimHub, out _);
        }

        /// <summary>
        /// Checks if there can be damage between two players, according to the FF rules.
        /// </summary>
        /// <param name="attackerHub">The person attacking.</param>
        /// <param name="victimHub">The person being attacked.</param>
        /// <param name="ffMulti"> FF multiplier. </param>
        /// <returns>True if the attacker can damage the victim.</returns>
        public static bool CheckFriendlyFirePlayer(ReferenceHub attackerHub, ReferenceHub victimHub, out float ffMulti)
        {
            ffMulti = 1f;
            Log.Debug("Entered CheckFriendlyFirePlayer", Loader.Loader.ShouldDebugBeShown);

            if (Server.FriendlyFire)
                return true;

            if (attackerHub is null || victimHub is null)
            {
                Log.Debug("CheckFriendlyFirePlayer attacker or victim reference hub was null", Loader.Loader.ShouldDebugBeShown);
                return true;
            }

            try
            {
                Player attacker = Player.Get(attackerHub);
                Player victim = Player.Get(victimHub);
                if (attacker is null || victim is null)
                {
                    Log.Debug("CheckFriendlyFirePlayer attack or victim player object was null", Loader.Loader.ShouldDebugBeShown);
                    return true;
                }

                if (attacker == victim)
                {
                    Log.Debug("CheckFriendlyFirePlayer, attacker was victim", Loader.Loader.ShouldDebugBeShown);
                    return true;
                }

                if (!victim.UniqueRole.Equals(string.Empty))
                {
                    // If 035 is being shot, then we need to check if we are an 035, then check if the attacker is allowed to attack us
                    if (victim.CustomRoleFriendlyFireMultiplier.Count > 0)
                    {
                        if (victim.CustomRoleFriendlyFireMultiplier.TryGetValue(victim.UniqueRole, out Dictionary<RoleType, float> pairedData))
                        {
                            if (pairedData.ContainsKey(attacker.Role))
                            {
                                ffMulti = pairedData[attacker.Role];
                                Log.Debug($"CheckFriendlyFirePlayer, victum had unique role {ffMulti}", Loader.Loader.ShouldDebugBeShown);
                                return ffMulti > 0;
                            }
                        }
                    }
                }
                else if(!attacker.UniqueRole.Equals(string.Empty))
                {
                    // If 035 is attacking, whether to allow or disallow based on victim role.
                    if (attacker.CustomRoleFriendlyFireMultiplier.Count > 0)
                    {
                        if (attacker.CustomRoleFriendlyFireMultiplier.TryGetValue(attacker.UniqueRole, out Dictionary<RoleType, float> pairedData))
                        {
                            if (pairedData.ContainsKey(victim.Role))
                            {
                                ffMulti = pairedData[victim.Role];
                                Log.Debug($"CheckFriendlyFirePlayer, attack had unique role {ffMulti}", Loader.Loader.ShouldDebugBeShown);
                                return ffMulti > 0;
                            }
                        }
                    }
                }

                // If we're SCP then we need to check if we can attack other SCP, or D-Class, etc. This is default FF logic without unique roles.
                if (attacker.FriendlyFireMultiplier.Count > 0)
                {
                    if (attacker.FriendlyFireMultiplier.TryGetValue(victim.Role, out float ffMult))
                    {
                        ffMulti = ffMult;
                        Log.Debug($"CheckFriendlyFirePlayer, Friendlyfire for non-unique role {ffMulti}", Loader.Loader.ShouldDebugBeShown);
                        return ffMulti > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Debug($"CheckFriendlyFirePlayer failed to handle friendly fire because: {ex}", Loader.Loader.ShouldDebugBeShown);
            }

            Log.Debug($"CheckFriendlyFirePlayer will return false and run default NW logic", Loader.Loader.ShouldDebugBeShown);
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

                new(OpCodes.Ldc_I4_1),

                new(OpCodes.Stloc, ffMulti.LocalIndex),

                new(OpCodes.Ldloca, ffMulti.LocalIndex),

                // Pass both over.
                new(OpCodes.Call, Method(typeof(IndividualFriendlyFire), nameof(IndividualFriendlyFire.CheckFriendlyFirePlayer), new[] { typeof(ReferenceHub), typeof(ReferenceHub), typeof(float).MakeByRefType() })),

                new (OpCodes.Brtrue_S, uniqueFFMulti),
            });

            int ffMultiplierIndexOffset = 0;

            // int ffMultiplierIndex = newInstructions.FindLast(index, instruction => instruction.LoadsField(Field(typeof(AttackerDamageHandler), nameof(AttackerDamageHandler._ffMultiplier)))) + ffMultiplierIndexOffset;
            int ffMultiplierIndex = newInstructions.FindLastIndex(instruction => instruction.Calls(Method(typeof(StandardDamageHandler), nameof(StandardDamageHandler.ProcessDamage)))) + ffMultiplierIndexOffset;

            newInstructions[ffMultiplierIndex].WithLabels(normalProcessing);

            // int ffMultiplierIndex = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ret) + ffMultiplierIndexOffset;
            newInstructions.InsertRange(ffMultiplierIndex, new CodeInstruction[]
            {
                new (OpCodes.Br, normalProcessing),
                new CodeInstruction(OpCodes.Ldarg_0).WithLabels(uniqueFFMulti),
                new (OpCodes.Ldloc, ffMulti.LocalIndex),
                new (OpCodes.Ldarg_0),
                new (OpCodes.Callvirt, PropertyGetter(typeof(AttackerDamageHandler), nameof(AttackerDamageHandler.Damage))),
                new (OpCodes.Mul),
                new (OpCodes.Callvirt, PropertySetter(typeof(AttackerDamageHandler), nameof(AttackerDamageHandler.Damage))),
                new (OpCodes.Ldarg_0),
                new (OpCodes.Ldarg_1),
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

                new(OpCodes.Call, Method(typeof(IndividualFriendlyFire), nameof(IndividualFriendlyFire.CheckFriendlyFirePlayer), new[] { typeof(ReferenceHub), typeof(ReferenceHub) })),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
