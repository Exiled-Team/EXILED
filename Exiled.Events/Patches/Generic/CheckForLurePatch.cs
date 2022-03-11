// -----------------------------------------------------------------------
// <copyright file="CheckForLurePatch.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
#pragma warning disable SA1313

    using System;
    using System.Linq;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using PlayerStatsSystem;

    using UnityEngine;

    using Player = Exiled.API.Features.Player;

    /// <summary>
    /// Patch the <see cref="CharacterClassManager"/>.
    /// Adds the <see cref="CharacterClassManager.AllowContain"/> event.
    /// </summary>
    internal static class CheckForLurePatch
    {
        [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.AllowContain))]
        private static bool Prefix(CharacterClassManager __instance)
        {
            try
            {
                foreach (Player player in Player.List)
                {
                    if (Scp106Container.CanBeKilled(player))
                    {
                        player.Hurt(new UniversalDamageHandler(-1f, DeathTranslations.UsedAs106Bait, null));
                        __instance._lureSpj.SetState(false, true);
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                Log.Error($"Exiled.Events.Patches.Generic.CheckForLurePatch: {ex}\n{ex.StackTrace}");
                return true;
            }
        }
    }
}
