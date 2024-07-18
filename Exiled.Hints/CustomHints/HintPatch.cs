// -----------------------------------------------------------------------
// <copyright file="HintPatch.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using Hints;

namespace Exiled.API.Features.CustomHints
{
    using System;
    using System.Collections.Generic;

    using HarmonyLib;
    using Hints;
    using Mirror;

    /// <summary>
    /// The patch for hints.
    /// </summary>
    [HarmonyPatch(typeof(HintDisplay))]
    [HarmonyPatch("Show")]
    public static class HintPatch
    {
#pragma warning disable SA1313
        private static bool Prefix(HintDisplay __instance, ref global::Hints.Hint hint)
#pragma warning restore SA1313
        {
            if (hint.GetType() != typeof(TextHint))
            {
                return true;
            }

            TextHint textHint = (TextHint)hint;
            HintSys.Display(textHint.Text, textHint.DurationScalar, [Player.Get(__instance.netIdentity)]);
            return false;
        }
    }
}