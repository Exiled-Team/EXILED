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

            int i = 0;

            // Search for the third "ldarg.0".
            for (int ldargCounter = 0; ldargCounter != 3 && i < newInstructions.Count; i++)
            {
                if (newInstructions[i].opcode == OpCodes.Ldarg_0)
                    ldargCounter++;
            }

            // Declare TeleportingEventArgs, to be able to store its object with "stloc.0".
            generator.DeclareLocal(typeof(TeleportingEventArgs));

            // Generate the return label.
            var returnLabel = generator.DefineLabel();

            // TeleportingEventArgs teleportingEventArgs = new TeleportingEventArgs(API.Features.Player.Get(this.gameObject), this.portalPosition, true);
            // Scp106.OnTeleporting(teleportingEventArgs);
            // this.portalPosition = teleportingEventArgs.PortalPosition;
            // if (!teleportingEventArgs.IsAllowed)
            //   return
            newInstructions.InsertRange(i, new[]
            {
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
                new CodeInstruction(OpCodes.Ldarg_0),
            });

            // Add the label to the last "ret".
            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            return newInstructions;
        }
    }
}
