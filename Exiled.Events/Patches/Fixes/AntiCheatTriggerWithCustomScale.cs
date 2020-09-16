// -----------------------------------------------------------------------
// <copyright file="AntiCheatTriggerWithCustomScale.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    using System;

    using Exiled.API.Features;

    using HarmonyLib;

    using UnityEngine;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

    /// <summary>
    /// Patches <see cref="PlayerMovementSync.AnticheatIsIntersecting(Vector3)"/>.
    /// Fixes triggering due to the R.3 code by using the Y coordinate from the player's scale to multiply the offset.
    /// </summary>
    [HarmonyPatch(typeof(PlayerMovementSync), nameof(PlayerMovementSync.AnticheatIsIntersecting), new[] { typeof(Vector3) })]
    internal static class AntiCheatTriggerWithCustomScale
    {
        private static bool Prefix(PlayerMovementSync __instance, Vector3 pos, bool __result)
        {
            try
            {
                var point0 = pos - (PlayerMovementSync._yAxisOffset * __instance.transform.localScale.y);
                var point1 = pos + (PlayerMovementSync._yAxisOffset * __instance.transform.localScale.y);

                int num = Physics.OverlapCapsuleNonAlloc(point0, point1, 0.38f, PlayerMovementSync._sphereHits, PlayerMovementSync._r3CollidableSurfaces);
                for (int i = 0; i < num; i++)
                {
                    PlayableScps.Scp096 scp;
                    if ((__instance._hub.characterClassManager.CurClass == RoleType.Scp106
                        && (PlayerMovementSync._sphereHits[i].gameObject.layer == 27
                            || PlayerMovementSync._sphereHits[i].gameObject.layer == 14))
                        || (PlayerMovementSync._sphereHits[i].gameObject.layer == 27
                            && (scp = __instance._hub.scpsController.CurrentScp as PlayableScps.Scp096) != null
                            && scp.Enraged))
                    {
                        continue;
                    }

                    if (PlayerMovementSync._sphereHits[i].gameObject.layer == 27)
                    {
                        Door componentInParent = PlayerMovementSync._sphereHits[i].GetComponentInParent<Door>();
                        if (componentInParent != null && componentInParent.curCooldown > 0f && !componentInParent.isOpen)
                        {
                            continue;
                        }
                    }

                    __result = true;
                }

                __result = false;

                return false;
            }
            catch (Exception ex)
            {
                Log.Error($"{typeof(AntiCheatTriggerWithCustomScale).FullName}.{nameof(Prefix)}:\n{ex}");
                return true;
            }
        }
    }
}
