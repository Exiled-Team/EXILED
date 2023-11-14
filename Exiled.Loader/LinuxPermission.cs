// -----------------------------------------------------------------------
// <copyright file="LinuxPermission.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Loader
{
    using Mono.Unix;

    /// <summary>
    /// A set of extensions to easily interact with Linux/Unix environment.
    /// </summary>
    internal static class LinuxPermission
    {
        /// <summary>
        /// Sets rw and execution permissions given a file, for the current user and group.
        /// </summary>
        /// <param name="path">The path of the file.</param>
        internal static void SetFileUserAndGroupReadWriteExecutePermissions(string path)
        {
            UnixFileSystemInfo.GetFileSystemEntry(path).FileAccessPermissions |= FileAccessPermissions.UserReadWriteExecute | FileAccessPermissions.GroupReadWriteExecute;
        }
    }
}