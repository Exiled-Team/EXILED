// -----------------------------------------------------------------------
// <copyright file="VersionComparer.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Updater
{
    using System;
    using System.Collections.Generic;

#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable SA1602 // Enumeration items should be documented

    [Flags]
    internal enum CompareType
    {
        Equals = 1 << 0,
        More = 1 << 1,
        MoreOrEquals = 1 << 2,

        AllowLessThanZero = 1 << 3,
    }

    /// <summary>
    /// Default version comparer using <see cref="Version.CompareTo(Version)"/>.
    /// </summary>
    internal sealed class VersionComparer : IComparer<Version>
    {
        /// <summary>
        /// Readonly instance of <see cref="VersionComparer"/>.
        /// </summary>
        public static readonly VersionComparer Instance = new VersionComparer();

        /// <inheritdoc />
        public int Compare(Version x, Version y)
        {
            return x.CompareTo(y);
        }

#if DEBUG
        internal static bool CustomVersionGreaterOrEquals(Version v1, Version v2)
        {
            return CustomNumberCompare(v1.Major, v2.Major, CompareType.MoreOrEquals)
                || (CustomNumberCompare(v1.Minor, v2.Minor, CompareType.MoreOrEquals) &&
                    CustomNumberCompare(v1.Major, v2.Major, CompareType.MoreOrEquals))
                || (CustomNumberCompare(v1.Build, v2.Build, CompareType.MoreOrEquals | CompareType.AllowLessThanZero) &&
                    CustomNumberCompare(v1.Minor, v2.Minor, CompareType.MoreOrEquals) &&
                    CustomNumberCompare(v1.Major, v2.Major, CompareType.MoreOrEquals))
                || (CustomNumberCompare(v2.Revision, v2.Revision, CompareType.MoreOrEquals | CompareType.AllowLessThanZero) &&
                    CustomNumberCompare(v1.Build, v2.Build, CompareType.MoreOrEquals | CompareType.AllowLessThanZero) &&
                    CustomNumberCompare(v1.Minor, v2.Minor, CompareType.MoreOrEquals) &&
                    CustomNumberCompare(v1.Major, v2.Major, CompareType.MoreOrEquals));
        }
#endif

        internal static bool CustomVersionGreater(Version v1, Version v2)
        {
            return CustomNumberCompare(v1.Major, v2.Major, CompareType.More)
                || (CustomNumberCompare(v1.Minor, v2.Minor, CompareType.More) &&
                    CustomNumberCompare(v1.Major, v2.Major, CompareType.MoreOrEquals))
                || (CustomNumberCompare(v1.Build, v2.Build, CompareType.More | CompareType.AllowLessThanZero) &&
                    CustomNumberCompare(v1.Minor, v2.Minor, CompareType.MoreOrEquals) &&
                    CustomNumberCompare(v1.Major, v2.Major, CompareType.MoreOrEquals))
                || (CustomNumberCompare(v2.Revision, v2.Revision, CompareType.More | CompareType.AllowLessThanZero) &&
                    CustomNumberCompare(v1.Build, v2.Build, CompareType.MoreOrEquals | CompareType.AllowLessThanZero) &&
                    CustomNumberCompare(v1.Minor, v2.Minor, CompareType.MoreOrEquals) &&
                    CustomNumberCompare(v1.Major, v2.Major, CompareType.MoreOrEquals));
        }

        internal static bool CustomVersionEquals(Version v1, Version v2)
        {
            return CustomNumberCompare(v1.Major, v2.Major, CompareType.Equals)
                && CustomNumberCompare(v1.Minor, v2.Minor, CompareType.Equals)
                && CustomNumberCompare(v1.Build, v2.Build, CompareType.Equals | CompareType.AllowLessThanZero)
                && CustomNumberCompare(v2.Revision, v2.Revision, CompareType.Equals | CompareType.AllowLessThanZero);
        }

        internal static bool CustomNumberCompare(int value1, int value2, CompareType compareType)
        {
            bool result = false;

            if ((compareType & CompareType.More) != 0)
                result = value1 > value2;
            else if ((compareType & CompareType.Equals) != 0)
                result = value1 == value2;
            else if ((compareType & CompareType.MoreOrEquals) != 0)
                result = value1 >= value2;

            if ((compareType & CompareType.AllowLessThanZero) != 0)
                result = result || value1 < 0 || value2 < 0;

            return result;
        }
    }
}
