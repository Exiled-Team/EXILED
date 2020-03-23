using EXILED.Extensions;
using Harmony;
using System;
using UnityEngine;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(Scp173PlayerScript), nameof(Scp173PlayerScript.FixedUpdate))]
	public class Scp173Patch
	{
		public static bool Prefix(Scp173PlayerScript __instance)
		{
			if (EventPlugin.Scp173PatchDisable)
				return true;

			try
			{
				__instance.DoBlinkingSequence();

				if (!__instance.iAm173)
					return false;

				__instance._allowMove = true;

				foreach (GameObject player in PlayerManager.players)
				{
					ReferenceHub hub = player.GetPlayer();

					if (hub.characterClassManager.CurClass == RoleType.Tutorial)
						continue;

					Scp173PlayerScript component = player.GetComponent<Scp173PlayerScript>();

					if (!component.SameClass && component.LookFor173(__instance.gameObject, true) && __instance.LookFor173(component.gameObject, false))
					{
						__instance._allowMove = false;
						break;
					}
				}

				return false;
			}
			catch (Exception exception)
			{
				Log.Error($"Scp173Patch error: {exception}");
				return true;
			}
		}
	}
}