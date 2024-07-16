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

            int index = newInstructions.FindIndex(instruction => instruction.LoadsField(Field(typeof(AlphaWarheadOutsitePanel), nameof(AlphaWarheadOutsitePanel.keycardEntered)))) + 3;

            Label thisFunctionLabel = generator.DefineLabel();

            // SurfaceButtonInteractEventArgs ev = new SurfaceButtonInteractEventArgs(Player.Get(PlayerInteract._hub), bool)
            // Warhead.OnSurfaceButtonInteract(ev)
            // if (!ev.IsAllowed)
            // return;
            newInstructions.InsertRange(
                index,
                new[]
                {
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),

                    // PlayerInteract._hub
                    new CodeInstruction(OpCodes.Ldfld, Field(typeof(PlayerInteract), nameof(PlayerInteract._hub))),

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
                    new(OpCodes.Brfalse_S, thisFunctionLabel),
                });

            newInstructions[newInstructions.Count - 1].labels.Add(thisFunctionLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
