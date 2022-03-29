// -----------------------------------------------------------------------
// <copyright file="Joined.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1118
#pragma warning disable SA1600
    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs;
    using Exiled.Loader.Features;

    using HarmonyLib;

    using MEC;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="ReferenceHub.Awake"/>.
    /// Adds the <see cref="Handlers.Player.Joined"/> event.
    /// </summary>
    [HarmonyPatch(typeof(ReferenceHub), nameof(ReferenceHub.Awake))]
    internal static class Joined
    {
        internal static void CallEvent(ReferenceHub hub, out API.Features.Player player)
        {
            try
            {
#if DEBUG
                API.Features.Log.Debug("Creating new player object");
#endif
                player = new API.Features.Player(hub);
#if DEBUG
                API.Features.Log.Debug($"Object exists {player is not null}");
                API.Features.Log.Debug($"Creating player object for {hub.nicknameSync.Network_displayName}", true);
#endif
                API.Features.Player.UnverifiedPlayers.Add(hub, player);
                API.Features.Player p = player;
                Timing.CallDelayed(0.25f, () =>
                {
                    if (p.IsMuted)
                        p.ReferenceHub.characterClassManager.SetDirtyBit(2UL);
                });

                Handlers.Player.OnJoined(new JoinedEventArgs(player));
            }
            catch (Exception e)
            {
                API.Features.Log.Error($"{nameof(CallEvent)}: {e}\n{e.StackTrace}");
                player = null;
            }
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            LocalBuilder out_rh = generator.DeclareLocal(typeof(ReferenceHub));

            Label cdc = generator.DefineLabel();
            Label je = generator.DefineLabel();

            newInstructions[newInstructions.Count - 1].labels.Add(cdc);

            newInstructions.InsertRange(newInstructions.Count - 1, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ReferenceHub), nameof(ReferenceHub.isDedicatedServer))),
                new CodeInstruction(OpCodes.Brtrue_S, cdc),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(ReferenceHub), nameof(ReferenceHub.HostHub))),
                new CodeInstruction(OpCodes.Ldnull),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Brtrue_S, cdc),
                new CodeInstruction(OpCodes.Ldsfld, Field(typeof(PlayerManager), nameof(PlayerManager.localPlayer))),
                new CodeInstruction(OpCodes.Ldnull),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Brtrue_S, cdc),
                new CodeInstruction(OpCodes.Ldsfld, Field(typeof(PlayerManager), nameof(PlayerManager.players))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(List<GameObject>), nameof(List<GameObject>.Count))),
                new CodeInstruction(OpCodes.Ldsfld, Field(typeof(CustomNetworkManager), nameof(CustomNetworkManager.slots))),
                new CodeInstruction(OpCodes.Bge_S, je),
                new CodeInstruction(OpCodes.Ldc_I4_4),
                new CodeInstruction(OpCodes.Call, Method(typeof(MultiAdminFeatures), nameof(MultiAdminFeatures.CallEvent))),
                new CodeInstruction(OpCodes.Ldarg_0).WithLabels(je),
                new CodeInstruction(OpCodes.Ldloca_S, out_rh),
                new CodeInstruction(OpCodes.Call, Method(typeof(Joined), nameof(Joined.CallEvent))),
                new CodeInstruction(OpCodes.Pop),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
