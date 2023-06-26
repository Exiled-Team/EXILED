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

    using Exiled.Events.EventArgs.Server;
    using GameCore;
    using HarmonyLib;
    using MEC;
    using PlayerRoles;
    using PluginAPI.Core;
    using PluginAPI.Enums;
    using PluginAPI.Events;
    using RoundRestarting;
    using UnityEngine;

    /// <summary>
    ///     Patches <see cref="RoundSummary.Start" />.
    ///     Adds the <see cref="Handlers.Server.EndingRound" /> and <see cref="Handlers.Server.RoundEnded" /> event.
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
                    if (roundSummary.KeepRoundOnOne && ReferenceHub.AllHubs.Count((ReferenceHub x) => x.characterClassManager.InstanceMode is not ClientInstanceMode.DedicatedServer) < 2)
                        continue;

                    if (RoundSummary.RoundInProgress() && Time.unscaledTime - time >= 15f)
                    {
                        RoundSummary.SumInfo_ClassList newList = default;
                        foreach (ReferenceHub referenceHub in ReferenceHub.AllHubs)
                        {
                            switch (referenceHub.GetTeam())
                            {
                                case Team.SCPs:
                                    if (referenceHub.GetRoleId() == RoleTypeId.Scp0492)
                                        newList.zombies++;
                                    else
                                        newList.scps_except_zombies++;
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
                        newList.warhead_kills = AlphaWarheadController.Detonated ? AlphaWarheadController.Singleton.WarheadKills : (-1);
                        yield return float.NegativeInfinity;
                        int facilityForces = newList.mtf_and_guards + newList.scientists;
                        int chaosInsurgency = newList.chaos_insurgents + newList.class_ds;
                        int anomalies = newList.scps_except_zombies + newList.zombies;
                        int num = newList.class_ds + RoundSummary.EscapedClassD;
                        int num2 = newList.scientists + RoundSummary.EscapedScientists;
                        RoundSummary.SurvivingSCPs = newList.scps_except_zombies;
                        float dEscapePercentage = (roundSummary.classlistStart.class_ds == 0) ? 0 : (num / roundSummary.classlistStart.class_ds);
                        float sEscapePercentage = (roundSummary.classlistStart.scientists == 0) ? 1 : (num2 / roundSummary.classlistStart.scientists);
                        bool shouldRoundEnd;
                        if (newList.class_ds <= 0 && facilityForces <= 0)
                        {
                            shouldRoundEnd = true;
                        }
                        else
                        {
                            int num3 = 0;
                            if (facilityForces > 0)
                            {
                                num3++;
                            }

                            if (chaosInsurgency > 0)
                            {
                                num3++;
                            }

                            if (anomalies > 0)
                            {
                                num3++;
                            }

                            shouldRoundEnd = num3 <= 1;
                        }

                        if (!roundSummary._roundEnded)
                        {
                            RoundEndConditionsCheckCancellationData.RoundEndConditionsCheckCancellation cancellation = EventManager.ExecuteEvent<RoundEndConditionsCheckCancellationData>(new RoundEndConditionsCheckEvent(shouldRoundEnd)).Cancellation;
                            int num4 = (int)cancellation;
                            if (num4 != 1)
                            {
                                if (num4 == 2)
                                {
                                    if (!roundSummary._roundEnded)
                                    {
                                        continue;
                                    }
                                }
                            }
                            else
                            {
                                shouldRoundEnd = true;
                            }
                        }

                        bool flag2 = facilityForces > 0;
                        bool flag3 = chaosInsurgency > 0;
                        bool flag4 = anomalies > 0;
                        RoundSummary.LeadingTeam leadingTeam = RoundSummary.LeadingTeam.Draw;
                        if (flag2)
                        {
                            leadingTeam = (RoundSummary.EscapedScientists >= RoundSummary.EscapedClassD) ? RoundSummary.LeadingTeam.FacilityForces : RoundSummary.LeadingTeam.Draw;
                        }
                        else if (flag4 || (flag4 && flag3))
                        {
                            leadingTeam = (RoundSummary.EscapedClassD > RoundSummary.SurvivingSCPs) ? RoundSummary.LeadingTeam.ChaosInsurgency : ((RoundSummary.SurvivingSCPs > RoundSummary.EscapedScientists) ? RoundSummary.LeadingTeam.Anomalies : RoundSummary.LeadingTeam.Draw);
                        }
                        else if (flag3)
                        {
                            leadingTeam = (RoundSummary.EscapedClassD >= RoundSummary.EscapedScientists) ? RoundSummary.LeadingTeam.ChaosInsurgency : RoundSummary.LeadingTeam.Draw;
                        }

                        EndingRoundEventArgs endingRoundEventArgs = new(leadingTeam, newList, shouldRoundEnd);
                        Handlers.Server.OnEndingRound(endingRoundEventArgs);
                        leadingTeam = (RoundSummary.LeadingTeam)endingRoundEventArgs.LeadingTeam;
                        if (endingRoundEventArgs.IsRoundEnded)
                        {
                            roundSummary._roundEnded = true;
                            RoundEndCancellationData roundEndCancellationData = EventManager.ExecuteEvent<RoundEndCancellationData>(new RoundEndEvent(leadingTeam));
                            while (roundEndCancellationData.IsCancelled)
                            {
                                if (roundEndCancellationData.Delay <= 0f)
                                    break;

                                yield return Timing.WaitForSeconds(roundEndCancellationData.Delay);
                                roundEndCancellationData = EventManager.ExecuteEvent<RoundEndCancellationData>(new RoundEndEvent(leadingTeam));
                            }

                            if (Statistics.FastestEndedRound.Duration > RoundStart.RoundLength)
                            {
                                Statistics.FastestEndedRound = new Statistics.FastestRound(leadingTeam, RoundStart.RoundLength, DateTime.Now);
                            }

                            Statistics.CurrentRound.ClassDAlive = newList.class_ds;
                            Statistics.CurrentRound.ScientistsAlive = newList.scientists;
                            Statistics.CurrentRound.MtfAndGuardsAlive = newList.mtf_and_guards;
                            Statistics.CurrentRound.ChaosInsurgencyAlive = newList.chaos_insurgents;
                            Statistics.CurrentRound.ZombiesAlive = newList.zombies;
                            Statistics.CurrentRound.ScpsAlive = newList.scps_except_zombies;
                            Statistics.CurrentRound.WarheadKills = newList.warhead_kills;
                            FriendlyFireConfig.PauseDetector = true;
                            string text = string.Concat(new object[] { "Round finished! Anomalies: ", anomalies, " | Chaos: ", chaosInsurgency, " | Facility Forces: ", facilityForces, " | D escaped percentage: ", dEscapePercentage, " | S escaped percentage: : ", sEscapePercentage });
                            GameCore.Console.AddLog(text, Color.gray, false, GameCore.Console.ConsoleLogType.Log);
                            ServerLogs.AddLog(ServerLogs.Modules.Logger, text, ServerLogs.ServerLogType.GameEvent, false);
                            yield return Timing.WaitForSeconds(1.5f);
                            int num5 = Mathf.Clamp(ConfigFile.ServerConfig.GetInt("auto_round_restart_time", 10), 5, 1000);

                            if (roundSummary != null)
                            {
                                roundSummary.RpcShowRoundSummary(roundSummary.classlistStart, newList, leadingTeam, RoundSummary.EscapedClassD, RoundSummary.EscapedScientists, RoundSummary.KilledBySCPs, num5, (int)RoundStart.RoundLength.TotalSeconds);
                                RoundEndedEventArgs roundEndedEventArgs = new((API.Enums.LeadingTeam)leadingTeam, newList, num5);

                                Handlers.Server.OnRoundEnded(roundEndedEventArgs);
                            }

                            yield return Timing.WaitForSeconds(num5 - 1);

                            roundSummary.RpcDimScreen();
                            yield return Timing.WaitForSeconds(1f);
                            RoundRestart.InitiateRoundRestart();
                        }
                    }
                }
            }

            yield break;
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