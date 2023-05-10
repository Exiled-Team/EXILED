// -----------------------------------------------------------------------
// <copyright file="SavingVoice.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp939
{
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
    using Exiled.Events.EventArgs.Scp939;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using PlayerRoles.PlayableScps.Scp939;
    using PlayerRoles.PlayableScps.Scp939.Mimicry;
    using PlayerStatsSystem;
    using VoiceChat;

    /// <summary>
    ///     Patches <see cref="MimicryRecorder.OnAnyPlayerKilled(ReferenceHub, DamageHandlerBase)" />
    ///     to add the <see cref="Scp939.SavingVoice" /> event.
    /// </summary>
    [HarmonyPatch(typeof(MimicryRecorder), nameof(MimicryRecorder.OnAnyPlayerKilled))]
    internal static class SavingVoice
    {
        private static bool Prefix(MimicryRecorder __instance, ReferenceHub ply, DamageHandlerBase dh)
        {
            if (dh is not Scp939DamageHandler scp939DamageHandler)
                return false;

            if (scp939DamageHandler.Attacker.Hub != __instance.Owner)
                return false;

            if (VoiceChatMutes.IsMuted(ply))
                return false;

            // Should consider whether to allow users to have this with their own custom DNT
            if (!__instance.IsPrivacyAccepted(ply))
                return false;

            SavingVoiceEventArgs ev = new(__instance.Owner, ply);
            Scp939.OnSavingVoice(ev);

            if (!ev.IsAllowed)
            {
                return false;
            }

            __instance._syncPlayer = ply;
            __instance._syncMute = false;

            if (__instance.Owner.isLocalPlayer)
                __instance.SaveRecording(ply);
            else
                __instance.ServerSendRpc(false);

            __instance._serverSentVoices.Add(ply);
            __instance._serverSentConfirmations.Remove(ply);
            return false;
        }
    }
}