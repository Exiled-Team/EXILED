// -----------------------------------------------------------------------
// <copyright file="Banned.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Core.Generic.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="BanHandler.IssueBan(BanDetails, BanHandler.BanType, bool)" />.
    /// Adds the <see cref="Handlers.Player.Banned" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.Banned))]
    [HarmonyPatch(typeof(BanHandler), nameof(BanHandler.IssueBan))]
    internal static class Banned
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder issuingPlayer = generator.DeclareLocal(typeof(Player));

            newInstructions.InsertRange(
                0,
                new CodeInstruction[]
                {
                    // issuingPlayer = GetBanningPlayer(ban.Issuer)
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(BanDetails), nameof(BanDetails.Issuer))),
                    new(OpCodes.Call, Method(typeof(Banned), nameof(GetBanningPlayer))),
                    new(OpCodes.Stloc, issuingPlayer.LocalIndex),
                });

            int offset = -6;
            int index = newInstructions.FindIndex(instruction => instruction.Calls(Method(typeof(FileManager), nameof(FileManager.AppendFile)))) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player.Get(ban.Id)
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Ldfld, Field(typeof(BanDetails), nameof(BanDetails.Id))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(string) })),

                    // issuingPlayer
                    new(OpCodes.Ldloc, issuingPlayer.LocalIndex),

                    // ban
                    new(OpCodes.Ldarg_0),

                    // banType
                    new(OpCodes.Ldarg_1),

                    // forced
                    new(OpCodes.Ldarg_2),

                    // BannedEventArgs ev = new(Player, Player, BanDetails, BanType)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(BannedEventArgs))[0]),

                    // Handlers.Player.OnBanned(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnBanned))),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private static Player GetBanningPlayer(string identifier) => identifier.Contains("(") ? Player.Get(identifier.Substring(identifier.LastIndexOf('(') + 1).TrimEnd(')')) ?? Server.Host : Server.Host;
    }
}