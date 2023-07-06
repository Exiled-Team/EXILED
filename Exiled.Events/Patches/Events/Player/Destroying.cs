// -----------------------------------------------------------------------
// <copyright file="Destroying.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;

    using API.Features;
    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patch the <see cref="ReferenceHub.OnDestroy" />.
    ///     Adds the <see cref="Handlers.Player.Destroying" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.Destroying))]
    [HarmonyPatch(typeof(ReferenceHub), nameof(ReferenceHub.OnDestroy))]
    internal static class Destroying
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label continueLabel = generator.DefineLabel();
            Label jmp = generator.DefineLabel();

            LocalBuilder player = generator.DeclareLocal(typeof(Player));

            newInstructions[0].labels.Add(continueLabel);

            newInstructions.InsertRange(
                0,
                new CodeInstruction[]
                {
                    // Player player = Player.Get(this)
                    //
                    // if (player == null)
                    //    goto continueLabel;
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, player.LocalIndex),
                    new(OpCodes.Ldnull),
                    new(OpCodes.Ceq),
                    new(OpCodes.Brtrue_S, continueLabel),

                    // if (player.IsVerified)
                    //  goto jmp
                    new(OpCodes.Ldloc_S, player.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.IsVerified))),
                    new(OpCodes.Brtrue_S, jmp),

                    // if (!player.IsNpc)
                    //  goto continueLabel;
                    new(OpCodes.Ldloc_S, player.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(API.Features.Player.IsNPC))),
                    new(OpCodes.Brfalse_S, continueLabel),

                    // jmp:
                    // DestroyingEventArgs ev = new(Player)
                    new CodeInstruction(OpCodes.Ldloc_S, player.LocalIndex).WithLabels(jmp),
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(DestroyingEventArgs))[0]),

                    // Handlers.Player.OnDestroying(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnDestroying))),

                    // Player.Dictionary.Remove(player.GameObject)
                    new(OpCodes.Call, PropertyGetter(typeof(Player), nameof(Player.Dictionary))),
                    new(OpCodes.Ldloc_S, player.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.GameObject))),
                    new(OpCodes.Callvirt, Method(typeof(Dictionary<GameObject, Player>), nameof(Dictionary<GameObject, Player>.Remove), new[] { typeof(GameObject) })),
                    new(OpCodes.Pop),

                    // Player.UnverifiedPlayers.Remove(this.gameObject)
                    new(OpCodes.Call, PropertyGetter(typeof(Player), nameof(Player.UnverifiedPlayers))),
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, PropertyGetter(typeof(ReferenceHub), nameof(ReferenceHub.gameObject))),
                    new(OpCodes.Callvirt, Method(typeof(ConditionalWeakTable<ReferenceHub, Player>), nameof(ConditionalWeakTable<ReferenceHub, Player>.Remove), new[] { typeof(ReferenceHub) })),
                    new(OpCodes.Pop),

                    // if (player.UserId == null)
                    //    goto continueLabel;
                    new(OpCodes.Ldloc_S, player.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.UserId))),
                    new(OpCodes.Ldnull),
                    new(OpCodes.Ceq),
                    new(OpCodes.Brtrue_S, continueLabel),

                    // Player.UserIdsCache.Remove(player.UserId)
                    new(OpCodes.Call, PropertyGetter(typeof(Player), nameof(Player.UserIdsCache))),
                    new(OpCodes.Ldloc_S, player.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.UserId))),
                    new(OpCodes.Callvirt, Method(typeof(Dictionary<string, Player>), nameof(Dictionary<string, Player>.Remove), new[] { typeof(string) })),
                    new(OpCodes.Pop),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}