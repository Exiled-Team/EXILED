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
    using API.Features.Core.Generic.Pools;

    using Exiled.API.Features.Roles;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using PlayerRoles.FirstPersonControl;
    using PlayerRoles.FirstPersonControl.NetworkMessages;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// patches <see cref="FpcNoclip.IsActive" /> to add the
    /// <see cref="Handlers.Player.TogglingNoClip" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.TogglingNoClip))]
    [HarmonyPatch(typeof(FpcNoclipToggleMessage), nameof(FpcNoclipToggleMessage.ProcessMessage))]
    internal static class TogglingNoClip
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label retLabel = generator.DefineLabel();
            LocalBuilder ev = generator.DeclareLocal(typeof(TogglingNoClipEventArgs));
            LocalBuilder isPermited = generator.DeclareLocal(typeof(bool));

            int offset = 1;
            int index = newInstructions.FindIndex(x => x.operand == (object)Method(typeof(FpcNoclip), nameof(FpcNoclip.IsPermitted))) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // isPermited = FpcNoclip.IsPermitted(referenceHub)
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Player.Get(hub)
                    new CodeInstruction(OpCodes.Ldloc_0),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // player.IsNoclipEnabled
                    new(OpCodes.Dup),
                    new(OpCodes.Call, PropertyGetter(typeof(Player), nameof(Player.IsNoclipEnabled))),

                    // isPermited
                    new(OpCodes.Ldloc_S, ev.LocalIndex),

                    // TogglingNoClipEventArgs ev = new(Player, bool, bool);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(TogglingNoClipEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Player.OnTogglingNoClip(ev);
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnTogglingNoClip))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(TogglingNoClipEventArgs), nameof(TogglingNoClipEventArgs.IsAllowed))),
                    new(OpCodes.Ldc_I4_0),
                    new(OpCodes.Ceq),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}