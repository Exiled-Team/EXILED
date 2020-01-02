using System;
using Harmony;
using UnityEngine;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof (GameCore.Console), "AddLog", new Type[] {typeof (string), typeof (Color), typeof (bool)})]
	public class RoundEndPatch
	{
		public static void Prefix(string text, Color c, bool nospace)
		{
			if (!text.StartsWith("Round finished! Anomalies: ") || EXILED.plugin.GetRoundDuration() < 2)
				return;
			try
			{
				Events.InvokeRoundEnd();
			}
			catch (Exception e)
			{
				Plugin.Error($"Round end event error: {e}");
			}
		}
	}
}