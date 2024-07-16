// -----------------------------------------------------------------------
// <copyright file="SurfaceButtonInteract.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Warhead
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Core.Generic.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Warhead;

    using HarmonyLib;

    using static HarmonyLib.AccessTools;

    using Warhead = Handlers.Warhead;

    /// <summary>
    /// Patches <see cref="PlayerInteract.UserCode_CmdSwitchAWButton" />.
    /// Adds the <see cref="Warhead.SurfaceButtonInteract" /> event.
    /// </summary>
    [EventPatch(typeof(Warhead), nameof(Warhead.SurfaceButtonInteract))]
    [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.UserCode_CmdSwitchAWButton))]
    internal static class SurfaceButtonInteract
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            // Getting the start of BypassMode (IL_0028: ldarg.0)
            int index = newInstructions.FindLastIndex(instruction => instruction.ToString().Contains("BypassMode")) - 2;

            // Getting the ChckDis jump to the new label. (IL_0025: brtrue.s)
            int chckDis_call_after_indx = newInstructions.FindLastIndex(instruction => instruction.ToString().Contains("ChckDis")) + 1;
            CodeInstruction chckDis_instruction = newInstructions[chckDis_call_after_indx];

            Label thisFunctionLabel = generator.DefineLabel();

            // SurfaceButtonInteractEventArgs ev = new SurfaceButtonInteractEventArgs(Player.Get(PlayerInteract._hub), bool)
            // Warhead.OnSurfaceButtonInteract(ev)
            // if (!ev.IsAllowed)
            // return;
            newInstructions.InsertRange(
                index,
                new[]
                {
                    new CodeInstruction(OpCodes.Ldarg_0),

                    // PlayerInteract._hub
                    new CodeInstruction(OpCodes.Ldfld, Field(typeof(PlayerInteract), "_hub")),

                    // Player.Get(PlayerInteract._hub)
                    new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    new CodeInstruction(OpCodes.Ldc_I4_1),

                    // new ButtonInteractEventArgs(Player.Get(PlayerInteract._hub), bool)
                    new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(SurfaceButtonInteractEventArgs))[0]),
                    new CodeInstruction(OpCodes.Dup),

                    // Warhead.OnSurfaceButtonInteract(ev)
                    new CodeInstruction(OpCodes.Call, Method(typeof(Warhead), nameof(Warhead.OnSurfaceButtonInteract))),
                    new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(SurfaceButtonInteractEventArgs), nameof(SurfaceButtonInteractEventArgs.IsAllowed))),

                    // if (!ev.IsAllowed)
                    // return;
                    new CodeInstruction(chckDis_instruction),
                    new CodeInstruction(OpCodes.Ret),
                });

            // Getting the new index of our function.
            int hub_indx = newInstructions.FindLastIndex(instruction => instruction.ToString().Contains("Player::Get")) - 2;
            newInstructions[hub_indx].WithLabels(thisFunctionLabel);
            chckDis_instruction.operand = thisFunctionLabel;

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
