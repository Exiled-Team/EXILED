// -----------------------------------------------------------------------
// <copyright file="Detonated.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Warhead
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Warhead;
    using Handlers;
    using HarmonyLib;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="AlphaWarheadController.Detonate" />
    ///     to add <see cref="Warhead.Detonating"/> and <see cref="Warhead.Detonated"/> events.
    /// </summary>
    [EventPatch(typeof(Warhead), nameof(Warhead.Detonated))]
    [EventPatch(typeof(Warhead), nameof(Warhead.Detonating))]
    [HarmonyPatch(typeof(AlphaWarheadController), nameof(AlphaWarheadController.Detonate))]
    internal static class Detonated
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label continueLabel = generator.DefineLabel();
            LocalBuilder ev = generator.DeclareLocal(typeof(DetonatingEventArgs));

            newInstructions[0].labels.Add(continueLabel);

            newInstructions.InsertRange(
                0,
                new CodeInstruction[]
                {
                    // DetonatingEventArgs ev = new();
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(DetonatingEventArgs))[0]),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Handlers.Warhead.OnDetonating(ev);
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Call, Method(typeof(Warhead), nameof(Warhead.OnDetonating))),

                    // if (ev.IsAllowed)
                    //    goto continueLabel;
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(DetonatingEventArgs), nameof(DetonatingEventArgs.IsAllowed))),
                    new(OpCodes.Brtrue_S, continueLabel),

                    // this.Info.StartTime = 0.0;
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldflda, Field(typeof(AlphaWarheadController), nameof(AlphaWarheadController.Info))),
                    new(OpCodes.Ldc_R8, 0.0),
                    new(OpCodes.Stfld, Field(typeof(AlphaWarheadSyncInfo), nameof(AlphaWarheadSyncInfo.StartTime))),

                    // this.NetworkInfo = this.Info;
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(AlphaWarheadController), nameof(AlphaWarheadController.Info))),
                    new(OpCodes.Callvirt, PropertySetter(typeof(AlphaWarheadController), nameof(AlphaWarheadController.NetworkInfo))),

                    // return;
                    new(OpCodes.Ret),
                });

            const int offset = 1;
            int index = newInstructions.FindIndex(
                instruction => instruction.Calls(Method(typeof(AlphaWarheadController), nameof(AlphaWarheadController.RpcShake)))) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Warhead.OnDetonated();
                    new CodeInstruction(OpCodes.Call, Method(typeof(Warhead), nameof(Warhead.OnDetonated))),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}