// -----------------------------------------------------------------------
// <copyright file="SelectTeamAndSpawnCustom.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Server
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;
    using System.Text;

    using Exiled.API.Features.Pools;
    using Exiled.Events.EventArgs.Server;
    using Exiled.Events.Handlers;

    using GameCore;
    using HarmonyLib;
    using Mirror;
    using Mono.Cecil.Cil;
    using PlayerRoles;
    using PlayerRoles.Spectating;
    using PluginAPI.Events;
    using Respawning;
    using Respawning.NamingRules;
    using UnityEngine;
    using Utils.NonAllocLINQ;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patch the <see cref="RespawnManager.Update" />.
    ///     Adds the <see cref="Server.RespawningTeam" /> event.
    /// </summary>
    [HarmonyPatch(typeof(RespawnManager), nameof(RespawnManager.Update))]
    internal static class SelectTeamAndSpawnCustom
    {
        private static SelectTeamEventArgs.CustomTeamRespawnInfo customTeamRespawnInfo;

        [HarmonyDebug]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            const int dominatingTeamVarIndex = 1;
            const int teamHandlerVarIndex = 2;

            LocalBuilder effectTimeVar = generator.DeclareLocal(typeof(float));
            Label endLabel = generator.DefineLabel();
            CodeMatcher codeMatcher = new CodeMatcher(instructions, generator);

            // Go beffore the call of the NW Event
            codeMatcher
                .MatchStartForward(
                    new CodeMatch(OpCodes.Ldloc_1),
                    new CodeMatch(OpCodes.Newobj),
                    new CodeMatch(instuction => (instuction.operand as MethodInfo)?.Name == nameof(EventManager.ExecuteEvent)))
                .ThrowIfInvalid("MatchStartForward to found EventManager.ExecuteEvent() faild");

            // Save the label use to ignore the trow and remove it
            var throwlabel = codeMatcher.Labels[0];
            codeMatcher.Labels.Clear();

            // Insert the label
            //
            // effectTimeVar = pawnableTeamHandlerBase.EffectTime;
            // if (!SelectTeam.ProcessEvent(this, ref dominatingTeam, ref effectTime))
            //     return;
            //
            // replace
            // _timeForNextSequence = SpawnableTeamHandlerBase.EffectTime
            // by
            // _timeForNextSequence = effectTime
            codeMatcher
                .Insert(
                    new CodeInstruction(OpCodes.Ldloc_S, teamHandlerVarIndex))
                .AddLabels(new List<Label>() { throwlabel })
                .Advance(1)
                .InsertAndAdvance(
                    new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(SpawnableTeamHandlerBase), nameof(SpawnableTeamHandlerBase.EffectTime))),
                    new CodeInstruction(OpCodes.Stloc_S, effectTimeVar.LocalIndex),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldloca_S, dominatingTeamVarIndex),
                    new CodeInstruction(OpCodes.Ldloca_S, effectTimeVar.LocalIndex),
                    new CodeInstruction(OpCodes.Call, Method(typeof(SelectTeamAndSpawnCustom), nameof(ProcessEventSelectTeam))),
                    new CodeInstruction(OpCodes.Brfalse_S, endLabel))
                .MatchStartForward(
                    new CodeMatch(OpCodes.Ldarg_0),
                    new CodeMatch(OpCodes.Ldloc_2),
                    new CodeMatch(instuction => (instuction.operand as MethodInfo)?.Name == "get_EffectTime"))
                .ThrowIfInvalid("MatchStartForward to found SpawnableTeamHandlerBase.EffectTime faild")
                .Advance(1)
                .RemoveInstructions(2)
                .Insert(
                    new CodeInstruction(OpCodes.Ldloc_S, effectTimeVar.LocalIndex))
                .MatchStartForward(
                    new CodeMatch(OpCodes.Ldarg_0),
                    new CodeMatch(instuction => (instuction.operand as MethodInfo)?.Name == nameof(RespawnManager.Spawn)))
                .InsertAndAdvance(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call, Method(typeof(SelectTeamAndSpawnCustom), nameof(ProcessEventSpawnCustomTeam))),
                    new CodeInstruction(OpCodes.Brfalse_S, endLabel))
                .End()
                .Labels.Add(endLabel);

            // TODO: add devent le spawn un if qui fait vanila spawn ou custom team

            /*
            foreach (var instruction in codeMatcher.Instructions())
            {
                Exiled.API.Features.Log.Info(instruction);
            }*/

            return codeMatcher.Instructions();
        }

        private static bool ProcessEventSelectTeam(RespawnManager respawnManager, ref SpawnableTeamType dominatingTeam, ref float effectTime)
        {
            SelectTeamEventArgs ev = new SelectTeamEventArgs(effectTime, dominatingTeam);

            Server.OnSelectTeam(ev);

            if (!ev.IsAllowed)
            {
                respawnManager.RestartSequence();
                return false;
            }

            // We avoid calling a useless method and we don't call the NW event (to not break some NW plugin)
            // That code is use to add new Team at the respawn and ignore the spawn of default team of the game
            if (ev.SelectedTeam == SpawnableTeamType.None)
            {
                respawnManager.NextKnownTeam = SpawnableTeamType.None;
                respawnManager._curSequence = RespawnManager.RespawnSequencePhase.PlayingEntryAnimations;
                respawnManager._timeForNextSequence = ev.TimeBeforeSpawning;
                customTeamRespawnInfo = ev.CustomSpawnableTeam;
                return false;
            }

            dominatingTeam = ev.SelectedTeam;
            effectTime = ev.TimeBeforeSpawning;

            return true;
        }

        private static bool ProcessEventSpawnCustomTeam(RespawnManager respawnManager)
        {
            if (respawnManager.NextKnownTeam != SpawnableTeamType.None)
                return true;

            List<Exiled.API.Features.Player> players = ReferenceHub.AllHubs.Where(respawnManager.CheckSpawnable).Select(Exiled.API.Features.Player.Get).ToList();
            RespawningCustomTeamEventArgs ev = new RespawningCustomTeamEventArgs(players, customTeamRespawnInfo.PlayerAmount, customTeamRespawnInfo.TeamId);

            Server.OnRespawningCustomTeam(ev);

            return false;
        }
    }
}