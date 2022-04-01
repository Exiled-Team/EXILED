// -----------------------------------------------------------------------
// <copyright file="ChangingMuteStatus.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player {
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Assets._Scripts.Dissonance;

    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patch the <see cref="DissonanceUserSetup.AdministrativelyMuted"/>.
    /// Adds the <see cref="Player.ChangingMuteStatus"/> event.
    /// </summary>
    [HarmonyPatch(typeof(DissonanceUserSetup), nameof(DissonanceUserSetup.AdministrativelyMuted), MethodType.Setter)]
    internal static class ChangingMuteStatus {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            LocalBuilder player = generator.DeclareLocal(typeof(API.Features.Player));

            Label retLabel = generator.DefineLabel();
            Label jccLabel = generator.DefineLabel();
            Label cdcLabel = generator.DefineLabel();

            newInstructions[0].labels.Add(cdcLabel);

            newInstructions.InsertRange(0, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(DissonanceUserSetup), nameof(DissonanceUserSetup.netId))),
                new CodeInstruction(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(uint) })),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc_S, player.LocalIndex),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangingMuteStatusEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnChangingMuteStatus))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ChangingMuteStatusEventArgs), nameof(ChangingMuteStatusEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brtrue_S, cdcLabel),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Brtrue_S, jccLabel),
                new CodeInstruction(OpCodes.Ldloc_S, player.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(API.Features.Player), nameof(API.Features.Player.UserId))),
                new CodeInstruction(OpCodes.Call, Method(typeof(MuteHandler), nameof(MuteHandler.IssuePersistentMute))),
                new CodeInstruction(OpCodes.Ldloc_S, player.LocalIndex).WithLabels(jccLabel),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(API.Features.Player), nameof(API.Features.Player.UserId))),
                new CodeInstruction(OpCodes.Call, Method(typeof(MuteHandler), nameof(MuteHandler.RevokePersistentMute))),
                new CodeInstruction(OpCodes.Br_S, retLabel),
            });

            newInstructions[newInstructions.Count - 1].WithLabels(retLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
