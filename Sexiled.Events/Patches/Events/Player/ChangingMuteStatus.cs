// -----------------------------------------------------------------------
// <copyright file="ChangingMuteStatus.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using Sexiled.API.Features;
using Sexiled.Events.EventArgs;

namespace Sexiled.Events.Patches.Events.Player
{
#pragma warning disable SA1313
    using System;

    using Sexiled.Events.EventArgs;
    using Sexiled.Events.Handlers;

    using HarmonyLib;

    /// <summary>
    /// Patch the <see cref="CharacterClassManager.NetworkMuted"/>.
    /// Adds the <see cref="Handlers.Player.ChangingMuteStatus"/> event.
    /// </summary>
    [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.NetworkMuted), MethodType.Setter)]
    internal static class ChangingMuteStatus
    {
        private static bool Prefix(CharacterClassManager __instance, bool value)
        {
            try
            {
                ChangingMuteStatusEventArgs ev = new ChangingMuteStatusEventArgs(API.Features.Player.Get(__instance._hub), value, true);

                Handlers.Player.OnChangingMuteStatus(ev);

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
            catch (Exception e)
            {
                Log.Error($"{typeof(ChangingMuteStatus).FullName}.{nameof(Prefix)}:\n{e}");
                return true;
            }
        }
    }
}
