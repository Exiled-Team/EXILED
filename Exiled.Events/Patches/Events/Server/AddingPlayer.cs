namespace Exiled.Events.Patches.Events.Server
{
    using Exiled.Events.EventArgs;
    using HarmonyLib;
    using UnityEngine;

    /// <summary>
    /// Patches <see cref="PlayerManager.AddPlayer"/>
    /// Adds the <see cref="Handlers.Server.AddingPlayer"/> event.
    /// </summary>
    [HarmonyPatch(typeof(PlayerManager), nameof(PlayerManager.AddPlayer))]
    internal static class AddingPlayer
    {
        private static bool Prefix(GameObject gameObject, int maxPlayers)
        {
            AddingPlayerEventArgs ev = new AddingPlayerEventArgs(gameObject, maxPlayers);
            Handlers.Server.OnAddingPlayer(ev);

            return ev.IsAllowed;
        }
    }
}
