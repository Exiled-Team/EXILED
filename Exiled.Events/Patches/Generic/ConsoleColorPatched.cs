// -----------------------------------------------------------------------
// <copyright file="ConsoleColorPatched.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
#pragma warning disable SA1117 // Parameters should be on same line or separate lines

#pragma warning disable SA1312 // Variable names should begin with lower-case letter
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
    using HarmonyLib;

    using UnityEngine;

    /// <summary>
    /// Patches <see cref="ServerConsole.PrintFormattedString"/>.
    /// </summary>
    [HarmonyPatch(typeof(ServerConsole), nameof(ServerConsole.PrintFormattedString))]
    internal class ConsoleColorPatched
    {
        private static readonly Dictionary<Color, int> AnsiColors = new()
        {
            { new Color(0, 0, 0),                                   37 },  // Black -> Convert to White
            { new Color(128 / 255.0f, 0, 0),                        31 },  // Red
            { new Color(0, 128 / 255.0f, 0),                        32 },  // Green
            { new Color(128 / 255.0f, 128 / 255.0f, 0),             33 },  // Yellow
            { new Color(0, 0, 128 / 255.0f),                        34 },  // Blue
            { new Color(128 / 255.0f, 0, 128 / 255.0f),             35 },  // Magenta
            { new Color(0, 128 / 255.0f, 128 / 255.0f),             36 },  // Cyan
            { new Color(192 / 255.0f, 192 / 255.0f, 192 / 255.0f),  37 },  // White
            { new Color(128 / 255.0f, 128 / 255.0f, 128 / 255.0f),  37 },  // Dark Gray -> Convert to White
            { new Color(1, 0, 0),                                   91 },  // Dark Red
            { new Color(0, 1, 0),                                   92 },  // Dark Green
            { new Color(1, 1, 0),                                   93 },  // Dark Yellow
            { new Color(0, 0, 1),                                   94 },  // Dark Blue
            { new Color(1, 0, 1),                                   95 },  // Dark Magenta
            { new Color(0, 1, 1),                                   96 },  // Dark Cyan
            { new Color(1, 1, 1),                                   97 },  // Dark White
        };

        private static bool Prefix(ServerConsole __instance, string text, ConsoleColor defaultColor)
        {
            string result = text;

            // Color, Bold, Italic, Size
            result = Regex.Replace(result, @"<(color|b|i|u|size)=("".*?""|'.*?'|[^'"">]+)>(.*?)<\/\1>", match =>
            {
                string tag = match.Groups[1].Value.ToLower();
                string value = match.Groups[2].Value;
                string content = match.Groups[3].Value;
                if (PluginAPI.Core.Log.DisableBetterColors)
                    return content;
                return content = tag switch
                {
                    "color" => Misc.TryParseColor(value, out Color32 color32) ? $"\u001b[{ClosestAnsiColor(color32)}m{content}\u001b[22m" : content,
                    "b" => $"\u001b[1m{content}\u001b[22m",
                    "i" => $"\u001b[2m{content}\u001b[23m",
                    "u" => $"\u001b[4m{content}\u001b[24m",
                    _ => content,
                };
            }, RegexOptions.IgnoreCase);

            ServerStatic.ServerOutput?.AddLog(result, defaultColor);
            return false;
        }

        private static int ClosestAnsiColor(Color color)
        {
            // Initialize variables for closest color
            int closestColor = 0; // Default to reset
            double closestDistance = double.MaxValue;

            // Calculate distance from given color to each Bukkit API color
            foreach (KeyValuePair<Color, int> AnsiColor in AnsiColors)
            {
                double distance = Math.Pow(color.r - (AnsiColor.Key.r / 1), 2) +
                                  Math.Pow(color.g - (AnsiColor.Key.g / 1), 2) +
                                  Math.Pow(color.b - (AnsiColor.Key.b / 1), 2);

                // Update closest color if distance is smaller
                if (distance < closestDistance)
                {
                    closestColor = AnsiColor.Value;
                    closestDistance = distance;
                }
            }

            return closestColor;
        }
    }
}