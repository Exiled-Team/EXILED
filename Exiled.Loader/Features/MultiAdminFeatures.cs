// -----------------------------------------------------------------------
// <copyright file="MultiAdminFeatures.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Loader.Features
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

#pragma warning disable SA1602 // Enumeration items should be documented
#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable SA1201 // Elements should appear in the correct order
#pragma warning disable SA1310 // Field names should not contain underscore
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    /// <summary>
    /// This class implements all possible MultiAdmin features.
    /// </summary>
    public static class MultiAdminFeatures
    {
        public enum EventType
        {
            ROUND_END,
            WAITING_FOR_PLAYERS,
            ROUND_START,
            SERVER_START,
            SERVER_FULL,
        }

        public enum ActionType
        {
            SET_SUPPORTED_FEATURES,
        }

        [Flags]
        public enum ModFeatures
        {
            None = 0,

            CustomEvents = 1 << 0,

            All = CustomEvents,
        }

        public const string MULTIADMIN_PREFIX = "multiadmin";
        public const string MULTIADMIN_CONSOLE_PREFIX = "-" + MULTIADMIN_PREFIX;
        public const string MULTIADMIN_EVENT_SUFFIX = "event";

        public static readonly char[] MultiAdminValueSeparator = { ':' };
        public static readonly string MultiAdminValueSeparatorStr = new(MultiAdminValueSeparator);

        public static readonly char[] MultiAdminKeySeparator = { '-' };
        public static readonly string MultiAdminKeySeparatorStr = new(MultiAdminKeySeparator);

        public static readonly bool MultiAdminUsed;
        public static readonly string MultiAdminVersion;
        public static readonly ModFeatures MultiAdminModFeatures;

        static MultiAdminFeatures()
        {
            foreach (string startArg in Environment.GetCommandLineArgs())
            {
                if (startArg.StartsWith(MULTIADMIN_CONSOLE_PREFIX, StringComparison.OrdinalIgnoreCase))
                {
                    MultiAdminUsed = true;

                    IEnumerable<string> separatedInfo = startArg.Split(MultiAdminValueSeparator).Skip(1);
                    MultiAdminVersion = separatedInfo.ElementAtOrDefault(0);
                    string features = separatedInfo.ElementAtOrDefault(1);
                    if (!string.IsNullOrEmpty(features) && int.TryParse(features, out int modFeatures))
                    {
                        MultiAdminModFeatures = (ModFeatures)modFeatures;
                        return;
                    }

                    ServerConsole.AddLog($"Failed to parse MultiAdmin ModFeatures! Source: {features}", ConsoleColor.Red);
                    break;
                }
            }
        }

        public static bool CallEvent(EventType eventType)
        {
            if (!MultiAdminUsed)
                return false;

            if (ServerStatic.ServerOutput is null)
                return false;

            ServerStatic.ServerOutput.AddLog(ConvertToMultiAdminAvailable(eventType), ConsoleColor.White);
            return true;
        }

        public static bool CallAction(ActionType actionType, object value = null)
        {
            if (!MultiAdminUsed)
                return false;

            if (ServerStatic.ServerOutput is null)
                return false;

            string multiAdminAvailable = ConvertToMultiAdminAvailable(actionType);
            if (!(value is null))
                multiAdminAvailable = string.Concat(multiAdminAvailable, MultiAdminValueSeparatorStr, value);

            ServerStatic.ServerOutput.AddLog(multiAdminAvailable, ConsoleColor.White);
            return true;
        }

        public static string ConvertToMultiAdminAvailable(EventType eventType)
        {
            string eventName = PrepareStr(eventType.ToString());
            return string.Concat(MULTIADMIN_PREFIX, MultiAdminValueSeparatorStr, eventName, MultiAdminKeySeparatorStr, MULTIADMIN_EVENT_SUFFIX);
        }

        public static string ConvertToMultiAdminAvailable(ActionType actionType)
        {
            string actionName = PrepareStr(actionType.ToString());
            return string.Concat(MULTIADMIN_PREFIX, MultiAdminValueSeparatorStr, actionName);
        }

        public static string PrepareStr(string value) => value.Replace("_", MultiAdminKeySeparatorStr).ToLowerInvariant();
    }
}