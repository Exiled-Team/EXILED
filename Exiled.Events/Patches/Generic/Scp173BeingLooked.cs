// -----------------------------------------------------------------------
// <copyright file="Scp173BeingLooked.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;

    using HarmonyLib;

    using MapGeneration;

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
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label jne = generator.DefineLabel();
            Label cnt = generator.DefineLabel();
            Label isTutorial = generator.DefineLabel();
            Label samePlayer = generator.DefineLabel();

            int addCheckOffset = 4;
            int addCheck = newInstructions.FindLastIndex(instruction => instruction.Calls(Method(typeof(Physics), nameof(Physics.Linecast), new[] { typeof(Vector3), typeof(Vector3), typeof(int) }))) + addCheckOffset;

            LocalBuilder player = generator.DeclareLocal(typeof(Player));

            LocalBuilder turnedPlayers = generator.DeclareLocal(typeof(HashSet<Player>));

            newInstructions.InsertRange(addCheck, new CodeInstruction[]
            {
                // Player.get(current player in all hub)
                new(OpCodes.Ldloc_3),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Dup),
                new(OpCodes.Stloc, player.LocalIndex),

                // Player is null (Generic static ref hub for example)
                new(OpCodes.Brfalse_S, jne),

                // Player.role == Tutorial
                new(OpCodes.Ldloc, player.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.Role))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(API.Features.Roles.Role), nameof(API.Features.Roles.Role.Type))),
                new(OpCodes.Ldc_I4_S, (int)RoleType.Tutorial),

                // Skip to checking if tutorial should be blocked
                new(OpCodes.Beq_S, isTutorial),

                // Scp173.TurnedPlayers.Remove(Current 173)
                new(OpCodes.Call, PropertyGetter(typeof(API.Features.Scp173), nameof(API.Features.Scp173.TurnedPlayers))),
                new(OpCodes.Stloc, turnedPlayers.LocalIndex),
                new(OpCodes.Ldloc, turnedPlayers.LocalIndex),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(PlayableScp), nameof(PlayableScp.Hub))),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Callvirt, Method(typeof(HashSet<Player>), nameof(HashSet<Player>.Remove))),

                // Scp173.TurnedPlayers.Contains(Current looking player)
                new(OpCodes.Ldloc, turnedPlayers.LocalIndex),
                new(OpCodes.Ldloc, player.LocalIndex),
                new(OpCodes.Callvirt, Method(typeof(HashSet<Player>), nameof(HashSet<Player>.Contains))),

                // If true, skip adding to watching
                new(OpCodes.Brtrue_S, cnt),

                // If the player is tutorial, and allowed to not block, skip adding to watching
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(Exiled.Events.Events), nameof(Exiled.Events.Events.Instance))).WithLabels(isTutorial),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Exiled.Events.Events), nameof(Exiled.Events.Events.Config))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Config), nameof(Exiled.Events.Events.Config.CanTutorialBlockScp173))),
                new(OpCodes.Brfalse_S, cnt),
            });

            int offset = -3;
            int defaultLogic = newInstructions.FindIndex(addCheck, instruction => instruction.LoadsField(Field(typeof(PlayableScps.Scp173), nameof(PlayableScps.Scp173._observingPlayers)))) + offset;

            int continueBr = newInstructions.FindIndex(addCheck, instruction => instruction.opcode == OpCodes.Br_S);

            newInstructions[defaultLogic].labels.Add(jne);
            newInstructions[continueBr].labels.Add(cnt);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
