// -----------------------------------------------------------------------
// <copyright file="ActivatingWarheadPanel.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Pools;

    using Exiled.API.Enums;
    using Exiled.API.Features.Items;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using InventorySystem.Items.Keycards;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patch the <see cref="PlayerInteract.UserCode_CmdSwitchAWButton" />.
    ///     Adds the <see cref="Handlers.Player.ActivatingWarheadPanel" /> event.
    /// </summary>
    [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.UserCode_CmdSwitchAWButton))]
    internal static class ActivatingWarheadPanel
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder player = generator.DeclareLocal(typeof(Player));
            LocalBuilder isAllowed = generator.DeclareLocal(typeof(bool));
            LocalBuilder curItem = generator.DeclareLocal(typeof(Item));
            LocalBuilder keycard = generator.DeclareLocal(typeof(Keycard));

            Label eventLabel = generator.DefineLabel();
            Label retLabel = generator.DefineLabel();

            int offset = 1;
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Stloc_0) + offset;

            int instructionsToRemove = 25;
            newInstructions.RemoveRange(index, instructionsToRemove);

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // isAllowed = false;
                new(OpCodes.Ldc_I4_0),
                new(OpCodes.Stloc_S, isAllowed.LocalIndex),

                // if (Player.Get(this._hub).IsBypassModeEnabled)
                //    isAllowed = true;
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(PlayerInteract), nameof(PlayerInteract._hub))),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, player.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.IsBypassModeEnabled))),
                new(OpCodes.Dup),
                new(OpCodes.Stloc, isAllowed.LocalIndex),
                new(OpCodes.Brtrue_S, eventLabel),

                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldloc_S, player),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.Position))),
                new(OpCodes.Call, Method(typeof(PlayerInteract), nameof(PlayerInteract.ChckDis))),
                new(OpCodes.Brfalse_S, eventLabel),

                // if (Player.Get(this._hub).CurrentItem != null)
                //    if (player.CurrentItem is Keycard keycard)
                //        if (keycard.Permissions.HasFlag(AlphaWarhead))
                //            isAllowed = true;
                new(OpCodes.Ldloc_S, player.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.CurrentItem))),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, curItem),
                new(OpCodes.Brfalse_S, eventLabel),
                new(OpCodes.Ldloc_S, curItem),
                new(OpCodes.Isinst, typeof(Keycard)),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, keycard.LocalIndex),
                new(OpCodes.Brfalse_S, eventLabel),
                new(OpCodes.Ldloc_S, keycard),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Keycard), nameof(Keycard.Permissions))),
                new(OpCodes.Box, typeof(KeycardPermissions)),
                new(OpCodes.Ldc_I4_8),
                new(OpCodes.Box, typeof(KeycardPermissions)),
                new(OpCodes.Call, Method(typeof(System.Enum), nameof(System.Enum.HasFlag))),
                new(OpCodes.Brfalse_S, eventLabel),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Stloc_S, isAllowed.LocalIndex),
            });

            offset = -2;
            index = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Ldloc_2) + offset;

            newInstructions.InsertRange(index, new[]
            {
                // ev = new(player, isAllowed)
                new CodeInstruction(OpCodes.Ldloc, player.LocalIndex).WithLabels(eventLabel),
                new(OpCodes.Ldloc, isAllowed.LocalIndex),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ActivatingWarheadPanelEventArgs))[0]),
                new(OpCodes.Dup),

                // Handlers.Player.OnActivatingWarheadPanel
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnActivatingWarheadPanel))),

                // if (!ev.IsAllowed)
                //    return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(ActivatingWarheadPanelEventArgs), nameof(ActivatingWarheadPanelEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, retLabel),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(retLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}