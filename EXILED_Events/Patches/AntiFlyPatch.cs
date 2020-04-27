using System;
using System.Collections.Generic;
using EXILED.Extensions;
using Harmony;
using MEC;
using Mirror;
using UnityEngine;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(PlyMovementSync), nameof(PlyMovementSync.AntiFly))]
	public class AntiFlyPatch
	{ 
		public static bool Prefix (PlyMovementSync __instance, Vector3 pos) => EventPlugin.AntiFlyPatchDisable;
	}
}