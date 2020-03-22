using Harmony;
using System;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(NineTailedFoxAnnouncer), nameof(NineTailedFoxAnnouncer.AnnounceNtfEntrance))]
	public class AnnounceNtfEntranceEvent
	{
		public static bool Prefix(ref int _scpsLeft, ref int _mtfNumber, ref char _mtfLetter)
		{
			try
			{
				bool allow = true;

				Events.InvokeAnnounceNtfEntrance(ref _scpsLeft, ref _mtfNumber, ref _mtfLetter, ref allow);

				return allow;
			}
			catch (Exception exception)
			{
				Log.Error($"AnnounceNtfEntranceEvent error: {exception}");
				return true;
			}
		}
	}
}
