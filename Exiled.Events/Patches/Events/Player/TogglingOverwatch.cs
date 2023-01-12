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

    using API.Features.Pools;

    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// patches <see cref="ServerRoles.SetOverwatchStatus(byte)"/> to add the <see cref="Handlers.Player.TogglingOverwatch"/> event.
    /// </summary>
    [HarmonyPatch(typeof(ServerRoles), nameof(ServerRoles.SetOverwatchStatus), typeof(byte))]
    internal static class TogglingOverwatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label isAllowed = generator.DefineLabel();
            Label ret = generator.DefineLabel();

            newInstructions.InsertRange(
                0,
                new[]
                {
                    // Player.Get(ServerRoles._hub)
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, Field(typeof(ServerRoles), nameof(ServerRoles._hub))),
                    new CodeInstruction(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),

                    // status
                    new CodeInstruction(OpCodes.Ldarg_1),

                    // TogglingOverwatchEventArgs ev = new(Player, bool)
                    new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(TogglingOverwatchEventArgs))[0]),
                    new CodeInstruction(OpCodes.Dup),
                    new CodeInstruction(OpCodes.Dup),

                    // Handlers.Player.OnTogglingOverwatch(ev)
                    new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnTogglingOverwatch))),

                    // if (!ev.IsAllowed)
                    //    goto bef;
                    new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(TogglingOverwatchEventArgs), nameof(TogglingOverwatchEventArgs.IsAllowed))),
                    new CodeInstruction(OpCodes.Brtrue_S, isAllowed),

                    // pop the event still in the stack and return
                    new CodeInstruction(OpCodes.Pop),
                    new CodeInstruction(OpCodes.Br_S, ret),

                    // isAllowed:
                    new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(TogglingOverwatchEventArgs), nameof(TogglingOverwatchEventArgs.IsEnabled))).WithLabels(isAllowed),
                    new CodeInstruction(OpCodes.Starg_S, 1),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(ret);

            foreach (CodeInstruction instruction in newInstructions)
                yield return instruction;

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}