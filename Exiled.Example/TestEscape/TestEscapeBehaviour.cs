// -----------------------------------------------------------------------
// <copyright file="TestEscapeBehaviour.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Example.TestEscape
{
    using Exiled.API.Features;
    using Exiled.CustomModules.API.Enums;
    using Exiled.CustomModules.API.Features.CustomEscapes;
    using Exiled.CustomModules.Events.EventArgs.CustomEscapes;

    /// <inheritdoc />
    public class TestEscapeBehaviour : EscapeBehaviour
    {
        /// <inheritdoc />
        protected override UUEscapeScenarioType CalculateEscapeScenario()
        {
            return CustomEscapeScenarioType.CustomScenario;
        }

        /// <inheritdoc />
        protected override void OnEscaping(EscapingEventArgs ev)
        {
            base.OnEscaping(ev);

            Log.ErrorWithContext($"Player {ev.Player.Nickname} is escaping");
        }
    }
}