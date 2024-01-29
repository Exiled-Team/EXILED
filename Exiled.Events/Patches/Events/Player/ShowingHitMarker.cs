// -----------------------------------------------------------------------
// <copyright file="ShowingHitMarker.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.API.Features.Core.Generic.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;
    using HarmonyLib;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Hitmarker.SendHitmarkerDirectly(ReferenceHub, float)"/>
    /// to add <see cref="Handlers.Player.ShowingHitMarker"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.OnShowingHitMarker))]
    [HarmonyPatch(typeof(Hitmarker), nameof(Hitmarker.SendHitmarkerDirectly), typeof(ReferenceHub), typeof(float))]
    internal class ShowingHitMarker
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int index = newInstructions.FindIndex(x => x.opcode == OpCodes.Ldarg_1);

            Label continueLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(DisplayingHitmarkerEventArgs));

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player.Get(hub);
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // size
                    new(OpCodes.Ldarg_1),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // DisplayingHitmarkerEventArgs ev = new(Player, float, true);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(DisplayingHitmarkerEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Handlers.Player.OnShowingHitMarker(ev);
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnShowingHitMarker))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(DisplayingHitmarkerEventArgs), nameof(DisplayingHitmarkerEventArgs.IsAllowed))),
                    new(OpCodes.Brtrue_S, continueLabel),

                    new(OpCodes.Ret),

                    // size = ev.Size;
                    new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex).WithLabels(continueLabel),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(DisplayingHitmarkerEventArgs), nameof(DisplayingHitmarkerEventArgs.Size))),
                    new(OpCodes.Starg_S, 1),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}