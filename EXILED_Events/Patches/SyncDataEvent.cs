using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using UnityEngine;
using EXILED;

namespace EXILED.Patches
{
    [HarmonyPatch(typeof(AnimationController), nameof(AnimationController.CallCmdSyncData))]
    public class SyncDataEvent
    {
        public static bool Prefix(AnimationController __instance, int state, Vector2 v2)
        {
            if (EventPlugin.CmdSyncDataEventDisable)
                return true;

            try
            {
                bool allow = true;
                Events.InvokeSyncData(__instance.gameObject, ref state, ref v2, ref allow);
                return allow;
            }
            catch (Exception e)
            {
                Log.Error($"CmdSyncDataEvent Error: {e}");
                return true;
            }
        }

    }
}
