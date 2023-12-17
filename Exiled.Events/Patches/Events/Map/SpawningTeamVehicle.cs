// -----------------------------------------------------------------------
// <copyright file="SpawningTeamVehicle.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Map;

    using HarmonyLib;

    using Respawning;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="RespawnEffectsController.ExecuteAllEffects(RespawnEffectsController.EffectType, SpawnableTeamType)"/>.
    /// Adds the <see cref="Handlers.Map.SpawningTeamVehicle"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Map), nameof(Handlers.Map.SpawningTeamVehicle))]
    [HarmonyPatch(typeof(RespawnEffectsController), nameof(RespawnEffectsController.ExecuteAllEffects))]
    internal static class SpawningTeamVehicle
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label retLabel = generator.DefineLabel();
            Label continueLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(SpawningTeamVehicleEventArgs));

            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                // if (type != RespawnEffectsController.EffectType.Selection)
                //    goto continueLabel;
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldc_I4_0),
                new(OpCodes.Ceq),
                new(OpCodes.Brfalse_S, continueLabel),

                // team
                new(OpCodes.Ldarg_1),

                // true
                new(OpCodes.Ldc_I4_1),

                // SpawningTeamVehicleEventArgs ev = new(SpawnableTeamType, bool);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(SpawningTeamVehicleEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev.LocalIndex),

                // Handlers.Map.OnSpawningTeamVehicle(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Map), nameof(Handlers.Map.OnSpawningTeamVehicle))),

                // if (!ev.IsAllowed)
                //    return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(SpawningTeamVehicleEventArgs), nameof(SpawningTeamVehicleEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, retLabel),

                // team = ev.Team
                new(OpCodes.Ldloc_S, ev),
                new(OpCodes.Callvirt, PropertyGetter(typeof(SpawningTeamVehicleEventArgs), nameof(SpawningTeamVehicleEventArgs.Team))),
                new(OpCodes.Starg_S, 1),

                new CodeInstruction(OpCodes.Nop).WithLabels(continueLabel),
            });

            newInstructions[newInstructions.Count - 1].WithLabels(retLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}