// -----------------------------------------------------------------------
// <copyright file="Teleporting.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp106
{
#pragma warning disable SA1118
#pragma warning disable SA1313
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using UnityEngine;

    /// <summary>
    /// Patches <see cref="Scp106PlayerScript.CallCmdUsePortal"/>.
    /// Adds the <see cref="Scp106.Teleporting"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp106PlayerScript), nameof(Scp106PlayerScript.CallCmdUsePortal))]
    internal static class Teleporting
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var newInstructions = new List<CodeInstruction>(instructions);

            // Search for "ldfld bool Scp106PlayerScript::iAm106" and subtract 1 index to get the index of the third "ldarg.0".
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldfld &&
                (FieldInfo)instruction.operand == AccessTools.Field(typeof(Scp106PlayerScript), nameof(Scp106PlayerScript.iAm106))) - 1;

            // Declare TeleportingEventArgs, to be able to store its instance with "stloc.0".
            generator.DeclareLocal(typeof(TeleportingEventArgs));

            // Generate the return label.
            var returnLabel = generator.DefineLabel();

            // Copy [Label1] from "ldarg.0" and then clear them.
            var startLabels = new List<Label>(newInstructions[index].labels);
            newInstructions[index].labels.Clear();

            // TeleportingEventArgs ev = new TeleportingEventArgs(API.Features.Player.Get(this.gameObject), this.portalPosition, true);
            //
            // Scp106.OnTeleporting(ev);
            //
            // this.portalPosition = ev.PortalPosition;
            //
            // if (!ev.IsAllowed)
            //   return
            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(Component), nameof(Component.gameObject))),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(GameObject) })),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Scp106PlayerScript), nameof(Scp106PlayerScript.portalPosition))),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(TeleportingEventArgs))[0]),
                new CodeInstruction(OpCodes.Stloc_0),
                new CodeInstruction(OpCodes.Ldloc_0),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Scp106), nameof(Scp106.OnTeleporting))),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldloc_0),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(TeleportingEventArgs), nameof(TeleportingEventArgs.PortalPosition))),
                new CodeInstruction(OpCodes.Stfld, AccessTools.Field(typeof(Scp106PlayerScript), nameof(Scp106PlayerScript.portalPosition))),
                new CodeInstruction(OpCodes.Ldloc_0),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(TeleportingEventArgs), nameof(TeleportingEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse_S, returnLabel),
            });

            // Add [Label1] to "ldarg.0".
            newInstructions[index].labels.AddRange(startLabels);

            // Add the label to the last "ret".
            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            return newInstructions;
        }
    }
}
