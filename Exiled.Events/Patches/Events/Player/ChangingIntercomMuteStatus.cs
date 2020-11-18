// -----------------------------------------------------------------------
// <copyright file="ChangingIntercomMuteStatus.cs" company="Exiled Team">
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
    /// Patch the <see cref="CharacterClassManager.NetworkIntercomMuted"/>.
    /// Adds the <see cref="Player.ChangingIntercomMuteStatus"/> event.
    /// </summary>
    [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.NetworkIntercomMuted), MethodType.Setter)]
    internal static class ChangingIntercomMuteStatus
    {
        private static bool Prefix(CharacterClassManager __instance, bool value)
        {
            ChangingIntercomMuteStatusEventArgs ev = new ChangingIntercomMuteStatusEventArgs(API.Features.Player.Get(__instance._hub), value, true);

            Player.OnChangingIntercomMuteStatus(ev);

            if (!ev.IsAllowed)
            {
                if (value == true)
                {
                    MuteHandler.RevokePersistentMute("ICOM-" + __instance.UserId);
                }
                else
                {
                    MuteHandler.IssuePersistentMute("ICOM-" + __instance.UserId);
                }

                return false;
            }

            return true;
        }
    }
}
