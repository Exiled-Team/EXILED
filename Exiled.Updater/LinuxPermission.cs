// -----------------------------------------------------------------------
// <copyright file="LinuxPermission.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Updater
{
    using Mono.Unix;

    internal static class LinuxPermission
    {
        internal static void SetFileUserAndGroupReadWriteExecutePermissions(string path)
        {
            UnixFileSystemInfo.GetFileSystemEntry(path).FileAccessPermissions |= FileAccessPermissions.UserReadWriteExecute | FileAccessPermissions.GroupReadWriteExecute;
        }
    }
}