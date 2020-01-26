using Harmony;
using UnityEngine;

namespace EXILED.Events.Patches
{
	[HarmonyPatch(typeof (PlyMovementSync), "AntiFly", typeof (Vector3))]
	public class AntiAntiFly
	{
		public static bool Prefix(PlyMovementSync __instance, ref Vector3 pos) => EventPlugin.AntiFlyPatchDisable;
	}
}