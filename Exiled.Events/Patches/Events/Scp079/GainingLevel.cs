// -----------------------------------------------------------------------
// <copyright file="GainingLevel.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp079
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp079PlayerScript.Lvl"/>.
    /// Adds the <see cref="Scp079.GainingLevel"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp079PlayerScript), nameof(Scp079PlayerScript.Lvl), MethodType.Setter)]
    internal static class GainingLevel
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label returnLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(GainingLevelEventArgs));

            // GainingLevelEventArgs ev = new GainingLevelEventArgs(Player, newLvl, true);
            //
            // Handlers.Scp079.OnGainingLevel(ev);
            //
            // newLvl = ev.NewLevel;
            //
            // if (!ev.IsAllowed)
            //     return;
            newInstructions.InsertRange(0, new CodeInstruction[]
            {
<<<<<<< HEAD
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(Scp079PlayerScript), nameof(Scp079PlayerScript.gameObject))),
                new CodeInstruction(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(GameObject) })),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(GainingLevelEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Call, Method(typeof(Scp079), nameof(Handlers.Scp079.OnGainingLevel))),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(GainingLevelEventArgs), nameof(GainingLevelEventArgs.NewLevel))),
                new CodeInstruction(OpCodes.Starg_S, 1),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(GainingLevelEventArgs), nameof(GainingLevelEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse_S, returnLabel),
=======
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, PropertyGetter(typeof(Scp079PlayerScript), nameof(Scp079PlayerScript.gameObject))),
                new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(GameObject) })),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(GainingLevelEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Call, Method(typeof(Scp079), nameof(Handlers.Scp079.OnGainingLevel))),
                new(OpCodes.Call, PropertyGetter(typeof(GainingLevelEventArgs), nameof(GainingLevelEventArgs.NewLevel))),
                new(OpCodes.Starg_S, 1),
                new(OpCodes.Callvirt, PropertyGetter(typeof(GainingLevelEventArgs), nameof(GainingLevelEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, returnLabel),
>>>>>>> Exiled-Team-dev
            });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
