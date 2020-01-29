using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EXILED.Patches
{
    [HarmonyPatch(typeof(BanHandler), "IssueBan")]
    public class BanHandlerOverride
    {
        public static void Postfix(BanDetails ban, BanHandler.BanType banType) => Events.InvokePlayerBanned(ban, banType);
    }
}
