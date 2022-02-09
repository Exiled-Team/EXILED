// -----------------------------------------------------------------------
// <copyright file="OpeningRemoteAdmin.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using Mirror;

    using NorthwoodLib.Pools;

    using RemoteAdmin;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="QueryProcessor.TargetReplyEncrypted(NetworkConnection, byte[], bool, bool, string)"/>.
    /// Adds the <see cref="Handlers.Player.OpeningRemoteAdmin"/> event.
    /// </summary>
    [HarmonyPatch(typeof(QueryProcessor), nameof(QueryProcessor.TargetReplyEncrypted))]
    internal static class OpeningRemoteAdmin
    {
        private static readonly Dictionary<API.Features.Player, bool> TrackedReplies = new Dictionary<API.Features.Player, bool>();

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label retLabel = generator.DefineLabel();

            newInstructions.InsertRange(0, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(NetworkConnection) })),
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(OpeningRemoteAdminEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnOpeningRemoteAdmin))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(OpeningRemoteAdminEventArgs), nameof(OpeningRemoteAdminEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse_S, retLabel),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(retLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
