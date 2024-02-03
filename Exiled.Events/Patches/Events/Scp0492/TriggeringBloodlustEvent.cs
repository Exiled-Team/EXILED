// -----------------------------------------------------------------------
// <copyright file="TriggeringBloodlustEvent.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp0492
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.API.Features.Core.Generic.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp0492;

    using HarmonyLib;

    using PlayerRoles.PlayableScps.Scp049.Zombies;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches the <see cref="ZombieBloodlustAbility.AnyTargets"/> method to add the <see cref="Handlers.Scp0492.TriggeringBloodlust"/>.
    /// </summary>
    [EventPatch(typeof(Handlers.Scp0492), nameof(Handlers.Scp0492.TriggeringBloodlust))]
    [HarmonyPatch(typeof(ZombieBloodlustAbility), nameof(ZombieBloodlustAbility.AnyTargets))]
    internal static class TriggeringBloodlustEvent
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);
            Label continueLabel = newInstructions[newInstructions.FindIndex(i => i.opcode == OpCodes.Leave_S) + 1].labels[0];

            const int offset = -1;
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Stloc_S) + offset;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Ldloc_1),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                new(OpCodes.Ldarg_1),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(TriggeringBloodlustEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Call, Method(typeof(Handlers.Scp0492), nameof(Handlers.Scp0492.OnTriggeringBloodlust))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(TriggeringBloodlustEventArgs), nameof(TriggeringBloodlustEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, continueLabel),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}