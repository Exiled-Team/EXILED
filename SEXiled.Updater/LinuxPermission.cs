// -----------------------------------------------------------------------
// <copyright file="LinuxPermission.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

#pragma warning disable SA1600 // Elements should be documented

namespace SEXiled.Updater
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
