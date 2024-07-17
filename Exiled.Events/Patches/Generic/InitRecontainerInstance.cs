// -----------------------------------------------------------------------
// <copyright file="InitRecontainerInstance.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
    using API.Features;

    using HarmonyLib;

    using PlayerRoles.PlayableScps.Scp079;

    /// <summary>
    /// Patches <see cref="Scp079Recontainer.Start"/>.
    /// </summary>
    [HarmonyPatch(typeof(Scp079Recontainer), nameof(Scp079Recontainer.Start))]
    internal class InitRecontainerInstance
    {
        private static void Postfix(Scp079Recontainer __instance) => Recontainer.Base = __instance;
    }
}