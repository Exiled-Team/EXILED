// -----------------------------------------------------------------------
// <copyright file="ChangingMuteStatus.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1313
    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    /// <summary>
    /// Patch the <see cref="CharacterClassManager.NetworkMuted"/>.
    /// Adds the <see cref="Player.ChangingMuteStatus"/> event.
    /// </summary>
    [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.NetworkMuted), MethodType.Setter)]
    internal static class ChangingMuteStatus
    {
        private static bool Prefix(CharacterClassManager __instance, bool value)
        {
            ChangingMuteStatusEventArgs ev = new ChangingMuteStatusEventArgs(API.Features.Player.Get(__instance._hub), value, true);

            Player.OnChangingMuteStatus(ev);

            if (!ev.IsAllowed)
            {
                if (value == true)
                {
                    MuteHandler.RevokePersistentMute(__instance.UserId);
                }
                else
                {
                    MuteHandler.IssuePersistentMute(__instance.UserId);
                }

                return false;
            }

            return true;
        }
    }
}
