// -----------------------------------------------------------------------
// <copyright file="GrenadesFailToExplodeFix.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Grenades;

    using HarmonyLib;

    using UnityEngine;

    /// <summary>
    /// Fixes a Null Reference Exception being thrown caused by calling <see cref="Grenade.ServersideExplosion"/> from the coroutine <see cref="Grenade._Fuse(float)"/>.
    /// </summary>
    [HarmonyPatch(typeof(Grenade), nameof(Grenade.SetFuseTime))]
    internal static class GrenadesFailToExplodeFix
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var inst in instructions)
            {
                yield return inst;

                if (inst.opcode == OpCodes.Call && (MethodInfo)inst.operand == AccessTools.Method(typeof(Grenade), nameof(Grenade._Fuse)))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(Component), nameof(Component.gameObject)));
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(MECExtensionMethods2), nameof(MECExtensionMethods2.CancelWith), new[] { typeof(IEnumerator<float>), typeof(GameObject) }));
                }
            }
        }
    }
}
