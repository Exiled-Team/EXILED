// -----------------------------------------------------------------------
// <copyright file="Scp173BeingLooked.cs" company="Exiled Team">
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

    using PlayableScps;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="PlayableScps.Scp173.UpdateObservers"/>.
    /// </summary>
    [HarmonyPatch(typeof(PlayableScps.Scp173), nameof(PlayableScps.Scp173.UpdateObservers))]
    internal static class Scp173BeingLooked
    {
        /// <summary>
        /// Checks if the current player is to be skipped.
        /// </summary>
        /// <param name="instance"> Scp173 instance <see cref="PlayableScps.Scp173"/>. </param>
        /// <param name="curPlayerHub"> Current player referencehub. </param>
        /// <returns> True if to be skipped, false if not. </returns>
        public static bool SkipPlayer(ref PlayableScps.Scp173 instance, ReferenceHub curPlayerHub)
        {
            Player player = Player.Get(curPlayerHub);
            if (player is not null)
            {
                if (player.Role.Type == RoleType.Tutorial && !Exiled.Events.Events.Instance.Config.CanTutorialBlockScp173)
                {
                    instance._observingPlayers.Remove(curPlayerHub);
                    return true;
                }
                else if (API.Features.Scp173.TurnedPlayers.Contains(player))
                {
                    instance._observingPlayers.Remove(curPlayerHub);
                    return true;
                }
            }

            return false;
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label cnt = generator.DefineLabel();

            int removeTurnedPeanutOffset = 2;
            int removeTurnedPeanut = newInstructions.FindIndex(instruction => instruction.Calls(PropertyGetter(typeof(HashSet<ReferenceHub>), nameof(HashSet<ReferenceHub>.Count)))) + removeTurnedPeanutOffset;

            newInstructions.InsertRange(removeTurnedPeanut, new CodeInstruction[]
            {
                new(OpCodes.Call, PropertyGetter(typeof(API.Features.Scp173), nameof(API.Features.Scp173.TurnedPlayers))),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(PlayableScp), nameof(PlayableScp.Hub))),
                new(OpCodes.Callvirt, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Callvirt, Method(typeof(HashSet<Player>), nameof(HashSet<Player>.Remove))),
                new(OpCodes.Pop),
            });

            int skipPlayerCheckOffset = 2;
            int skipPlayerCheck = newInstructions.FindIndex(instruction => instruction.Calls(PropertyGetter(typeof(PlayerMovementSync), nameof(PlayerMovementSync.RealModelPosition)))) + skipPlayerCheckOffset;

            newInstructions.InsertRange(skipPlayerCheck, new CodeInstruction[]
            {
                new(OpCodes.Ldarga, 0),
                new(OpCodes.Ldloc_3),
                new(OpCodes.Call, Method(typeof(Scp173BeingLooked), nameof(Scp173BeingLooked.SkipPlayer), new[] { typeof(API.Features.Scp173).MakeByRefType(), typeof(ReferenceHub) })),

                // If true, skip adding to watching
                new(OpCodes.Brtrue, cnt),
            });

            int continueBr = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Br);

            newInstructions[continueBr].labels.Add(cnt);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
