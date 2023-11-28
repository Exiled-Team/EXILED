// -----------------------------------------------------------------------
// <copyright file="ValidatingVisibility.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp939
{
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp939;
    using HarmonyLib;
    using PlayerRoles.PlayableScps.Scp939;

    /// <summary>
    /// Patches <see cref="Scp939VisibilityController.ValidateVisibility(ReferenceHub hub)"/>
    /// to add <see cref="ValidatingVisibility"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Scp939), nameof(Handlers.Scp939.ValidatedVisibility))]
    [HarmonyPatch(typeof(Scp939VisibilityController), nameof(Scp939VisibilityController.ValidateVisibility))]
    internal class ValidatingVisibility
    {
        private static void Postfix(Scp939VisibilityController __instance, ReferenceHub hub, ref bool __result)
        {
            ValidatingVisibilityEventArgs ev = new(__instance.Owner, hub, __result);
            Handlers.Scp939.OnValidatedVisibility(ev);
            __result = ev.IsAllowed;
        }
    }
}