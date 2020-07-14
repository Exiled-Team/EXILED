// -----------------------------------------------------------------------
// <copyright file="GainingExperience.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp079
{
#pragma warning disable SA1313
    using System;
    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;
    using HarmonyLib;
    using UnityEngine;

    /// <summary>
    /// Patches <see cref="Scp079PlayerScript.CallRpcGainExp(ExpGainType, RoleType)"/>.
    /// Adds the <see cref="Scp079.GainingExperience"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp079PlayerScript), nameof(Scp079PlayerScript.CallRpcGainExp))]
    internal class GainingExperience
    {
        private static bool Prefix(Scp079PlayerScript __instance, ExpGainType type, RoleType details)
        {
            try
            {
                var ev = new GainingExperienceEventArgs(API.Features.Player.Get(__instance.gameObject), type, (float)details);

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
                                ev.Amount = __instance.GetManaFromLabel("SCP Kill Assist", __instance.expEarnWays);
                                num = 11;
                                break;
                            case Team.MTF:
                                ev.Amount = __instance.GetManaFromLabel("MTF Kill Assist", __instance.expEarnWays);
                                num = 9;
                                break;
                            case Team.CHI:
                                ev.Amount = __instance.GetManaFromLabel("Chaos Kill Assist", __instance.expEarnWays);
                                num = 8;
                                break;
                            case Team.RSC:
                                ev.Amount = __instance.GetManaFromLabel("Scientist Kill Assist", __instance.expEarnWays);
                                num = 10;
                                break;
                            case Team.CDP:
                                ev.Amount = __instance.GetManaFromLabel("Class-D Kill Assist", __instance.expEarnWays);
                                num = 7;
                                break;
                            default:
                                ev.Amount = 0f;
                                break;
                        }

                        num--;

                        if (type == ExpGainType.PocketAssist)
                        {
                            ev.Amount /= 2f;
                        }

                        break;
                    }

                    case ExpGainType.DirectKill:
                    case ExpGainType.HardwareHack:
                        break;
                    case ExpGainType.AdminCheat:
                        ev.Amount = (float)details;
                        break;
                    case ExpGainType.GeneralInteractions:
                    {
                        switch (details)
                        {
                            case RoleType.ClassD:
                                ev.Amount = __instance.GetManaFromLabel("Door Interaction", __instance.expEarnWays);
                                break;
                            case RoleType.Spectator:
                                ev.Amount = __instance.GetManaFromLabel("Tesla Gate Activation", __instance.expEarnWays);
                                break;
                            case RoleType.Scientist:
                                ev.Amount = __instance.GetManaFromLabel("Lockdown Activation", __instance.expEarnWays);
                                break;
                            case RoleType.Scp079:
                                ev.Amount = __instance.GetManaFromLabel("Elevator Use", __instance.expEarnWays);
                                break;
                        }

                        if (ev.Amount != 0f)
                        {
                            float num4 = 1f / Mathf.Clamp(__instance.levels[__instance.curLvl].manaPerSecond / 1.5f, 1f, 7f);

                            ev.Amount = Mathf.Round(ev.Amount * num4 * 10f) / 10f;
                        }

                        break;
                    }

                    default:
                        return false;
                }

                Scp079.OnGainingExperience(ev);

                if (ev.IsAllowed && ev.Amount > 0)
                {
                    __instance.AddExperience(ev.Amount);
                    return false;
                }

                return false;
            }
            catch (Exception e)
            {
                Exiled.API.Features.Log.Error($"Exiled.Events.Patches.Events.Scp079.GainingExperience: {e}\n{e.StackTrace}");

                return true;
            }
        }
    }
}
