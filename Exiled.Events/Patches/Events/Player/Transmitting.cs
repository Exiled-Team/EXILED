// -----------------------------------------------------------------------
// <copyright file="Transmitting.cs" company="Exiled Team">
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
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using PlayerRoles.Voice;

    using VoiceChat.Playbacks;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="PersonalRadioPlayback.Update()" />.
    /// Adds the <see cref="Handlers.Player.Transmitting" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.Transmitting))]
    [HarmonyPatch(typeof(PersonalRadioPlayback), nameof(PersonalRadioPlayback.Update))]
    internal static class Transmitting
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label retLabel = generator.DefineLabel();

            const int offset = -2;
            int index = newInstructions.FindIndex(
                instruction => instruction.Calls(Method(typeof(PersonalRadioPlayback), nameof(PersonalRadioPlayback.IsTransmitting)))) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // this
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),

                    // HandleTransmitting(PersonalRadioPlayback)
                    new(OpCodes.Call, Method(typeof(Transmitting), nameof(HandleTransmitting))),

                    // return false if not allowed
                    new(OpCodes.Brfalse_S, retLabel),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(retLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private static bool HandleTransmitting(PersonalRadioPlayback radioPlayback)
        {
            ReferenceHub hub = radioPlayback._owner;

            if (hub == null || Player.Get(hub) is not Player player || Server.Host.ReferenceHub == hub)
                return false;

            TransmittingEventArgs ev = new(player, ((IVoiceRole)player.RoleManager.CurrentRole).VoiceModule);

            Handlers.Player.OnTransmitting(ev);

            return ev.IsAllowed;
        }
    }
}