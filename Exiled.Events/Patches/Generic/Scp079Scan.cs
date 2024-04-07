// -----------------------------------------------------------------------
// <copyright file="Scp079Scan.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Core.Generic.Pools;

    using HarmonyLib;

    using PlayerRoles;
    using PlayerRoles.PlayableScps.Scp079;

    using static HarmonyLib.AccessTools;

    using ExiledEvents = Exiled.Events.Events;

    /// <summary>
    /// Patches <see cref="Scp079ScannerTracker.AddTarget(ReferenceHub)"/>.
    /// </summary>
    [HarmonyPatch(typeof(Scp079ScannerTracker), nameof(Scp079ScannerTracker.AddTarget))]
    internal static class Scp079Scan
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();
            Label skip = generator.DefineLabel();

            // if ((referenceHub.roleManager.CurrentRole.RoleTypeId == RoleTypeId.Tutorial && ExiledEvents.Instance.Config.TutorialNotAffectedByScp079Scan) || Scp079Role.TurnedPlayers.Contains(Player.Get(referenceHub)))
            //     return;
            newInstructions.InsertRange(
                0,
                new[]
                {
                    // if (referenceHub.roleManager.CurrentRole.RoleTypeId == RoleTypeId.Tutorial)
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Ldfld, Field(typeof(ReferenceHub), nameof(ReferenceHub.roleManager))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(PlayerRoleManager), nameof(PlayerRoleManager.CurrentRole))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(PlayerRoleBase), nameof(PlayerRoleBase.RoleTypeId))),
                    new(OpCodes.Ldc_I4_S, (sbyte)RoleTypeId.Tutorial),
                    new(OpCodes.Bne_Un_S, skip),

                    // if (!ExiledEvents.Instance.Config.TutorialNotAffectedByScp079Scan)
                    new(OpCodes.Call, PropertyGetter(typeof(ExiledEvents), nameof(ExiledEvents.Instance))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Plugin<Config>), nameof(Plugin<Config>.Config))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Config), nameof(Config.TutorialNotAffectedByScp079Scan))),
                    new(OpCodes.Brfalse_S, returnLabel),

                    // if (Scp079Role.TurnedPlayers.Contains(Player.Get(referenceHub)))
                    new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(API.Features.Roles.Scp079Role), nameof(API.Features.Roles.Scp079Role.TurnedPlayers))).WithLabels(skip),
                    new(OpCodes.Ldarg_1),
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