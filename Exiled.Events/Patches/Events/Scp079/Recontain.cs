// -----------------------------------------------------------------------
// <copyright file="Recontain.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Core.Generic.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp079;
    using Exiled.Events.Handlers;

    using HarmonyLib;
    using PlayerRoles.PlayableScps.Scp079;
    using PlayerStatsSystem;

    using static HarmonyLib.AccessTools;

    using Player = API.Features.Player;

    /// <summary>
    /// Patches <see cref="Scp079Recontainer.TryKill079()" />.
    /// Adds the <see cref="Scp079.Recontained" /> event.
    /// </summary>
    [EventPatch(typeof(Scp079), nameof(Scp079.Recontained))]
    [HarmonyPatch(typeof(Scp079Recontainer), nameof(Scp079Recontainer.TryKill079))]
    internal static class Recontain
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            CodeInstruction[] recontainedEvent = new CodeInstruction[]
                {
                    // Handlers.Scp079.OnRecontained(new RecontainedEventArgs(Player.Get(referenceHub2)));
                    new(OpCodes.Ldloc, 5),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(RecontainedEventArgs))[0]),
                    new(OpCodes.Call, Method(typeof(Scp079), nameof(Scp079.OnRecontained))),
                };

            int offset = 1;
            int index = newInstructions.FindIndex(x => x.operand == (object)Method(typeof(PlayerStats), nameof(PlayerStats.DealDamage))) + offset;

            newInstructions.InsertRange(index, recontainedEvent);

            index = newInstructions.FindLastIndex(x => x.operand == (object)Method(typeof(PlayerStats), nameof(PlayerStats.DealDamage))) + offset;

            newInstructions.InsertRange(index, recontainedEvent);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}