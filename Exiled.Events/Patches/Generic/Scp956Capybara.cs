// -----------------------------------------------------------------------
// <copyright file="Scp956Capybara.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using System.Collections.Generic;

    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using HarmonyLib;
    using UnityEngine;

    using static HarmonyLib.AccessTools;

    [HarmonyPatch(typeof(Scp956Pinata), nameof(Scp956Pinata.UpdateAi))]
    internal class Scp956Capybara
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int offset = -3;
            int index = newInstructions.FindIndex(x => x.Calls(Method(typeof(Random), nameof(Random.Range)))) + offset;

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private static void SetCapybara(Scp956Pinata __instance)
        {
            if (Exiled.Events.Events.Instance.Config.Is956Capybara || Scp956.IsCapybara)
                __instance.Network_carpincho = 69;
        }
    }
}