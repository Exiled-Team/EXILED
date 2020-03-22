using Harmony;
using System;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(DecontaminationLCZ), nameof(DecontaminationLCZ.RpcPlayAnnouncement))]
	public class AnnounceDecontaminationEvent
	{
		public static bool Prefix(ref int id, ref bool global)
		{
			try
			{
				bool allow = true;

				Events.InvokeAnnounceDecontamination(ref id, ref global, ref allow);

				return allow;
			}
			catch (Exception exception)
			{
				Log.Error($"AnnounceDecontaminationEvent error: {exception}");
				return true;
			}
		}
	}
}
