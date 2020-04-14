using System.Collections.Generic;
using Harmony;
using Mirror;
using UnityEngine;
using System;

namespace EXILED.Patches
{
	public class ZabsNoclip
	{
		public static readonly RaycastHit[] AnticheatRaycastBuffer = new RaycastHit[1];
		public static Dictionary<PlyMovementSync, Vector3> LastSafePosition = new Dictionary<PlyMovementSync, Vector3>();
		public static Dictionary<PlyMovementSync, byte> ViolationsS = new Dictionary<PlyMovementSync, byte>();
		public static Dictionary<PlyMovementSync, byte> ViolationsL = new Dictionary<PlyMovementSync, byte>();
		public static Dictionary<PlyMovementSync, float> ResetS = new Dictionary<PlyMovementSync, float>();
		public static Dictionary<PlyMovementSync, float> ResetL = new Dictionary<PlyMovementSync, float>();
		public static Dictionary<PlyMovementSync, bool> PositionForced = new Dictionary<PlyMovementSync, bool>();

		public static bool CheckAnticheatSafe(Vector3 pos)
		{
			try
			{
				return Physics.RaycastNonAlloc(pos, Vector3.up, AnticheatRaycastBuffer, 1.7f,
					       FallDamage._staticGroundMask) == 0 && Physics.RaycastNonAlloc(pos, Vector3.down,
					       AnticheatRaycastBuffer, 1.7f, FallDamage._staticGroundMask) != 0;
			}
			catch (Exception e)
			{
				Log.Error(e.ToString());
				return false;
			}
		}

	}

	[HarmonyPatch(typeof(PlyMovementSync), nameof(PlyMovementSync.OverridePosition))]
	public class OverridePositionPatch
	{
		public static void Postfix(PlyMovementSync __instance, Vector3 pos, float rot, bool forceGround = false)
		{
			try
			{
				if (ZabsNoclip.LastSafePosition.ContainsKey(__instance))
					ZabsNoclip.LastSafePosition[__instance] = pos;
				else
					ZabsNoclip.LastSafePosition.Add(__instance, pos);

				if (ZabsNoclip.PositionForced.ContainsKey(__instance))
					ZabsNoclip.PositionForced[__instance] = true;
				else
					ZabsNoclip.PositionForced.Add(__instance, true);
			}
			catch (Exception e)
			{
				Log.Error(e.ToString());
			}
		}
	}

	[HarmonyPatch(typeof(PlyMovementSync), nameof(PlyMovementSync.Update))]
	public class UpdatePatch
	{
		public static bool Prefix(PlyMovementSync __instance)
		{
			try
			{
				if (!NetworkServer.active)
					return false;
				UpdateRealModelPatch.ServerUpdate(__instance, out bool wasChanged);
				AntiFlyPatch.AntiFly(__instance, ref __instance.RealModelPosition, wasChanged);

				if (__instance._safeTime > 0f)
					__instance._safeTime = Mathf.Clamp(__instance._safeTime - Time.deltaTime, 0, __instance._safeTime);
				else if (__instance.InSafeTime)
					__instance.InSafeTime = false;

				return false;
			}
			catch (Exception e)
			{
				Log.Error(e.ToString());
				return true;
			}
		}
	}

	[HarmonyPatch(typeof(PlyMovementSync), nameof(PlyMovementSync.ServerUpdateRealModel))]
	public class UpdateRealModelPatch
	{
		public static bool Prefix(PlyMovementSync __instance)
		{
			try
			{
				ServerUpdate(__instance, out bool wasChanged);
				return false;
			}
			catch (Exception e)
			{
				Log.Error(e.ToString());
				return true;
			}
		}
		
		public static void ServerUpdate(PlyMovementSync __instance, out bool wasChanged)
		{
			try
			{
				if (!ZabsNoclip.LastSafePosition.ContainsKey(__instance))
					ZabsNoclip.LastSafePosition.Add(__instance, __instance.gameObject.transform.position);
				if (!ZabsNoclip.ViolationsS.ContainsKey(__instance))
					ZabsNoclip.ViolationsS.Add(__instance, 0);
				if (!ZabsNoclip.ViolationsL.ContainsKey(__instance))
					ZabsNoclip.ViolationsL.Add(__instance, 0);
				if (!ZabsNoclip.PositionForced.ContainsKey(__instance))
					ZabsNoclip.PositionForced.Add(__instance, false);


				if (ZabsNoclip.PositionForced[__instance])
				{
					__instance._receivedPosition = ZabsNoclip.LastSafePosition[__instance];
					ZabsNoclip.PositionForced[__instance] = false;
					wasChanged = true;
					return;
				}

			
				wasChanged = false;
				if (!NetworkServer.active)
					return;
			
				if (__instance.WhitelistPlayer || __instance.NoclipWhitelisted)
				{
					__instance.RealModelPosition = __instance._receivedPosition;
					ZabsNoclip.LastSafePosition[__instance] = __instance._receivedPosition;
					return;
				}

				if (!__instance._successfullySpawned)
					return;
				float num1 = __instance._hub.characterClassManager.Classes.SafeGet(__instance._hub.characterClassManager.CurClass).runSpeed;
				if (__instance._sinkhole != null && __instance._sinkhole.Enabled)
					num1 *= (float) (1.0 - __instance._sinkhole.slowAmount / 100.0);
				if (__instance._receivedPosition.y > 1500.0)
				{
					if (__instance._hub.characterClassManager.CurClass != RoleType.Spectator)
					{
						__instance._receivedPosition = __instance.RealModelPosition;
						__instance.TargetForcePosition(__instance._hub.characterClassManager.connectionToClient, __instance.RealModelPosition);
						wasChanged = true;
					}
					else
						__instance.RealModelPosition = Vector3.up * 2048f;
				}
				else
				{
					if (__instance._hub.characterClassManager.CurClass == RoleType.Scp079)
					{
						__instance.RealModelPosition = Vector3.up * 2080f;
					}
					else
					{
						__instance._hub.falldamage.CalculateGround();
						if (!__instance._hub.falldamage.isGrounded)
							__instance.RealModelPosition.y = __instance._receivedPosition.y;
						Vector3 change = __instance._receivedPosition - __instance.RealModelPosition;
						__instance._debugDistance = change.magnitude;
						if (__instance._hub.characterClassManager.CurClass == RoleType.Scp173)
						{
							__instance.RealModelPosition = __instance._receivedPosition;
						}
						else
						{
							float num2;
							if (__instance._hub.animationController.sneaking)
							{
								float walkSpeed = __instance._hub.characterClassManager.Classes.SafeGet(__instance._hub.characterClassManager.CurClass).walkSpeed;
								num2 = walkSpeed * 0.4f * __instance.MaxLatency * __instance.Tolerance;
								num1 = 0.4f * walkSpeed;
								if (__instance._scp207.Enabled)
									num1 = 0.8f * walkSpeed;
							}
							else
							{
								num2 = num1 * __instance.MaxLatency * __instance.Tolerance;
								if (__instance._hub.characterClassManager.Scp096.iAm096)
									num1 = __instance._hub.characterClassManager.Scp096.ServerGetTopSpeed();
								if (__instance._scp207.Enabled)
									num1 *= 1.2f;
							}
							if (__instance._debugDistance > (double) num2)
							{
								if (__instance._safeTime > 0.0)
									return;
								ZabsNoclip.ViolationsL[__instance]++;
								ZabsNoclip.ViolationsS[__instance]++;
								__instance.TargetForcePosition(__instance.connectionToClient, __instance.RealModelPosition);
								wasChanged = true;
								return;
							}

							if (__instance._receivedPosition.y - ZabsNoclip.LastSafePosition[__instance].y > 5f)
							{
								__instance.RealModelPosition = ZabsNoclip.LastSafePosition[__instance];
								ZabsNoclip.ViolationsL[__instance]++;
								ZabsNoclip.ViolationsS[__instance]++;
								__instance.TargetForcePosition(__instance.connectionToClient, __instance.RealModelPosition);
								wasChanged = true;
								return;
							}
							
							Vector3 vector3_2 = num1 * __instance.Tolerance * Time.deltaTime * change.normalized;
							float diff = __instance.RealModelPosition.y - ZabsNoclip.LastSafePosition[__instance].y;
							float dist = Vector3.Distance(ZabsNoclip.LastSafePosition[__instance],
								__instance.RealModelPosition);
						
							int num3 =
								((dist < 10f && Math.Abs(diff) < 3f) || (diff < 0f && diff > -24f && dist < 24f))
									? Physics.RaycastNonAlloc(
										new Ray(__instance.RealModelPosition, __instance._receivedPosition - __instance.RealModelPosition),
										__instance._hits, (__instance._receivedPosition - __instance.RealModelPosition).magnitude,
										__instance.CollidableSurfaces)
									: Physics.RaycastNonAlloc(
										new Ray(ZabsNoclip.LastSafePosition[__instance], __instance._receivedPosition - ZabsNoclip.LastSafePosition[__instance]),
										__instance._hits, (__instance._receivedPosition - ZabsNoclip.LastSafePosition[__instance]).magnitude,
										__instance.CollidableSurfaces);
						
							for (int i = 0; i < num3; i++)
							{
								if (__instance._hub.characterClassManager.CurClass != RoleType.Scp106 && (__instance._hits[i].transform.gameObject.layer != 27 || __instance._hits[i].transform.GetComponentInParent<Door>().curCooldown <= 0f))
								{
									__instance.TargetForcePosition(__instance.connectionToClient, __instance.RealModelPosition);
									wasChanged = true;
									return;
								}
							}

							if (ZabsNoclip.CheckAnticheatSafe(__instance.RealModelPosition))
								ZabsNoclip.LastSafePosition[__instance] = __instance.RealModelPosition;
							
							if (__instance._debugDistance < (double) num2)
							{
								if (vector3_2.magnitude > (double) __instance._debugDistance)
								{
									__instance.RealModelPosition = __instance._receivedPosition;
									__instance._distanceTraveled += __instance._debugDistance;
								}
								else
								{
									__instance.RealModelPosition += vector3_2;
									__instance._distanceTraveled += vector3_2.magnitude;
								}
							}
						}
					}
					__instance._speedCounter += Time.deltaTime * 2f;
					if (__instance._speedCounter < 1.0)
						return;
					--__instance._speedCounter;
					__instance.AverageMovementSpeed = __instance._distanceTraveled * 2f;
					__instance._distanceTraveled = 0.0f;
				}
			}
			catch (Exception e)
			{
				Log.Error(e.ToString());
				wasChanged = false;
				
			}
		}
	}

	[HarmonyPatch(typeof(PlyMovementSync), nameof(PlyMovementSync.FixedUpdate))]
	public class FixedUpdatePatch
	{
		public static void Prefix(PlyMovementSync __instance)
		{
			try
			{
				if (!ZabsNoclip.ViolationsS.ContainsKey(__instance))
					ZabsNoclip.ViolationsS.Add(__instance, 0);
				if (!ZabsNoclip.ViolationsL.ContainsKey(__instance))
					ZabsNoclip.ViolationsL.Add(__instance, 0);
				if (!ZabsNoclip.ResetS.ContainsKey(__instance))
					ZabsNoclip.ResetS.Add(__instance, 0);
				if (!ZabsNoclip.ResetL.ContainsKey(__instance))
					ZabsNoclip.ResetL.Add(__instance, 0);
			
				byte violationsL = ZabsNoclip.ViolationsL[__instance];
				byte violationsS = ZabsNoclip.ViolationsL[__instance];
				float resetS = ZabsNoclip.ViolationsL[__instance];
				float resetL = ZabsNoclip.ViolationsL[__instance];
			
				if (violationsS > 0)
				{
					if (violationsS >= 5)
					{
						__instance.AntiCheatKillPlayer("Killed by anti-cheat system for multiple violations\n(debug code: 2A - anti-noclip violation limit exceeded");
						resetS = 0f;
						resetL = 0f;
						violationsS = 0;
						violationsL = 0;
					}
					else
					{
						resetS += Time.fixedDeltaTime;
						if (resetS > 3f)
						{
							resetS = 0f;
							violationsS = 0;
						}
					}
				}

				if (violationsL > 0)
				{
					if (violationsL >= 5)
					{
						__instance.AntiCheatKillPlayer("Killed by anti-cheat system for multiple violations\n(debug code: 2B - anti-noclip violation limit exceeded)");
						resetS = 0f;
						resetL = 0f;
						violationsS = 0;
						violationsL = 0;
					}
					else
					{
						resetL += Time.fixedDeltaTime;
						if (resetL > 25f)
						{
							resetL = 0f;
							violationsL = 0;
						}
					}
				}

				ZabsNoclip.ViolationsL[__instance] = violationsL;
				ZabsNoclip.ViolationsS[__instance] = violationsS;
				ZabsNoclip.ResetS[__instance] = resetS;
				ZabsNoclip.ResetL[__instance] = resetL;
			}
			catch (Exception e)
			{
				Log.Error(e.ToString());
			}
		}
	}
}