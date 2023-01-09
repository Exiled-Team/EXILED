// -----------------------------------------------------------------------
// <copyright file="ChangingSpectatedPlayerPatch.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using PlayerRoles.Spectating;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="SpectatorRole.SyncedSpectatedNetId" /> setter.
    ///     Adds the <see cref="Handlers.Player.ChangingSpectatedPlayer" />.
    /// </summary>
    [HarmonyPatch(typeof(SpectatorRole), nameof(SpectatorRole.SyncedSpectatedNetId), MethodType.Setter)]
    internal static class ChangingSpectatedPlayerPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            newInstructions.InsertRange(
                0,
                new[]
                {
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new(OpCodes.Ldarga_S, 1),
                    new(OpCodes.Call, Method(typeof(ChangingSpectatedPlayerPatch), nameof(ProcessNewPlayer), new[] { typeof(SpectatorRole), typeof(uint).MakeByRefType() })),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        private static void ProcessNewPlayer(SpectatorRole spectator, ref int newNetId)
        {
            if (newNetId == 0)
                return;

            if (!spectator.TryGetOwner(out ReferenceHub owner))
                return;

            API.Features.Player player = API.Features.Player.Get(owner);
            API.Features.Player previousSpectatedPlayer = API.Features.Player.Get(spectator.SyncedSpectatedNetId);
            API.Features.Player getFuturePlayer = API.Features.Player.Get(newNetId);

            ChangingSpectatedPlayerEventArgs ev = new(player, previousSpectatedPlayer, getFuturePlayer, true);
            Handlers.Player.OnChangingSpectatedPlayer(ev);

            // NOT RECOMMENDED PER IRACLE AND I BELIEVING CLIENT SIDE PICKS TARGET.
            if (!ev.IsAllowed)
                return;

            newNetId = (int)(ev.NewTarget != null ? ev.NewTarget.NetworkIdentity.netId : ev.Player.NetworkIdentity.netId);
        }
    }
}