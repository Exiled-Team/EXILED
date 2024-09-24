// -----------------------------------------------------------------------
// <copyright file="TestGamemodeGameState.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Example.TestGamemode
{
    using Exiled.API.Features;
    using Exiled.CustomModules.API.Features.CustomGameModes;

    /// <inheritdoc />
    public class TestGamemodeGameState : GameState
    {
        /// <inheritdoc />
        public override void Start(bool isForced = false)
        {
            Map.Broadcast(5, "Test Gamemode Started");
            base.Start(isForced);
        }

        /// <inheritdoc />
        public override void End(bool isForced = false)
        {
            Map.Broadcast(5, "Test Gamemode Ended");
            base.End(isForced);
        }

        /// <inheritdoc />
        protected override bool EvaluateEndingConditions()
        {
            return false;
        }
    }
}