using EXILED;
using Harmony;
using UnityEngine;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(Scp173PlayerScript), "FixedUpdate")]
	public class Scp173Override
	{
		public static bool Prefix(Scp173PlayerScript __instance)
		{
			if (EXILED.plugin.Scp173PatchDisable)
				return true;
			
			__instance.DoBlinkingSequence();
			if (!__instance.iAm173)
				return false;
			__instance._allowMove = true;
			foreach (GameObject player in PlayerManager.players)
			{
				if (ReferenceHub.GetHub(player).characterClassManager.CurClass == RoleType.Tutorial)
					continue;
				Scp173PlayerScript component = player.GetComponent<Scp173PlayerScript>();
				if (!component.SameClass && component.LookFor173(__instance.gameObject, true) &&
				    __instance.LookFor173(component.gameObject, false))
				{
					__instance._allowMove = false;
					break;
				}
			}

			return false;
		}
	}
}