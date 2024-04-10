// -----------------------------------------------------------------------
// <copyright file="DestroyRecontainerInstance.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Core.Generic.Pools;

    using HarmonyLib;

    using PlayerRoles.PlayableScps.Scp079;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp079Recontainer.OnDestroy"/>.
    /// </summary>
    [HarmonyPatch(typeof(Scp079Recontainer), nameof(Scp079Recontainer.OnDestroy))]
    internal class DestroyRecontainerInstance
    {
        private static void Postfix() => Recontainer.Base = null;
    }
}