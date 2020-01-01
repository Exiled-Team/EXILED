using EXILED;
using Harmony;
using UnityEngine;

namespace JokersPlugin.Patches
{
	[HarmonyPatch(typeof (PlyMovementSync), "AntiFly", typeof (Vector3))]
	public class AntiAntiFly
	{
		public static bool Prefix(PlyMovementSync __instance, ref Vector3 pos) => EXILED.plugin.AntiFlyPatchDisable;
	}
}