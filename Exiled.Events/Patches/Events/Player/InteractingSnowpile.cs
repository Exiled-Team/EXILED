// -----------------------------------------------------------------------
// <copyright file="InteractingSnowpile.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;
    using HarmonyLib;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="global::Snowpile.ServerInteract"/>
    /// to add <see cref="Handlers.Player.InteractingSnowpile"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.InteractingSnowpile))]
    [HarmonyPatch(typeof(global::Snowpile), nameof(global::Snowpile.ServerInteract))]
    internal class InteractingSnowpile
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int offset = -1;
            int index = newInstructions.FindLastIndex(x => x.operand is FieldInfo info && info == Field(typeof(ReferenceHub), nameof(ReferenceHub.inventory))) + offset;

            Label skipCheckLabel = generator.DefineLabel();
            Label retLabel = generator.DefineLabel();

            newInstructions[index].labels.Add(skipCheckLabel);
            newInstructions[newInstructions.Count - 1].labels.Add(retLabel);

            LocalBuilder ev = generator.DeclareLocal(typeof(InteractingSnowpileEventArgs));

            offset = 1;
            index = newInstructions.FindIndex(x => x.opcode == OpCodes.Ret) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player.Get(hub);
                    new CodeInstruction(OpCodes.Ldarg_1).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // Snowpile.Get(this);
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, Method(typeof(Snowpile), nameof(Snowpile.Get), new[] { typeof(global::Snowpile) })),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // InteractingSnowpileEventArgs ev = new(Player, Snowpile, true);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(InteractingSnowpileEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Handlers.Player.OnInteractingSnowpile(ev);
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnInteractingSnowpile))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(InteractingSnowpileEventArgs), nameof(InteractingSnowpileEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, retLabel),

                    // if (!ev.ShouldCheck)
                    //    goto skipCheckLabel;
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(InteractingSnowpileEventArgs), nameof(InteractingSnowpileEventArgs.ShouldCheck))),
                    new(OpCodes.Brfalse_S, skipCheckLabel),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}