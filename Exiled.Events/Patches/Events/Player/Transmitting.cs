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

    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using Mirror;

    using Exiled.API.Features.Pools;

    using PlayerRoles.Voice;

    using VoiceChat.Playbacks;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="PersonalRadioPlayback.IsTransmitting(ReferenceHub)" />.
    ///     Adds the <see cref="Handlers.Player.Transmitting" /> event.
    /// </summary>
    [HarmonyPatch(typeof(PersonalRadioPlayback), nameof(PersonalRadioPlayback.IsTransmitting))]
    internal static class Transmitting
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label retLabel = generator.DefineLabel();

            const int offset = -1;
            int index = newInstructions.FindIndex(
                instruction => instruction.Calls(PropertyGetter(typeof(NetworkBehaviour), nameof(NetworkBehaviour.isLocalPlayer)))) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // hub
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),

                    // voiceModule
                    new(OpCodes.Ldloc_1),

                    // HandleTransmitting(ReferenceHub, VoiceModule)
                    new(OpCodes.Call, Method(typeof(Transmitting), nameof(HandleTransmitting))),

                    // return false if not allowed
                    new(OpCodes.Brfalse_S, retLabel),
                });

            // -2 to return false
            newInstructions[newInstructions.Count - 2].WithLabels(retLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private static bool HandleTransmitting(ReferenceHub hub, VoiceModuleBase voiceModule)
        {
            if (hub == null || Player.Get(hub) is not Player player)
                return false;

            TransmittingEventArgs ev = new(player, voiceModule);

            Handlers.Player.OnTransmitting(ev);

            return ev.IsAllowed;
        }
    }
}