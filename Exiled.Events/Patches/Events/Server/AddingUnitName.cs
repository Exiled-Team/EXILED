// -----------------------------------------------------------------------
// <copyright file="AddingUnitName.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Server
{
    using Exiled.Events.EventArgs;
    using HarmonyLib;
    using Respawning;
    using Respawning.NamingRules;

    /// <summary>
    /// Patches <see cref="UnitNamingRule.AddCombination"/>.
    /// Adds the <see cref="Handlers.Server.AddingUnitName"/> event.
    /// </summary>
    [HarmonyPatch(typeof(UnitNamingRule), nameof(UnitNamingRule.AddCombination))]
    internal static class AddingUnitName
    {
        private static bool Prefix(ref string regular, SpawnableTeamType type)
        {
            AddingUnitNameEventArgs ev = new AddingUnitNameEventArgs(regular);
            Handlers.Server.OnAddingUnitName(ev);

            if (!ev.IsAllowed)
                return false;

            regular = ev.UnitName;
            return true;
        }
    }
}
