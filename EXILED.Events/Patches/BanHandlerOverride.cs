using Harmony;

namespace EXILED.Events.Patches
{
    [HarmonyPatch(typeof(BanHandler), "IssueBan")]
    public class BanHandlerOverride
    {
        public static void PostFix(BanDetails ban, BanHandler.BanType banType) => Events.Events.InvokePlayerBanned(ban, banType);
    }
}
