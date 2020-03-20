using Grenades;
using Harmony;
using MEC;
using System.Collections.Generic;
using UnityEngine;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(Grenade), nameof(Grenade.ServersideExplosion))]
	public class GrenadeExplosionPatch
	{
		public static void Prefix(Grenade __instance)
		{
			if (EventPlugin.GrenadeExplosionEventDisabled)
				return;

			foreach (GameObject obj in PlayerManager.players)
			{
				if (Vector3.Distance(obj.transform.position, __instance.gameObject.transform.position) > 45f)
				{
					Timing.RunCoroutine(SetBlind(obj.GetComponent<FlashEffect>()));
					continue;
				}

				FlashEffect effect = obj.GetComponent<FlashEffect>();
				WeaponManager manager = obj.GetComponent<WeaponManager>();

				if (!effect.Flashable(__instance.NetworkthrowerGameObject, __instance.gameObject.transform.position, manager.raycastServerMask))
					Timing.RunCoroutine(SetBlind(effect));
			}
		}

		private static IEnumerator<float> SetBlind(FlashEffect effect)
		{
			for (int i = 0; i < 60; i++)
			{
				effect.CallCmdBlind(false);
				yield return Timing.WaitForOneFrame;
			}
		}
	}
}