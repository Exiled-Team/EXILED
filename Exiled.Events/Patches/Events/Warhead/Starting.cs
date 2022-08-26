// -----------------------------------------------------------------------
// <copyright file="Starting.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Warhead
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Warhead;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patch the <see cref="PlayerInteract.UserCode_CmdDetonateWarhead" />.
    ///     Adds the <see cref="Handlers.Warhead.Starting" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Warhead), nameof(Handlers.Warhead.Starting))]
    [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.UserCode_CmdDetonateWarhead))]
    internal static class Starting
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            // The index offset.
            const int offset = 0;

            // Search for the last "ldsfld".
            int index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldsfld) + offset;

            // Get the return label.
            Label returnLabel = generator.DefineLabel();

            // if (!Warhead.CanBeStarted)
            //   return;
            //
            // var ev = new StartingEventArgs(Player.Get(this.gameObject), true);
            //
            // Handlers.Warhead.OnStarting(ev);
            //
            // if (!ev.IsAllowed)
            //   return;
            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(Warhead), nameof(Warhead.CanBeStarted))).MoveLabelsFrom(newInstructions[index]),
                new(OpCodes.Brfalse_S, returnLabel),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(PlayerInteract), nameof(PlayerInteract._hub))),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(StartingEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Call, Method(typeof(Handlers.Warhead), nameof(Handlers.Warhead.OnStarting))),
                new(OpCodes.Call, PropertyGetter(typeof(StartingEventArgs), nameof(StartingEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, returnLabel),
                new(OpCodes.Ldsfld, Field(typeof(AlphaWarheadController), nameof(AlphaWarheadController.Host))),
                new(OpCodes.Dup),
                new(OpCodes.Ldc_I4_0),
                new(OpCodes.Stfld, Field(typeof(AlphaWarheadController), nameof(AlphaWarheadController.doorsOpen))),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Call, PropertySetter(typeof(AlphaWarheadController), nameof(AlphaWarheadController.NetworkinProgress))),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
