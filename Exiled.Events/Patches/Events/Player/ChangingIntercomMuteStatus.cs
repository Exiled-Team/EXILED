// -----------------------------------------------------------------------
// <copyright file="ChangingIntercomMuteStatus.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patch the <see cref="CharacterClassManager.NetworkIntercomMuted"/>.
    /// Adds the <see cref="ChangingIntercomMuteStatus"/> event.
    /// </summary>
    [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.NetworkIntercomMuted), MethodType.Setter)]
    internal static class ChangingIntercomMuteStatus
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label retLabel = generator.DefineLabel();
            Label jccLabel = generator.DefineLabel();
            Label cdcLabel = generator.DefineLabel();

            newInstructions[0].labels.Add(cdcLabel);

            newInstructions.InsertRange(0, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(CharacterClassManager), nameof(CharacterClassManager._hub))),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangingIntercomMuteStatusEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnChangingIntercomMuteStatus))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ChangingIntercomMuteStatusEventArgs), nameof(ChangingIntercomMuteStatusEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brtrue_S, cdcLabel),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Brtrue_S, jccLabel),
                new CodeInstruction(OpCodes.Ldstr, "ICOM-"),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(CharacterClassManager), nameof(CharacterClassManager.UserId))),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(string), nameof(string.Concat), new[] { typeof(string), typeof(string) })),
                new CodeInstruction(OpCodes.Call, Method(typeof(MuteHandler), nameof(MuteHandler.IssuePersistentMute))),
                new CodeInstruction(OpCodes.Ldstr, "ICOM-").WithLabels(jccLabel),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(CharacterClassManager), nameof(CharacterClassManager.UserId))),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(string), nameof(string.Concat), new[] { typeof(string), typeof(string) })),
                new CodeInstruction(OpCodes.Call, Method(typeof(MuteHandler), nameof(MuteHandler.RevokePersistentMute))),
                new CodeInstruction(OpCodes.Br_S, retLabel),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(retLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
