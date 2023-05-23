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

    using API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.API.Features.Roles;
    using HarmonyLib;

    using PlayerRoles;
    using PlayerRoles.PlayableScps.Scp049;

    using static HarmonyLib.AccessTools;

    using ExiledEvents = Exiled.Events.Events;
    using Scp049Role = Exiled.API.Features.Roles.Scp049Role;

    /// <summary>
    /// Patches <see cref="Scp049SenseAbility.CanFindTarget(out ReferenceHub)"/>.
    /// </summary>
    [HarmonyPatch(typeof(Scp049SenseAbility), nameof(Scp049SenseAbility.CanFindTarget))]
    internal static class Scp049BeingLooked
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder player = generator.DeclareLocal(typeof(Player));

            Label returnLabel;
            Label skipLabel = generator.DefineLabel();
            Label nextCheck = generator.DefineLabel();

            int offset = 1;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Brfalse);
            returnLabel = (Label)newInstructions[index].operand;

            index += offset;

            newInstructions[index].labels.Add(skipLabel);
            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // if ((Player player = Player.Get(referencehub) is null) GOTO skipLabel;
                new(OpCodes.Ldloc_S, 6),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, player.LocalIndex),
                new(OpCodes.Brfalse, skipLabel),

                // if (player.Role.Type != RoleTypeId.Tutorial) GOTO NextCheck;
                new(OpCodes.Ldloc_S, player.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.Role))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Role), nameof(Role.Type))),
                new(OpCodes.Ldc_I4_S, (int)RoleTypeId.Tutorial),
                new(OpCodes.Bne_Un_S, nextCheck),

                // if (ExiledEvents.Instance.Config.CanScp049SenseTutorial) GOTO returnLabel;
                new(OpCodes.Call, PropertyGetter(typeof(ExiledEvents), nameof(ExiledEvents.Instance))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ExiledEvents), nameof(ExiledEvents.Config))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Config), nameof(Config.CanScp049SenseTutorial))),
                new(OpCodes.Brfalse_S, returnLabel),

                // if (Scp049Role.TurnedPlayers.Contains(player)) GOTO returnLabel;
                new CodeInstruction(OpCodes.Call, Method(typeof(Scp049Role), nameof(Scp049Role.TurnedPlayers))).WithLabels(nextCheck),
                new(OpCodes.Ldloc_S, player.LocalIndex),
                new(OpCodes.Callvirt, Method(typeof(HashSet<Player>), nameof(HashSet<Player>.Contains))),
                new(OpCodes.Brfalse_S, returnLabel),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}