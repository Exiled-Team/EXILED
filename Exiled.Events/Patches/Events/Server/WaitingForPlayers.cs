// -----------------------------------------------------------------------
// <copyright file="WaitingForPlayers.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Server {
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.API.Features;

    using HarmonyLib;

#pragma warning disable SA1600 // Elements should be documented

    /// <summary>
    /// Patches a method, the class in which it's defined, is compiler-generated, <see cref="CharacterClassManager"/>.
    /// Adds the WaitingForPlayers event.
    /// </summary>
    internal static class WaitingForPlayers {
        private static readonly HarmonyMethod PatchMethod = new HarmonyMethod(typeof(WaitingForPlayers), nameof(Transpiler));

        private static Type type;
        private static MethodInfo method;

        internal static void Patch() {
            const string innerName = "<Init>d__115";
            const string methodName = "MoveNext";

            type = AccessTools.Inner(typeof(CharacterClassManager), innerName);
            method = type != null ? AccessTools.Method(type, methodName) : null;
            if (type == null || method == null) {
                Log.Error($"Couldn't find `{innerName}::{methodName}` ({type == null}, {method == null}) inside `CharacterClassManager`: Server::WaitingForPlayers event won't fire");
                return;
            }

            Exiled.Events.Events.Instance.Harmony.Patch(method, transpiler: PatchMethod);
        }

        internal static void Unpatch() {
            if (type != null && method != null) {
                Exiled.Events.Events.Instance.Harmony.Unpatch(method, PatchMethod.method);

                type = null;
                method = null;
            }
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
            bool did = false;
            foreach (CodeInstruction inst in instructions) {
                if (!did && inst.opcode == OpCodes.Ldstr && ((string)inst.operand) == "Waiting for players...") {
                    did = true;
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Handlers.Server), nameof(Handlers.Server.OnWaitingForPlayers)));
                }

                yield return inst;
            }
        }
    }
}
