using Harmony;
using Mirror;
using System;
using UnityEngine;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(Scp079PlayerScript), nameof(Scp079PlayerScript.TargetLevelChanged))]
	public class Scp079LvlGainEvent
	{
		public static bool Prefix(Scp079PlayerScript __instance, NetworkConnection conn, int newLvl)
		{
			try
			{
				bool allow = true;

				Events.InvokeScp079LvlGain(__instance.gameObject, __instance.curLvl, ref newLvl, ref allow);

				return allow;
			}
			catch (Exception exception)
			{
				Log.Error($"Scp079LvlGainEvent error: {exception}");
				return true;
			}
		}
	}

	[HarmonyPatch(typeof(Scp079PlayerScript), nameof(Scp079PlayerScript.CallRpcGainExp))]
	public class Scp079ExpGainEvent
	{
		public static bool Prefix(Scp079PlayerScript __instance, ExpGainType type, RoleType details)
		{
			if (EventPlugin.Scp079ExpGainEventDisable)
				return true;

			try
			{
				bool allow = true;
				float amount = 0f;

				switch (type)
				{
					case ExpGainType.KillAssist:
					case ExpGainType.PocketAssist:
						{
							Team team = __instance.GetComponent<CharacterClassManager>().Classes.SafeGet(details).team;
							int num = 6;

							switch (team)
							{
								case Team.SCP:
									amount = __instance.GetManaFromLabel("SCP Kill Assist", __instance.expEarnWays);
									num = 11;
									break;
								case Team.MTF:
									amount = __instance.GetManaFromLabel("MTF Kill Assist", __instance.expEarnWays);
									num = 9;
									break;
								case Team.CHI:
									amount = __instance.GetManaFromLabel("Chaos Kill Assist", __instance.expEarnWays);
									num = 8;
									break;
								case Team.RSC:
									amount = __instance.GetManaFromLabel("Scientist Kill Assist", __instance.expEarnWays);
									num = 10;
									break;
								case Team.CDP:
									amount = __instance.GetManaFromLabel("Class-D Kill Assist", __instance.expEarnWays);
									num = 7;
									break;
								default:
									amount = 0f;
									break;
							}

							num--;

							if (type == ExpGainType.PocketAssist)
							{
								amount /= 2f;
							}

							break;
						}
					case ExpGainType.DirectKill:
					case ExpGainType.HardwareHack:
						break;
					case ExpGainType.AdminCheat:
						amount = (float)details;
						break;
					case ExpGainType.GeneralInteractions:
						{
							switch (details)
							{
								case RoleType.ClassD:
									amount = __instance.GetManaFromLabel("Door Interaction", __instance.expEarnWays);
									break;
								case RoleType.Spectator:
									amount = __instance.GetManaFromLabel("Tesla Gate Activation", __instance.expEarnWays);
									break;
								case RoleType.Scientist:
									amount = __instance.GetManaFromLabel("Lockdown Activation", __instance.expEarnWays);
									break;
								case RoleType.Scp079:
									amount = __instance.GetManaFromLabel("Elevator Use", __instance.expEarnWays);
									break;
							}

							if (amount != 0f)
							{
								float num4 = 1f / Mathf.Clamp(__instance.levels[__instance.curLvl].manaPerSecond / 1.5f, 1f, 7f);

								amount = Mathf.Round(amount * num4 * 10f) / 10f;
							}

							break;
						}
					default:
						return false;
				}

				Events.InvokeScp079ExpGain(__instance.gameObject, type, ref allow, ref amount);

				if (allow && amount > 0)
				{
					__instance.AddExperience(amount);
					return false;
				}

				return false;
			}
			catch (Exception exception)
			{
				Log.Error($"Scp079ExpGainEvent error: {exception}");
				return true;
			}
		}
	}
}