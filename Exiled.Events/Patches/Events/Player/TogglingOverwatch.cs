// -----------------------------------------------------------------------
// <copyright file="TogglingOverwatch.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs;
    using HarmonyLib;
    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// patches <see cref="ServerRoles.SetOverwatchStatus(bool)"/> to add the <see cref="Handlers.Player.TogglingOverwatch"/> event.
    /// </summary>
    [EventPatch(tyepof(Handlers.Player), nameof(Handlers.Player.TogglingOverwatch))]
    [HarmonyPatch(typeof(ServerRoles), nameof(ServerRoles.SetOverwatchStatus), typeof(bool))]
    internal static class TogglingOverwatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label bef = generator.DefineLabel();
            Label ret = generator.DefineLabel();

            newInstructions.InsertRange(0, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(ServerRoles), nameof(ServerRoles._hub))),
                new CodeInstruction(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(TogglingOverwatchEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnTogglingOverwatch))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(TogglingOverwatchEventArgs), nameof(TogglingOverwatchEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brtrue_S, bef),
                new CodeInstruction(OpCodes.Pop),
                new CodeInstruction(OpCodes.Br_S, ret),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(TogglingOverwatchEventArgs), nameof(TogglingOverwatchEventArgs.IsEnabled))).WithLabels(bef),
                new CodeInstruction(OpCodes.Starg_S, 1),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(ret);

            foreach (var instruction in newInstructions)
                yield return instruction;

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
