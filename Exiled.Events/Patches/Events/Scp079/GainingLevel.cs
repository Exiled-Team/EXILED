// -----------------------------------------------------------------------
// <copyright file="GainingLevel.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp079
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs;
    using Exiled.Events.EventArgs.Scp079;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    using Player = Exiled.API.Features.Player;

    /// <summary>
    ///     Patches <see cref="Scp079PlayerScript.Lvl" />.
    ///     Adds the <see cref="Scp079.GainingLevel" /> event.
    /// </summary>
    [EventPatch(typeof(Scp079), nameof(Scp079.GainingLevel))]
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
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, PropertyGetter(typeof(Scp079PlayerScript), nameof(Scp079PlayerScript.gameObject))),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(GameObject) })),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(GainingLevelEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Call, Method(typeof(Scp079), nameof(Scp079.OnGainingLevel))),
                new(OpCodes.Call, PropertyGetter(typeof(GainingLevelEventArgs), nameof(GainingLevelEventArgs.NewLevel))),
                new(OpCodes.Starg_S, 1),
                new(OpCodes.Callvirt, PropertyGetter(typeof(GainingLevelEventArgs), nameof(GainingLevelEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, returnLabel),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
