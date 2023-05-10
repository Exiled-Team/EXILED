// -----------------------------------------------------------------------
// <copyright file="Focus.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp939
{
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
    using Exiled.Events.EventArgs.Scp939;
    using Exiled.Events.Handlers;

    using HarmonyLib;
    using Mirror;

    using PlayerRoles.PlayableScps.Scp939;

    /// <summary>
    ///     Patches <see cref="Scp939FocusKeySync.ServerProcessCmd(NetworkReader)" />
    ///     to add the <see cref="Scp939.ChangingFocus" /> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp939FocusKeySync), nameof(Scp939FocusKeySync.ServerProcessCmd))]
    internal static class Focus
    {
        private static bool Prefix(Scp939FocusKeySync __instance, NetworkReader reader)
        {
            bool state = reader.ReadBool();
            ChangingFocusEventArgs ev = new(__instance.Owner, state);
            Scp939.OnChangingFocus(ev);

            if (ev.IsAllowed)
            {
                __instance.FocusKeyHeld = state;
            }

            return false;
        }
    }
}
