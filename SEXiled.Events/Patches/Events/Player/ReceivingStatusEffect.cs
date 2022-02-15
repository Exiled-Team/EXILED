// -----------------------------------------------------------------------
// <copyright file="ReceivingStatusEffect.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.Patches.Events.Player
{
#pragma warning disable SA1118

    using System.Collections.Generic;
    using System.Reflection.Emit;

    using CustomPlayerEffects;

    using SEXiled.API.Features;
    using SEXiled.Events.EventArgs;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches the <see cref="PlayerEffect.Intensity"/> method.
    /// Adds the <see cref="Handlers.Player.ReceivingEffect"/> event.
    /// </summary>
    [HarmonyPatch(typeof(PlayerEffect), nameof(PlayerEffect.Intensity), MethodType.Setter)]
    internal static class ReceivingStatusEffect
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);
            const int offset = 1;
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Beq_S) + offset;
            LocalBuilder ev = generator.DeclareLocal(typeof(ReceivingEffectEventArgs));
            LocalBuilder player = generator.DeclareLocal(typeof(Player));
            Label returnLabel = generator.DefineLabel();
            Label continueLabel = generator.DefineLabel();

            newInstructions.InsertRange(index, new[]
            {
                // Player.Get(this.Hub)
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(PlayerEffect), nameof(PlayerEffect.Hub))),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc, player.LocalIndex),
                new CodeInstruction(OpCodes.Brfalse, continueLabel),
                new CodeInstruction(OpCodes.Ldloc, player.LocalIndex),

                // this
                new CodeInstruction(OpCodes.Ldarg_0),

                // value
                new CodeInstruction(OpCodes.Ldarg_1),

                // this._intensity
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(PlayerEffect), nameof(PlayerEffect._intensity))),

                // var ev = new ReceivingEventArgs(player, effect, state, currentState)
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(ReceivingEffectEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc, ev.LocalIndex),
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnReceivingEffect))),

                // if (!ev.IsAllowed)
                //    return;
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ReceivingEffectEventArgs), nameof(ReceivingEffectEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse, returnLabel),
                new CodeInstruction(OpCodes.Ldloc, ev.LocalIndex),
                new CodeInstruction(OpCodes.Dup),

                // value = ev.State
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ReceivingEffectEventArgs), nameof(ReceivingEffectEventArgs.State))),
                new CodeInstruction(OpCodes.Starg, 1),

                // this.Duration = ev.Duration
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ReceivingEffectEventArgs), nameof(ReceivingEffectEventArgs.Duration))),
                new CodeInstruction(OpCodes.Stfld, Field(typeof(PlayerEffect), nameof(PlayerEffect.Duration))),
            });

            newInstructions[index + 25].WithLabels(continueLabel);
            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
