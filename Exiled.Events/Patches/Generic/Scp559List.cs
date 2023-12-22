// -----------------------------------------------------------------------
// <copyright file="Scp559List.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using Exiled.API.Features;
    using HarmonyLib;

#pragma warning disable SA1313

    /// <summary>
    /// Patches <see cref="Scp559Cake.Start"/>
    /// to control <see cref="Scp559.List"/>.
    /// </summary>
    [HarmonyPatch(typeof(Scp559Cake), nameof(Scp559Cake.Start))]
    internal class Scp559List
    {
        private static void Postfix(Scp559Cake __instance)
        {
            _ = new Scp559(__instance);
        }
    }
}