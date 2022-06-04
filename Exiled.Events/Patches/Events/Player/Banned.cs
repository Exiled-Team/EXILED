// -----------------------------------------------------------------------
// <copyright file="Banned.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="BanHandler.IssueBan(BanDetails, BanHandler.BanType)" />.
    ///     Adds the <see cref="Handlers.Player.Banned" /> event.
    /// </summary>
    [HarmonyPatch(typeof(BanHandler), nameof(BanHandler.IssueBan))]
    internal static class Banned
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            LocalBuilder issuingPlayer = generator.DeclareLocal(typeof(Player));

            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(BanDetails), nameof(BanDetails.Issuer))),
                new(OpCodes.Call, Method(typeof(Banned), nameof(GetBanningPlayer))),
                new(OpCodes.Stloc, issuingPlayer.LocalIndex),
            });

            int offset = -6;
            int index = newInstructions.FindIndex(i =>
                i.opcode == OpCodes.Call && (MethodInfo) i.operand ==
                Method(typeof(FileManager), nameof(FileManager.AppendFile))) + offset;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(BanDetails), nameof(BanDetails.Id))),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(string) })),
                new(OpCodes.Ldloc, issuingPlayer.LocalIndex),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(BannedEventArgs))[0]),
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnBanned))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        private static Player GetBanningPlayer(string identifier)
        {
            return identifier.Contains("(") ? Player.Get(identifier.Substring(identifier.LastIndexOf('(') + 1).TrimEnd(')')) ?? Server.Host : Server.Host;
        }
    }
}
