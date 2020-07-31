// -----------------------------------------------------------------------
// <copyright file="Respawn.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System.Linq;

    using Exiled.API.Enums;

    using Respawning;

    /// <summary>
    /// A set of tools to handle team respawns more easily.
    /// </summary>
    public static class Respawn
    {
        /// <summary>
        /// Gets the actual <see cref="RespawnEffectsController"/>.
        /// </summary>
        public static RespawnEffectsController Controller => RespawnEffectsController.AllControllers.Where(controller => controller != null).FirstOrDefault();

        /// <summary>
        /// Play effects when a certain class spawns.
        /// </summary>
        /// <param name="effects">The effects to be played.</param>
        public static void PlayEffects(byte[] effects) => Controller.RpcPlayEffects(effects);

        /// <summary>
        /// Play effects when a certain class spawns.
        /// </summary>
        /// <param name="effects">The effects to be played.</param>
        public static void PlayEffects(RespawnEffectType[] effects) => PlayEffects(effects.Select(effect => (byte)effect).ToArray());
    }
}
