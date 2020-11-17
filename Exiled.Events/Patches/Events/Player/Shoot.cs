// -----------------------------------------------------------------------
// <copyright file="Shoot.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1313
    using System;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;
    using Exiled.Loader;

    using HarmonyLib;

    using UnityEngine;

    /// <summary>
    /// Patches <see cref="WeaponManager.CallCmdShoot(GameObject, HitBoxType, Vector3, Vector3, Vector3)"/>.
    /// Adds the <see cref="Handlers.Player.Shooting"/> and <see cref="Handlers.Player.Shot"/> events.
    /// </summary>
    [HarmonyPatch(typeof(WeaponManager), nameof(WeaponManager.CallCmdShoot))]
    internal static class Shoot
    {
        private static bool Prefix(WeaponManager __instance, GameObject target, HitBoxType hitboxType, Vector3 dir, Vector3 sourcePos, Vector3 targetPos)
        {
            try
            {
                if (!__instance._iawRateLimit.CanExecute(true))
                    return false;

                int itemIndex = __instance._hub.inventory.GetItemIndex();
                if (itemIndex < 0
                    || itemIndex >= __instance._hub.inventory.items.Count
                    || __instance.curWeapon < 0
                    || ((__instance._reloadCooldown > 0f
                            || __instance._fireCooldown > 0f)
                        && !__instance.isLocalPlayer)
                    || __instance._hub.inventory.curItem != __instance.weapons[__instance.curWeapon].inventoryID
                    || __instance._hub.inventory.items[itemIndex].durability <= 0.0)
                {
                    return false;
                }

                if (Vector3.Distance(__instance._hub.playerMovementSync.RealModelPosition, sourcePos) > 5.5f)
                {
                    __instance.GetComponent<CharacterClassManager>().TargetConsolePrint(__instance.connectionToClient, "Shot rejected - Code W.6 (difference between real source position and provided source position is too big)", "gray");
                    return false;
                }

                if (sourcePos.y - __instance._hub.playerMovementSync.LastSafePosition.y > 1.78f)
                {
                    __instance.GetComponent<CharacterClassManager>().TargetConsolePrint(__instance.connectionToClient, "Shot rejected - Code W.7 (Y axis difference between last safe position and provided source position is too big)", "gray");
                    return false;
                }

                if (Math.Abs(sourcePos.y - __instance._hub.playerMovementSync.RealModelPosition.y) > 2.7f)
                {
                    __instance.GetComponent<CharacterClassManager>().TargetConsolePrint(__instance.connectionToClient, "Shot rejected - Code W.8 (|Y| axis difference between real position and provided source position is too big)", "gray");
                    return false;
                }

                // >Exiled
                Log.Debug("Invoking shooting event", Loader.ShouldDebugBeShown);

                var shootingEventArgs = new ShootingEventArgs(Player.Get(__instance.gameObject), target, targetPos);

                Handlers.Player.OnShooting(shootingEventArgs);

                if (!shootingEventArgs.IsAllowed)
                    return false;

                targetPos = shootingEventArgs.Position;

                // <Exiled
                __instance._hub.inventory.items.ModifyDuration(itemIndex, __instance._hub.inventory.items[itemIndex].durability - 1f);
                __instance.scp268.ServerDisable();
                __instance._fireCooldown = 1f / (__instance.weapons[__instance.curWeapon].shotsPerSecond * __instance.weapons[__instance.curWeapon].allEffects.firerateMultiplier) * 0.9f;

                float sourceRangeScale = __instance.weapons[__instance.curWeapon].allEffects.audioSourceRangeScale;
                sourceRangeScale = sourceRangeScale * sourceRangeScale * 70f;
                __instance.GetComponent<Scp939_VisionController>().MakeNoise(Mathf.Clamp(sourceRangeScale, 5f, 100f));

                bool flag = target != null;
                if (targetPos == Vector3.zero)
                {
                    if (Physics.Raycast(sourcePos, dir, out RaycastHit raycastHit, 500f, __instance.raycastMask))
                    {
                        HitboxIdentity component = raycastHit.collider.GetComponent<HitboxIdentity>();
                        if (component != null)
                        {
                            WeaponManager componentInParent = component.GetComponentInParent<WeaponManager>();
                            if (componentInParent != null)
                            {
                                flag = false;
                                target = componentInParent.gameObject;
                                hitboxType = component.id;
                                targetPos = componentInParent.transform.position;
                            }
                        }
                    }
                }
                else if (Physics.Linecast(sourcePos, targetPos, out RaycastHit raycastHit, __instance.raycastMask))
                {
                    HitboxIdentity component = raycastHit.collider.GetComponent<HitboxIdentity>();
                    if (component != null)
                    {
                        WeaponManager componentInParent = component.GetComponentInParent<WeaponManager>();
                        if (componentInParent != null)
                        {
                            if (componentInParent.gameObject == target)
                            {
                                flag = false;
                            }
                            else if (componentInParent.scp268.Enabled)
                            {
                                flag = false;
                                target = componentInParent.gameObject;
                                hitboxType = component.id;
                                targetPos = componentInParent.transform.position;
                            }
                        }
                    }
                }

                ReferenceHub referenceHub = null;
                if (target != null)
                {
                    referenceHub = ReferenceHub.GetHub(target);
                }

                if (referenceHub != null && __instance.GetShootPermission(referenceHub.characterClassManager, false))
                {
                    if (Math.Abs(__instance._hub.playerMovementSync.RealModelPosition.y - referenceHub.playerMovementSync.RealModelPosition.y) > 35f)
                    {
                        __instance.GetComponent<CharacterClassManager>().TargetConsolePrint(__instance.connectionToClient, "Shot rejected - Code W.1 (too big Y-axis difference between source and target)", "gray");
                        return false;
                    }

                    if (Vector3.Distance(referenceHub.playerMovementSync.RealModelPosition, targetPos) > 5f)
                    {
                        __instance.GetComponent<CharacterClassManager>().TargetConsolePrint(__instance.connectionToClient, "Shot rejected - Code W.2 (difference between real target position and provided target position is too big)", "gray");
                        return false;
                    }

                    if (Physics.Linecast(__instance._hub.playerMovementSync.RealModelPosition, sourcePos, __instance.raycastServerMask))
                    {
                        __instance.GetComponent<CharacterClassManager>().TargetConsolePrint(__instance.connectionToClient, "Shot rejected - Code W.3 (collision between source positions detected)", "gray");
                        return false;
                    }

                    if (flag && Physics.Linecast(sourcePos, targetPos, __instance.raycastServerMask))
                    {
                        __instance.GetComponent<CharacterClassManager>().TargetConsolePrint(__instance.connectionToClient, "Shot rejected - Code W.4 (collision on shot line detected)", "gray");
                        return false;
                    }

                    if (referenceHub.gameObject == __instance.gameObject)
                    {
                        __instance.GetComponent<CharacterClassManager>().TargetConsolePrint(__instance.connectionToClient, "Shot rejected - Code W.5 (target is itself)", "gray");
                        return false;
                    }

                    Vector3 positionOffset = referenceHub.playerMovementSync.RealModelPosition - __instance._hub.playerMovementSync.RealModelPosition;

                    if (Vector3.Angle(positionOffset, __instance.transform.forward) > 45 && Math.Abs(positionOffset.y) > 10f && positionOffset.sqrMagnitude < 0.25f)
                    {
                        __instance.GetComponent<CharacterClassManager>().TargetConsolePrint(__instance.connectionToClient, "Shot rejected - Code W.12 (too big angle)", "gray");
                        return false;
                    }

                    Vector2 rotationOffset = __instance._lastRotation - __instance._hub.playerMovementSync.Rotations;
                    if (rotationOffset.sqrMagnitude < 0.001f)
                    {
                        __instance._lastRotation = __instance._hub.playerMovementSync.Rotations;

                        __instance.GetComponent<CharacterClassManager>().TargetConsolePrint(__instance.connectionToClient, "Shot rejected - Code W.9 (no recoil)", "gray");
                        return false;
                    }

                    __instance._lastRotation = __instance._hub.playerMovementSync.Rotations;

                    float num2 = Vector3.Distance(__instance.camera.transform.position, target.transform.position);
                    float num3 = __instance.weapons[__instance.curWeapon].damageOverDistance.Evaluate(num2);

                    switch (referenceHub.characterClassManager.CurClass)
                    {
                        case RoleType.Scp106:
                            num3 /= 10f;
                            goto IL_6D1;

                        case RoleType.Scp049:
                        case RoleType.Scp079:
                        case RoleType.Scp096:
                        case RoleType.Scp173:
                        case RoleType.Scp93953:
                        case RoleType.Scp93989:
                            goto IL_6D1;

                        default:
                            switch (hitboxType)
                            {
                                case HitBoxType.HEAD:
                                    num3 *= 4;
                                    float num4 = 1 / (__instance.weapons[__instance.curWeapon].shotsPerSecond * __instance.weapons[__instance.curWeapon].allEffects.firerateMultiplier);
                                    __instance._headshotsL++;
                                    __instance._headshotsS++;
                                    __instance._headshotsResetS = num4 * 1.86f;
                                    __instance._headshotsResetL = num4 * 2.9f;

                                    if (__instance._headshotsS >= 3)
                                    {
                                        __instance._hub.playerMovementSync.AntiCheatKillPlayer("Headshots limit exceeded in time window A\n(debug code: W.10)", "W.10");
                                        return false;
                                    }

                                    if (__instance._headshotsL >= 4)
                                    {
                                        __instance._hub.playerMovementSync.AntiCheatKillPlayer("Headshots limit exceeded in time window B\n(debug code: W.11)", "W.11");
                                        return false;
                                    }

                                    break;

                                case HitBoxType.ARM:
                                case HitBoxType.LEG:
                                    num3 /= 2;
                                    break;
                            }

                            break;
                    }

                IL_6D1:
                    num3 *= __instance.weapons[__instance.curWeapon].allEffects.damageMultiplier;
                    num3 *= __instance.overallDamagerFactor;

                    // >Exiled
                    Log.Debug("Invoking late shoot.", Loader.ShouldDebugBeShown);

                    var shotEventArgs = new ShotEventArgs(Player.Get(__instance.gameObject), target, hitboxType, num2, num3);

                    Handlers.Player.OnShot(shotEventArgs);

                    if (!shotEventArgs.CanHurt)
                        return false;

                    // <Exiled
                    __instance._hub.playerStats.HurtPlayer(new PlayerStats.HitInfo(shotEventArgs.Damage, __instance._hub.LoggedNameFromRefHub(), DamageTypes.FromWeaponId(__instance.curWeapon), __instance._hub.queryProcessor.PlayerId), referenceHub.gameObject, false);
                    __instance.RpcConfirmShot(true, __instance.curWeapon);
                    __instance.PlaceDecal(true, new Ray(__instance.camera.position, dir), (int)referenceHub.characterClassManager.CurClass, num2);
                    return false;
                }
                else
                {
                    if (target != null && hitboxType == HitBoxType.WINDOW && target.GetComponent<BreakableWindow>() != null)
                    {
                        float time = Vector3.Distance(__instance.camera.transform.position, target.transform.position);
                        float damage = __instance.weapons[__instance.curWeapon].damageOverDistance.Evaluate(time);
                        target.GetComponent<BreakableWindow>().ServerDamageWindow(damage);
                        __instance.RpcConfirmShot(true, __instance.curWeapon);
                        return false;
                    }

                    __instance.PlaceDecal(false, new Ray(__instance.camera.position, dir), __instance.curWeapon, 0f);
                    __instance.RpcConfirmShot(false, __instance.curWeapon);
                    return false;
                }
            }
            catch (Exception e)
            {
                Exiled.API.Features.Log.Error($"{typeof(Shoot).FullName}.{nameof(Prefix)}:\n{e}");

                return true;
            }
        }
    }
}
