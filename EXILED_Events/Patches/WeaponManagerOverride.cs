using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EXILED.Patches
{
    // joker kinda cute uwu
    [HarmonyPatch(typeof(WeaponManager), "CallCmdShoot")]
    public class WeaponManagerOverride
    {
      public static bool Prefix(WeaponManager __instance, GameObject target, string hitboxType, Vector3 dir,
        Vector3 sourcePos, Vector3 targetPos)
      {
        try
        {
          if (!__instance._iawRateLimit.CanExecute(true))
            return false;
          int itemIndex = __instance.hub.inventory.GetItemIndex();
          if (itemIndex < 0 || itemIndex >= __instance.hub.inventory.items.Count || __instance.curWeapon < 0 ||
              (__instance.reloadCooldown > 0.0 || __instance.fireCooldown > 0.0) && !__instance.isLocalPlayer ||
              (__instance.hub.inventory.curItem != __instance.weapons[__instance.curWeapon].inventoryID ||
               __instance.hub.inventory.items[itemIndex].durability <= 0.0))
            return false;
          if (Vector3.Distance(__instance.camera.transform.position, sourcePos) > 6.5)
          {
            __instance.GetComponent<CharacterClassManager>().TargetConsolePrint(__instance.connectionToClient,
              "Shot rejected - Code 2.2 (difference between real source position and provided source position is too big)",
              "gray");
          }
          else
          {
            __instance.hub.inventory.items.ModifyDuration(itemIndex,
              __instance.hub.inventory.items[itemIndex].durability - 1f);
            __instance.scp268.ServerDisable();
            __instance.fireCooldown =
              (float) (1.0 / (__instance.weapons[__instance.curWeapon].shotsPerSecond *
                              (double) __instance.weapons[__instance.curWeapon].allEffects.firerateMultiplier) *
                       0.800000011920929);
            float sourceRangeScale = __instance.weapons[__instance.curWeapon].allEffects.audioSourceRangeScale;
            __instance.GetComponent<Scp939_VisionController>()
              .MakeNoise(Mathf.Clamp((float) (sourceRangeScale * (double) sourceRangeScale * 70.0), 5f, 100f));
            bool flag = target != null;
            if (targetPos == Vector3.zero)
            {
              RaycastHit raycastHit;
              if (Physics.Raycast(sourcePos, dir, out raycastHit, 500f, (int) __instance.raycastMask))
              {
                HitboxIdentity component = raycastHit.collider.GetComponent<HitboxIdentity>();
                if (component != null)
                {
                  WeaponManager componentInParent = component.GetComponentInParent<WeaponManager>();
                  if (componentInParent != null)
                  {
                    flag = false;
                    target = componentInParent.gameObject;
                    hitboxType = component.id;
                    targetPos = componentInParent.transform.position;
                  }
                }
              }
            }
            else
            {
              RaycastHit raycastHit;
              if (Physics.Linecast(sourcePos, targetPos, out raycastHit, (int) __instance.raycastMask))
              {
                HitboxIdentity component = raycastHit.collider.GetComponent<HitboxIdentity>();
                if (component != null)
                {
                  WeaponManager componentInParent = component.GetComponentInParent<WeaponManager>();
                  if (componentInParent != null)
                  {
                    if (componentInParent.gameObject == target)
                      flag = false;
                    else if (componentInParent.scp268.Enabled)
                    {
                      flag = false;
                      target = componentInParent.gameObject;
                      hitboxType = component.id;
                      targetPos = componentInParent.transform.position;
                    }
                  }
                }
              }
            }

            CharacterClassManager c = null;
            if (target != null)
              c = target.GetComponent<CharacterClassManager>();
            if (c != null && __instance.GetShootPermission(c, false))
            {
              if (Math.Abs(__instance.camera.transform.position.y - c.transform.position.y) > 40.0)
                __instance.GetComponent<CharacterClassManager>().TargetConsolePrint(__instance.connectionToClient,
                  "Shot rejected - Code 2.1 (too big Y-axis difference between source and target)", "gray");
              else if (Vector3.Distance(c.transform.position, targetPos) > 6.5)
                __instance.GetComponent<CharacterClassManager>().TargetConsolePrint(__instance.connectionToClient,
                  "Shot rejected - Code 2.3 (difference between real target position and provided target position is too big)",
                  "gray");
              else if (Physics.Linecast(__instance.camera.transform.position, sourcePos, __instance.raycastServerMask))
                __instance.GetComponent<CharacterClassManager>().TargetConsolePrint(__instance.connectionToClient,
                  "Shot rejected - Code 2.4 (collision between source positions detected)", "gray");
              else if (flag && Physics.Linecast(sourcePos, targetPos, __instance.raycastServerMask))
              {
                __instance.GetComponent<CharacterClassManager>().TargetConsolePrint(__instance.connectionToClient,
                  "Shot rejected - Code 2.5 (collision on shot line detected)", "gray");
              }
              else
              {
                float num1 = Vector3.Distance(__instance.camera.transform.position, target.transform.position);
                float num2 = __instance.weapons[__instance.curWeapon].damageOverDistance.Evaluate(num1);
                string upper = hitboxType.ToUpper();
                if (upper != "HEAD")
                {
                  if (upper != "LEG")
                  {
                    if (upper == "SCP106")
                      num2 /= 10f;
                  }
                  else
                    num2 /= 2f;
                }
                else
                  num2 *= 4f;

                bool allow = true;
                Events.InvokeOnShoot(__instance.hub, target, num2, num1, ref allow);
                if (!allow)
                  return false;

                __instance.hub.playerStats.HurtPlayer(
                  new PlayerStats.HitInfo(
                    num2 * __instance.weapons[__instance.curWeapon].allEffects.damageMultiplier *
                    __instance.overallDamagerFactor,
                    __instance.hub.nicknameSync.MyNick + " (" + __instance.hub.characterClassManager.UserId + ")",
                    DamageTypes.FromWeaponId(__instance.curWeapon), __instance.hub.queryProcessor.PlayerId),
                  c.gameObject);
                __instance.RpcConfirmShot(true, __instance.curWeapon);
                __instance.PlaceDecal(true, new Ray(__instance.camera.position, dir), (int) c.CurClass, num1);
              }
            }
            else if (target != null && hitboxType == "window" && target.GetComponent<BreakableWindow>() != null)
            {
              float damage = __instance.weapons[__instance.curWeapon].damageOverDistance
                .Evaluate(Vector3.Distance(__instance.camera.transform.position, target.transform.position));
              target.GetComponent<BreakableWindow>().ServerDamageWindow(damage);
              __instance.RpcConfirmShot(true, __instance.curWeapon);
            }
            else
            {
              __instance.PlaceDecal(false, new Ray(__instance.camera.position, dir), __instance.curWeapon, 0.0f);
              __instance.RpcConfirmShot(false, __instance.curWeapon);
            }
          }

          return false;
        }
        catch (Exception e)
        {
          Plugin.Error($"OnShoot: {e}");
          return true;
        }
      }

      public static void BreakWindow(WeaponManager __instance, float distance, BreakableWindow target)
		{
			if (distance > 40f) // Distance protection
			{
				__instance.GetComponent<CharacterClassManager>().TargetConsolePrint(__instance.connectionToClient, "Shot rejected", "red");
				return;
			}
			float damage = __instance.weapons[__instance.curWeapon].damageOverDistance.Evaluate(distance);
			target.GetComponent<BreakableWindow>().ServerDamageWindow(damage);
			__instance.RpcConfirmShot(true, __instance.curWeapon);
		}
    }
}
