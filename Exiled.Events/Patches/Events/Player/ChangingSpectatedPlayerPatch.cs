// -----------------------------------------------------------------------
// <copyright file="ChangingSpectatedPlayerPatch.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

#pragma warning disable SA1118 // Parameter should not span multiple lines

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs;

    using HarmonyLib;
    using NorthwoodLib.Pools;

    using Player = Exiled.Events.Handlers.Player;

    /// <summary>
    /// Patches <see cref="SpectatorManager.CurrentSpectatedPlayer"/> setter.
    /// Adds the <see cref="Handlers.Player.ChangingSpectatedPlayer"/>.
    /// </summary>
    [HarmonyPatch(typeof(SpectatorManager), nameof(SpectatorManager.CurrentSpectatedPlayer), MethodType.Setter)]
    internal static class ChangingSpectatedPlayerPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);
            Label continueLabel = generator.DefineLabel();

            LocalBuilder player = generator.DeclareLocal(typeof(API.Features.Player));

            int index = newInstructions.FindIndex(x => x.opcode == OpCodes.Ret) + 1;
            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    /*
                     *  var player = Player.Get(__instance._hub);
                     *  if (player != null)
                     *  {
                     *      var ev = new ChangingSpectatedPlayerEventArgs(player, Player.Get(__instance.CurrentSpectatedPlayer), Player.Get(value));
                     *
                     *      Exiled.Events.Handlers.Player.OnChangingSpectatedPlayer(ev);
                     *  }
                     */

                    // var player = Player.Get(__instance._hub);
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                    new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(SpectatorManager), nameof(SpectatorManager._hub))),
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new System.Type[] { typeof(ReferenceHub) })),
                    new CodeInstruction(OpCodes.Dup),

                    // if (player != null)
                    new CodeInstruction(OpCodes.Stloc, player),
                    new CodeInstruction(OpCodes.Brfalse_S, continueLabel),
                    new CodeInstruction(OpCodes.Ldloc, player),

                    // Player.Get(__instance.CurrentSpectatedPlayer)
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(SpectatorManager), nameof(SpectatorManager._currentSpectatedPlayer))),
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new System.Type[] { typeof(ReferenceHub) })),

                    // Player.Get(value)
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new System.Type[] { typeof(ReferenceHub) })),

                    // var ev = new ChangingSpectatedPlayerEventArgs(player, Player.Get(__instance.CurrentSpectatedPlayer), Player.Get(value))
                    new CodeInstruction(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(ChangingSpectatedPlayerEventArgs))[0]),

                    // Exiled.Events.Handlers.Player.OnChangingSpectatedPlayer(ev);
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Player), nameof(Player.OnChangingSpectatedPlayer))),

                    new CodeInstruction(OpCodes.Nop).WithLabels(continueLabel),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
            yield break;
        }
    }
}
