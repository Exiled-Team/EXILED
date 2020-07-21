// -----------------------------------------------------------------------
// <copyright file="Scp173BeingLooked.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
#pragma warning disable SA1313
    using System;

    using Exiled.API.Features;

    using HarmonyLib;

    using UnityEngine;

    /// <summary>
    /// Patches <see cref="Scp173PlayerScript.FixedUpdate"/>.
    /// </summary>
    [HarmonyPatch(typeof(Scp173PlayerScript), nameof(Scp173PlayerScript.FixedUpdate))]
    internal static class Scp173BeingLooked
    {
        private static bool Prefix(Scp173PlayerScript __instance)
        {
            try
            {
                __instance.DoBlinkingSequence();

                if (!__instance.iAm173)
                    return false;

                __instance._allowMove = true;

                foreach (GameObject gameObject in PlayerManager.players)
                {
                    Player player = Player.Get(gameObject);

                    if (player.Role == RoleType.Tutorial)
                        continue;

                    Scp173PlayerScript component = player.ReferenceHub.GetComponent<Scp173PlayerScript>();

                    if (!component.SameClass && component.LookFor173(__instance.gameObject, true) && __instance.LookFor173(component.gameObject, false))
                    {
                        __instance._allowMove = false;
                        break;
                    }
                }

                return false;
            }
            catch (Exception exception)
            {
                Log.Error($"Scp173BeingLooked error: {exception}");
                return true;
            }
        }
    }
}
