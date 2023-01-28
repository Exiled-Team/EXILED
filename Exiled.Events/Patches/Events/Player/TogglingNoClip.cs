// -----------------------------------------------------------------------
// <copyright file="TogglingNoClip.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Pools;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using PlayerRoles.FirstPersonControl.NetworkMessages;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     patches <see cref="FpcNoclipToggleMessage.ProcessMessage" /> to add the
    ///     <see cref="Handlers.Player.TogglingNoClip" /> event.
    /// </summary>
    [HarmonyPatch(typeof(FpcNoclipToggleMessage), nameof(FpcNoclipToggleMessage.ProcessMessage))]
    internal static class TogglingNoClip
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(TogglingNoClipEventArgs));
            LocalBuilder permitted = generator.DeclareLocal(typeof(bool));

            int index = newInstructions.FindIndex(x => x.opcode == OpCodes.Ldloc_0) + 2;

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // set FpcNoclip.IsPermitted() to local variable
                    new (OpCodes.Stloc_S, permitted.LocalIndex),

                    // Player.Get(this._hub)
                    new(OpCodes.Ldloc_0),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // FpcNoclip.IsPermitted()
                    new(OpCodes.Ldloc_S, permitted.LocalIndex),

                    // TogglingNoClipEventArgs ev = new(Player, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(TogglingNoClipEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Handlers.Player.OnTogglingNoClip(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnTogglingNoClip))),

                    // if (!ev.IsAllowed)
                    //    return;
                    // Note: return is called because if it's false, the if check in the method will just ret.
                    new(OpCodes.Callvirt, PropertyGetter(typeof(TogglingNoClipEventArgs), nameof(TogglingNoClipEventArgs.IsAllowed))),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}