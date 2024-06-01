// -----------------------------------------------------------------------
// <copyright file="ActivatingWarheadPanel.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Pools;
    using Exiled.API.Enums;
    using Exiled.API.Features.Items;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;
    using HarmonyLib;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patch the <see cref="PlayerInteract.UserCode_CmdSwitchAWButton" />.
    /// Adds the <see cref="Handlers.Player.ActivatingWarheadPanel" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.ActivatingWarheadPanel))]
    [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.UserCode_CmdSwitchAWButton))]
    internal static class ActivatingWarheadPanel
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label continueLabel = generator.DefineLabel();
            Label ev = generator.DefineLabel();
            Label cardCheck = generator.DefineLabel();

            LocalBuilder player = generator.DeclareLocal(typeof(Player));
            LocalBuilder allowed = generator.DeclareLocal(typeof(bool));
            LocalBuilder card = generator.DeclareLocal(typeof(Keycard));

            int index = newInstructions.FindIndex(i => i.Is(OpCodes.Ldfld, Field(typeof(PlayerInteract), nameof(PlayerInteract._sr))));

            newInstructions.RemoveRange(index, 17);

            newInstructions[index].labels.Add(continueLabel);

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player player = Player.Get(this._hub);
                    new(OpCodes.Ldfld, Field(typeof(PlayerInteract), nameof(PlayerInteract._hub))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                    new(OpCodes.Stloc_S, player.LocalIndex),

                    // allowed = false;
                    new(OpCodes.Ldc_I4_0),
                    new(OpCodes.Stloc_S, allowed.LocalIndex),

                    // if (player.IsBypassModeEnabled)
                    //      allowed = true;
                    new CodeInstruction(OpCodes.Ldloc_S, player.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.IsBypassModeEnabled))),
                    new(OpCodes.Brfalse_S, cardCheck),

                    new(OpCodes.Ldc_I4_1),
                    new(OpCodes.Stloc_S, allowed.LocalIndex),
                    new(OpCodes.Br_S, ev),

                    // if (player.CurrentItem != null && player.CurrentItem is Keycard card && card.Permissions.HasFlag(KeycardPermissions.AlphaWarhead))
                    //      allowed = true;
                    new CodeInstruction(OpCodes.Ldloc_S, player.LocalIndex).WithLabels(cardCheck),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.CurrentItem))),
                    new(OpCodes.Brfalse_S, ev),
                    new(OpCodes.Ldloc_S, player.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.CurrentItem))),
                    new(OpCodes.Isinst, typeof(Keycard)),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, card.LocalIndex),
                    new(OpCodes.Brfalse_S, ev),
                    new(OpCodes.Ldloc_S, card.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Keycard), nameof(Keycard.Permissions))),
                    new(OpCodes.Box, typeof(KeycardPermissions)),
                    new(OpCodes.Ldc_I4_8),
                    new(OpCodes.Box, typeof(KeycardPermissions)),
                    new(OpCodes.Call, Method(typeof(Enum), nameof(Enum.HasFlag))),
                    new(OpCodes.Stloc_S, allowed.LocalIndex),

                    // player
                    new CodeInstruction(OpCodes.Ldloc_S, player.LocalIndex).WithLabels(ev),

                    // allowed
                    new(OpCodes.Ldloc_S, allowed.LocalIndex),

                    // ActivatingWarheadPanekEventArgs ev = new(player, allowed);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ActivatingWarheadPanelEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Player.OnActivatingWarheadPanel(ev);
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnActivatingWarheadPanel))),

                    // if (!ev.IsAllowed)
                    //      return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ActivatingWarheadPanelEventArgs), nameof(ActivatingWarheadPanelEventArgs.IsAllowed))),
                    new(OpCodes.Brtrue_S, continueLabel),

                    new(OpCodes.Ret),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}