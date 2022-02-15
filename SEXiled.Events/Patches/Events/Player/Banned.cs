// -----------------------------------------------------------------------
// <copyright file="Banned.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.Patches.Events.Player
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using SEXiled.API.Features;
    using SEXiled.Events.EventArgs;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    using Server = SEXiled.API.Features.Server;

    /// <summary>
    /// Patches <see cref="BanHandler.IssueBan(BanDetails, BanHandler.BanType)"/>.
    /// Adds the <see cref="Handlers.Player.Banned"/> event.
    /// </summary>
    [HarmonyPatch(typeof(BanHandler), nameof(BanHandler.IssueBan))]
    internal static class Banned
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            LocalBuilder issuingPlayer = generator.DeclareLocal(typeof(Player));

            newInstructions.InsertRange(0, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(BanDetails), nameof(BanDetails.Issuer))),
                new CodeInstruction(OpCodes.Call, Method(typeof(Banned), nameof(Banned.GetBanningPlayer))),
                new CodeInstruction(OpCodes.Stloc, issuingPlayer.LocalIndex),
            });

            int offset = -6;
            int index = newInstructions.FindIndex(i =>
                i.opcode == OpCodes.Call && (MethodInfo)i.operand ==
                Method(typeof(FileManager), nameof(FileManager.AppendFile))) + offset;

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(BanDetails), nameof(BanDetails.Id))),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(string) })),
                new CodeInstruction(OpCodes.Ldloc, issuingPlayer.LocalIndex),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(BannedEventArgs))[0]),
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnBanned))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        private static Player GetBanningPlayer(string identifier) => identifier.Contains("(") ? Player.Get(identifier.Substring(identifier.LastIndexOf('(') + 1).TrimEnd(')')) ?? Server.Host : Server.Host;
    }
}
