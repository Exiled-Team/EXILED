// -----------------------------------------------------------------------
// <copyright file="Spawning.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1313

    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    /// <summary>
    /// Patches <see cref="CharacterClassManager.ApplyProperties(bool, bool)"/>.
    /// Adds the <see cref="Spawning"/> event.
    /// </summary>
    [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.ApplyProperties))]
    internal static class Spawning
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            // Player RoleType Vector3 float
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            // Find the index of the ldarg.0 before the only ldfld CharacterClassManager::SpawnProtected
            const int offset = -1;
            int index = newInstructions.FindIndex(
                i =>
                    (i.opcode == OpCodes.Call) && ((MethodInfo)i.operand ==
                                                   AccessTools.PropertyGetter(typeof(CharacterClassManager), nameof(CharacterClassManager.SpawnProtected)))) + offset;

            // Remove all existing this._pms.OnPlayerClassChange calls (we will want to call this ourselves after our even fires, to allow their spawn position to change.)
            newInstructions.RemoveAll(i =>
                i.opcode == OpCodes.Call && (MethodInfo)i.operand == AccessTools.Method(typeof(PlayerMovementSync), nameof(PlayerMovementSync.OnPlayerClassChange)));

            LocalBuilder ev = generator.DeclareLocal(typeof(SpawningEventArgs));

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player.Get(this._hub)
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Ldfld, AccessTools.Field(typeof(CharacterClassManager), nameof(CharacterClassManager._hub))),
                    new(OpCodes.Call, AccessTools.Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // this.CurClass
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, AccessTools.Field(typeof(CharacterClassManager), nameof(CharacterClassManager.CurClass))),

                    // var ev = new SpawningEventArgs(Player, RoleType)
                    // Exiled.Events.Handlers.Player.OnSpawning(ev);
                    new(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(SpawningEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc, ev.LocalIndex),
                    new(OpCodes.Call, AccessTools.Method(typeof(Handlers.Player), nameof(Handlers.Player.OnSpawning))),

                    // this._pms.OnPlayerClassChange(ev.Position, ev.RotationY)
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, AccessTools.Field(typeof(CharacterClassManager), nameof(CharacterClassManager._pms))),
                    new(OpCodes.Ldloc, ev.LocalIndex),
                    new(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(SpawningEventArgs), nameof(SpawningEventArgs.Position))),
                    new(OpCodes.Ldloc, ev.LocalIndex),
                    new(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(SpawningEventArgs), nameof(SpawningEventArgs.RotationY))),
                    new(OpCodes.Call, AccessTools.Method(typeof(PlayerMovementSync), nameof(PlayerMovementSync.OnPlayerClassChange))),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}