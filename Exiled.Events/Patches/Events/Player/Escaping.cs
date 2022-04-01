// -----------------------------------------------------------------------
// <copyright file="Escaping.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player {
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="CharacterClassManager.UserCode_CmdRegisterEscape"/> for <see cref="Handlers.Player.Escaping"/>.
    /// </summary>
    [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.UserCode_CmdRegisterEscape))]
    internal static class Escaping {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);
            int offset = -1;
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Stloc_0) + offset;
            Label returnLabel = generator.DefineLabel();
            LocalBuilder ev = generator.DeclareLocal(typeof(EscapingEventArgs));
            LocalBuilder role = generator.DeclareLocal(typeof(RoleType));

            newInstructions.InsertRange(0, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(CharacterClassManager), nameof(CharacterClassManager._hub))),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(EscapingEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc, ev.LocalIndex),
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnEscaping))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(EscapingEventArgs), nameof(EscapingEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse, returnLabel),
                new CodeInstruction(OpCodes.Ldloc, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(EscapingEventArgs), nameof(EscapingEventArgs.NewRole))),
                new CodeInstruction(OpCodes.Stloc, role.LocalIndex),
            });

            foreach (CodeInstruction ins in newInstructions.FindAll(i => i.opcode == OpCodes.Call && (MethodInfo)i.operand == Method(typeof(CharacterClassManager), nameof(CharacterClassManager.SetPlayersClass)))) {
                index = newInstructions.IndexOf(ins) - 5;
                newInstructions.RemoveAt(index);
                newInstructions.Insert(index, new CodeInstruction(OpCodes.Ldloc, role.LocalIndex));
            }

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
