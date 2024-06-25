// -----------------------------------------------------------------------
// <copyright file="PlayerCuffedPatch.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using HarmonyLib;
    using InventorySystem.Disarming;
    using PlayerRoles;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="DisarmingHandlers.ServerProcessDisarmMessage"/>.
    /// </summary>
    [HarmonyPatch(typeof(DisarmingHandlers), nameof(DisarmingHandlers.ServerProcessDisarmMessage))]
    internal static class TutorialHandcuffImmunity
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label continueLabel = generator.DefineLabel();
            Label skipLabel = generator.DefineLabel();

            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                // if (PlayerCuffed.IsTutorial(DisarmMessage.PlayerToDisarm))
                //     return;
                new(OpCodes.Ldarg_1),
                new(OpCodes.Ldfld, Field(typeof(DisarmMessage), nameof(DisarmMessage.PlayerToDisarm))),
                new(OpCodes.Call, Method(typeof(TutorialHandcuffImmunity), nameof(IsHandcuffImmune))),

                new(OpCodes.Brfalse_S, continueLabel),
                new(OpCodes.Ret),

                new CodeInstruction(OpCodes.Nop).WithLabels(continueLabel),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
        private static bool IsHandcuffImmune(ReferenceHub target) => Exiled.Events.Events.Instance.Config.CanTutorialsBeHandcuffed ? false : Player.Get(target)?.Role.Type == RoleTypeId.Tutorial;
    }
}
