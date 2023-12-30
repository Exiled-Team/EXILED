// -----------------------------------------------------------------------
// <copyright file="UsingTapeItem.cs" company="Exiled Team">
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
    /// Patches <see cref="InventorySystem.Items.FlamingoTapePlayer.TapeItem.ServerProcessCmd"/>
    /// to add <see cref="Handlers.Player.UsingTapeItem"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.UsingTapeItem))]
    [HarmonyPatch(typeof(InventorySystem.Items.FlamingoTapePlayer.TapeItem), nameof(InventorySystem.Items.FlamingoTapePlayer.TapeItem.ServerProcessCmd))]
    internal class UsingTapeItem
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int offset = 2;
            int index = newInstructions.FindIndex(x => x.opcode == OpCodes.Ceq) + offset;

            FieldInfo fieldInfo = newInstructions[index - 1].operand as FieldInfo;

            Label retLabel = generator.DefineLabel();
            LocalBuilder ev = generator.DeclareLocal(typeof(UsingTapeEventArgs));

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // Player.Get(this.Owner);
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(InventorySystem.Items.FlamingoTapePlayer.TapeItem), nameof(InventorySystem.Items.FlamingoTapePlayer.TapeItem.Owner))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // this
                    new(OpCodes.Ldarg_0),

                    // success
                    new(OpCodes.Ldloc_0),
                    new(OpCodes.Ldfld, fieldInfo),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // UsingTapeItemEventArgs ev = new(Player, ItemBase, bool, true);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(UsingTapeEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Handlers.Player.OnUsingTapeItem(ev);
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnUsingTapeItem))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(UsingTapeEventArgs), nameof(UsingTapeEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, retLabel),

                    // success = ev.Success
                    new(OpCodes.Ldloc_0),
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(UsingTapeEventArgs), nameof(UsingTapeEventArgs.Success))),
                    new(OpCodes.Stfld, fieldInfo),
                });

            newInstructions[newInstructions.Count - 1].labels.Add(retLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}