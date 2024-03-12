// -----------------------------------------------------------------------
// <copyright file="GhostModePatch.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Pools;
    using API.Features.Roles;

    using HarmonyLib;

    using PlayerRoles.FirstPersonControl.NetworkMessages;
    using PlayerRoles.Visibility;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="FpcServerPositionDistributor.WriteAll"/> to add <see cref="FpcRole.IsInvisible"/> and <see cref="FpcRole.IsInvisibleFor"/> functional.
    /// </summary>
    [HarmonyPatch(typeof(FpcServerPositionDistributor), nameof(FpcServerPositionDistributor.WriteAll))]
    internal class GhostModePatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            const int offset = 6;
            int index = newInstructions.FindIndex(
                instruction => instruction.Calls(Method(typeof(VisibilityController), nameof(VisibilityController.ValidateVisibility)))) + offset;

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // receiver
                    new(OpCodes.Ldarg_0),

                    // referenceHub
                    new(OpCodes.Ldloc_S, 5),

                    // flag2
                    new(OpCodes.Ldloca_S, 7),

                    // HandleGhostMode(ReferenceHub, ReferenceHub, ref bool)
                    new(OpCodes.Call, Method(typeof(GhostModePatch), nameof(HandleGhostMode), new[] { typeof(ReferenceHub), typeof(ReferenceHub), typeof(bool).MakeByRefType() })),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private static void HandleGhostMode(ReferenceHub hubReceiver, ReferenceHub hubTarget, ref bool isInvisible)
        {
            // If the player is already invisible, return
            if (isInvisible)
                return;

            if (Player.Get(hubReceiver) is not Player receiver || Player.Get(hubTarget) is not Player target)
                return;

            isInvisible = target.Role.Is(out FpcRole role) && (role.IsInvisible || role.IsInvisibleFor.Contains(receiver));
        }
    }
}
