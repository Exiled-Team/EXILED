// -----------------------------------------------------------------------
// <copyright file="Died.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using PlayerStatsSystem;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="PlayerStats.KillPlayer(DamageHandlerBase)"/>.
    /// Adds the <see cref="Handlers.Player.Died"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.Dying))]
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.Died))]
    [HarmonyPatch(typeof(PlayerStats), nameof(PlayerStats.KillPlayer))]
    internal static class Died
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);
            Label ret = generator.DefineLabel();
            LocalBuilder player = generator.DeclareLocal(typeof(Player));
            LocalBuilder oldRole = generator.DeclareLocal(typeof(RoleType));

            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(PlayerStats), nameof(PlayerStats._hub))),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Dup),
                new(OpCodes.Stloc, player.LocalIndex),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(DyingEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnDying))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(DyingEventArgs), nameof(DyingEventArgs.IsAllowed))),
                new(OpCodes.Brfalse, ret),
                new(OpCodes.Ldloc, player.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.Role))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(API.Features.Roles.Role), nameof(API.Features.Roles.Role.Type))),
                new(OpCodes.Stloc, oldRole.LocalIndex),
            });

            int index = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Ret);

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Ldloc, player.LocalIndex),
                new(OpCodes.Ldloc, oldRole.LocalIndex),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(DiedEventArgs))[0]),
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnDied))),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
