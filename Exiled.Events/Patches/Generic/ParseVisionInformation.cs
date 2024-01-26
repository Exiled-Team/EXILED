// -----------------------------------------------------------------------
// <copyright file="ParseVisionInformation.cs" company="Exiled Team">
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
    using PlayerRoles.PlayableScps.Scp096;

    using static HarmonyLib.AccessTools;

    using ExiledEvents = Exiled.Events.Events;
    using Scp096Role = API.Features.Roles.Scp096Role;

    /// <summary>
    /// Patches <see cref="Scp096TargetsTracker.IsObservedBy(ReferenceHub)"/>.
    /// Adds the <see cref="Scp096Role.TurnedPlayers"/> support.
    /// </summary>
    [HarmonyPatch(typeof(Scp096TargetsTracker), nameof(Scp096TargetsTracker.IsObservedBy))]
    internal static class ParseVisionInformation
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label continueLabel = generator.DefineLabel();

            Label returnLabel = generator.DefineLabel();

            // Second check pointer
            // We use it to pass execution
            // to the second check if the first check fails,
            // otherwise the second check won't be executed
            Label secondCheckPointer = generator.DefineLabel();

            newInstructions[0].WithLabels(continueLabel);

            // if (referenceHub.roleManager.CurrentRole.RoleTypeId == RoleTypeId.Tutorial && !ExiledEvents.Instance.Config.CanTutorialTriggerScp096
            // || Scp096Role.TurnedPlayers.Contains(Player.Get(referenceHub)))
            //      return false;
            newInstructions.InsertRange(
                0,
                new[]
                {
                    // if ((referenceHub.roleManager.CurrentRole.RoleTypeId == RoleTypeId.Tutorial &&
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Ldfld, Field(typeof(ReferenceHub), nameof(ReferenceHub.roleManager))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(PlayerRoleManager), nameof(PlayerRoleManager.CurrentRole))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(PlayerRoleBase), nameof(PlayerRoleBase.RoleTypeId))),
                    new(OpCodes.Ldc_I4_S, (sbyte)RoleTypeId.Tutorial),
                    new(OpCodes.Bne_Un_S, secondCheckPointer),

                    // !ExiledEvents.Instance.Config.CanTutorialTriggerScp096)
                    new(OpCodes.Call, PropertyGetter(typeof(ExiledEvents), nameof(ExiledEvents.Instance))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Plugin<Config>), nameof(Plugin<Config>.Config))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Config), nameof(Config.CanTutorialTriggerScp096))),
                    new(OpCodes.Brfalse_S, returnLabel),

                    // || Scp096Role.TurnedPlayers.Contains(Player.Get(referenceHub))
                    new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(Scp096Role), nameof(Scp096Role.TurnedPlayers))).WithLabels(secondCheckPointer),
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                    new(OpCodes.Callvirt, Method(typeof(HashSet<Player>), nameof(HashSet<Player>.Contains))),
                    new(OpCodes.Brfalse_S, continueLabel),

                    // return false;
                    new CodeInstruction(OpCodes.Ldc_I4_0).WithLabels(returnLabel),
                    new(OpCodes.Ret),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}