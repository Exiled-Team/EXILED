using CustomPlayerEffects;
using Harmony;
using RemoteAdmin;
using System;
using UnityEngine;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(Scp106PlayerScript), nameof(Scp106PlayerScript.CallCmdMovePlayer))]
	public class PocketDimensionEvents
	{
		public static bool Prefix(Scp106PlayerScript __instance, GameObject ply, int t)
		{
			if (EventPlugin.Scp106PocketDimensionDamageEventPatchDisable)
				return true;

			try
			{
				if (!__instance._iawRateLimit.CanExecute(true))
					return false;
				if (ply == null)
					return false;

				CharacterClassManager component = ply.GetComponent<CharacterClassManager>();
				if (component == null)
					return false;
				if (!ServerTime.CheckSynchronization(t) || !__instance.iAm106 ||
					Vector3.Distance(__instance.GetComponent<PlyMovementSync>().RealModelPosition,
						ply.transform.position) >= 3f || !component.IsHuman())
					return false;

				CharacterClassManager component2 = ply.GetComponent<CharacterClassManager>();
				if (component2.GodMode)
					return false;
				if (component2.Classes.SafeGet(component2.CurClass).team == Team.SCP)
					return false;

				__instance.GetComponent<CharacterClassManager>().RpcPlaceBlood(ply.transform.position, 1, 2f);
				if (Scp106PlayerScript.blastDoor.isClosed)
				{
					__instance.GetComponent<CharacterClassManager>().RpcPlaceBlood(ply.transform.position, 1, 2f);
					__instance.GetComponent<PlayerStats>().HurtPlayer(
						new PlayerStats.HitInfo(500f,
							__instance.GetComponent<NicknameSync>().MyNick + " (" +
							__instance.GetComponent<CharacterClassManager>().UserId + ")", DamageTypes.Scp106,
							__instance.GetComponent<QueryProcessor>().PlayerId), ply);
				}
				else
				{
					CharacterClassManager component3 = ply.GetComponent<CharacterClassManager>();
					// 079 shit
					foreach (Scp079PlayerScript scp079PlayerScript in Scp079PlayerScript.instances)
					{
						Scp079Interactable.ZoneAndRoom
							otherRoom = ply.GetComponent<Scp079PlayerScript>().GetOtherRoom();
						Scp079Interactable.InteractableType[] filter = new Scp079Interactable.InteractableType[]
						{
							Scp079Interactable.InteractableType.Door, Scp079Interactable.InteractableType.Light,
							Scp079Interactable.InteractableType.Lockdown, Scp079Interactable.InteractableType.Tesla,
							Scp079Interactable.InteractableType.ElevatorUse
						};
						bool flag = false;
						foreach (Scp079Interaction scp079Interaction in scp079PlayerScript.ReturnRecentHistory(12f,
							filter))
						{
							foreach (Scp079Interactable.ZoneAndRoom zoneAndRoom in scp079Interaction.interactable
								.currentZonesAndRooms)
							{
								if (zoneAndRoom.currentZone == otherRoom.currentZone &&
									zoneAndRoom.currentRoom == otherRoom.currentRoom)
								{
									flag = true;
								}
							}
						}

						if (flag)
						{
							scp079PlayerScript.RpcGainExp(ExpGainType.PocketAssist, component3.CurClass);
						}
					}

					// Invoke enter

					bool allowEnter = true;

					Events.InvokePocketDimEnter(ply, ref allowEnter);

					if (!allowEnter)
						return false;

					ply.GetComponent<PlyMovementSync>().OverridePosition(Vector3.down * 1998.5f, 0f, true);

					// Invoke damage.

					bool allowDamage = true;


					Events.InvokePocketDimDamage(ply, ref allowDamage);

					if (allowDamage)
						__instance.GetComponent<PlayerStats>().HurtPlayer(
							new PlayerStats.HitInfo(40f,
								__instance.GetComponent<NicknameSync>().MyNick + " (" +
								__instance.GetComponent<CharacterClassManager>().UserId + ")", DamageTypes.Scp106,
								__instance.GetComponent<QueryProcessor>().PlayerId), ply);

				}

				PlayerEffectsController componentInParent = ply.GetComponentInParent<PlayerEffectsController>();
				componentInParent.GetEffect<Corroding>("Corroding").isInPd = true;
				componentInParent.EnableEffect("Corroding");

				return false;
			}
			catch (Exception exception)
			{
				Log.Error($"PocketDimDamageEvent error: {exception}");
				return true;
			}
		}
	}
}
