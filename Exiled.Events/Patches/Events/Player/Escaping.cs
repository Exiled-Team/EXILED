// -----------------------------------------------------------------------
// <copyright file="Escaping.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="CharacterClassManager.UserCode_CmdRegisterEscape"/> for <see cref="Handlers.Player.Escaping"/>.
    /// </summary>
    [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.UserCode_CmdRegisterEscape))]
    internal static class Escaping
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);
            Label returnLabel = generator.DefineLabel();
            LocalBuilder ev = generator.DeclareLocal(typeof(EscapingEventArgs));
            LocalBuilder role = generator.DeclareLocal(typeof(RoleType));

            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(CharacterClassManager), nameof(CharacterClassManager._hub))),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(EscapingEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc, ev.LocalIndex),
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnEscaping))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(EscapingEventArgs), nameof(EscapingEventArgs.IsAllowed))),
                new(OpCodes.Brfalse, returnLabel),
                new(OpCodes.Ldloc, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(EscapingEventArgs), nameof(EscapingEventArgs.NewRole))),
                new(OpCodes.Stloc, role.LocalIndex),
            });

            for (int i = 0; i < newInstructions.Count; i++)
            {
                if (newInstructions[i].opcode == OpCodes.Call && (MethodInfo)newInstructions[i].operand == Method(typeof(CharacterClassManager), nameof(CharacterClassManager.SetPlayersClass)))
                {
                    int index = i - 5;
                    newInstructions[index] = new(OpCodes.Ldloc_S, role.LocalIndex);
                }
            }

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
