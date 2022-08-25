// -----------------------------------------------------------------------
// <copyright file="InitLureContainerInstance.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="LureSubjectContainer.Start"/> to initialize <see cref="Scp106Container"/>.
    /// </summary>
    [HarmonyPatch(typeof(LureSubjectContainer), nameof(LureSubjectContainer.Start))]
    internal class InitLureContainerInstance
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            newInstructions.InsertRange(
                0,
                new CodeInstruction[]
                {
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, Method(typeof(InitLureContainerInstance), nameof(Start))),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        private static void Start(LureSubjectContainer container)
        {
            Scp106Container.Base = container;
            Scp106Container.BoxCollider = container.GetComponent<BoxCollider>();
        }
    }
}