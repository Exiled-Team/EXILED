// -----------------------------------------------------------------------
// <copyright file="AnnouncingDecontamination.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs.Map;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using LightContainmentZoneDecontamination;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="DecontaminationController.UpdateTime" />.
    ///     Adds the <see cref="AnnouncingDecontamination" /> event.
    /// </summary>
    [HarmonyPatch(typeof(DecontaminationController), nameof(DecontaminationController.UpdateTime))]
    internal static class AnnouncingDecontamination
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            const int offset = 0;
            int index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Bne_Un_S) + offset;

            // var ev = new AnnouncingDecontaminationEventArgs(int, bool);
            //
            // Map.OnAnnouncingDecontamination(ev);
            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(DecontaminationController), nameof(DecontaminationController._nextPhase))),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(AnnouncingDecontaminationEventArgs))[0]),
                new(OpCodes.Call, Method(typeof(Map), nameof(Map.OnAnnouncingDecontamination))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
