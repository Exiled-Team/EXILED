// -----------------------------------------------------------------------
// <copyright file="ReceivingStatusEffect.cs" company="Exiled Team">
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

    using CustomPlayerEffects;

    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches the <see cref="StatusEffectBase.Intensity"/> method.
    /// Adds the <see cref="Handlers.Player.ReceivingEffect"/> event.
    /// </summary>
    [HarmonyPatch(typeof(StatusEffectBase), nameof(StatusEffectBase.ForceIntensity))]
    internal static class ReceivingStatusEffect
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(ReceivingEffectEventArgs));
            LocalBuilder player = generator.DeclareLocal(typeof(Player));

            Label returnLabel = generator.DefineLabel();
            Label continueLabel = generator.DefineLabel();

            const int offset = 1;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ret) + offset;

            List<Label> startingLabels = newInstructions[index].ExtractLabels();

            newInstructions[index].WithLabels(continueLabel);

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player player = Player.Get(this.Hub)
                    //
                    // if (player == null)
                    //    goto continueLabel;
                    new CodeInstruction(OpCodes.Ldarg_0).WithLabels(startingLabels),
                    new(OpCodes.Call, PropertyGetter(typeof(StatusEffectBase), nameof(StatusEffectBase.Hub))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, player.LocalIndex),
                    new(OpCodes.Brfalse_S, continueLabel),

                    // player
                    new(OpCodes.Ldloc_S, player.LocalIndex),

                    // this
                    new(OpCodes.Ldarg_0),

                    // value
                    new(OpCodes.Ldarg_1),

                    // this._intensity
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(StatusEffectBase), nameof(StatusEffectBase._intensity))),

                    // this.Duration
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(StatusEffectBase), nameof(StatusEffectBase.Duration))),

                    // ReceivingEventArgs ev = new(Player, StatusEffectBase, byte, byte, float)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ReceivingEffectEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnReceivingEffect))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ReceivingEffectEventArgs), nameof(ReceivingEffectEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),

                    // value = ev.State
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ReceivingEffectEventArgs), nameof(ReceivingEffectEventArgs.Intensity))),
                    new(OpCodes.Starg_S, 1),

                    // this.Duration = ev.Duration
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ReceivingEffectEventArgs), nameof(ReceivingEffectEventArgs.Duration))),
                    new(OpCodes.Callvirt, PropertySetter(typeof(StatusEffectBase), nameof(StatusEffectBase.Duration))),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}