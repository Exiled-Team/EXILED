// -----------------------------------------------------------------------
// <copyright file="PlayingVoice.cs" company="Exiled Team">
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
    using Mirror;
    using PlayerRoles.PlayableScps.Scp939;
    using PlayerRoles.PlayableScps.Scp939.Mimicry;
    using Utils.Networking;

    /// <summary>
    ///     Patches <see cref="MimicryRecorder.ServerProcessCmd(NetworkReader)" />
    ///     to add the <see cref="Scp939.PlayingVoice" /> event.
    /// </summary>
    [HarmonyPatch(typeof(MimicryRecorder), nameof(MimicryRecorder.ServerProcessCmd))]
    internal static class PlayingVoice
    {
        private static bool Prefix(MimicryRecorder __instance, NetworkReader reader)
        {
            __instance.ServerProcessCmd(reader);
            ReferenceHub rh = reader.ReadReferenceHub();
            if (!__instance._serverSentVoices.Contains(rh))
                return false;
            if (!__instance._serverSentConfirmations.Add(rh))
                return false;

            PlayingVoiceEventArgs ev = new(__instance.Owner, rh);
            Scp939.OnPlayingVoice(ev);
            if (!ev.IsAllowed)
                return false;

            __instance.ServerSendRpc((ReferenceHub x) => x == rh);
            return false;
        }
    }
}
