// -----------------------------------------------------------------------
// <copyright file="NpcExtensions.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Extensions
{
    using Exiled.API.Features;
    using UnityEngine;

    /// <summary>
    /// Various extension methods.
    /// </summary>
    public static class NpcExtensions
    {
        /// <summary>
        /// Checks if a <see cref="Player"/> is an npc.
        /// </summary>
        /// <param name="player">The player to check.</param>
        /// <returns>A value indicating the player is an NPC.</returns>
        public static bool IsNpc(this Player player) => player.GameObject.IsNpc();

        /// <summary>
        /// Checks if a <see cref="ReferenceHub"/> is an npc.
        /// </summary>
        /// <param name="referenceHub">The player to check.</param>
        /// <returns>A value indicating the player is an NPC.</returns>
        public static bool IsNpc(this ReferenceHub referenceHub) => referenceHub.gameObject.IsNpc();

        /// <summary>
        /// Checks a <see cref="GameObject"/> to see if it belongs to an npc.
        /// </summary>
        /// <param name="gameObject">The object to check.</param>
        /// <returns>A value indicating the gameobject is an NPC.</returns>
        public static bool IsNpc(this GameObject gameObject) => Npc.Dictionary.ContainsKey(gameObject);
    }
}
