// -----------------------------------------------------------------------
// <copyright file="TryUseBody.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp3114
{
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp3114;
    using Exiled.Events.Handlers;
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
    using HarmonyLib;
    using Mirror;
    using PlayerRoles.PlayableScps.Scp3114;
    using static NineTailedFoxAnnouncer;
    using static PlayerRoles.PlayableScps.Scp3114.Scp3114VoiceLines;
    using UnityEngine;

    /// <summary>
    ///     Patches <see cref="Scp3114VoiceLines.ServerPlayConditionally(VoiceLinesName)()" />.
    ///     Adds the <see cref="Handlers.Scp3114.VoiceLines" /> event.
    /// </summary>
    [EventPatch(typeof(Scp3114), nameof(Scp3114.VoiceLines))]
    [HarmonyPatch(typeof(Scp3114VoiceLines), nameof(Scp3114VoiceLines.ServerPlayConditionally))]
    internal class VoiceLines
    {
        private static bool Prefix(Scp3114VoiceLines.VoiceLinesName lineToPlay, Scp3114VoiceLines __instance)
        {
            if (!NetworkServer.active)
            {
                return false;
            }

            VoiceLinesDefinition voiceLinesDefinition = null;
            float num = float.PositiveInfinity;
            VoiceLinesDefinition[] voiceLines = __instance._voiceLines;
            foreach (VoiceLinesDefinition voiceLinesDefinition2 in voiceLines)
            {
                num = Mathf.Min(num, voiceLinesDefinition2.LastUseElapsedSeconds);
                if (voiceLinesDefinition2.Label == lineToPlay)
                {
                    voiceLinesDefinition = voiceLinesDefinition2;
                }
            }
            VoiceLinesEventArgs ev = new(__instance.Owner, voiceLinesDefinition);
            Handlers.Scp3114.OnVoiceLines(ev);

            voiceLinesDefinition = ev.VoiceLine;

            if (!ev.IsAllowed)
                return false;

            if (voiceLinesDefinition != null && !(voiceLinesDefinition.MinIdleTime > num) && voiceLinesDefinition.TryDrawNext(out var clipId))
            {
                __instance._syncName = (byte)lineToPlay;
                __instance._syncId = (byte)clipId;
                __instance.ServerSendRpc(toAll: true);
            }

            return false;
        }
    }
}