using Harmony;
using UnityEngine;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(PlyMovementSync), nameof(PlyMovementSync.AntiFly), typeof(Vector3))]
	public class AntiAntiFly
	{
		public static bool Prefix(PlyMovementSync __instance, ref Vector3 pos) => EventPlugin.AntiFlyPatchDisable;
	}
}