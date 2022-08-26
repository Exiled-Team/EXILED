// -----------------------------------------------------------------------
// <copyright file="CreatingPortal.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp106
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp106;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    using Player = Exiled.API.Features.Player;

    /// <summary>
    ///     Patches <see cref="Scp106PlayerScript.UserCode_CmdMakePortal" />.
    ///     Adds the <see cref="Handlers.Scp106.CreatingPortal" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Scp106), nameof(Handlers.Scp106.CreatingPortal))]
    [HarmonyPatch(typeof(Scp106PlayerScript), nameof(Scp106PlayerScript.UserCode_CmdMakePortal))]
    internal static class CreatingPortal
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            // The index offset;
            const int offset = 0;

            // Search for the last "ldarg.0".
            int index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldarg_0) + offset;

            // Get the return label.
            object returnLabel = newInstructions[index - 1].operand;

            // Declare CreatingPortalEventArgs local variable.
            LocalBuilder ev = generator.DeclareLocal(typeof(CreatingPortalEventArgs));

            // var ev = new CreatingPortalEventArgs(Player.Get(this.gameObject), raycastHit.point - Vector3.up, true);
            //
            // Handlers.Scp106.OnCreatingPortal(ev);
            //
            // if (!ev.IsAllowed)
            //   return;
            //
            // this.SetPortalPosition(ev.Position);
            //
            // return;
            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, PropertyGetter(typeof(Component), nameof(Component.gameObject))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(GameObject) })),
                    new(OpCodes.Ldloca_S, 1),
                    new(OpCodes.Call, PropertyGetter(typeof(RaycastHit), nameof(RaycastHit.point))),
                    new(OpCodes.Call, PropertyGetter(typeof(Vector3), nameof(Vector3.up))),
                    new(OpCodes.Call, Method(typeof(Vector3), "op_Subtraction")),
                    new(OpCodes.Ldc_I4_1),
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(CreatingPortalEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),
                    new(OpCodes.Call, Method(typeof(Scp106), nameof(Scp106.OnCreatingPortal))),
                    new(OpCodes.Call, PropertyGetter(typeof(CreatingPortalEventArgs), nameof(CreatingPortalEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, PropertyGetter(typeof(Vector3), nameof(Vector3.zero))),
                    new(OpCodes.Ldloc, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(CreatingPortalEventArgs), nameof(CreatingPortalEventArgs.Position))),
                    new(OpCodes.Call, Method(typeof(Scp106PlayerScript), nameof(Scp106PlayerScript.SetPortalPosition))),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}