using System;
using EXILED.Shared.Helpers;
using Harmony;
using UnityEngine;
using Console = GameCore.Console;

namespace EXILED.Events.Patches
{
    [HarmonyPatch(typeof(Console), "AddLog", typeof(string), typeof(Color), typeof(bool))]
    public class RoundEndPatch
    {
        public static void Prefix(string text, Color c, bool nospace)
        {
            if (!text.StartsWith("Round finished! Anomalies: ") || EventPlugin.GetRoundDuration() < 2)
                return;
            try
            {
                Events.Events.InvokeRoundEnd();
            }
            catch (Exception e)
            {
                LogHelper.Error($"Round end event error: {e}");
            }
        }
    }
}