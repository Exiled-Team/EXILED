// -----------------------------------------------------------------------
// <copyright file="FixJailbirdHitWindowsCrash.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
#pragma warning disable CS1591 // Commentaire XML manquant pour le type ou le membre visible publiquement
#pragma warning disable SA1600 // Elements should be documented

    using CustomPlayerEffects;
    using HarmonyLib;
    using InventorySystem.Items.Jailbird;
    using Mirror;
    using PlayerRoles.FirstPersonControl;
    using PlayerStatsSystem;
    using RelativePositioning;
    using UnityEngine;
    using Utils.Networking;
    using Utils.NonAllocLINQ;

    /// <summary>
    /// Patches <see cref="JailbirdHitreg.ServerAttack"/> delegate.
    /// Fix than NW don't send the a null referenceHub in client NetwordReader causing crash when player hit windows.
    /// Prefix taken from https://github.com/GraczBezNicku/Jailbird-Glass-Fix.
    /// </summary>
    [HarmonyPatch(typeof(JailbirdHitreg), nameof(JailbirdHitreg.ServerAttack))]
    public static class FixJailbirdHitWindowsCrash
    {
        public static bool Prefix(ref bool __result, JailbirdHitreg __instance, bool isCharging, NetworkReader reader)
        {
            ReferenceHub owner = __instance._item.Owner;
            bool result = false;
            if (reader != null)
            {
                RelativePosition relativePosition = reader.ReadRelativePosition();
                Quaternion claimedRot = reader.ReadQuaternion();
                JailbirdHitreg.BacktrackedPlayers.Add(new FpcBacktracker(owner, relativePosition.Position, claimedRot, 0.1f, 0.15f));
                byte b = reader.ReadByte();
                for (int i = 0; i < b; i++)
                {
                    try
                    {
                        bool flag = reader.TryReadReferenceHub(out ReferenceHub target);
                        RelativePosition relativePosition2 = reader.ReadRelativePosition();
                        if (flag)
                        {
                            JailbirdHitreg.BacktrackedPlayers.Add(new FpcBacktracker(target, relativePosition2.Position, 0.4f));
                        }
                    }
                    catch
                    {
                        // Ignore.
                    }
                }
            }

            __instance.DetectDestructibles();
            Vector3 forward = __instance._item.Owner.PlayerCameraReference.forward;
            float num = isCharging ? __instance._damageCharge : __instance._damageMelee;
            for (int j = 0; j < JailbirdHitreg._detectionsLen; j++)
            {
                IDestructible destructible = JailbirdHitreg.DetectedDestructibles[j];
                if (destructible.Damage(num, new JailbirdDamageHandler(owner, num, forward), destructible.CenterOfMass))
                {
                    result = true;
                    if (!isCharging)
                    {
                        __instance.TotalMeleeDamageDealt += num;
                    }
                    else
                    {
                        if (destructible is HitboxIdentity hitboxIdentity)
                        {
                            hitboxIdentity.TargetHub.playerEffectsController.EnableEffect<Flashed>(__instance._flashDuration, true);
                        }
                    }
                }
            }

            JailbirdHitreg.BacktrackedPlayers.ForEach(x =>
            {
                x.RestorePosition();
            });
            JailbirdHitreg.BacktrackedPlayers.Clear();
            __result = result;

            return false;
        }
    }
}