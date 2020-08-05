// -----------------------------------------------------------------------
// <copyright file="LinuxPermissionNative.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

#pragma warning disable SA1600 // Elements should be documented

namespace Exiled.Updater
{
    using Mono.Unix;

    internal static class LinuxPermissionNative
    {
        internal static void SetExecutionAccess(string path)
        {
            UnixFileSystemInfo.GetFileSystemEntry(path).FileAccessPermissions |= FileAccessPermissions.UserReadWriteExecute | FileAccessPermissions.GroupReadWriteExecute;
        }
    }
}
