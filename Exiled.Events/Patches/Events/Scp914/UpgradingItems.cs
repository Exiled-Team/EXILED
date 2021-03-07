// -----------------------------------------------------------------------
// <copyright file="UpgradingItems.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp914
{
#pragma warning disable SA1118
    using System.Collections.Generic;

    using System.Linq;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;

    using global::Scp914;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp914Machine.ProcessItems"/>.
    /// Adds the <see cref="Scp914.UpgradingItems"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp914Machine), nameof(Scp914Machine.ProcessItems))]
    internal static class UpgradingItems
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            // The first index offset.
            var offset = 0;

            // Search for the first "nop".
            var index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Nop) + offset;

            // Declare UpgradingItemsEventArgs, to be able to store its instance with "stloc.s".
            var ev = generator.DeclareLocal(typeof(UpgradingItemsEventArgs));

            newInstructions.InsertRange(index, new[]
            {
                // var ev = new UpgradingItemsEventArgs(this, this.players.Select(player => API.Features.Player.Get(player.gameObject)).ToList(), this.items, this.knobState);
                //
                // Scp914.OnUpgradingItems(ev);
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Scp914Machine), nameof(Scp914Machine.players))),
                new CodeInstruction(OpCodes.Call, Method(typeof(UpgradingItems), nameof(UpgradingItems.FromCharacterClassManagersToPlayers))),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Scp914Machine), nameof(Scp914Machine.items))),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Scp914Machine), nameof(Scp914Machine.knobState))),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(UpgradingItemsEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Call, Method(typeof(Scp914), nameof(Scp914.OnUpgradingItems))),

                // this.players.Clear();
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Scp914Machine), nameof(Scp914Machine.players))),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(List<CharacterClassManager>), nameof(List<CharacterClassManager>.Clear))),

                // this.players.AddRange(ev.Players.Select(player => player.ReferenceHub.characterClassManager).ToList());
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Scp914Machine), nameof(Scp914Machine.players))),
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(UpgradingItemsEventArgs), nameof(UpgradingItemsEventArgs.Players))),
                new CodeInstruction(OpCodes.Call, Method(typeof(UpgradingItems), nameof(UpgradingItems.FromPlayersToCharacterClassManagers))),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(List<CharacterClassManager>), nameof(List<CharacterClassManager>.AddRange))),
            });

            // The second index offset.
            offset = 1;
            var endFinallyOffset = 6;

            // Search for the fifth "call".
            index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Leave_S) + offset;

            // Set the return label to the first "endfinally" .
            var returnLabel = newInstructions[index + endFinallyOffset].WithLabels(generator.DefineLabel()).labels[0];

            newInstructions.InsertRange(index, new[]
            {
                // if (!ev.IsAllowed)
                //   return;
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex).MoveBlocksFrom(newInstructions[index]),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(UpgradingItemsEventArgs), nameof(UpgradingItemsEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse_S, returnLabel),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        private static List<API.Features.Player> FromCharacterClassManagersToPlayers(List<CharacterClassManager> characterClassManagers)
        {
            return characterClassManagers.Select(player => API.Features.Player.Get(player.gameObject)).ToList();
        }

        private static List<CharacterClassManager> FromPlayersToCharacterClassManagers(List<API.Features.Player> players)
        {
            return players.Select(player => player.ReferenceHub.characterClassManager).ToList();
        }
    }
}
