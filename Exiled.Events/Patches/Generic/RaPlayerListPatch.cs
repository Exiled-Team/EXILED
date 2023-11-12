// -----------------------------------------------------------------------
// <copyright file="RaPlayerListPatch.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using System.Linq;

    using Exiled.API.Features;
    using HarmonyLib;
    using RemoteAdmin.Communication;

    /// <summary>
    /// Implements <see cref="Player.RAEmojies"/>.
    /// </summary>
    [HarmonyPatch(typeof(RaPlayerList), nameof(RaPlayerList.GetPrefix))]
    internal class RaPlayerListPatch
    {
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
        private static void Postfix(ref string __result, ReferenceHub hub)
        {
            if (Player.TryGet(hub, out Player player) && player.RAEmojies.Any())
                __result += string.Join(" ", player.RAEmojies.Select(current => $"<color=white>[{current}]</color>")) + " ";
        }
    }
}