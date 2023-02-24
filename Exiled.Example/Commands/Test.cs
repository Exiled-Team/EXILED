// -----------------------------------------------------------------------
// <copyright file="Test.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Example.Commands
{
    using System;

    using CommandSystem;

    using Exiled.API.Features;

    using MapGeneration.Distributors;

    /// <summary>
    /// This is an example of how commands should be made.
    /// </summary>
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Test : ICommand
    {
        /// <inheritdoc/>
        public string Command { get; } = "test";

        /// <inheritdoc/>
        public string[] Aliases { get; } = new[] { "t" };

        /// <inheritdoc/>
        public string Description { get; } = "A simple test command.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            // foreach (Room room in Room.List)
            //     Log.Warn(room);
            foreach (Door door in Door.List)
                Log.Warn(door);

            /*foreach (Camera camera in Camera.List)
                Log.Warn(camera);

            /*foreach (Locker locker in Map.Lockers)
                Log.Warn(locker);

            /*foreach (TeslaGate tesla in TeslaGate.List)
                Log.Warn(tesla);*/

            response = $"asasas sent the command!";

            // Return true if the command was executed successfully; otherwise, false.
            return true;
        }
    }
}