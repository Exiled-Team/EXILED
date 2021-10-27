// -----------------------------------------------------------------------
// <copyright file="ChangedSpectatedPlayerPatch.cs" company="Exiled Team">
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
    /// Adds the <see cref="Handlers.Player.ChangedSpectatedPlayer"/>.
    /// </summary>
    [HarmonyPatch(typeof(SpectatorManager), nameof(SpectatorManager.CurrentSpectatedPlayer), MethodType.Setter)]
    internal static class ChangedSpectatedPlayerPatch
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
                     *  var spectator = Player.Get(__instance._hub);
                     *  if (spectator != null)
                     *  {
                     *      var ev = new ChangingSpectatedPlayerEventArgs(spectator, Player.Get(__instance.CurrentSpectatedPlayer), Player.Get(value));
                     *
                     *      Handlers.CustomEvents.InvokeChangingSpectatedPlayer(ev);
                     *  }
                     */

                    // var spectator = Player.Get(__instance._hub);
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                    new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(SpectatorManager), nameof(SpectatorManager._hub))),
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new System.Type[] { typeof(ReferenceHub) })),
                    new CodeInstruction(OpCodes.Dup),

                    // if (spectator != null)
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

                    // var ev = new ChangingSpectatedPlayerEventArgs(spectator, Player.Get(__instance.CurrentSpectatedPlayer), Player.Get(value))
                    new CodeInstruction(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(ChangedSpectatedPlayerEventArgs))[0]),

                    // Handlers.CustomEvents.InvokeChangingSpectatedPlayer(ev);
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Player), nameof(Player.OnChangedSpectatedPlayer))),

                    new CodeInstruction(OpCodes.Nop).WithLabels(continueLabel),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
            yield break;
        }
    }
}
