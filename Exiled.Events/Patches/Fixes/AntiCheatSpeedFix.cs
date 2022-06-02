// -----------------------------------------------------------------------
// <copyright file="AntiCheatSpeedFix.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

    using System;
    using System.Collections.Generic;

    using CustomPlayerEffects;

    using Exiled.API.Features;

    using HarmonyLib;

    using InventorySystem.Items;
    using InventorySystem.Items.Armor;

    using PlayableScps;

    using UnityEngine;

    /// <summary>
    /// Fixes Cassie ignoring unit name if it's changed via <see cref="Map.ChangeUnitColor(int, string)"/>.
    /// </summary>
    [HarmonyPatch(typeof(FirstPersonController), nameof(FirstPersonController.GetSpeed))]
    public static class AntiCheatSpeedFix
    {
        private static bool Prefix(FirstPersonController __instance, out float speed, bool isServerSide)
        {
            try
            {
                __instance.curRole = __instance._hub.characterClassManager.Classes.SafeGet(__instance._hub.characterClassManager.CurClass);
                bool flag = __instance._hub.characterClassManager.IsAnyScp();
                if (isServerSide)
                {
                    if (!__instance.isLocalPlayer)
                        __instance.IsSneaking = __instance._hub.animationController.MoveState == PlayerMovementState.Sneaking && !__instance._hub.characterClassManager.IsAnyScp();

                    speed = __instance.staminaController.AllowMaxSpeed ? __instance.curRole.runSpeed : __instance.curRole.walkSpeed;
                    if (!flag)
                    {
                        // Only need to add this two lines
                        var player = Player.Get(__instance._hub);
                        speed *= __instance.staminaController.AllowMaxSpeed ? player.RunningSpeed : player.WalkSpeed;
                    }
                }
                else
                {
                    speed = 0f;
                }

                if (flag)
                {
                    if (__instance._hub.scpsController.CurrentScp != null)
                    {
                        IMovementVariation currentScp = __instance._hub.scpsController.CurrentScp;
                        speed = currentScp.MaxSpeed;
                    }
                    else
                    {
                        RoleType curClass = __instance._hub.characterClassManager.CurClass;
                        if (curClass == RoleType.Scp106 && __instance.Slowdown106)
                            speed = 1f;
                    }
                }

                if (!isServerSide)
                    __instance.MovementLock = CursorManager.IsMovementLocked();

                if (__instance.MovementLock > MovementLockType.Unlocked && !isServerSide)
                {
                    speed = 0f;
                }
                else
                {
                    if (__instance.IsSneaking)
                        speed *= 0.4f;

                    foreach (KeyValuePair<Type, PlayerEffect> keyValuePair in __instance._hub.playerEffectsController.AllEffects)
                    {
                        if (keyValuePair.Value.IsEnabled && keyValuePair.Value is IMovementSpeedEffect movementSpeedEffect)
                        {
                            speed = movementSpeedEffect.GetMovementSpeed(speed);
                            if (movementSpeedEffect.DisableSprint)
                            {
                                __instance.IsSprinting = false;
                            }
                        }
                    }

                    if (__instance._hub.inventory.TryGetBodyArmor(out BodyArmor bodyArmor))
                    {
                        BodyArmorUtils.GetMovementProperties(__instance._hub.characterClassManager.CurRole.team, bodyArmor, out float num, out float num2);
                        speed *= num;
                    }

                    if (__instance._hub.inventory.CurInstance is IMobilityModifyingItem mobilityModifyingItem)
                    {
                        speed *= mobilityModifyingItem.MovementSpeedMultiplier;
                        if (speed > mobilityModifyingItem.MovementSpeedLimiter && mobilityModifyingItem.MovementSpeedLimiter >= 0f)
                            speed = mobilityModifyingItem.MovementSpeedLimiter;
                    }
                }

                if (isServerSide)
                {
                    return false;
                }

                speed *= __instance.input.sqrMagnitude;
                __instance.input = Vector2.Lerp(__instance.input, new Vector2(__instance.horizontal, __instance.vertical), 0.6f);
                if (__instance.input.sqrMagnitude > 1f)
                {
                    __instance.input.Normalize();
                }

                return false;
            }
            catch(Exception ex)
            {
                Log.Error("e" + ex);
                speed = 0;
                return true;
            }
        }
    }
}
