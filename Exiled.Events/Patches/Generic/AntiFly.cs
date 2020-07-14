// -----------------------------------------------------------------------
// <copyright file="AntiFly.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
#pragma warning disable SA1313
    using Exiled.Events;
    using HarmonyLib;
    using UnityEngine;

    /// <summary>
    /// Patches <see cref="PlayerMovementSync.AntiFly(Vector3, bool)"/>.
    /// </summary>
    [HarmonyPatch(typeof(PlayerMovementSync), nameof(PlayerMovementSync.AntiFly))]
    internal class AntiFly
    {
        private static bool Prefix() => Events.Instance.Config.IsAntyFlyEnabled;
    }
}
