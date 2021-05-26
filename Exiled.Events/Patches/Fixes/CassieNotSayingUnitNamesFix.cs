// -----------------------------------------------------------------------
// <copyright file="CassieNotSayingUnitNamesFix.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    #pragma warning disable SA1313

    using System.Text.RegularExpressions;

    using Exiled.API.Features;

    using HarmonyLib;

    using Respawning.NamingRules;

    /// <summary>
    /// Fixes Cassie ignoring unit name if it's changed via <see cref="Map.ChangeUnitColor(int, string)"/>.
    /// </summary>
    [HarmonyPatch(typeof(NineTailedFoxNamingRule), nameof(NineTailedFoxNamingRule.GetCassieUnitName))]
    public static class CassieNotSayingUnitNamesFix
    {
        private static bool Prefix(NineTailedFoxNamingRule __instance, string regular, ref string __result)
        {
            try
            {
                string[] array = Regex.Replace(regular, "<[^>]*?>", string.Empty).Split(new char[] { '-' });

                __result = $"NATO_{array[0][0]} {array[1]}";
            }
            catch
            {
                Log.Error("Error, couldn't convert '" + regular + "' into a CASSIE-readable form.");
                __result = "ERROR";
            }

            return false;
        }
    }
}
