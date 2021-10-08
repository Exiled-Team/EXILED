// -----------------------------------------------------------------------
// <copyright file="ChangingMuteStatus.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1313
    using System;

    using Assets._Scripts.Dissonance;

    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    /// <summary>
    /// Patch the <see cref="DissonanceUserSetup.AdministrativelyMuted"/>.
    /// Adds the <see cref="Player.ChangingMuteStatus"/> event.
    /// </summary>
    [HarmonyPatch(typeof(DissonanceUserSetup), nameof(DissonanceUserSetup.AdministrativelyMuted), MethodType.Setter)]
    internal static class ChangingMuteStatus
    {
        private static bool Prefix(DissonanceUserSetup __instance, bool value)
        {
            try
            {
                API.Features.Player player = API.Features.Player.Get(__instance.netId);
                ChangingMuteStatusEventArgs ev = new ChangingMuteStatusEventArgs(player, value, true);

                Player.OnChangingMuteStatus(ev);

                if (!ev.IsAllowed)
                {
                    if (value == true)
                    {
                        MuteHandler.RevokePersistentMute(player.UserId);
                    }
                    else
                    {
                        MuteHandler.IssuePersistentMute(player.UserId);
                    }

                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                API.Features.Log.Error($"{typeof(ChangingMuteStatus).FullName}.{nameof(Prefix)}:\n{e}");
                return true;
            }
        }
    }
}
