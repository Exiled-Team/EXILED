using Harmony;

namespace EXILED.Patches
{
    [HarmonyPatch(typeof(BanHandler), "IssueBan")]
    public class BanHandlerOverride
    {
        public static void Postfix(BanDetails ban, BanHandler.BanType banType) => Events.InvokePlayerBanned(ban, banType);
    }
}
