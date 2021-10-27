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
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]), // [this]
                    new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(SpectatorManager), nameof(SpectatorManager._hub))), // [ReferenceHub]
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new System.Type[] { typeof(ReferenceHub) })), // [Player]
                    new CodeInstruction(OpCodes.Dup), // [Player, Player]

                    // if (spectator != null)
                    new CodeInstruction(OpCodes.Stloc, player), // [Player]
                    new CodeInstruction(OpCodes.Brfalse_S, continueLabel), // []
                    new CodeInstruction(OpCodes.Ldloc, player), // [Player]

                    // Player.Get(__instance.CurrentSpectatedPlayer)
                    new CodeInstruction(OpCodes.Ldarg_0), // [this, Player]
                    new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(SpectatorManager), nameof(SpectatorManager._currentSpectatedPlayer))), // [ReferenceHub(OldTarget), Player(Spectator)]
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new System.Type[] { typeof(ReferenceHub) })), // [Player(OldTarget), Player(Spectator)]

                    // Player.Get(value)
                    new CodeInstruction(OpCodes.Ldarg_1), // [ReferenceHub, Player(OldTarget), Player(Spectator)]
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new System.Type[] { typeof(ReferenceHub) })), // [Player(NewTarget), Player(OldTarget), Player(Spectator)]

                    // var ev = new ChangingSpectatedPlayerEventArgs(spectator, Player.Get(__instance.CurrentSpectatedPlayer), Player.Get(value))
                    new CodeInstruction(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(ChangedSpectatedPlayerEventArgs))[0]),  // [EventArgs]

                    // Handlers.CustomEvents.InvokeChangingSpectatedPlayer(ev);
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Player), nameof(Player.OnChangedSpectatedPlayer))),  // []

                    new CodeInstruction(OpCodes.Nop).WithLabels(continueLabel),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
            yield break;
        }
    }
}
