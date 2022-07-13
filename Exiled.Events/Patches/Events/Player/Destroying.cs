// -----------------------------------------------------------------------
// <copyright file="Destroying.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1600

    using System.Collections.Generic;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;

    using Exiled.API.Features;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.Destroying))]
    [HarmonyPatch(typeof(ReferenceHub), nameof(ReferenceHub.OnDestroy))]
    internal static class Destroying
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            LocalBuilder player = generator.DeclareLocal(typeof(Player));

            Label cdcLabel = generator.DefineLabel();

            newInstructions[0].labels.Add(cdcLabel);

            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Stloc_S, player.LocalIndex),
                new(OpCodes.Ldloc_S, player.LocalIndex),
                new(OpCodes.Ldnull),
                new(OpCodes.Ceq),
                new(OpCodes.Brtrue_S, cdcLabel),
                new(OpCodes.Ldloc_S, player.LocalIndex),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(DestroyingEventArgs))[0]),
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnDestroying))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
