// -----------------------------------------------------------------------
// <copyright file="ActivatingWarheadPanel.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1118
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patch the <see cref="PlayerInteract.CallCmdSwitchAWButton"/>.
    /// Adds the <see cref="Handlers.Player.ActivatingWarheadPanel"/> event.
    /// </summary>
    [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.CallCmdSwitchAWButton))]
    internal static class ActivatingWarheadPanel
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            // Index offsets.
            const int offset = 1;
            const int insideIfOffset = 0;
            const string permission = "CONT_LVL_3";

            // Search for the eighth "ldarg.0".
            var index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Stloc_1) + offset;

            // Search for the last "ldloc.0"
            var insideIfIndex = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldloc_0) + insideIfOffset;

            // Declare List<string> local variable.
            var list = generator.DeclareLocal(typeof(List<string>));

            // Define the continue label.
            var continueLabel = generator.DefineLabel();

            // Define the inside if label.
            var insideIfLabel = newInstructions[insideIfIndex].WithLabels(generator.DefineLabel()).labels[0];

            // Define the checked bypass or contains label.
            var checkedBypassLabel = generator.DefineLabel();

            // Define the checked contains label label.
            var checkedContainsLabel = generator.DefineLabel();

            // if (!this._sr.BypassMode && itemById == null)
            //   return;
            //
            // var list = ListPool<string>.Shared.Rent()
            // var ev = new ActivatingWarheadPanelEventArgs(Player.Get(this.gameObject), list, __instance._sr.BypassMode || itemById.permissions.Contains("CONT_LVL_3"));
            //
            // Handlers.Player.OnActivatingWarheadPanel(ev);
            //
            // ListPool<string>.Shared.Return(list);
            //
            // if (ev.IsAllowed)
            //   goto insideIfLabel;
            //
            // return;
            newInstructions.InsertRange(index, new[]
            {
                // if (this._sr.BypassMode)
                //   goto continueLabel
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(PlayerInteract), nameof(PlayerInteract._sr))),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(ServerRoles), nameof(ServerRoles.BypassMode))),
                new CodeInstruction(OpCodes.Brtrue_S, continueLabel),

                // if (itemByID != null)
                //   goto continueLabel
                new CodeInstruction(OpCodes.Ldloc_1),
                new CodeInstruction(OpCodes.Brtrue_S, continueLabel),

                // return
                new CodeInstruction(OpCodes.Ret),

                // var list = ListPool<string>.Shared.Rent()
                new CodeInstruction(OpCodes.Ldsfld, Field(typeof(ListPool<string>), nameof(ListPool<string>.Shared))).WithLabels(continueLabel),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(ListPool<string>), nameof(ListPool<string>.Rent), new Type[] { })),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc_S, list.LocalIndex),

                // list.Add("CONT_LVL_3)
                new CodeInstruction(OpCodes.Ldstr, permission),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(List<string>), nameof(List<string>.Add))),

                // Player.Get(this.gameObject)
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(Component), nameof(Component.gameObject))),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(GameObject) })),

                // list
                new CodeInstruction(OpCodes.Ldloc_S, list.LocalIndex),

                // if (this._sr.BypassMode)
                //   goto checkedBypassLabel;
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(PlayerInteract), nameof(PlayerInteract._sr))),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(ServerRoles), nameof(ServerRoles.BypassMode))),
                new CodeInstruction(OpCodes.Brtrue_S, checkedBypassLabel),

                // itemByID.permissions.Contains("CONT_LVL_3")
                //   goto checkedContainsLabel
                new CodeInstruction(OpCodes.Ldloc_1),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(global::Item), nameof(global::Item.permissions))),
                new CodeInstruction(OpCodes.Ldstr, permission),
                new CodeInstruction(OpCodes.Call, typeof(Enumerable).GetMethods().FirstOrDefault(method => method.Name == "Contains" && method.GetParameters().Length == 2).MakeGenericMethod(typeof(string))),
                new CodeInstruction(OpCodes.Br_S, checkedContainsLabel),

                new CodeInstruction(OpCodes.Ldc_I4_1).WithLabels(checkedBypassLabel),

                // var ev = new ActivatingWarheadPanelEventArgs(...)
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(ActivatingWarheadPanelEventArgs))[0]).WithLabels(checkedContainsLabel),
                new CodeInstruction(OpCodes.Dup),

                // Handlers.Player.OnActivatingWarheadPanel(ev);
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnActivatingWarheadPanel))),

                // ListPool<string>.Shared.Return(list);
                new CodeInstruction(OpCodes.Ldsfld, Field(typeof(ListPool<string>), nameof(ListPool<string>.Shared))),
                new CodeInstruction(OpCodes.Ldloc_S, list.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(ListPool<string>), nameof(ListPool<string>.Return))),

                // if (ev.IsAllowed)
                //   goto insideIfLabel
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ActivatingWarheadPanelEventArgs), nameof(ActivatingWarheadPanelEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brtrue_S, insideIfLabel),

                // return;
                new CodeInstruction(OpCodes.Ret),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
