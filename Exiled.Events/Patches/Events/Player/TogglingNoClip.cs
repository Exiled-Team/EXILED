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

    using Exiled.API.Features.Roles;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using PlayerRoles.FirstPersonControl;
    using PlayerRoles.FirstPersonControl.NetworkMessages;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     patches <see cref="FpcNoclip.IsActive" /> to add the
    ///     <see cref="Handlers.Player.TogglingNoClip" /> event.
    /// </summary>
    [HarmonyPatch(typeof(FpcNoclipToggleMessage), nameof(FpcNoclipToggleMessage.ProcessMessage))]
    internal static class TogglingNoClip
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label retLabel = generator.DefineLabel();
            LocalBuilder ev = generator.DeclareLocal(typeof(TogglingNoClipEventArgs));

            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Ldloc_0);

            Label checkLabel = newInstructions[index].ExtractLabels()[0];

            // Remove the base-game FpcNoclip.IsPermitted(hub) call, as we will be using that for our default value for ev.IsAllowed
            newInstructions.RemoveRange(index, 4);

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player.Get(hub)
                    new CodeInstruction(OpCodes.Ldloc_0).WithLabels(checkLabel),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                    new(OpCodes.Dup),

                    // GetInvertedNoClipStatus(player)
                    new(OpCodes.Call, Method(typeof(TogglingNoClip), nameof(GetInvertedNoClipStatus))),

                    // FpcNoclip.IsPermitted(hub)
                    new(OpCodes.Ldloc_0),
                    new(OpCodes.Call, Method(typeof(FpcNoclip), nameof(FpcNoclip.IsPermitted))),

                    // TogglingNoClipEventArgs ev = new(player, GetInvertedNoClipStatus(player), FpcNoclip.IsPermitted(hub));
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(TogglingNoClipEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev),

                    // Handlers.Player.OnTogglingNoClip(ev);
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnTogglingNoClip))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(TogglingNoClipEventArgs), nameof(TogglingNoClipEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, retLabel),

                    // if (ev.IsEnabled == GetNoClipStatus)
                    //    return;
                    // Note: If IsEnabled = true, and the player already has noclip, or IsEnabled = false and the player already doesn't have noclip, we return, since base-game code inverts the status.
                    new(OpCodes.Ldloc_S, ev),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(TogglingNoClipEventArgs), nameof(TogglingNoClipEventArgs.IsEnabled))),
                    new(OpCodes.Ldloc_S, ev),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(TogglingNoClipEventArgs), nameof(TogglingNoClipEventArgs.Player))),
                    new(OpCodes.Call, Method(typeof(TogglingNoClip), nameof(GetNoClipStatus))),
                    new(OpCodes.Beq_S, retLabel),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(retLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private static bool GetInvertedNoClipStatus(Player player) => player.Role is FpcRole fpc && !fpc.IsNoclipEnabled;

        private static bool GetNoClipStatus(Player player) => player.Role is FpcRole fpc && fpc.IsNoclipEnabled;
    }
}