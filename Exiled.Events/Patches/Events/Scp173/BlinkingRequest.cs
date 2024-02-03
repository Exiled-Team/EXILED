// -----------------------------------------------------------------------
// <copyright file="BlinkingRequest.cs" company="Exiled Team">
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
    using API.Features.Core.Generic.Pools;

    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp173;

    using HarmonyLib;

    using PlayerRoles.PlayableScps.Scp173;
    using PlayerRoles.Subroutines;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp173TeleportAbility.ServerProcessCmd" />.
    /// Adds the <see cref="Handlers.Scp173.BlinkingRequest" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Scp173), nameof(Handlers.Scp173.BlinkingRequest))]
    [HarmonyPatch(typeof(Scp173TeleportAbility), nameof(Scp173TeleportAbility.ServerProcessCmd))]
    internal static class BlinkingRequest
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();

            int offset = -3;
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Stloc_1) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player.Get(this.Owner)
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(StandardSubroutine<Scp173Role>), nameof(StandardSubroutine<Scp173Role>.Owner))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // this._observersTracker.Observers
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(Scp173TeleportAbility), nameof(Scp173TeleportAbility._observersTracker))),
                    new(OpCodes.Ldfld, Field(typeof(Scp173ObserversTracker), nameof(Scp173ObserversTracker.Observers))),

                    // BlinkingRequestEventArgs ev = new(Player, HashSet<Player>)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(BlinkingRequestEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Scp173.OnBlinkingRequest(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Scp173), nameof(Handlers.Scp173.OnBlinkingRequest))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(BlinkingRequestEventArgs), nameof(BlinkingRequestEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse, returnLabel),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}