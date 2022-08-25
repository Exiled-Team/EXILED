// -----------------------------------------------------------------------
// <copyright file="PlacingBlood.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs;
    using Exiled.Events.EventArgs.Map;

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

            LocalBuilder ev = generator.DeclareLocal(typeof(PlacingBloodEventArgs));

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
                new(OpCodes.Callvirt, PropertyGetter(typeof(Exiled.Events.Events), nameof(Exiled.Events.Events.Config))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Config), nameof(Config.CanSpawnBlood))),
                new(OpCodes.Brfalse_S, returnLabel),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(CharacterClassManager), nameof(CharacterClassManager._hub))),
                new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Ldarg_2),
                new(OpCodes.Ldarg_3),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(PlacingBloodEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev.LocalIndex),
                new(OpCodes.Call, Method(typeof(Handlers.Map), nameof(Handlers.Map.OnPlacingBlood))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(PlacingBloodEventArgs), nameof(PlacingBloodEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, returnLabel),
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(PlacingBloodEventArgs), nameof(PlacingBloodEventArgs.Position))),
                new(OpCodes.Starg_S, 1),
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(PlacingBloodEventArgs), nameof(PlacingBloodEventArgs.Type))),
                new(OpCodes.Starg_S, 2),
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(PlacingBloodEventArgs), nameof(PlacingBloodEventArgs.Multiplier))),
                new(OpCodes.Starg_S, 3),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
