// -----------------------------------------------------------------------
// <copyright file="Escaping.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Pools;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Escape.ServerHandlePlayer(ReferenceHub)"/> for <see cref="Handlers.Player.Escaping"/>.
    /// </summary>
    [HarmonyPatch(typeof(Escape), nameof(Escape.ServerHandlePlayer))]
    internal static class Escaping
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(EscapingEventArgs));

            const int offset = 0;
            int index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldc_I4_S) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player.Get(hub)
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // roleTypeId
                    new(OpCodes.Ldloc_0),

                    // EscapingEventArgs ev = new(Player, RoleTypeId)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(EscapingEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc, ev.LocalIndex),

                    // Handlers.Player.OnEscaping(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnEscaping))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(EscapingEventArgs), nameof(EscapingEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse, returnLabel),

                    // roleTypeId = ev.NewRole
                    new(OpCodes.Ldloc, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(EscapingEventArgs), nameof(EscapingEventArgs.NewRole))),
                    new(OpCodes.Stloc_0),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}