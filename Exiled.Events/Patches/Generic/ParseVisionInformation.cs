// -----------------------------------------------------------------------
// <copyright file="ParseVisionInformation.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
#pragma warning disable SA1313
    using Exiled.API.Features;

    using HarmonyLib;

    using PlayableScps;

    using RemoteAdmin;

    using UnityEngine;

    /// <summary>
    /// Patches <see cref="Scp096.ParseVisionInformation"/>.
    /// </summary>
    [HarmonyPatch(typeof(PlayableScps.Scp096), nameof(PlayableScps.Scp096.ParseVisionInformation))]
    internal static class ParseVisionInformation
    {
        private static bool Prefix(PlayableScps.Scp096 __instance, VisionInformation info)
        {
            PlayableScpsController playableScpsController = info.RaycastResult.transform.gameObject.GetComponent<PlayableScpsController>();
            if (!info.Looking || !info.RaycastHit || playableScpsController == null || playableScpsController.CurrentScp == null || playableScpsController.CurrentScp != __instance)
            {
                return false;
            }

            CharacterClassManager ccm = info.Source.GetComponent<CharacterClassManager>();
            QueryProcessor qp = info.Source.GetComponent<QueryProcessor>();
            if (ccm == null || qp == null || API.Features.Scp096.TurnedPlayers.Contains(Player.Get(info.Source)) || (!Exiled.Events.Events.Instance.Config.CanTutorialTriggerScp096 && ccm.CurClass == RoleType.Tutorial))
            {
                return false;
            }

            float delay = (1f - info.DotProduct) / 0.25f * (Vector3.Distance(info.Source.transform.position, info.Target.transform.position) * 0.1f);
            if (!__instance.Calming)
            {
                __instance.AddTarget(info.Source);
            }

            if (__instance.CanEnrage && info.Source != null)
            {
                __instance.PreWindup(delay);
            }

            return false;
        }
    }
}
