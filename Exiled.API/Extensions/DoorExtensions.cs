// -----------------------------------------------------------------------
// <copyright file="DoorExtensions.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Extensions
{

    using Exiled.API.Features;
    using InventorySystem.Items.Firearms;
    using RelativePositioning;

    public static class DoorExtensions
    {
        /// <summary>
        /// Plays a beep sound that only the target <paramref name="player"/> can hear.
        /// </summary>
        /// <param name="player">Target to play sound to.</param>
        public static void PlayBeepSound(this Player player) => MirrorExtensions.SendFakeTargetRpc(player, ReferenceHub.HostHub.networkIdentity, typeof(AmbientSoundPlayer), nameof(AmbientSoundPlayer.RpcPlaySound), 7);
    }
}
