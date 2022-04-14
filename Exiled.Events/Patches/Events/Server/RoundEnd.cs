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
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.API.Enums;
    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;

    using GameCore;

    using HarmonyLib;

    using MEC;

    using RoundRestarting;

    using UnityEngine;

    using Console = GameCore.Console;

    /// <summary>
    /// Patches <see cref="RoundSummary.Start"/>.
    /// Adds the <see cref="Server.EndingRound"/> and <see cref="Server.RoundEnded"/> event.
    /// </summary>
    [HarmonyPatch(typeof(RoundSummary), nameof(RoundSummary.Start))]
    internal static class RoundEnd
    {
        private static IEnumerator<float> Process(RoundSummary roundSummary)
        {
            float time = Time.unscaledTime;
            while (roundSummary is not null)
            {
                yield return Timing.WaitForSeconds(2.5f);

                while (RoundSummary.RoundLock || !RoundSummary.RoundInProgress() || Time.unscaledTime - time < 15f || (roundSummary._keepRoundOnOne && PlayerManager.players.Count < 2))
                    yield return Timing.WaitForOneFrame;

                RoundSummary.SumInfo_ClassList newList = default;
                foreach (KeyValuePair<GameObject, ReferenceHub> keyValuePair in ReferenceHub.GetAllHubs())
                {
                    if (keyValuePair.Value is null)
                        continue;

                    CharacterClassManager component = keyValuePair.Value.characterClassManager;
                    if (component.Classes.CheckBounds(component.CurClass))
                    {
                        switch (component.CurRole.team)
                        {
                            case Team.SCP:
                                if (component.CurClass == RoleType.Scp0492)
                                    newList.zombies++;
                                else
                                    newList.scps_except_zombies++;
                                continue;
                            case Team.MTF:
                                newList.mtf_and_guards++;
                                continue;
                            case Team.CHI:
                                newList.chaos_insurgents++;
                                continue;
                            case Team.RSC:
                                newList.scientists++;
                                continue;
                            case Team.CDP:
                                newList.class_ds++;
                                continue;
                            default:
                                continue;
                        }
                    }
                }

                yield return Timing.WaitForOneFrame;
                newList.warhead_kills = AlphaWarheadController.Host.detonated ? AlphaWarheadController.Host.warheadKills : -1;
                yield return Timing.WaitForOneFrame;
                newList.time = (int)Time.realtimeSinceStartup;
                yield return Timing.WaitForOneFrame;
                RoundSummary.roundTime = newList.time - roundSummary.classlistStart.time;
                int num1 = newList.mtf_and_guards + newList.scientists;
                int num2 = newList.chaos_insurgents + newList.class_ds;
                int num3 = newList.scps_except_zombies + newList.zombies;
                int num4 = newList.class_ds + RoundSummary.EscapedClassD;
                int num5 = newList.scientists + RoundSummary.EscapedScientists;
                float num6 = (roundSummary.classlistStart.class_ds == 0) ? 0f : (num4 / roundSummary.classlistStart.class_ds);
                float num7 = (roundSummary.classlistStart.scientists == 0) ? 1f : (num5 / roundSummary.classlistStart.scientists);

                RoundSummary.SurvivingSCPs = newList.scps_except_zombies;

                if (newList.class_ds <= 0 && num1 <= 0)
                {
                    roundSummary.RoundEnded = true;
                }
                else
                {
                    int num8 = 0;
                    if (num1 > 0)
                        num8++;
                    if (num2 > 0)
                        num8++;
                    if (num3 > 0)
                        num8++;
                    if (num8 <= 1)
                        roundSummary.RoundEnded = true;
                }

                EndingRoundEventArgs endingRoundEventArgs = new(LeadingTeam.Draw, newList, roundSummary.RoundEnded);

                if (num1 > 0)
                    endingRoundEventArgs.LeadingTeam = RoundSummary.EscapedScientists >= RoundSummary.EscapedClassD ? LeadingTeam.FacilityForces : LeadingTeam.Draw;
                else if (num3 > 0)
                    endingRoundEventArgs.LeadingTeam = RoundSummary.EscapedClassD > RoundSummary.SurvivingSCPs ? LeadingTeam.ChaosInsurgency : (RoundSummary.SurvivingSCPs > RoundSummary.EscapedScientists ? LeadingTeam.Anomalies : LeadingTeam.Draw);
                else if (num2 > 0)
                    endingRoundEventArgs.LeadingTeam = RoundSummary.EscapedClassD >= RoundSummary.EscapedScientists ? LeadingTeam.ChaosInsurgency : LeadingTeam.Draw;

                Server.OnEndingRound(endingRoundEventArgs);

                roundSummary.RoundEnded = endingRoundEventArgs.IsRoundEnded && endingRoundEventArgs.IsAllowed;

                if (roundSummary.RoundEnded)
                {
                    FriendlyFireConfig.PauseDetector = true;
                    string str = "Round finished! Anomalies: " + num3 + " | Chaos: " + num2 + " | Facility Forces: " + num1 + " | D escaped percentage: " + num6 + " | S escaped percentage: : " + num7;
                    Console.AddLog(str, Color.gray, false);
                    ServerLogs.AddLog(ServerLogs.Modules.Logger, str, ServerLogs.ServerLogType.GameEvent);
                    yield return Timing.WaitForSeconds(1.5f);
                    int timeToRoundRestart = Mathf.Clamp(ConfigFile.ServerConfig.GetInt("auto_round_restart_time", 10), 5, 1000);

                    if (roundSummary is not null)
                    {
                        RoundEndedEventArgs roundEndedEventArgs = new(endingRoundEventArgs.LeadingTeam, newList, timeToRoundRestart);

                        Server.OnRoundEnded(roundEndedEventArgs);

                        roundSummary.RpcShowRoundSummary(roundSummary.classlistStart, roundEndedEventArgs.ClassList, (RoundSummary.LeadingTeam)roundEndedEventArgs.LeadingTeam, RoundSummary.EscapedClassD, RoundSummary.EscapedScientists, RoundSummary.KilledBySCPs, roundEndedEventArgs.TimeToRestart);
                    }

                    yield return Timing.WaitForSeconds(timeToRoundRestart - 1);
                    roundSummary.RpcDimScreen();
                    yield return Timing.WaitForSeconds(1f);
                    RoundRestart.InitiateRoundRestart();
                    yield break;
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
                        && methodBase.Name != nameof(RoundSummary._ProcessServerSideCode))
                    {
                        yield return instruction;
                    }
                    else
                    {
                        yield return new(OpCodes.Call, AccessTools.Method(typeof(RoundEnd), nameof(Process)));
                        yield return new(OpCodes.Ldarg_0);
                        yield return new(OpCodes.Call, AccessTools.FirstMethod(typeof(MECExtensionMethods2), (m) =>
                        {
                            Type[] generics = m.GetGenericArguments();
                            ParameterInfo[] paramseters = m.GetParameters();
                            return m.Name == "CancelWith"
                            && generics.Length == 1
                            && paramseters.Length == 2
                            && paramseters[0].ParameterType == typeof(IEnumerator<float>)
                            && paramseters[1].ParameterType == generics[0];
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
