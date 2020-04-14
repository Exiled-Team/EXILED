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
    
    public static bool Prefix (PlyMovementSync __instance, Vector3 pos, out bool wasChanged)
		{
      try
      {
        wasChanged = false;
        if (!NetworkServer.active || __instance.WhitelistPlayer || (__instance.NoclipWhitelisted || !__instance._successfullySpawned))
          return false;
        if (wasChanged)
        {
          __instance._groundedY = __instance._lastSafePosition.y;
          __instance._flyTime = 0.0f;
        }
        else
        {
          bool flag = __instance._hub.characterClassManager.CurClass == RoleType.Spectator || __instance._hub.characterClassManager.CurClass == RoleType.None || __instance._hub.characterClassManager.CurClass == RoleType.Scp079;
          __instance._isGrounded = __instance._hub.falldamage.isCloseToGround;
          if (__instance._noclipPrevWl)
          {
            if (__instance._isGrounded)
            {
              __instance._noclipPrevWl = false;
              __instance._flyTime = 0.0f;
            }
            else
            {
              __instance._flyTime += Time.deltaTime;
              if (__instance._flyTime < 5.0)
                return false;
              __instance._noclipPrevWl = false;
            }
          }
          if (flag || __instance._isGrounded || (Math.Abs(__instance._groundedY - pos.y) <= 1.0 || Math.Abs(__instance._groundedY - __instance._lastSafePosition.y) <= 1.0))
          {
            __instance._flyTime = 0.0f;
            __instance._groundedY = pos.y;
          }
          else
          {
            __instance._flyTime += Time.deltaTime;
            if (!__instance._isGrounded)
            {
              if (__instance._groundedY < pos.y - EventPlugin.AntiFlyThreashold)
              {
                if (__instance._safeTime > 0.0)
                  return false;
                __instance.AntiCheatKillPlayer("Killed by the anti-cheat system for flying\n(debug code: 1.3 - vertical flying)");
                return false;
              }
              if (__instance._groundedY > (double) pos.y)
                __instance._groundedY = pos.y;
            }
            Vector3 vector3 = pos;
            vector3.y -= 50f;
            if (__instance._receivedPosition != Vector3.up * 2048f && !Physics.Linecast(pos, vector3, __instance.CollidableSurfaces))
            {
              vector3.y += 23.8f;
              if (Physics.OverlapBoxNonAlloc(vector3, new Vector3(0.5f, 25f, 0.5f), PlyMovementSync.AntiFlyBuffer, new Quaternion(0.0f, 0.0f, 0.0f, 0.0f), __instance.CollidableSurfaces) == 0)
              {
                if (__instance._safeTime > 0.0)
                  return false;
                __instance.AntiCheatKillPlayer("Killed by the anti-cheat system for flying\n(debug code: 1.2 - no surface detected underneath the player)");
                return false;
              }
            }
            if (__instance._flyTime < 2.20000004768372)
              return false;
            __instance.AntiCheatKillPlayer("Killed by the anti-cheat system for flying\n(debug code: 1.1 - flying time limit exceeded)");
          }
        }

        return false;
      }
      catch (Exception e)
      {
        Log.Error(e.ToString());
        wasChanged = false;
        return true;
      }
    }

    public static IEnumerator<float> ResetCounter(ReferenceHub hub)
    {
      yield return Timing.WaitForSeconds(120f);
      if (AntiFlyCounter.ContainsKey(hub.GetUserId()))
        AntiFlyCounter[hub.GetUserId()] = 0;
    }
	}
}