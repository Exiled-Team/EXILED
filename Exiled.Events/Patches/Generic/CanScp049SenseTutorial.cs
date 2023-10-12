// -----------------------------------------------------------------------
// <copyright file="CanScp049SenseTutorial.cs" company="Exiled Team">
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

    using HarmonyLib;

    using PlayerRoles;
    using PlayerRoles.PlayableScps.Scp049;

    using static HarmonyLib.AccessTools;

    using ExiledEvents = Exiled.Events.Events;

    /// <summary>
    /// Patches <see cref="Scp049SenseAbility.CanFindTarget(out ReferenceHub)"/>.
    /// <see cref="Config.CanScp049SenseTutorial"/>.
    /// </summary>
    [HarmonyPatch(typeof(Scp049SenseAbility), nameof(Scp049SenseAbility.CanFindTarget))]
    internal static class CanScp049SenseTutorial
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Brfalse);

            Label returnLabel = (Label)newInstructions[index].operand;
            Label skip = generator.DefineLabel();
            Label continueLabel = generator.DefineLabel();

            index += 1;

            newInstructions[index].labels.Add(continueLabel);

            // if ((referenceHub.roleManager.CurrentRole.RoleTypeId == RoleTypeId.Tutorial && ExiledEvents.Instance.Config.CanScp049SenseTutorial) || Scp049Role.TurnedPlayers.Contains(Player.Get(referenceHub)))
            //     return;
            newInstructions.InsertRange(
                index,
                new[]
                {
                    // if (referenceHub.roleManager.CurrentRole.RoleTypeId == RoleTypeId.Tutorial)
                    new(OpCodes.Ldloca_S, 6),
                    new(OpCodes.Ldfld, Field(typeof(ReferenceHub), nameof(ReferenceHub.roleManager))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(PlayerRoleManager), nameof(PlayerRoleManager.CurrentRole))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(PlayerRoleBase), nameof(PlayerRoleBase.RoleTypeId))),
                    new(OpCodes.Ldc_I4_S, (sbyte)RoleTypeId.Tutorial),
                    new(OpCodes.Bne_Un_S, skip),

                    // if (!ExiledEvents.Instance.Config.CanScp049SenseTutorial)
                    new(OpCodes.Call, PropertyGetter(typeof(ExiledEvents), nameof(ExiledEvents.Instance))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Plugin<Config>), nameof(Plugin<Config>.Config))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Config), nameof(Config.CanScp049SenseTutorial))),
                    new(OpCodes.Brfalse_S, returnLabel),

                    // if (Scp049Role.TurnedPlayers.Contains(Player.Get(referenceHub)))
                    new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(API.Features.Roles.Scp049Role), nameof(API.Features.Roles.Scp049Role.TurnedPlayers))).WithLabels(skip),
                    new(OpCodes.Ldloca_S, 6),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                    new(OpCodes.Callvirt, Method(typeof(HashSet<Player>), nameof(HashSet<Player>.Contains))),
                    new(OpCodes.Brtrue_S, returnLabel),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}