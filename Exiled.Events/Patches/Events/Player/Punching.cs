// -----------------------------------------------------------------------
// <copyright file="Punching.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1118

    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using InventorySystem.Items.Usables.Scp330;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches the <see cref="MutantHands.ServerProcessMessage"/> method.
    /// Adds the <see cref="Handlers.Player.Punching"/> event.
    /// </summary>
    [HarmonyPatch(typeof(MutantHands), nameof(MutantHands.ServerProcessMessage))]
    public class Punching
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            // IL_0037: ldfld        class [UnityEngine.CoreModule]UnityEngine.Transform ReferenceHub::PlayerCameraReference
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent();
            const int offset = -6;
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Callvirt && (MethodInfo)i.operand == Method(typeof(Bounds), nameof(MovementTracer.GenerateBounds))) + offset;
            Label returnLabel = generator.DefineLabel();
            Log.Debug($"i {index}");
            newInstructions.InsertRange(index, new[]
            {
                // Player.Get(attacker)
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(MutantHands), nameof(MutantHands.Owner))),
                new CodeInstruction(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),

                // Player.Get(target)
                new CodeInstruction(OpCodes.Ldarg_3),

                // True
                new CodeInstruction(OpCodes.Ldc_I4_1),

                // var ev = new PunchingEventArgs(attacker, target, true)
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(PunchingEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),

                // Handlers.Player.OnPunching(ev)
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnPunching))),

                // if(!ev.isAllowed) return
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(PunchingEventArgs), nameof(PunchingEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse, returnLabel),
            });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
            {
                Log.Debug(newInstructions[z]);
                yield return newInstructions[z];
            }

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
