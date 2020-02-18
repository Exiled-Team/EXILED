using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EXILED.Patches
{
    [HarmonyPatch(typeof(BanPlayer), nameof(BanPlayer.BanUser))]
    public class BanUserOverride
    {
        public static bool Prefix(GameObject user, int duration, string reason, string issuer, bool isGlobalBan)
        {

            return false;
        }
    }
}
