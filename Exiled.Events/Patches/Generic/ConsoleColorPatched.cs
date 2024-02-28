// -----------------------------------------------------------------------
// <copyright file="ConsoleColorPatched.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    using HarmonyLib;
    using UnityEngine;

    /// <summary>
    /// Patches <see cref="ServerConsole.PrintFormattedString"/>.
    /// </summary>
    [HarmonyPatch(typeof(ServerConsole), nameof(ServerConsole.PrintFormattedString))]
    internal class ConsoleColorPatched
    {
        private static readonly AnsiUsage Testing = AnsiUsage.All;
        private static readonly Regex TagDetector = new(@"<([a-z]{1,13})(?:=([^>]+))?>(.*?)<\/\1>", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);

        private static readonly Dictionary<Color32, int> AnsiColors = new()
        {
            { new Color32(0, 0, 0, 255),        37 },  // Black (30) -> Convert to White
            { new Color32(128, 0, 0, 255),      31 },  // Red
            { new Color32(0, 128, 0, 255),      32 },  // Green
            { new Color32(128, 128, 0, 255),    33 },  // Yellow
            { new Color32(0, 0, 128, 255),      34 },  // Blue
            { new Color32(128, 0, 128, 255),    35 },  // Magenta
            { new Color32(0, 128, 128, 255),    36 },  // Cyan
            { new Color32(192, 192, 192, 255),  37 },  // White
            { new Color32(128, 128, 128, 255),  37 },  // Dark Gray (38) -> Convert to White
            { new Color32(255, 0, 0, 255),      91 },  // Dark Red
            { new Color32(0, 255, 0, 255),      92 },  // Dark Green
            { new Color32(255, 255, 0, 255),    93 },  // Dark Yellow
            { new Color32(0, 0, 255, 255),      94 },  // Dark Blue
            { new Color32(255, 0, 255, 255),    95 },  // Dark Magenta
            { new Color32(0, 255, 255, 255),    96 },  // Dark Cyan
            { new Color32(255, 255, 255, 255),  97 },  // Dark White
        };

        [Flags]
        private enum AnsiUsage
        {
            None = 0,
            Enable = 1,
            StartWithAnsi = 2 | Enable,
            ForceDefaultColor = 4 | Enable,
            FullColor = 8 | Enable,
            All = -1,
        }

        private static bool Prefix(ServerConsole __instance, string text, ConsoleColor defaultColor)
        {
            defaultColor = defaultColor is ConsoleColor.Black or ConsoleColor.DarkGray ? ConsoleColor.White : defaultColor;
            string defaultAnsiColor = Testing.HasFlag(AnsiUsage.ForceDefaultColor) && Misc.TryParseColor(ServerConsole.ConsoleColorToHex(defaultColor), out Color32 color32)
                ? ClosestAnsiColor(color32) : "39";

            bool find = true;

            while (find)
            {
                find = false;
                text = TagDetector.Replace(text, match =>
                {
                    if (match.Index is not 0 && text[match.Index - 1] is '\\')
                        return match.Value;
                    find = true;
                    string tag = match.Groups[1].Value.ToLower();
                    string value = match.Groups[2].Value;
                    string content = match.Groups[3].Value;

                    if (PluginAPI.Core.Log.DisableBetterColors && Testing.HasFlag(AnsiUsage.Enable))
                        return content;

                    return content = tag switch
                    {
                        "color" => Misc.TryParseColor(value, out Color32 color32) ? $"\u001b[{ClosestAnsiColor(color32)}m{content}\u001b[{defaultAnsiColor}m" : content,
                        "mark" => Misc.TryParseColor(value, out Color32 color32) ? $"\u001b[{ClosestAnsiColor(color32, true)}m{content}\u001b[49m" : content,
                        "b" => $"\u001b[1m{content}\u001b[22m",
                        "i" => $"\u001b[3m{content}\u001b[23m",
                        "u" => $"\u001b[4m{content}\u001b[24m",
                        _ => content,
                    };
                });
            }

            if (!PluginAPI.Core.Log.DisableBetterColors && Testing.HasFlag(AnsiUsage.StartWithAnsi))
                text = $"\u001b[{defaultAnsiColor}m" + text;
            ServerStatic.ServerOutput?.AddLog(text, defaultColor);
            return false;
        }

        private static string ClosestAnsiColor(Color32 color, bool isBackgroundColor = false)
        {
            if (Testing.HasFlag(AnsiUsage.FullColor))
                return $"{(isBackgroundColor ? "48" : "38")};2;{color.r};{color.g};{color.b}";

            // Initialize variables for closest color
            int closestColor = 37; // Default to reset
            double closestDistance = double.MaxValue;

            // Calculate distance from given color to each Ansi color
            foreach (KeyValuePair<Color32, int> ansiColor in AnsiColors)
            {
                double distance = Math.Pow(color.r - ansiColor.Key.r, 2) +
                                  Math.Pow(color.g - ansiColor.Key.g, 2) +
                                  Math.Pow(color.b - ansiColor.Key.b, 2);

                // Update closest color if distance is smaller
                if (distance < closestDistance)
                {
                    closestColor = ansiColor.Value;
                    closestDistance = distance;
                }
            }

            if (isBackgroundColor)
                closestColor += 10;

            return closestColor.ToString();
        }
    }
}