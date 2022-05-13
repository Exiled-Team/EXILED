namespace Exiled.CustomRoles.HarmonyPatch
{
    using System.Linq;
    using Exiled.API.Features;
    using Exiled.CustomRoles.API;
    using Exiled.Permissions.Extensions;

    /// <summary>
    /// Gets the SpeakPatch class.
    /// </summary>
    [HarmonyLib.HarmonyPatch(typeof(Radio), nameof(Radio.UserCode_CmdSyncTransmissionStatus))]
    public class SpeakPatch
    {
        /// <summary>
        /// Prefix for the speak patch.
        /// </summary>
        /// <returns>True if player can speak with V, false if not.</returns>
        public static bool Prefix(Radio __instance, bool b)
        {
            Exiled.API.Features.Player player = Exiled.API.Features.Player.Get(__instance._hub);
            if (string.IsNullOrEmpty(player?.UserId))
                return false;
            if (player.HasItem(ItemType.Radio))
                return __instance._dissonanceSetup.RadioAsHuman = b;
            if (player.Role.Team != Team.SCP)
                return false;

            Log.Debug($"Player {player} has role which can speak with V? {player.GetCustomRoles().Any(cRole => cRole.TrackedVPlayers.Contains(player))}", CustomRoles.Instance.Config.Debug);
            if (player.GetCustomRoles().Any(cRole => cRole.TrackedVPlayers.Contains(player)))
                return __instance._dissonanceSetup.MimicAs939 = b;

            return false;
        }
    }
}
