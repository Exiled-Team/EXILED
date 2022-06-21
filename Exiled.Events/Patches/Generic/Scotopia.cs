// -----------------------------------------------------------------------
// <copyright file="Scotopia.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    using Player = Exiled.API.Features.Player;

    /// <summary>
    /// Patches <see cref="LocalCurrentRoomEffects.FixedUpdate"/>.
    /// </summary>
    [HarmonyPatch(typeof(LocalCurrentRoomEffects), nameof(LocalCurrentRoomEffects.FixedUpdate))]
    internal static class Scotopia
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label continueLable = generator.DefineLabel();
            LocalBuilder player = generator.DeclareLocal(typeof(Player));

            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Call
            && (MethodInfo)i.operand == PropertySetter(typeof(LocalCurrentRoomEffects), nameof(LocalCurrentRoomEffects.NetworksyncFlicker)));

            newInstructions[index].WithLabels(continueLable);

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // Player player = Player.Get(this._hub)
                // if(player.IsScotopia.HasValue)
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(LocalCurrentRoomEffects), nameof(LocalCurrentRoomEffects._hub))),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Dup),
                new(OpCodes.Stloc, player.LocalIndex),
                new(OpCodes.Call, Method(typeof(Scotopia), nameof(Scotopia.HasValue))),
                new(OpCodes.Brfalse_S, continueLable),

                // this.NetworksyncFlicker = Scotopia.GetValue(player);
                new(OpCodes.Pop),
                new(OpCodes.Ldloc, player.LocalIndex),
                new(OpCodes.Call, Method(typeof(Scotopia), nameof(Scotopia.GetValue))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        private static bool GetValue(Player player) => player.IsScp ? player.IsScotopia.Value : !player.IsScotopia.Value;

        private static bool HasValue(Player player) => player.IsScotopia.HasValue;
    }
}
