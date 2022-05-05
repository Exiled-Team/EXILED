// -----------------------------------------------------------------------
// <copyright file="SetClass.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.NPCs
{
    using Exiled.API.Extensions;
    using HarmonyLib;
    using UnityEngine;

#pragma warning disable SA1118
    /// <summary>
    /// Patches <see cref="CharacterClassManager.SetPlayersClass"/> to force the lite argument to be true if the target is an npc.
    /// </summary>
    [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.SetPlayersClass))]
    internal static class SetClass
    {
        private static void Prefix(GameObject ply, ref bool lite)
        {
            if (ply.IsNpc())
                lite = true;
        }
    }
}
