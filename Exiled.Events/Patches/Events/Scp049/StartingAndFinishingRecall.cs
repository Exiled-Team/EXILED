// -----------------------------------------------------------------------
// <copyright file="StartingAndFinishingRecall.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp049
{
#pragma warning disable SA1313
    using Exiled.Events.EventArgs;
    using GameCore;
    using HarmonyLib;
    using Mirror;
    using UnityEngine;

    /// <summary>
    /// Patches <see cref="PlayableScps.Scp049.BodyCmd_ByteAndGameObject(byte, GameObject)"/>.
    /// Adds the <see cref="Handlers.Scp049.StartingRecall"/> and <see cref="Handlers.Scp049.FinishingRecall"/> event.
    /// </summary>
    [HarmonyPatch(typeof(PlayableScps.Scp049), nameof(PlayableScps.Scp049.BodyCmd_ByteAndGameObject))]
    internal class StartingAndFinishingRecall
    {
        private static bool Prefix(PlayableScps.Scp049 __instance, byte num, GameObject go)
        {
            if (num == 2)
            {
                if (!__instance._interactRateLimit.CanExecute() || go == null)
                    return false;

                Ragdoll component = go.GetComponent<Ragdoll>();
                if (component == null)
                    return false;

                ReferenceHub referenceHub = null;
                foreach (GameObject player in PlayerManager.players)
                {
                    ReferenceHub hub = ReferenceHub.GetHub(player);
                    if (hub.queryProcessor.PlayerId == component.owner.PlayerId)
                    {
                        referenceHub = hub;
                        break;
                    }
                }

                if (referenceHub == null)
                {
                    Console.AddDebugLog("SCPCTRL", "SCP-049 | Request 'finish recalling' rejected; no target found", MessageImportance.LessImportant);
                    return false;
                }

                if (!__instance._recallInProgressServer || referenceHub.gameObject != __instance._recallObjectServer || __instance._recallProgressServer < 0.85f)
                {
                    Console.AddDebugLog("SCPCTRL", "SCP-049 | Request 'finish recalling' rejected; Debug code: ", MessageImportance.LessImportant);
                    Console.AddDebugLog("SCPCTRL", "SCP-049 | CONDITION#1 " + (__instance._recallInProgressServer ? "<color=green>PASSED</color>" : ("<color=red>ERROR</color> - " + __instance._recallInProgressServer)), MessageImportance.LessImportant, true);
                    Console.AddDebugLog("SCPCTRL", "SCP-049 | CONDITION#2 " + ((referenceHub == __instance._recallObjectServer) ? "<color=green>PASSED</color>" : string.Concat("<color=red>ERROR</color> - ", referenceHub.queryProcessor.PlayerId, "-", (__instance._recallObjectServer == null) ? "null" : ReferenceHub.GetHub(__instance._recallObjectServer).queryProcessor.PlayerId.ToString())), MessageImportance.LessImportant);
                    Console.AddDebugLog("SCPCTRL", "SCP-049 | CONDITION#3 " + ((__instance._recallProgressServer >= 0.85f) ? "<color=green>PASSED</color>" : ("<color=red>ERROR</color> - " + __instance._recallProgressServer)), MessageImportance.LessImportant, true);
                    return false;
                }

                if (referenceHub.characterClassManager.CurClass != RoleType.Spectator)
                {
                    return false;
                }

                var ev = new FinishingRecallEventArgs(API.Features.Player.Get(referenceHub.gameObject), API.Features.Player.Get(__instance.Hub.gameObject));

                Handlers.Scp049.OnFinishingRecall(ev);

                if (!ev.IsAllowed)
                    return false;

                Console.AddDebugLog("SCPCTRL", "SCP-049 | Request 'finish recalling' accepted", MessageImportance.LessImportant);
                RoundSummary.changed_into_zombies++;
                referenceHub.characterClassManager.SetClassID(RoleType.Scp0492);
                referenceHub.GetComponent<PlayerStats>().Health = referenceHub.characterClassManager.Classes.Get(RoleType.Scp0492).maxHP;
                if (component.CompareTag("Ragdoll"))
                {
                    NetworkServer.Destroy(component.gameObject);
                }

                __instance._recallInProgressServer = false;
                __instance._recallObjectServer = null;
                __instance._recallProgressServer = 0f;
                return false;
            }

            if (num != 1)
                return true;
            {
                if (!__instance._interactRateLimit.CanExecute())
                    return false;

                if (go == null)
                    return false;

                Ragdoll component2 = go.GetComponent<Ragdoll>();
                if (component2 == null)
                {
                    Console.AddDebugLog("SCPCTRL", "SCP-049 | Request 'start recalling' rejected; provided object is not a dead body", MessageImportance.LessImportant);
                    return false;
                }

                if (!component2.allowRecall)
                {
                    Console.AddDebugLog("SCPCTRL", "SCP-049 | Request 'start recalling' rejected; provided object can't be recalled", MessageImportance.LessImportant);
                    return false;
                }

                ReferenceHub referenceHub2 = null;
                foreach (GameObject player2 in PlayerManager.players)
                {
                    ReferenceHub hub2 = ReferenceHub.GetHub(player2);
                    if (hub2 != null && hub2.queryProcessor.PlayerId == component2.owner.PlayerId)
                    {
                        referenceHub2 = hub2;
                        break;
                    }
                }

                if (referenceHub2 == null)
                {
                    Console.AddDebugLog("SCPCTRL", "SCP-049 | Request 'start recalling' rejected; target not found", MessageImportance.LessImportant);
                    return false;
                }

                if (Vector3.Distance(component2.transform.position, __instance.Hub.PlayerCameraReference.transform.position) >= PlayableScps.Scp049.ReviveDistance * 1.3f)
                    return false;

                var ev = new StartingRecallEventArgs(API.Features.Player.Get(referenceHub2.gameObject), API.Features.Player.Get(__instance.Hub.gameObject));

                Handlers.Scp049.OnStartingRecall(ev);

                if (!ev.IsAllowed)
                    return false;

                Console.AddDebugLog("SCPCTRL", "SCP-049 | Request 'start recalling' accepted", MessageImportance.LessImportant);
                __instance._recallObjectServer = referenceHub2.gameObject;
                __instance._recallProgressServer = 0f;
                __instance._recallInProgressServer = true;
                return false;
            }
        }
    }
}
