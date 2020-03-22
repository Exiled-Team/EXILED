using Harmony;
using System;
using UnityEngine;
using Console = GameCore.Console;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(Console), nameof(Console.AddLog), typeof(string), typeof(Color), typeof(bool))]
	public class RoundEndEvent
	{
		public static void Prefix(string text, Color c, bool nospace)
		{
			if (!text.StartsWith("Round finished! Anomalies: ") || EventPlugin.GetRoundDuration() < 2)
				return;

			try
			{
				Events.InvokeRoundEnd();
			}
			catch (Exception exception)
			{
				Log.Error($"RoundEndEvent error: {exception}");
			}
		}
	}
}