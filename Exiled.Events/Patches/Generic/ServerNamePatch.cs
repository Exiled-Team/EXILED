// -----------------------------------------------------------------------
// <copyright file="ServerNamePatch.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using HarmonyLib;
    using NorthwoodLib.Pools;

    using static Exiled.Events.Events;
    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches the <see cref="ServerConsole.ReloadServerName"/>.
    /// </summary>
    [HarmonyPatch(typeof(ServerConsole), nameof(ServerConsole.ReloadServerName))]
    internal static class ServerNamePatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);
            Label skipLabel = generator.DefineLabel();

            newInstructions.InsertRange(newInstructions.Count - 1, new CodeInstruction[]
            {
                new(OpCodes.Call, PropertyGetter(typeof(Exiled.Events.Events), nameof(Instance))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Exiled.Events.Events), nameof(Exiled.Events.Events.Config))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Config), nameof(Config.IsNameTrackingEnabled))),
                new(OpCodes.Brfalse_S, skipLabel),

                new(OpCodes.Ldstr, "{0} <color=#00000000><size=1>Exiled {1}</size></color>"),

                new(OpCodes.Ldsfld, Field(typeof(ServerConsole), nameof(ServerConsole._serverName))),

                new(OpCodes.Call, PropertyGetter(typeof(Exiled.Events.Events), nameof(Instance))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Exiled.Events.Events), nameof(Exiled.Events.Events.Version))),
                new(OpCodes.Ldc_I4_3),
                new(OpCodes.Call, Method(typeof(Version), nameof(Version.ToString), new[] { typeof(int) })),

                new(OpCodes.Call, Method(typeof(string), nameof(string.Format), new[] { typeof(string), typeof(object), typeof(object) })),
                new(OpCodes.Stsfld, Field(typeof(ServerConsole), nameof(ServerConsole._serverName))),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(skipLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}