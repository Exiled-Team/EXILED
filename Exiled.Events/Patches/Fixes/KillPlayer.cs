// -----------------------------------------------------------------------
// <copyright file="KillPlayer.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.DamageHandlers;
    using EventArgs.Player;
    using Exiled.API.Features.Pools;
    using HarmonyLib;
    using Mirror;
    using PlayerStatsSystem;

    using static HarmonyLib.AccessTools;

    using DamageHandlerBase = PlayerStatsSystem.DamageHandlerBase;

    /// <summary>
    /// Prefix of KillPlayer action.
    /// </summary>
    [HarmonyPatch(typeof(PlayerStats), nameof(PlayerStats.KillPlayer))]
    internal class KillPlayer
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(KillingPlayerEventArgs));
            LocalBuilder handler = generator.DeclareLocal(typeof(GenericDamageHandler));

            Label retLabel = generator.DefineLabel();
            Label continueLabel = generator.DefineLabel();
            Label skipLabel = generator.DefineLabel();

            newInstructions[newInstructions.Count - 1].labels.Add(retLabel);
            newInstructions[0].labels.Add(skipLabel);

            newInstructions.InsertRange(
                0,
                new[]
                {
                    new(OpCodes.Ldsfld, Field(typeof(DamageHandlers), nameof(DamageHandlers.IdsByTypeHash))),
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Call, Method(typeof(DamageHandlerBase), nameof(DamageHandlerBase.GetType))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Type), nameof(Type.FullName))),
                    new(OpCodes.Call, Method(typeof(Extensions), nameof(Extensions.GetStableHashCode))),
                    new(OpCodes.Call, Method(typeof(Dictionary<int, byte>), nameof(Dictionary<int, byte>.ContainsKey))),
                    new(OpCodes.Brtrue_S, retLabel),

                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Isinst, typeof(GenericDamageHandler)),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, handler.LocalIndex),
                    new(OpCodes.Brfalse_S, continueLabel),

                    new(OpCodes.Ldloc_S, handler.LocalIndex),
                    new(OpCodes.Starg_S, 1),
                    new(OpCodes.Br_S, skipLabel),

                    new CodeInstruction(OpCodes.Ldarg_0).WithLabels(continueLabel),
                    new(OpCodes.Ldfld, Field(typeof(PlayerStats), nameof(PlayerStats._hub))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    new(OpCodes.Ldarg_1),

                    new(OpCodes.Ldc_I4_1),

                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(KillingPlayerEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnKillPlayer))),

                    new(OpCodes.Callvirt, PropertyGetter(typeof(KillingPlayerEventArgs), nameof(KillingPlayerEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, retLabel),

                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(KillingPlayerEventArgs), nameof(KillingPlayerEventArgs.Handler))),
                    new(OpCodes.Starg_S, 1),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}