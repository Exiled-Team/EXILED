// -----------------------------------------------------------------------
// <copyright file="Containing.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp106
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs.Scp106;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    using Player = Exiled.API.Features.Player;

    /// <summary>
    ///     Patches <see cref="PlayerInteract.UserCode_CmdContain106" />.
    ///     Adds the <see cref="Handlers.Scp106.Containing" /> event.
    /// </summary>
    [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.UserCode_CmdContain106))]
    internal static class Containing
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            // The index offset.
            const int offset = 1;

            // Search for the last "bne.un.s" and add 1 to get the index of the last "ldloca.s".
            int index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Bne_Un_S) + offset;

            // Get the return label.
            object returnLabel = newInstructions[index - 1].operand;

            // var ev = new ContainingEventArgs(Player.Get(keyValuePair.Key), Player.Get(PlayerInteract._hub), true);
            //
            // Handlers.Scp106.OnContaining(ev);
            //
            // if (!ev.IsAllowed)
            //   return;
            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Ldloca_S, 2),
                new(OpCodes.Call, PropertyGetter(typeof(KeyValuePair<GameObject, ReferenceHub>), nameof(KeyValuePair<GameObject, ReferenceHub>.Key))),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(GameObject) })),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(PlayerInteract), nameof(PlayerInteract._hub))),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ContainingEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Call, Method(typeof(Scp106), nameof(Scp106.OnContaining))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ContainingEventArgs), nameof(ContainingEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, returnLabel),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
