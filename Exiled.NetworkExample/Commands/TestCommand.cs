// -----------------------------------------------------------------------
// <copyright file="TestCommand.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.NetworkExample.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Exiled.Network.API;
    using Exiled.Network.API.Attributes;
    using Exiled.Network.API.Interfaces;

    /// <inheritdoc/>
    [NPCommand]
    public class TestCommand : ICommand
    {
        /// <inheritdoc/>
        public string CommandName => "test";

        /// <inheritdoc/>
        public string Description { get; } = "Test Command";

        /// <inheritdoc/>
        public string Permission { get; } = string.Empty;

        /// <inheritdoc/>
        public bool IsRaCommand => true;

        /// <inheritdoc/>
        public void Invoke(PlayerFuncs player, List<string> arguments)
        {
            player.SendRAMessage("Test response");
        }
    }
}
