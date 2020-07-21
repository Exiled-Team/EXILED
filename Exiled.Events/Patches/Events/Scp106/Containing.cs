// -----------------------------------------------------------------------
// <copyright file="Containing.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp106
{
#pragma warning disable SA1313
    using System.Collections.Generic;
    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;
    using HarmonyLib;
    using UnityEngine;

    /// <summary>
    /// Patches <see cref="PlayerInteract.CallCmdContain106"/>.
    /// Adds the <see cref="Scp106.Containing"/> event.
    /// </summary>
    [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.CallCmdContain106))]
    internal static class Containing
    {
        private static bool Prefix(PlayerInteract __instance)
        {
            if (!__instance._playerInteractRateLimit.CanExecute(true) || (__instance._hc.CufferId > 0 && !PlayerInteract.CanDisarmedInteract))
            {
                return false;
            }

            if (!UnityEngine.Object.FindObjectOfType<LureSubjectContainer>().allowContain || (__instance._ccm.CurRole.team == Team.SCP && __instance._ccm.CurClass != RoleType.Scp106) || !__instance.ChckDis(GameObject.FindGameObjectWithTag("FemurBreaker").transform.position) || UnityEngine.Object.FindObjectOfType<OneOhSixContainer>().used || __instance._ccm.CurRole.team == Team.RIP)
            {
                return false;
            }

            bool flag = false;
            foreach (KeyValuePair<GameObject, ReferenceHub> keyValuePair in ReferenceHub.GetAllHubs())
            {
                if (keyValuePair.Value.characterClassManager.GodMode && keyValuePair.Value.characterClassManager.CurClass == RoleType.Scp106)
                {
                    flag = true;
                }
            }

            if (!flag)
            {
                foreach (KeyValuePair<GameObject, ReferenceHub> keyValuePair2 in ReferenceHub.GetAllHubs())
                {
                    if (keyValuePair2.Value.characterClassManager.CurClass == RoleType.Scp106)
                    {
                        var ev = new ContainingEventArgs(API.Features.Player.Get(keyValuePair2.Key));

                        Scp106.OnContaining(ev);

                        if (ev.IsAllowed)
                        {
                            keyValuePair2.Key.GetComponent<Scp106PlayerScript>().Contain(__instance._hub);
                        }
                    }
                }

                __instance.RpcContain106(__instance.gameObject);
                UnityEngine.Object.FindObjectOfType<OneOhSixContainer>().Networkused = true;
            }

            __instance.OnInteract();
            return false;
        }
    }
}
