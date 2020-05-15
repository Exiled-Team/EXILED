using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(DamageTypes), nameof(DamageTypes.ToIndex))]
	public class DamageTypesPatchToIndex
	{
		public static bool Prefix(ref int __result, DamageTypes.DamageType damageType)
		{
			if (EventPlugin.DamageTypesPatchDisable) return true;
			for (int i = 0; i < EventPlugin.damageTypes.Count; i++)
			{
				if (EventPlugin.damageTypes[i] == damageType)
				{
					__result = i;
					return false;
				}
			}
			__result = 0;
			return false;
		}
	}

	[HarmonyPatch(typeof(DamageTypes), nameof(DamageTypes.FromIndex))]
	public class DamageTypesPatchFromIndex
	{
		public static bool Prefix(ref DamageTypes.DamageType __result, int id)
		{
			if (EventPlugin.DamageTypesPatchDisable) return true;

			if (id > 0 && id < EventPlugin.damageTypes.Count)
			{
				__result = EventPlugin.damageTypes[id];
				return false;
			}
			__result = DamageTypes.None;
			return false;
		}
	}

	[HarmonyPatch(typeof(DamageTypes), nameof(DamageTypes.FromWeaponId))]
	public class DamageTypesPatchFromWeaponId
	{
		public static bool Prefix(ref DamageTypes.DamageType __result, int weaponId)
		{
			if (EventPlugin.DamageTypesPatchDisable) return true;

			for (int i = 0; i < EventPlugin.damageTypes.Count; i++)
			{
				if (EventPlugin.damageTypes[i].isWeapon && EventPlugin.damageTypes[i].weaponId == weaponId)
				{
					__result = EventPlugin.damageTypes[i];
					return false;
				}
			}
			__result = DamageTypes.None;
			return false;
		}
	}

	[HarmonyPatch(typeof(RagdollManager), nameof(RagdollManager.GetCause))]
	public class RagdollManagerPatch
	{
		public static bool Prefix(ref string __result, PlayerStats.HitInfo info, bool ragdoll)
		{
			if (EventPlugin.RagdollManagerGetCausePatchDisable) return true;
			if (info.GetDamageType() is DamageType damageType)
			{
				__result = damageType.cause;
				return false;
			}
			return true;
		}
	}
}
