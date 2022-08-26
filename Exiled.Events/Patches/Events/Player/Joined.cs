// -----------------------------------------------------------------------
// <copyright file="Joined.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1600

    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;
    using Exiled.Loader.Features;

    using HarmonyLib;

    using MEC;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="ReferenceHub.Awake" />.
    ///     Adds the <see cref="Handlers.Player.Joined" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.Joined))]
    [HarmonyPatch(typeof(ReferenceHub), nameof(ReferenceHub.Awake))]
    internal static class Joined
    {
        internal static void CallEvent(ReferenceHub hub, out Player player)
        {
            try
            {
#if DEBUG
                Log.Debug("Creating new player object");
#endif
                player = new Player(hub);
#if DEBUG
                Log.Debug($"Object exists {player is not null}");
                Log.Debug($"Creating player object for {hub.nicknameSync.Network_displayName}");
#endif
                Player.UnverifiedPlayers.Add(hub, player);
                Player p = player;
                Timing.CallDelayed(0.25f, () =>
                {
                    if (p.IsMuted)
                        p.ReferenceHub.characterClassManager.SetDirtyBit(2UL);
                });

                Handlers.Player.OnJoined(new JoinedEventArgs(player));
            }
            catch (Exception e)
            {
                Log.Error($"{nameof(CallEvent)}: {e}\n{e.StackTrace}");
                player = null;
            }
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            LocalBuilder out_ply = generator.DeclareLocal(typeof(Player));

            Label cdc = generator.DefineLabel();
            Label je = generator.DefineLabel();

            newInstructions[newInstructions.Count - 1].labels.Add(cdc);

            newInstructions.InsertRange(newInstructions.Count - 1, new[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ReferenceHub), nameof(ReferenceHub.isDedicatedServer))),
                new(OpCodes.Brtrue_S, cdc),
                new(OpCodes.Call, PropertyGetter(typeof(ReferenceHub), nameof(ReferenceHub.HostHub))),
                new(OpCodes.Ldnull),
                new(OpCodes.Ceq),
                new(OpCodes.Brtrue_S, cdc),
                new(OpCodes.Ldsfld, Field(typeof(PlayerManager), nameof(PlayerManager.localPlayer))),
                new(OpCodes.Ldnull),
                new(OpCodes.Ceq),
                new(OpCodes.Brtrue_S, cdc),
                new(OpCodes.Ldsfld, Field(typeof(PlayerManager), nameof(PlayerManager.players))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(List<GameObject>), nameof(List<GameObject>.Count))),
                new(OpCodes.Ldsfld, Field(typeof(CustomNetworkManager), nameof(CustomNetworkManager.slots))),
                new(OpCodes.Bge_S, je),
                new(OpCodes.Ldc_I4_4),
                new(OpCodes.Call, Method(typeof(MultiAdminFeatures), nameof(MultiAdminFeatures.CallEvent))),
                new CodeInstruction(OpCodes.Ldarg_0).WithLabels(je),
                new(OpCodes.Ldloca_S, out_ply),
                new(OpCodes.Call, Method(typeof(Joined), nameof(CallEvent))),
                new(OpCodes.Pop),
                new CodeInstruction(OpCodes.Ldarg_0).WithLabels(je),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(API.Features.Player))[0]),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(JoinedEventArgs))[0]),
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnJoined))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
