// -----------------------------------------------------------------------
// <copyright file="Teleporting.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.Patches.Events.Scp106
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using SEXiled.API.Features;
    using SEXiled.Events.EventArgs;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp106PlayerScript.UserCode_CmdUsePortal"/>.
    /// Adds the <see cref="Teleporting"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp106PlayerScript), nameof(Scp106PlayerScript.UserCode_CmdUsePortal))]
    internal static class Teleporting
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            // The index offset.
            const int offset = -1;

            // Search for "ldfld bool Scp106PlayerScript::iAm106" and subtract 1 to get the index of the third "ldarg.0".
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldfld &&
                (FieldInfo)instruction.operand == Field(typeof(Scp106PlayerScript), nameof(Scp106PlayerScript.iAm106))) + offset;

            // Declare TeleportingEventArgs, to be able to store its instance with "stloc.0".
            LocalBuilder ev = generator.DeclareLocal(typeof(TeleportingEventArgs));

            // Get the count to find the previous index
            int oldCount = newInstructions.Count;

            // Get the return label from the last instruction.
            Label returnLabel = newInstructions[newInstructions.Count - 1].labels[0];

            // TeleportingEventArgs ev = new TeleportingEventArgs(Player.Get(this.gameObject), this.portalPosition, true);
            //
            // Handlers.Scp106.OnTeleporting(ev);
            //
            // this.portalPosition = ev.PortalPosition;
            //
            // if (!ev.IsAllowed)
            //   return
            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Component), nameof(Component.gameObject))),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(GameObject) })),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Scp106PlayerScript), nameof(Scp106PlayerScript.portalPosition))),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(TeleportingEventArgs))[0]),
                new CodeInstruction(OpCodes.Stloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Scp106), nameof(Handlers.Scp106.OnTeleporting))),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(TeleportingEventArgs), nameof(TeleportingEventArgs.PortalPosition))),
                new CodeInstruction(OpCodes.Stfld, Field(typeof(Scp106PlayerScript), nameof(Scp106PlayerScript.portalPosition))),
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(TeleportingEventArgs), nameof(TeleportingEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse_S, returnLabel),
            });

            // Add the starting labels to the first injected instruction.
            // Calculate the difference and get the valid index - is better and easy than using a list
            newInstructions[index].MoveLabelsFrom(newInstructions[newInstructions.Count - oldCount + index]);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
