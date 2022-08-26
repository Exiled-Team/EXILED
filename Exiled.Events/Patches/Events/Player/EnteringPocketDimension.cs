// -----------------------------------------------------------------------
// <copyright file="EnteringPocketDimension.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="Scp106PlayerScript.UserCode_CmdMovePlayer(GameObject, int)" />.
    ///     Adds the <see cref="Handlers.Player.EnteringPocketDimension" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.EnteringPocketDimension))]
    [HarmonyPatch(typeof(Scp106PlayerScript), nameof(Scp106PlayerScript.UserCode_CmdMovePlayer))]
    internal static class EnteringPocketDimension
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            // The index offset.
            int offset = 3;

            // Search for the last "newobj".
            int index = newInstructions.FindLastIndex(instruction => instruction.operand is "gray") + offset;

            // Declare a local variable of the type "EnteringPocketDimensionEventArgs"
            LocalBuilder ev = generator.DeclareLocal(typeof(EnteringPocketDimensionEventArgs));

            // Define the return label and add it to the last "ret" instruction.
            Label returnLabel = generator.DefineLabel();

            // var ev = new EnteringPocketDimensionEventArgs(Player.Get(taker), Vector3.down * 1998.5f, Player.Get(this.gameObject), true);
            //
            // Handlers.Player.OnEnteringPocketDimension(ev);
            //
            // if (!ev.IsAllowed)
            //   return;
            newInstructions.InsertRange(index, new[]
            {
                // Player.Get(ply)
                new CodeInstruction(OpCodes.Ldarg_1).MoveLabelsFrom(newInstructions[index]),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(GameObject) })),

                // Vector3.down * 1998.5f
                new(OpCodes.Call, PropertyGetter(typeof(Vector3), nameof(Vector3.down))),
                new(OpCodes.Ldc_R4, 1998.5f),
                new(OpCodes.Call, Method(typeof(Vector3), "op_Multiply", new[] { typeof(Vector3), typeof(float) })),

                // Player.Get(this.gameObject)
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, PropertyGetter(typeof(Component), nameof(Component.gameObject))),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(GameObject) })),

                // true
                new(OpCodes.Ldc_I4_1),

                // var ev = new EnteringPocketDimensionEventArgs(...)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(EnteringPocketDimensionEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev.LocalIndex),

                // Handlers.Player.OnEnteringPocketDimension(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnEnteringPocketDimension))),

                // if (!ev.IsAllowed)
                //   return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(EnteringPocketDimensionEventArgs), nameof(EnteringPocketDimensionEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, returnLabel),
            });

            // Search for the first "OverridePosition" method.
            index = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Ret);

            // ev.Position
            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(EnteringPocketDimensionEventArgs), nameof(EnteringPocketDimensionEventArgs.Player))),
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(EnteringPocketDimensionEventArgs), nameof(EnteringPocketDimensionEventArgs.Position))),
                new(OpCodes.Callvirt, PropertySetter(typeof(Player), nameof(Player.Position))),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
