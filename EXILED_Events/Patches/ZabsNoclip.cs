using System;
using Harmony;
using Mirror;
using UnityEngine;

namespace EXILED.Patches
{
	//[HarmonyPatch(typeof(PlyMovementSync), nameof(PlyMovementSync.FixedUpdate))]  - Not currently necessary to be overwritten, only use for debugging purposes when necessary.
	public class lateUpdate
	{
		public static bool Prefix(PlyMovementSync __instance)
		{
			if (NetworkServer.active)
			{
				if (__instance._violationsS > 0)
				{
					if (__instance._violationsS >= 20)
					{
						__instance.AntiCheatKillPlayer(
							"Killed by the anti-cheat system for multiple violations\n(debug code: 2A - anti-noclip violations limit exceeded)");
						__instance._resetS = 0.0f;
						__instance._resetL = 0.0f;
						__instance._violationsS = 0;
						__instance._violationsL = 0;
					}
					else
					{
						__instance._resetS += Time.fixedUnscaledDeltaTime;
						if (__instance._resetS > 2.0)
						{
							__instance._resetS = 0.0f;
							__instance._violationsS = 0;
						}
					}
				}

				if (__instance._violationsL > 0)
				{
					if (__instance._violationsL >= 80)
					{
						__instance.AntiCheatKillPlayer(
							"Killed by the anti-cheat system for multiple violations\n(debug code: 2B - anti-noclip violations limit exceeded)");
						__instance._resetS = 0.0f;
						__instance._resetL = 0.0f;
						__instance._violationsS = 0;
						__instance._violationsL = 0;
					}
					else
					{
						__instance._resetL += Time.fixedUnscaledDeltaTime;
						if (__instance._resetL > 20.0)
						{
							__instance._resetL = 0.0f;
							__instance._violationsL = 0;
						}
					}
				}
			}

			return false;
		}

		[HarmonyPatch(typeof(PlyMovementSync), nameof(PlyMovementSync.ServerUpdateRealModel))]
		public class ZabsNoclip
		{
			public static bool Prefix(PlyMovementSync __instance, out bool wasChanged)
			{
				try
				{
					if (!NetworkServer.active) wasChanged = false;
					else
					{
						if (__instance._positionForced)
						{
							if (Vector3.Distance(__instance._receivedPosition, __instance._lastSafePosition) >= 4.0 &&
							    __instance._forcePositionTime <= 10.0)
							{
								__instance._receivedPosition = __instance._lastSafePosition;
								__instance._forcePositionTime += Time.unscaledDeltaTime;
								wasChanged = true;
								return false;
							}

							__instance._positionForced = false;
							__instance._forcePositionTime = 0.0f;
						}

						wasChanged = false;
						if (__instance.WhitelistPlayer || __instance.NoclipWhitelisted)
						{
							__instance.RealModelPosition = __instance._receivedPosition;
							__instance._lastSafePosition = __instance._receivedPosition;
						}
						else
						{
							if (!__instance._successfullySpawned) return false;
							float num1 = __instance._hub.characterClassManager.Classes
								.SafeGet(__instance._hub.characterClassManager.CurClass).runSpeed;
							if (__instance._sinkhole != null && __instance._sinkhole.Enabled)
								num1 *= (float) (1.0 - __instance._sinkhole.slowAmount / 100.0);
							if (__instance._receivedPosition.y > 1500.0)
							{
								if (__instance._hub.characterClassManager.CurClass != RoleType.Spectator)
								{
									__instance._receivedPosition = __instance.RealModelPosition;
									wasChanged = true;
									__instance.TargetForcePosition(
										__instance._hub.characterClassManager.connectionToClient,
										__instance.RealModelPosition);
								}
								else __instance.RealModelPosition = Vector3.up * 2048f;
							}
							else
							{
								if (__instance._hub.characterClassManager.CurClass == RoleType.Scp079)
									__instance.RealModelPosition = Vector3.up * 2080f;
								else
								{
									__instance._hub.falldamage.CalculateGround();
									__instance.TempAjustedPos = __instance.RealModelPosition;
									if (!__instance._hub.falldamage.isGrounded)
										__instance.RealModelPosition.y = __instance._receivedPosition.y;
									Vector3 vector3_1 = __instance._receivedPosition - __instance.RealModelPosition;
									__instance._debugDistance = vector3_1.magnitude;
									if (__instance._hub.characterClassManager.CurClass == RoleType.Scp173)
										__instance.RealModelPosition = __instance._receivedPosition;
									else
									{
										float num2;
										if (__instance._hub.animationController.sneaking)
										{
											float walkSpeed = __instance._hub.characterClassManager.Classes
												.SafeGet(__instance._hub.characterClassManager.CurClass).walkSpeed;
											num2 = walkSpeed * 0.4f * __instance.MaxLatency * __instance.Tolerance;
											num1 = 0.4f * walkSpeed;
											if (__instance._scp207.Enabled) num1 = 0.8f * walkSpeed;
										}
										else
										{
											num2 = num1 * __instance.MaxLatency * __instance.Tolerance;
											if (__instance._hub.characterClassManager.Scp096.iAm096)
												num1 = __instance._hub.characterClassManager.Scp096.ServerGetTopSpeed();
											if (__instance._scp207.Enabled) num1 *= 1.2f;
										}

										if (__instance._debugDistance > __instance.MaxLatency * (double) (num1 * 5.2f))
										{
											if (__instance._safeTime > 0.0) return false;
											//__instance.TargetForcePosition(__instance.connectionToClient, __instance.RealModelPosition);
											wasChanged = true;
											return false;
										}

										if (__instance._receivedPosition.y - (double) __instance._lastSafePosition.y >
										    5.0 && __instance._hub.characterClassManager.CurClass != RoleType.Spectator)
										{
											__instance.RealModelPosition = __instance._lastSafePosition;
											++__instance._violationsS;
											++__instance._violationsL;
											__instance.TargetForcePosition(__instance.connectionToClient,
												__instance._lastSafePosition);
											wasChanged = true;
											return false;
										}

										Vector3 vector3_2 =
											num1 * __instance.Tolerance * Time.deltaTime * vector3_1.normalized;
											
										/*float num3 = __instance.RealModelPosition.y - __instance._lastSafePosition.y;
										float num4 = Vector3.Distance(__instance._lastSafePosition,
											__instance.RealModelPosition);
										int num5 =
											(double) num4 < 10.0 && (double) Math.Abs(num3) < 3.0 ||
											(double) num3 < 0.0 && (double) num3 > -30.0 && (double) num4 < 30.0
												? Physics.RaycastNonAlloc(
													new Ray(__instance.RealModelPosition + Vector3.up * 1f,
														__instance._receivedPosition - __instance.RealModelPosition),
													__instance._hits,
													(__instance._receivedPosition - __instance.RealModelPosition)
													.magnitude, __instance.CollidableSurfaces)
												: Physics.RaycastNonAlloc(
													new Ray(__instance._lastSafePosition + Vector3.up * 1f,
														__instance._receivedPosition - __instance._lastSafePosition),
													__instance._hits,
													(__instance._receivedPosition - __instance._lastSafePosition)
													.magnitude, __instance.CollidableSurfaces);
										for (int index = 0; index < num5; ++index)
											if (__instance._hub.characterClassManager.CurClass != RoleType.Scp106 &&
											    __instance._hub.characterClassManager.CurClass != RoleType.Spectator &&
											    (__instance._hits[index].collider.gameObject.layer != 27 ||
											     __instance._hits[index].collider.GetComponentInParent<Door>()
												     .curCooldown <= 0.0))
											{
												__instance.RealModelPosition = __instance._lastSafePosition;
												//++__instance._violationsS;
												++__instance._violationsL;
												__instance.TargetForcePosition(__instance.connectionToClient, __instance._lastSafePosition);
												wasChanged = true;
												return false;
											}*/

										__instance.RealModelPosition = __instance.TempAjustedPos;
										if (FallDamage.CheckAnticheatSafe(__instance.RealModelPosition))
											__instance._lastSafePosition = __instance.RealModelPosition;
										if (__instance._debugDistance < __instance.MaxLatency * (double) num2)
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
								if (__instance._speedCounter < 1.0) return false;
								--__instance._speedCounter;
								__instance.AverageMovementSpeed = __instance._distanceTraveled * 2f;
								__instance._distanceTraveled = 0.0f;
							}
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
		}
	}
}