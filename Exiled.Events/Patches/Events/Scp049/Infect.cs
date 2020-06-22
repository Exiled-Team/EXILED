// -----------------------------------------------------------------------
// <copyright file="Infect.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp049
{
    using Exiled.Events.EventArgs;
    using HarmonyLib;
    using Mirror;
    using UnityEngine;

    /// <summary>
    /// Patches <see cref="PlayableScps.Scp049.BodyCmd_ByteAndGameObject(byte, GameObject)"/>.
    /// Adds the <see cref="Scp049.InfectPlayer"/> event.
    /// </summary>
    [HarmonyPatch(typeof(PlayableScps.Scp049), nameof(PlayableScps.Scp049.BodyCmd_ByteAndGameObject))]
    public class Infect
    {
        /// <summary>
        /// Prefix of <see cref="PlayableScps.Scp049.BodyCmd_ByteAndGameObject(byte, GameObject)"/>.
        /// </summary>
        /// <param name="__instance">The <see cref="PlayableScps.Scp049"/> instance.</param>
        /// <param name="num">The byte determining what type of action is taking place instance.</param>
        /// <param name="go">The player's game object.</param>
        /// <returns>Returns a value indicating whether the original method has to be executed or not.</returns>
        public static bool Prefix(PlayableScps.Scp049 __instance, byte num, GameObject go)
        {
            if (num == 2)
            {
                if (!__instance._interactRateLimit.CanExecute(true))
                {
                    return false;
                }
                if (go == null)
                {
                    return false;
                }
                Ragdoll component = go.GetComponent<Ragdoll>();
                if (component == null)
                {
                    return false;
                }
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
                    GameCore.Console.AddDebugLog("SCPCTRL", "SCP-049 | Request 'finish recalling' rejected; no target found", MessageImportance.LessImportant, false);
                    return false;
                }
                if (!__instance._recallInProgressServer || referenceHub.gameObject != __instance._recallObjectServer || __instance._recallProgressServer < 0.85f)
                {
                    GameCore.Console.AddDebugLog("SCPCTRL", "SCP-049 | Request 'finish recalling' rejected; Debug code: ", MessageImportance.LessImportant, false);
                    GameCore.Console.AddDebugLog("SCPCTRL", "SCP-049 | CONDITION#1 " + (__instance._recallInProgressServer ? "<color=green>PASSED</color>" : ("<color=red>ERROR</color> - " + __instance._recallInProgressServer.ToString())), MessageImportance.LessImportant, true);
                    GameCore.Console.AddDebugLog("SCPCTRL", "SCP-049 | CONDITION#2 " + ((referenceHub == __instance._recallObjectServer) ? "<color=green>PASSED</color>" : string.Concat(new object[]
                    {
                                "<color=red>ERROR</color> - ",
                                referenceHub.queryProcessor.PlayerId,
                                "-",
                                (__instance._recallObjectServer == null) ? "null" : ReferenceHub.GetHub(__instance._recallObjectServer).queryProcessor.PlayerId.ToString()
                    })), MessageImportance.LessImportant, false);
                    GameCore.Console.AddDebugLog("SCPCTRL", "SCP-049 | CONDITION#3 " + ((__instance._recallProgressServer >= 0.85f) ? "<color=green>PASSED</color>" : ("<color=red>ERROR</color> - " + __instance._recallProgressServer)), MessageImportance.LessImportant, true);
                    return false;
                }

                if (referenceHub.characterClassManager.CurClass != RoleType.Spectator)
                {
                    return false;
                }

                var ev = new InfectPlayerArgs(API.Features.Player.Get(referenceHub.gameObject));

                Exiled.Events.Handlers.Scp049.OnInfectPlayer(ev);

                if (!ev.IsAllowed)
                    return false;

                GameCore.Console.AddDebugLog("SCPCTRL", "SCP-049 | Request 'finish recalling' accepted", MessageImportance.LessImportant, false);
                RoundSummary.changed_into_zombies++;
                referenceHub.characterClassManager.SetClassID(RoleType.Scp0492);
                referenceHub.GetComponent<PlayerStats>().Health = (float)referenceHub.characterClassManager.Classes.Get(RoleType.Scp0492).maxHP;
                if (component.CompareTag("Ragdoll"))
                {
                    NetworkServer.Destroy(component.gameObject);
                }

                __instance._recallInProgressServer = false;
                __instance._recallObjectServer = null;
                __instance._recallProgressServer = 0f;
                return false;
            }
            return true;
        }
    }
}
