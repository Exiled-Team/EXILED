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
            LocalBuilder value = generator.DeclareLocal(typeof(bool?));

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
                new(OpCodes.Call, PropertyGetter(typeof(Player), nameof(Player.HasScotopia))),
                new(OpCodes.Stloc, value.LocalIndex),
                new(OpCodes.Ldloca, value.LocalIndex),
                new(OpCodes.Call, PropertyGetter(typeof(bool?), nameof(Player.HasScotopia.HasValue))),
                new(OpCodes.Brfalse_S, continueLable),

                // this.NetworksyncFlicker = player.IsScp ? player.HasScotopia.Value : !player.HasScotopia.Value;
                new(OpCodes.Pop),
                new(OpCodes.Ldloca, value.LocalIndex),
                new(OpCodes.Call, PropertyGetter(typeof(bool?), nameof(Player.HasScotopia.Value))),

                new(OpCodes.Ldloc, player.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.IsScp))),
                new(OpCodes.Brtrue_S, continueLable),
                new(OpCodes.Ldc_I4_0),
                new(OpCodes.Ceq),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
