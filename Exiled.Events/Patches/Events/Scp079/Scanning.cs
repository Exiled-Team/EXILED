// -----------------------------------------------------------------------
// <copyright file="Scanning.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp079
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.Events.EventArgs.Scp079;
    using HarmonyLib;
    using PlayerRoles.PlayableScps.Scp079;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp079ScannerSequence.UpdateSequence"/>
    /// to add <see cref="Handlers.Scp079.Scanning"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp079ScannerSequence), nameof(Scp079ScannerSequence.UpdateSequence))]
    internal class Scanning
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label continueLabel = generator.DefineLabel();

            int index = newInstructions.FindIndex(x => x.Is(OpCodes.Ldfld, Field(typeof(Scp079ScannerTracker), nameof(Scp079ScannerTracker._sequence)))) + 1;

            newInstructions[index].labels.Add(continueLabel);

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // if (this._sequence._curStep != ScanSequenceStep.ScanningFindNewTarget)
                    //      goto continueLabel;
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(Scp079ScannerTracker), nameof(Scp079ScannerTracker._sequence))),
                    new(OpCodes.Ldfld, Field(typeof(Scp079ScannerSequence), nameof(Scp079ScannerSequence._curStep))),
                    new(OpCodes.Ldc_I4_2),
                    new(OpCodes.Bne_Un_S, continueLabel),

                    // Player player = Player.Get(this.Owner);
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Scp079ScannerTracker), nameof(Scp079ScannerTracker.Owner))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // ScanningEventArgs ev = new(player, true);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ScanningEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Scp079.OnScanning(ev);
                    new(OpCodes.Call, Method(typeof(Handlers.Scp079), nameof(Handlers.Scp079.OnScanning))),

                    // if (ev.IsAllowed)
                    //      goto continueLabel;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ScanningEventArgs), nameof(ScanningEventArgs.IsAllowed))),
                    new(OpCodes.Brtrue_S, continueLabel),

                    // this._sequence._curStep = ScanSequenceStep.Init;
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(Scp079ScannerTracker), nameof(Scp079ScannerTracker._sequence))),
                    new(OpCodes.Ldc_I4_0),
                    new(OpCodes.Stfld, Field(typeof(Scp079ScannerSequence), nameof(Scp079ScannerSequence._curStep))),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}