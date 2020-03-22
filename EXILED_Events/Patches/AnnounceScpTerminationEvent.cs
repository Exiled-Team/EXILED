using Harmony;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(NineTailedFoxAnnouncer), nameof(NineTailedFoxAnnouncer.AnnounceScpTermination))]
	public class AnnounceScpTerminationEvent
	{
		public static bool Prefix(Role scp, ref PlayerStats.HitInfo hit, ref string groupId)
		{
			bool allow = true;

			Events.InvokeAnnounceScpTermination(scp, ref hit, ref groupId, ref allow);

			return allow;
		}
	}
}
