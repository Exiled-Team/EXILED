// -----------------------------------------------------------------------
// <copyright file="PlacingBlood.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="CharacterClassManager.RpcPlaceBlood(Vector3, int, float)"/>.
    /// Adds the <see cref="PlacingBlood"/> event.
    /// </summary>
    [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.RpcPlaceBlood))]
    internal static class PlacingBlood
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label returnLabel = generator.DefineLabel();
            Label cmp = generator.DefineLabel();
            Label jmp = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(PlacingBloodEventArgs));
            LocalBuilder rh = generator.DeclareLocal(typeof(ReferenceHub));

            // if (!Exiled.Events.Instance.Config.CanSpawnBlood)
            //     return;
            //
            // var ev = new PlacingBloodEventArgs(Player, Vector3, int, float, bool);
            //
            // Handlers.Map.OnPlacingBlood(ev);
            //
            // if (!ev.IsAllowed)
            //     return;
            //
            // pos = ev.Position;
            // type = ev.Type;
            // f = ev.Multiplier;
            newInstructions.InsertRange(0, new[]
            {
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(Exiled.Events.Events), nameof(Exiled.Events.Events.Instance))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Exiled.Events.Events), nameof(Exiled.Events.Events.Config))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Config), nameof(Config.CanSpawnBlood))),
                new CodeInstruction(OpCodes.Brfalse_S, returnLabel),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(CharacterClassManager), nameof(CharacterClassManager._hub))),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc_S, rh.LocalIndex),
                new CodeInstruction(OpCodes.Brfalse_S, cmp),
                new CodeInstruction(OpCodes.Ldloc_S, rh.LocalIndex),
                new CodeInstruction(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),
                new CodeInstruction(OpCodes.Br_S, jmp),
                new CodeInstruction(OpCodes.Ldnull).WithLabels(cmp),
                new CodeInstruction(OpCodes.Ldarg_1).WithLabels(jmp),
                new CodeInstruction(OpCodes.Ldarg_2),
                new CodeInstruction(OpCodes.Ldarg_3),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(PlacingBloodEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Map), nameof(Handlers.Map.OnPlacingBlood))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(PlacingBloodEventArgs), nameof(PlacingBloodEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse_S, returnLabel),
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(PlacingBloodEventArgs), nameof(PlacingBloodEventArgs.Position))),
                new CodeInstruction(OpCodes.Starg_S, 1),
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(PlacingBloodEventArgs), nameof(PlacingBloodEventArgs.Type))),
                new CodeInstruction(OpCodes.Starg_S, 2),
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(PlacingBloodEventArgs), nameof(PlacingBloodEventArgs.Multiplier))),
                new CodeInstruction(OpCodes.Starg_S, 3),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
