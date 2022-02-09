// -----------------------------------------------------------------------
// <copyright file="Enraging.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp096
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="PlayableScps.Scp096.Enrage"/>.
    /// Adds the <see cref="Handlers.Scp096.Enraging"/> event.
    /// </summary>
    [HarmonyPatch(typeof(PlayableScps.Scp096), nameof(PlayableScps.Scp096.Enrage))]
    internal static class Enraging
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label returnLabel = generator.DefineLabel();

            // EnragingEventArgs ev = new EnragingEventArgs(this, Player, true);
            //
            // Handlers.Scp096.OnEnraging(ev);
            //
            // if (!ev.IsAllowed)
            //     return;
            newInstructions.InsertRange(0, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(PlayableScps.Scp096), nameof(PlayableScps.Scp096.Hub))),
                new CodeInstruction(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(EnragingEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Scp096), nameof(Handlers.Scp096.OnEnraging))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(EnragingEventArgs), nameof(EnragingEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse_S, returnLabel),
            });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
