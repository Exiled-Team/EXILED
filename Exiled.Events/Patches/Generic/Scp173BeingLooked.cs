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

    using HarmonyLib;

    using PlayerRoles;
    using PlayerRoles.PlayableScps.Scp173;

    using static HarmonyLib.AccessTools;

    using ExiledEvents = Exiled.Events.Events;
    using Scp173Role = API.Features.Roles.Scp173Role;

    /// <summary>
    /// Patches <see cref="Scp173ObserversTracker.UpdateObserver(ReferenceHub)"/>.
    /// </summary>
    [HarmonyPatch(typeof(Scp173ObserversTracker), nameof(Scp173ObserversTracker.UpdateObserver))]
    internal static class Scp173BeingLooked
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label continueLabel = generator.DefineLabel();
            Label skipLabel = generator.DefineLabel();

            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                // if(Scp173BeingLooked.HelpMethod(Scp173BeingLooked, ReferenceHub))
                //     return this.Observers.Remove(ReferenceHub) ? -1 : 0;
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Call, Method(typeof(Scp173BeingLooked), nameof(HelpMethod))),

                new(OpCodes.Brfalse_S, continueLabel),

                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(Scp173ObserversTracker), nameof(Scp173ObserversTracker.Observers))),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Callvirt, Method(typeof(HashSet<ReferenceHub>), nameof(HashSet<ReferenceHub>.Remove))),
                new(OpCodes.Brtrue_S, skipLabel),

                new(OpCodes.Ldc_I4_0),
                new(OpCodes.Ret),

                new CodeInstruction(OpCodes.Ldc_I4_M1).WithLabels(skipLabel),
                new(OpCodes.Ret),

                new CodeInstruction(OpCodes.Nop).WithLabels(continueLabel),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private static bool HelpMethod(Scp173ObserversTracker instance, ReferenceHub targetHub)
        {
            return Player.Get(targetHub) is Player player && ((player.Role.Type == RoleTypeId.Tutorial && !ExiledEvents.Instance.Config.CanTutorialBlockScp173) || Scp173Role.TurnedPlayers.Contains(player)) && instance.IsObservedBy(targetHub, Scp173ObserversTracker.WidthMultiplier);
        }
    }
}