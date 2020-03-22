using GameCore;
using Harmony;
using System;
using UnityEngine;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(Handcuffs), nameof(Handcuffs.CallCmdCuffTarget))]
	public class PlayerHandcuffedEvent
	{
		public static bool Prefix(Handcuffs __instance, GameObject target)
		{
			if (EventPlugin.HandcuffEventPatchDisable)
				return true;

			try
			{
				if (!__instance._interactRateLimit.CanExecute() || target == null ||
					Vector3.Distance(target.transform.position, __instance.transform.position) >
					__instance.raycastDistance * 1.10000002384186)
					return false;
				Handcuffs handcuffs = ReferenceHub.GetHub(target).handcuffs;
				if (handcuffs == null || __instance.MyReferenceHub.inventory.curItem != ItemType.Disarmer ||
					(__instance.MyReferenceHub.characterClassManager.CurClass < RoleType.Scp173 ||
					 handcuffs.CufferId >= 0) || handcuffs.MyReferenceHub.inventory.curItem != ItemType.None)
					return false;
				Team team1 = __instance.MyReferenceHub.characterClassManager.Classes
					.SafeGet(__instance.MyReferenceHub.characterClassManager.CurClass).team;
				Team team2 = __instance.MyReferenceHub.characterClassManager.Classes
					.SafeGet(handcuffs.MyReferenceHub.characterClassManager.CurClass).team;
				bool flag = false;
				switch (team1)
				{
					case Team.MTF:
						if (team2 == Team.CHI || team2 == Team.CDP)
							flag = true;
						if (team2 == Team.RSC && ConfigFile.ServerConfig.GetBool("mtf_can_cuff_researchers"))
						{
							flag = true;
						}

						break;
					case Team.CHI:
						if (team2 == Team.MTF || team2 == Team.RSC)
							flag = true;
						if (team2 == Team.CDP && ConfigFile.ServerConfig.GetBool("ci_can_cuff_class_d"))
						{
							flag = true;
						}

						break;
					case Team.RSC:
						if (team2 == Team.CHI || team2 == Team.CDP)
						{
							flag = true;
						}

						break;
					case Team.CDP:
						if (team2 == Team.MTF || team2 == Team.RSC)
						{
							flag = true;
						}

						break;
				}

				if (!flag)
					return false;
				__instance.ClearTarget();

				bool allow = true;
				Events.InvokePlayerHandcuff(__instance.gameObject, target, ref allow);

				if (!allow)
					return false;
				handcuffs.NetworkCufferId = __instance.MyReferenceHub.queryProcessor.PlayerId;

				return false;
			}
			catch (Exception exception)
			{
				Log.Error($"PlayerHandcuffedEvent error: {exception}");
				return true;
			}
		}
	}

	[HarmonyPatch(typeof(Handcuffs), nameof(Handcuffs.CallCmdFreeTeammate))]
	public class PlayerHandcuffFreedEvent
	{
		public static bool Prefix(Handcuffs __instance, GameObject target)
		{
			if (EventPlugin.HandcuffEventPatchDisable)
				return true;

			try
			{
				if (!__instance._interactRateLimit.CanExecute(true) || target == null ||
					(Vector3.Distance(target.transform.position, __instance.transform.position) >
					 __instance.raycastDistance * 1.10000002384186 || __instance.MyReferenceHub.characterClassManager
						 .Classes.SafeGet(__instance.MyReferenceHub.characterClassManager.CurClass).team == Team.SCP))
					return false;

				bool allow = true;

				Events.InvokePlayerHandcuffFree(__instance.gameObject, target, ref allow);

				if (!allow)
					return false;

				ReferenceHub.GetHub(target).handcuffs.NetworkCufferId = -1;

				return false;
			}
			catch (Exception exception)
			{
				Log.Error($"PlayerHandcuffFreedEvent error: {exception}");
				return true;
			}
		}
	}

	[HarmonyPatch(typeof(Handcuffs), nameof(Handcuffs.ClearTarget))]
	public class PlayersHandcuffFreedEvent
	{
		public static bool Prefix(Handcuffs __instance)
		{
			if (EventPlugin.HandcuffEventPatchDisable)
				return true;

			try
			{
				foreach (GameObject player in PlayerManager.players)
				{
					Handcuffs handcuffs = ReferenceHub.GetHub(player).handcuffs;

					if (handcuffs.CufferId == __instance.MyReferenceHub.queryProcessor.PlayerId)
					{
						bool allow = true;

						Events.InvokePlayerHandcuffFree(__instance.gameObject, player, ref allow);

						if (allow)
							handcuffs.NetworkCufferId = -1;

						break;
					}
				}

				return false;
			}
			catch (Exception exception)
			{
				Log.Error($"PlayerHandcuffFreedEvent error: {exception}");
				return true;
			}
		}
	}
}