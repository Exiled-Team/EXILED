using Harmony;
using System;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.CmdStartRound))]
	public class RoundStartEvent
	{
		public static void Prefix()
		{
			try
			{
				Events.InvokeRoundStart();
			}
			catch (Exception exception)
			{
				Log.Error($"RoundStartEvent error: {exception}");
			}
		}
	}
}