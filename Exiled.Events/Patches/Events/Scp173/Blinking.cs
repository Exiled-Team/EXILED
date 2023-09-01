// -----------------------------------------------------------------------
// <copyright file="Blinking.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp173
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Pools;
    using Exiled.Events.EventArgs.Scp173;

    using HarmonyLib;

    using PlayerRoles;

    using PlayerRoles.PlayableScps.Scp173;
    using PlayerRoles.PlayableScps.Subroutines;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="Scp173BlinkTimer.ServerBlink(Vector3)" />.
    ///     Adds the <see cref="Handlers.Scp173.Blinking" /> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp173BlinkTimer), nameof(Scp173BlinkTimer.ServerBlink))]
    internal static class Blinking
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(BlinkingEventArgs));

            Label returnLabel = generator.DefineLabel();

            newInstructions.InsertRange(
                0,
                new[]
                {
                    // Player.Get(base.Role._lastOwner)
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ScpSubroutineBase), nameof(ScpSubroutineBase.Role))),
                    new(OpCodes.Ldfld, Field(typeof(PlayerRoleBase), nameof(PlayerRoleBase._lastOwner))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // this._observers.Observers
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(Scp173BlinkTimer), nameof(Scp173BlinkTimer._observers))),
                    new(OpCodes.Ldfld, Field(typeof(Scp173ObserversTracker), nameof(Scp173ObserversTracker.Observers))),
                    new(OpCodes.Call, Method(typeof(Blinking), nameof(GetObservingPlayers))),

                    // pos
                    new(OpCodes.Ldarg_1),

                    // BlinkingEventArgs ev = new(Player, List<Player>, Vector3)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(BlinkingEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc, ev.LocalIndex),

                    // Handlers.Scp173.OnBlinking(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Scp173), nameof(Handlers.Scp173.OnBlinking))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(BlinkingEventArgs), nameof(BlinkingEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse, returnLabel),

                    // pos = ev.BlinkPosition
                    new(OpCodes.Ldloc, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(BlinkingEventArgs), nameof(BlinkingEventArgs.BlinkPosition))),
                    new(OpCodes.Starg, 1),

                    // this._totalCooldown = ev.BlinkCooldown
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldloc, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(BlinkingEventArgs), nameof(BlinkingEventArgs.BlinkCooldown))),
                    new(OpCodes.Stfld, Field(typeof(Scp173BlinkTimer), nameof(Scp173BlinkTimer._totalCooldown))),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private static List<Player> GetObservingPlayers(HashSet<ReferenceHub> hubs) => hubs.Select(Player.Get).ToList();
    }
}