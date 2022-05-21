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

    using Mirror;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="CharacterClassManager.ApplyProperties(bool, bool)"/>.
    /// Adds the <see cref="Spawning"/> event.
    /// </summary>
    [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.ApplyProperties))]
    internal static class Spawning
    {
        /// <summary>
        /// Handles logic for <see cref="Player"/> to preserve the position and inventory after changing the role.
        /// </summary>
        /// <param name="player"> Current player object to build off of, assuming the data is still there. </param>
        /// <param name="__instance"> Instance of CharacterClassManager. </param>
        public static void HandleLiteSpawning(Player player, ref CharacterClassManager __instance)
        {
            if (player == null)
            {
                return;
            }

            __instance._pms.OnPlayerClassChange(player.Position, new global::PlayerMovementSync.PlayerRotation?(new global::PlayerMovementSync.PlayerRotation(new float?(0f), new float?(player.Position.y))));

            if (!__instance.SpawnProtected && global::CharacterClassManager.EnableSP && global::CharacterClassManager.SProtectedTeam.Contains((int)__instance.CurRole.team))
            {
                __instance.GodMode = true;
                __instance.SpawnProtected = true;
                __instance.ProtectedTime = Time.time;
            }
        }

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
            LocalBuilder player = generator.DeclareLocal(typeof(Player));

            Label localPlayerCheck = generator.DefineLabel();

            // There is a bug here. If you set role using Player.SetRole then later on doing it again, with lite set to false, this will spam create this player infinitely.
            newInstructions.InsertRange(index, new[]
            {
                // Player.Get(this._hub)
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                new(OpCodes.Ldfld, AccessTools.Field(typeof(CharacterClassManager), nameof(CharacterClassManager._hub))),
                new(OpCodes.Call, AccessTools.Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Dup),
                new(OpCodes.Stloc, player.LocalIndex),
                new(OpCodes.Brfalse_S, localPlayerCheck),

                // this.CurClass
                new(OpCodes.Ldloc, player.LocalIndex),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, AccessTools.Field(typeof(CharacterClassManager), nameof(CharacterClassManager.CurClass))),

                // var ev = new SpawningEventArg(Player, RoleType)
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

            const int addLiteOffset = 0;
            int addLiteIndex = newInstructions.FindLastIndex(instruction => instruction.Calls(PropertyGetter(typeof(NetworkBehaviour), nameof(NetworkBehaviour.isLocalPlayer)))) + addLiteOffset;

            newInstructions.InsertRange(addLiteIndex, new[]
            {
                // !NetworkServer.Active ? skip : process lite
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(NetworkServer), nameof(NetworkServer.active))),
                new (OpCodes.Brfalse_S, localPlayerCheck),

                // !lite, skip
                new CodeInstruction(OpCodes.Ldarg_1),
                new (OpCodes.Brfalse_S, localPlayerCheck),

                // Player.Get(this._hub)
                new CodeInstruction(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, AccessTools.Field(typeof(CharacterClassManager), nameof(CharacterClassManager._hub))),
                new(OpCodes.Call, AccessTools.Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // CCM instance.
                new CodeInstruction(OpCodes.Ldarga, 0),
                new CodeInstruction(OpCodes.Call, Method(typeof(Spawning), nameof(Spawning.HandleLiteSpawning), new[] { typeof(Player), typeof(CharacterClassManager).MakeByRefType() })),
            });

            int notActiveServerOffset = 0;
            int notActiveServer = newInstructions.FindLastIndex(instruction => instruction.Calls(PropertyGetter(typeof(NetworkBehaviour), nameof(NetworkBehaviour.isLocalPlayer)))) + notActiveServerOffset;

            newInstructions[notActiveServer].WithLabels(localPlayerCheck);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
