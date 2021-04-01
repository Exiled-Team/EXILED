// -----------------------------------------------------------------------
// <copyright file="RemovingHandcuffs.cs" company="Exiled Team">
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
    /// Patches <see cref="Handcuffs.ClearTarget"/>.
    /// Adds the <see cref="Handlers.Player.RemovingHandcuffs"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Handcuffs), nameof(Handcuffs.ClearTarget))]
    internal static class RemovingHandcuffs
    {
        private static bool Prefix(Handcuffs __instance)
        {
            try
            {
                foreach (API.Features.Player target in API.Features.Player.List)
                {
                    if (target == null)
                        continue;

                    if (target.CufferId == __instance.MyReferenceHub.queryProcessor.PlayerId)
                    {
                        var ev = new RemovingHandcuffsEventArgs(API.Features.Player.Get(__instance.gameObject), target);

                        Handlers.Player.OnRemovingHandcuffs(ev);

                        if (ev.IsAllowed)
                            target.CufferId = -1;

                        break;
                    }
                }

                return false;
            }
            catch (Exception e)
            {
                Log.Error($"Sexiled.Events.Patches.Events.Player.RemovingHandcuffs: {e}\n{e.StackTrace}");

                return true;
            }
        }
    }
}
