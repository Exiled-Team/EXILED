using Harmony;
using System;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.CmdStartRound))]
	public class RoundStartPatch
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