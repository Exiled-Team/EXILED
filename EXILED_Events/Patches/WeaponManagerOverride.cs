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
        public static void Prefix(WeaponManager __instance, GameObject target, string hitboxType, Vector3 dir, Vector3 sourcePos, Vector3 targetPos)
        {
            // Nw spahgetti code
            if (__instance._iawRateLimit.CanExecute(true))
                return;
            ReferenceHub hub = __instance.hub;
            int itemIndex = __instance.hub.inventory.GetItemIndex();
            if (itemIndex < 0 || itemIndex >= __instance.hub.inventory.items.Count)
                return;
            if (__instance.curWeapon < 0 || ((__instance.reloadCooldown > 0f || __instance.fireCooldown > 0f) && !__instance.isLocalPlayer || hub.inventory.curItem != __instance.weapons[__instance.curWeapon].inventoryID || hub.inventory.items[itemIndex].durability <= 0f))
                return;
            if (Vector3.Distance(__instance.camera.transform.position, sourcePos) > 6.5f)
            {
                __instance.GetComponent<CharacterClassManager>().TargetConsolePrint(__instance.connectionToClient, "Shot rejected", "red");
                return;
            }
            hub.inventory.items.ModifyDuration(itemIndex, hub.inventory.items[itemIndex].durability - 1f);
            __instance.scp268.ServerDisable();
            __instance.fireCooldown = 1f / (__instance.weapons[__instance.curWeapon].shotsPerSecond * __instance.weapons[__instance.curWeapon].allEffects.firerateMultiplier) * 0.8f;
            float num = __instance.weapons[__instance.curWeapon].allEffects.audioSourceRangeScale;
            num = num * num * 70f;
            __instance.GetComponent<Scp939_VisionController>().MakeNoise(Mathf.Clamp(num, 5f, 100f));
            bool _f = target != null;
            RaycastHit raycastHit2;
            if (targetPos == Vector3.zero)
            {
                RaycastHit raycastHit;
                if (Physics.Raycast(sourcePos, dir, out raycastHit, 500f, __instance.raycastMask))
                {
                    HitboxIdentity component = raycastHit.collider.GetComponent<HitboxIdentity>();
                    if (component != null)
                    {
                        WeaponManager componentInParent = component.GetComponentInParent<WeaponManager>();
                        if (componentInParent != null)
                        {
                            _f = false;
                            target = componentInParent.gameObject;
                            hitboxType = component.id;
                            targetPos = componentInParent.transform.position;
                        }
                    }
                }
            }
            else if (Physics.Linecast(sourcePos, targetPos, out raycastHit2, __instance.raycastMask))
            {
                HitboxIdentity component2 = raycastHit2.collider.GetComponent<HitboxIdentity>();
                if (component2 != null)
                {
                    WeaponManager componentInParent2 = component2.GetComponentInParent<WeaponManager>();
                    if (componentInParent2 != null)
                    {
                        if (componentInParent2.gameObject == target)
                            _f = false;
                        else if (componentInParent2.scp268.Enabled)
                        {
                            _f = false;
                            target = componentInParent2.gameObject;
                            hitboxType = component2.id;
                            targetPos = componentInParent2.transform.position;
                        }
                    }
                }
            }
			CharacterClassManager characterClassManager = null;
			if (target != null)
				characterClassManager = target.GetComponent<CharacterClassManager>();
			if (characterClassManager != null && __instance.GetShootPermission(characterClassManager, false))
			{
				if (Math.Abs(__instance.camera.transform.position.y - characterClassManager.transform.position.y) > 40f)
				{
					__instance.GetComponent<CharacterClassManager>().TargetConsolePrint(__instance.connectionToClient, "Shot rejected", "red");
					return;
				}
				if (Vector3.Distance(characterClassManager.transform.position, targetPos) > 6.5f)
				{
					__instance.GetComponent<CharacterClassManager>().TargetConsolePrint(__instance.connectionToClient, "Shot rejected", "red");
					return;
				}
				if (Physics.Linecast(__instance.camera.transform.position, sourcePos, __instance.raycastServerMask))
				{
					__instance.GetComponent<CharacterClassManager>().TargetConsolePrint(__instance.connectionToClient, "Shot rejected", "red");
					return;
				}
				if (_f && Physics.Linecast(sourcePos, targetPos, __instance.raycastServerMask))
				{
					__instance.GetComponent<CharacterClassManager>().TargetConsolePrint(__instance.connectionToClient, "Shot rejected", "red");
					return;
				}
				if (characterClassManager.gameObject == __instance.gameObject)
				{
					__instance.GetComponent<CharacterClassManager>().TargetConsolePrint(__instance.connectionToClient, "Shot rejected", "red");
					return;
				}
				float num2 = Vector3.Distance(__instance.camera.transform.position, target.transform.position);
				float num3 = __instance.weapons[__instance.curWeapon].damageOverDistance.Evaluate(num2);
				string a = hitboxType.ToUpper();
				switch(a)
				{
					case "SCP106":
						num3 /= 10f;
						break;
					case "LEG":
						num3 /= 2f;
						break;
					default:
						num3 *= 4f;
						break;
				}
				num3 *= __instance.weapons[__instance.curWeapon].allEffects.damageMultiplier;
				num3 *= __instance.overallDamagerFactor;
				bool Allow = true;

				Events.InvokeOnShoot(hub, target, num3, num2, ref Allow);

				if (Allow)
				{
					__instance.hub.playerStats.HurtPlayer(new PlayerStats.HitInfo(num3, __instance.hub.nicknameSync.MyNick + " (" + __instance.hub.characterClassManager.UserId + ")", DamageTypes.FromWeaponId(__instance.curWeapon), __instance.hub.queryProcessor.PlayerId), characterClassManager.gameObject);
					__instance.RpcConfirmShot(true, __instance.curWeapon);
					__instance.PlaceDecal(true, new Ray(__instance.camera.position, dir), (int)characterClassManager.CurClass, num2);
				}
				return;
			}
			else
			{
				// Game object stuff
				bool Allow = true;

				Events.InvokeOnShoot(hub, target, 0, Vector3.Distance(__instance.camera.transform.position, target.transform.position), ref Allow);

				if (Allow)
				{
					if (target != null && hitboxType == "window" && target.GetComponent<BreakableWindow>() != null)
					{
						BreakWindow(__instance, Vector3.Distance(__instance.camera.transform.position, target.transform.position), target.GetComponent<BreakableWindow>());
						return;
					}
					__instance.PlaceDecal(false, new Ray(__instance.camera.position, dir), __instance.curWeapon, 0f);
					__instance.RpcConfirmShot(false, __instance.curWeapon);
					return;
				}
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
