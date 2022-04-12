// -----------------------------------------------------------------------
// <copyright file="ReceivingStatusEffect.cs" company="Exiled Team">
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

    using CustomPlayerEffects;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

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
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Ret) + offset;
            LocalBuilder ev = generator.DeclareLocal(typeof(ReceivingEffectEventArgs));
            LocalBuilder player = generator.DeclareLocal(typeof(Player));
            Label returnLabel = generator.DefineLabel();
            Label continueLabel = generator.DefineLabel();

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // Player.Get(this.Hub)
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(PlayerEffect), nameof(PlayerEffect.Hub))),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Dup),
                new(OpCodes.Stloc, player.LocalIndex),
                new(OpCodes.Brfalse, continueLabel),
                new(OpCodes.Ldloc, player.LocalIndex),

                // this
                new(OpCodes.Ldarg_0),

                // value
                new(OpCodes.Ldarg_1),

                // this._intensity
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(PlayerEffect), nameof(PlayerEffect._intensity))),

                // var ev = new ReceivingEventArgs(player, effect, state, currentState)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ReceivingEffectEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc, ev.LocalIndex),
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnReceivingEffect))),

                // if (!ev.IsAllowed)
                //    return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(ReceivingEffectEventArgs), nameof(ReceivingEffectEventArgs.IsAllowed))),
                new(OpCodes.Brfalse, returnLabel),
                new(OpCodes.Ldloc, ev.LocalIndex),
                new(OpCodes.Dup),

                // value = ev.State
                new(OpCodes.Callvirt, PropertyGetter(typeof(ReceivingEffectEventArgs), nameof(ReceivingEffectEventArgs.State))),
                new(OpCodes.Starg, 1),

                // this.Duration = ev.Duration
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ReceivingEffectEventArgs), nameof(ReceivingEffectEventArgs.Duration))),
                new(OpCodes.Stfld, Field(typeof(PlayerEffect), nameof(PlayerEffect.Duration))),
            });

            newInstructions[index + 25].labels.Add(continueLabel);
            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        private static void LogThing(byte old, byte @new) => Log.Warn($"Old: {old} New: {@new}");
    }
}
