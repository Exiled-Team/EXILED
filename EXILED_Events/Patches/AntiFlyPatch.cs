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
	    public static Dictionary<string, int> AntiFlyCounter = new Dictionary<string,int>();
		public static Dictionary<string, CoroutineHandle> ResetCoroutines = new Dictionary<string, CoroutineHandle>();
    
	    public static bool Prefix (PlyMovementSync __instance, Vector3 pos) => false;
	}
}