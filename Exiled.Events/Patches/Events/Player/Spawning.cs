// -----------------------------------------------------------------------
// <copyright file="Spawning.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1313
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

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
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Ldfld && (FieldInfo)i.operand == AccessTools.Field(typeof(CharacterClassManager), nameof(CharacterClassManager.SpawnProtected))) + offset;

            // Remove all existing this._pms.OnPlayerClassChange calls (we will want to call this ourselves after our even fires, to allow their spawn position to change.)
            foreach (CodeInstruction instruction in newInstructions.FindAll(i =>
                i.opcode == OpCodes.Call && (MethodInfo)i.operand == AccessTools.Method(typeof(PlayerMovementSync), nameof(PlayerMovementSync.OnPlayerClassChange))))
                newInstructions.Remove(instruction);
            LocalBuilder ev = generator.DeclareLocal(typeof(SpawningEventArgs));

            newInstructions.InsertRange(index, new[]
            {
                // Player.Get(this._hub)
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(CharacterClassManager), nameof(CharacterClassManager._hub))),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // this.CurClass
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(CharacterClassManager), nameof(CharacterClassManager.CurClass))),

                // var ev = new SpawningEventArg(Player, RoleType)
                // Exiled.Events.Handlers.Player.OnSpawning(ev);
                new CodeInstruction(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(SpawningEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc, ev.LocalIndex),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Handlers.Player), nameof(Handlers.Player.OnSpawning))),

                // this._pms.OnPlayerClassChange(ev.Position, ev.RotationY)
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(CharacterClassManager), nameof(CharacterClassManager._pms))),
                new CodeInstruction(OpCodes.Ldloc, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(SpawningEventArgs), nameof(SpawningEventArgs.Position))),
                new CodeInstruction(OpCodes.Ldloc, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(SpawningEventArgs), nameof(SpawningEventArgs.RotationY))),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PlayerMovementSync), nameof(PlayerMovementSync.OnPlayerClassChange))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
