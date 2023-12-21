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

    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.Events.EventArgs.Server;

    using HarmonyLib;
    using PlayerRoles;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="RoundSummary._ProcessServerSideCode()" />.
    /// Adds the <see cref="Handlers.Server.EndingRound" /> and <see cref="Handlers.Server.RoundEnded" /> event.
    /// Adds the <see cref="Round.IgnoredPlayers" /> Propperty.
    /// </summary>
    [HarmonyPatch]
    internal static class RoundEnd
    {
#pragma warning disable SA1600 // Elements should be documented
        public static Type PrivateType { get; internal set; }

        private static MethodInfo TargetMethod()
        {
            PrivateType = typeof(RoundSummary).GetNestedTypes(all)[5];
            return Method(PrivateType, "MoveNext");
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            const string LeadingTeam = "<leadingTeam>5__9";
            const string NewList = "<newList>5__3";

            int offset = -1;
            int index = newInstructions.FindIndex(x => x.Calls(Method(typeof(PlayerRolesUtils), nameof(PlayerRolesUtils.GetTeam), new Type[] { typeof(ReferenceHub), }))) + offset;

            Label jmp = generator.DefineLabel();

            // if (Round.IgnoredPlayers.Contains(referencehub)
            //  goto jmp;
            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    new(OpCodes.Call, PropertyGetter(typeof(Round), nameof(Round.IgnoredPlayers))),
                    new(OpCodes.Ldloc_S, 10),
                    new(OpCodes.Call, Method(typeof(HashSet<ReferenceHub>), nameof(HashSet<ReferenceHub>.Contains))),
                    new(OpCodes.Brtrue_S, jmp),
                });

            offset = 4;
            index = newInstructions.FindIndex(x => x.Calls(Method(typeof(PlayerRolesUtils), nameof(PlayerRolesUtils.GetTeam), new Type[] { typeof(ReferenceHub), }))) + offset;

            newInstructions[index].labels.Add(jmp);

            // Replace ChaosTargetCount == 0 with ChaosTargetCount <= 0
            offset = 1;
            index = newInstructions.FindIndex(x => x.Calls(PropertyGetter(typeof(RoundSummary), nameof(RoundSummary.ChaosTargetCount)))) + offset;
            Label label = (Label)newInstructions[index].operand;
            newInstructions.RemoveAt(index);

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    new(OpCodes.Ldc_I4_0),
                    new(OpCodes.Bgt_S, label),
                });

            offset = -1;
            index = newInstructions.FindIndex(x => x.opcode == OpCodes.Ldfld && x.operand == (object)Field(typeof(RoundSummary), nameof(RoundSummary._roundEnded))) + offset;

            LocalBuilder evEndingRound = generator.DeclareLocal(typeof(EndingRoundEventArgs));

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // this.leadingTeam
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Ldfld, Field(PrivateType, LeadingTeam)),

                    // this.newList
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(PrivateType, NewList)),

                    // shouldRoundEnd
                    new(OpCodes.Ldloc_S, 4),

                    // isForceEnd
                    new(OpCodes.Ldloc_1),
                    new(OpCodes.Ldfld, Field(typeof(RoundSummary), nameof(RoundSummary._roundEnded))),

                    // EndingRoundEventArgs evEndingRound = new(RoundSummary.LeadingTeam, RoundSummary.SumInfo_ClassList, bool, bool);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(EndingRoundEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Server.OnEndingRound(evEndingRound);
                    new(OpCodes.Call, Method(typeof(Handlers.Server), nameof(Handlers.Server.OnEndingRound))),
                    new(OpCodes.Stloc_S, evEndingRound.LocalIndex),

                    // this.leadingTeam = ev.LeadingTeam
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldloc_S, evEndingRound.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(EndingRoundEventArgs), nameof(EndingRoundEventArgs.LeadingTeam))),
                    new(OpCodes.Stfld, Field(PrivateType, LeadingTeam)),

                    // this._roundEnded = ev.IsForceEnded
                    new(OpCodes.Ldloc_1),
                    new(OpCodes.Ldloc_S, evEndingRound.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(EndingRoundEventArgs), nameof(EndingRoundEventArgs.IsForceEnded))),
                    new(OpCodes.Stfld, Field(typeof(RoundSummary), nameof(RoundSummary._roundEnded))),

                    // flag = ev.IsAllowed
                    new(OpCodes.Ldloc_S, evEndingRound.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(EndingRoundEventArgs), nameof(EndingRoundEventArgs.IsAllowed))),
                    new(OpCodes.Stloc_S, 4),
                });

            offset = 7;
            index = newInstructions.FindLastIndex(x => x.opcode == OpCodes.Ldstr && x.operand == (object)"auto_round_restart_time") + offset;

            LocalBuilder timeToRestartIndex = (LocalBuilder)newInstructions[index - 1].operand;
            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // this.leadingTeam
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(PrivateType, LeadingTeam)),

                    // this.newList
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(PrivateType, NewList)),

                    // timeToRestart
                    new(OpCodes.Ldloc_S, timeToRestartIndex),

                    // RoundEndedEventArgs evEndedRound = new(RoundSummary.LeadingTeam, RoundSummary.SumInfo_ClassList, bool);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(RoundEndedEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Server.OnRoundEnded(evEndedRound);
                    new(OpCodes.Call, Method(typeof(Handlers.Server), nameof(Handlers.Server.OnRoundEnded))),

                    // timeToRestart = ev.TimeToRestart
                    new(OpCodes.Callvirt, PropertyGetter(typeof(RoundEndedEventArgs), nameof(RoundEndedEventArgs.TimeToRestart))),
                    new(OpCodes.Stloc_S, timeToRestartIndex),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
