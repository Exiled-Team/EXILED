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
        private static bool Prefix(string regular, SpawnableTeamType type)
        {
            AddingUnitNameEventArgs ev = new AddingUnitNameEventArgs(regular, type);
            Handlers.Server.OnAddingUnitName(ev);
            return ev.IsAllowed;
        }
    }
}
