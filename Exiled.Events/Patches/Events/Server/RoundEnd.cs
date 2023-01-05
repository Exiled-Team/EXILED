// -----------------------------------------------------------------------
// <copyright file="RoundEnd.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Server
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.API.Enums;
    using Exiled.Events.EventArgs.Server;
    using Exiled.Events.Handlers;

    using GameCore;

    using HarmonyLib;

    using MEC;
    using PlayerRoles;
    using PluginAPI.Enums;
    using PluginAPI.Events;
    using RoundRestarting;

    using UnityEngine;

    using Console = GameCore.Console;

    /// <summary>
    ///     Patches <see cref="RoundSummary.Start" />.
    ///     Adds the <see cref="Server.EndingRound" /> and <see cref="Server.RoundEnded" /> event.
    /// </summary>
    [HarmonyPatch(typeof(RoundSummary), nameof(RoundSummary.Start))]
    internal static class RoundEnd
    {
        private static IEnumerator<float> Process(RoundSummary roundSummary)
        {
            float time = Time.unscaledTime;
            while (roundSummary != null)
            {
                yield return Timing.WaitForSeconds(2.5f);
                if (!RoundSummary.RoundLock)
                {
                    if (roundSummary.KeepRoundOnOne)
                    {
                        if (ReferenceHub.AllHubs.Count((ReferenceHub x) => x.characterClassManager.InstanceMode != ClientInstanceMode.DedicatedServer) < 2)
                        {
                            continue;
                        }
                    }

                    if (RoundSummary.RoundInProgress() && Time.unscaledTime - time >= 15f)
                    {
                        RoundSummary.SumInfo_ClassList newList = default(RoundSummary.SumInfo_ClassList);
                        foreach (ReferenceHub hub in ReferenceHub.AllHubs)
                        {
                            switch (hub.GetTeam())
                            {
                                case Team.SCPs:
                                    if (hub.GetRoleId() == RoleTypeId.Scp0492)
                                    {
                                        newList.zombies++;
                                    }
                                    else
                                    {
                                        newList.scps_except_zombies++;
                                    }

                                    break;
                                case Team.FoundationForces:
                                    newList.mtf_and_guards++;
                                    break;
                                case Team.ChaosInsurgency:
                                    newList.chaos_insurgents++;
                                    break;
                                case Team.Scientists:
                                    newList.scientists++;
                                    break;
                                case Team.ClassD:
                                    newList.class_ds++;
                                    break;
                            }
                        }

                        yield return float.NegativeInfinity;

                        newList.warhead_kills = AlphaWarheadController.Detonated ? AlphaWarheadController.Singleton.WarheadKills : -1;

                        yield return float.NegativeInfinity;

                        int num = newList.mtf_and_guards + newList.scientists;
                        int num2 = newList.chaos_insurgents + newList.class_ds;
                        int num3 = newList.scps_except_zombies + newList.zombies;
                        int num4 = newList.class_ds + RoundSummary.EscapedClassD;
                        int num5 = newList.scientists + RoundSummary.EscapedScientists;

                        RoundSummary.SurvivingSCPs = newList.scps_except_zombies;

                        float num6 = roundSummary.classlistStart.class_ds == 0 ? 0 : (num4 / roundSummary.classlistStart.class_ds);
                        float num7 = roundSummary.classlistStart.scientists == 0 ? 1 : (num5 / roundSummary.classlistStart.scientists);
                        if (newList.class_ds <= 0 && num <= 0)
                        {
                            roundSummary._roundEnded = true;
                        }
                        else
                        {
                            int num8 = 0;
                            if (num > 0)
                            {
                                num8++;
                            }

                            if (num2 > 0)
                            {
                                num8++;
                            }

                            if (num3 > 0)
                            {
                                num8++;
                            }

                            roundSummary._roundEnded = num8 <= 1;
                        }

                        EndingRoundEventArgs endingRoundEventArgs = new(LeadingTeam.Draw, newList, roundSummary._roundEnded);

                        bool flag = num > 0;
                        bool flag2 = num2 > 0;
                        bool flag3 = num3 > 0;

                        if (flag)
                            endingRoundEventArgs.LeadingTeam = RoundSummary.EscapedScientists >= RoundSummary.EscapedClassD ? LeadingTeam.FacilityForces : LeadingTeam.Draw;
                        else if (flag3 || (flag3 && flag2))
                            endingRoundEventArgs.LeadingTeam = RoundSummary.EscapedClassD > RoundSummary.SurvivingSCPs ? LeadingTeam.ChaosInsurgency : (RoundSummary.SurvivingSCPs > RoundSummary.EscapedScientists) ? LeadingTeam.Anomalies : LeadingTeam.Draw;
                        else if (flag2)
                            endingRoundEventArgs.LeadingTeam = RoundSummary.EscapedClassD >= RoundSummary.EscapedScientists ? LeadingTeam.ChaosInsurgency : LeadingTeam.Draw;

                        Server.OnEndingRound(endingRoundEventArgs);

                        roundSummary._roundEnded = endingRoundEventArgs.IsRoundEnded && endingRoundEventArgs.IsAllowed;

                        if (roundSummary._roundEnded)
                        {
                            EventManager.ExecuteEvent(ServerEventType.RoundEnd, Array.Empty<object>());

                            FriendlyFireConfig.PauseDetector = true;

                            string text = string.Concat(new object[]
                            {
                            "Round finished! Anomalies: ",
                            num3,
                            " | Chaos: ",
                            num2,
                            " | Facility Forces: ",
                            num,
                            " | D escaped percentage: ",
                            num6,
                            " | S escaped percentage: : ",
                            num7,
                            });

                            Console.AddLog(text, Color.gray, false, Console.ConsoleLogType.Log);

                            ServerLogs.AddLog(ServerLogs.Modules.Logger, text, ServerLogs.ServerLogType.GameEvent, false);

                            yield return Timing.WaitForSeconds(1.5f);

                            int timeToRoundRestart = Mathf.Clamp(ConfigFile.ServerConfig.GetInt("auto_round_restart_time", 10), 5, 1000);

                            if (roundSummary != null)
                            {
                                RoundEndedEventArgs roundEndedEventArgs = new(endingRoundEventArgs.LeadingTeam, newList, timeToRoundRestart);

                                Server.OnRoundEnded(roundEndedEventArgs);

                                roundSummary.RpcShowRoundSummary(roundSummary.classlistStart, newList, (RoundSummary.LeadingTeam)roundEndedEventArgs.LeadingTeam, RoundSummary.EscapedClassD, RoundSummary.EscapedScientists, RoundSummary.KilledBySCPs, timeToRoundRestart, (int)RoundStart.RoundLength.TotalSeconds);
                            }

                            yield return Timing.WaitForSeconds(timeToRoundRestart - 1);

                            roundSummary.RpcDimScreen();

                            yield return Timing.WaitForSeconds(1f);

                            RoundRestart.InitiateRoundRestart();
                        }
                    }
                }
            }
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Call)
                {
                    if (instruction.operand is MethodBase methodBase
                        && (methodBase.Name != nameof(RoundSummary._ProcessServerSideCode)))
                    {
                        yield return instruction;
                    }
                    else
                    {
                        yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(RoundEnd), nameof(Process)));
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(
                            OpCodes.Call,
                            AccessTools.FirstMethod(
                                typeof(MECExtensionMethods2),
                                m =>
                                {
                                    Type[] generics = m.GetGenericArguments();
                                    ParameterInfo[] paramseters = m.GetParameters();
                                    return (m.Name == "CancelWith")
                                           && (generics.Length == 1)
                                           && (paramseters.Length == 2)
                                           && (paramseters[0].ParameterType == typeof(IEnumerator<float>))
                                           && (paramseters[1].ParameterType == generics[0]);
                                }).MakeGenericMethod(typeof(RoundSummary)));
                    }
                }
                else
                {
                    yield return instruction;
                }
            }
        }
    }
}