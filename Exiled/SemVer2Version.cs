// -----------------------------------------------------------------------
// <copyright file="SemVer2Version.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled
{
    using System;
    using System.Text.RegularExpressions;

    public readonly struct SemVer2Version
    {
        public static readonly Regex SemVerRegex = new Regex(
            "^(0|[1-9]\\d*)\\.(0|[1-9]\\d*)\\.(0|[1-9]\\d*)(?:-((?:0|[1-9]\\d*|\\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\\.(?:0|[1-9]\\d*|\\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\\+([0-9a-zA-Z-]+(?:\\.[0-9a-zA-Z-]+)*))?$",
            RegexOptions.Compiled | RegexOptions.Singleline);

        public readonly int Major;
        public readonly int Minor;
        public readonly int Patch;

        public readonly Version Backwards;

        public SemVer2Version(int major, int minor, int patch, Version backwards)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
            Backwards = backwards;
        }

        public static SemVer2Version Parse(string version)
        {
            if (!TryParse(version, out var output))
                throw new ArgumentException();

            return output;
        }

        public static bool TryParse(string version, out SemVer2Version output)
        {
            output = default;

            var match = SemVerRegex.Match(version);

            if (!match.Success)
                return false;

            int major, minor, patch = -1;

            major = int.Parse(match.Groups[1].Value);
            minor = int.Parse(match.Groups[2].Value);
            patch = int.Parse(match.Groups[3].Value);

            var backwards = new Version(major, minor, patch);

            output = new SemVer2Version(major, minor, patch, backwards);
            return true;
        }
    }
}
