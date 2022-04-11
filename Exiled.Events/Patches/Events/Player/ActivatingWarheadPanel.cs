// -----------------------------------------------------------------------
// <copyright file="ActivatingWarheadPanel.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patch the <see cref="PlayerInteract.UserCode_CmdSwitchAWButton"/>.
    /// Adds the <see cref="Handlers.Player.ActivatingWarheadPanel"/> event.
    /// </summary>
    [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.UserCode_CmdSwitchAWButton))]
    internal static class ActivatingWarheadPanel
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            const int offset = 0;
            int index = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Brfalse_S) + offset;

            LocalBuilder isAllowed = generator.DeclareLocal(typeof(bool));

            newInstructions.InsertRange(index, new[]
            {
                // new ActivatingWarheadPanelEventArgs(_hub, isAllowed);
                new CodeInstruction(OpCodes.Stloc_S, isAllowed.LocalIndex),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(PlayerInteract), nameof(PlayerInteract._hub))),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new CodeInstruction(OpCodes.Ldloc_S, isAllowed.LocalIndex),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(ActivatingWarheadPanelEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),

                // Player.OnActivatingWarheadPanel(ev);
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnActivatingWarheadPanel))),

                // Load IsAllowed for the original code to evaluate
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ActivatingWarheadPanelEventArgs), nameof(ActivatingWarheadPanelEventArgs.IsAllowed))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
