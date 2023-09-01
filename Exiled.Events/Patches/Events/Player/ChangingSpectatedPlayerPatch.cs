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

    using API.Features.Pools;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;
    using PlayerRoles;
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
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);
            LocalBuilder owner = generator.DeclareLocal(typeof(ReferenceHub));

            newInstructions.InsertRange(
                0,
                new[]
                {
                    // _ = this.TryGetOwner(out ReferenceHub owner)
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldloca_S, owner.LocalIndex),
                    new CodeInstruction(OpCodes.Callvirt, Method(typeof(PlayerRoleBase), nameof(PlayerRoleBase.TryGetOwner), new[] { typeof(ReferenceHub).MakeByRefType() })),
                    new CodeInstruction(OpCodes.Pop),

                    // owner
                    new CodeInstruction(OpCodes.Ldloc_S, owner.LocalIndex),

                    // this.SyncedSpectatedNetId
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(SpectatorRole), nameof(SpectatorRole.SyncedSpectatedNetId))),

                    // value
                    new(OpCodes.Ldarg_1),

                    // Handlers.Player.OnChangingSpectatedPlayer(new(owner, this.SyncedSpectatedNetId, value));
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangingSpectatedPlayerEventArgs))[0]),
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnChangingSpectatedPlayer))),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}