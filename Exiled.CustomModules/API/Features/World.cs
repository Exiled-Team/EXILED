// -----------------------------------------------------------------------
// <copyright file="World.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features
{
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.API.Features.Core;
    using Exiled.API.Features.Core.Generic;
    using Exiled.CustomModules.API.Enums;
    using Exiled.CustomModules.API.Features.CustomGameModes;
    using Exiled.CustomModules.API.Features.CustomRoles;
    using Exiled.CustomModules.Events.EventArgs.CustomRoles;
    using Exiled.Events.EventArgs.Server;
    using MEC;

    using Server = Exiled.API.Features.Server;

    /// <summary>
    /// The world base.
    /// </summary>
    public class World : StaticActor<World>
    {
        /// <summary>
        /// The default dequeue rate.
        /// </summary>
#pragma warning disable SA1310
        public const float DEFAULT_DEQUEUE_RATE = 4f;
#pragma warning restore SA1310

        private GameState gameState;
        private CustomGameMode enqueuedGameMode;
        private CoroutineHandle queueHandle;
        private SummaryInfo summaryInfo;

        /// <summary>
        /// Gets the <see cref="CustomGameModes.GameState"/>.
        /// <para/>
        /// Requires <see cref="UUModuleType.CustomGameModes"/> to be enabled.
        /// </summary>
        public GameState GameState => gameState ??= GetComponent<GameState>();

        /// <summary>
        /// Gets or sets the dequeue rate, expressed in seconds.
        /// <para/>
        /// Requires <see cref="UUModuleType.CustomGameModes"/> to be enabled.
        /// <para/>
        /// If enqueued, the game mode will be continuously evaluated until it starts or the enqueued game mode changes.
        /// <br/>
        /// The <see cref="DequeueRate"/> defines the delay between each evaluation check.
        /// </summary>
        /// <seealso cref="DEFAULT_DEQUEUE_RATE"/>
        public float DequeueRate { get; set; } = DEFAULT_DEQUEUE_RATE;

        /// <summary>
        /// Gets the enqueued game mode.
        /// <para/>
        /// Requires <see cref="UUModuleType.CustomGameModes"/> to be enabled.
        /// </summary>
        public uint EnqueuedGameMode => enqueuedGameMode.Id;

        /// <summary>
        /// Gets or sets the running game mode.
        /// <para/>
        /// Requires <see cref="UUModuleType.CustomGameModes"/> to be enabled.
        /// </summary>
        public uint RunningGameMode { get; protected internal set; }

        /// <summary>
        /// Gets the current summary info.
        /// </summary>
        public SummaryInfo SummaryInfo
        {
            get
            {
                summaryInfo.Update();
                return summaryInfo;
            }
        }

        /// <summary>
        /// Enqueues a custom game mode for execution.
        /// <para/>
        /// Requires <see cref="UUModuleType.CustomGameModes"/> to be enabled.
        /// </summary>
        /// <param name="customGameMode">The custom game mode to enqueue.</param>
        /// <param name="continuously">Whether to continuously enqueue the game mode.</param>
        public virtual void EnqueueGameMode(CustomGameMode customGameMode, bool continuously = false)
        {
            if (!customGameMode)
                return;

            if (queueHandle.IsRunning)
                Timing.KillCoroutines(queueHandle);

            enqueuedGameMode = customGameMode;
            queueHandle = Timing.RunCoroutine(DequeueGameMode());
        }

        /// <summary>
        /// Enqueues a custom game mode by its ID for execution.
        /// <para/>
        /// Requires <see cref="UUModuleType.CustomGameModes"/> to be enabled.
        /// </summary>
        /// <param name="id">The ID of the custom game mode to enqueue.</param>
        /// <param name="continuously">Whether to continuously enqueue the game mode.</param>
        public virtual void EnqueueGameMode(object id, bool continuously = false)
        {
            if (!CustomGameMode.TryGet(id, out CustomGameMode gameMode))
                return;

            EnqueueGameMode(gameMode, continuously);
        }

        /// <summary>
        /// Clears the current game mode queue.
        /// <para/>
        /// Requires <see cref="UUModuleType.CustomGameModes"/> to be enabled.
        /// </summary>
        public virtual void ClearQueue()
        {
            if (!enqueuedGameMode)
                return;

            if (TryGetComponent(enqueuedGameMode.GameState, out EActor comp))
                comp.Destroy();

            enqueuedGameMode = null;
        }

        /// <summary>
        /// Starts the execution of the enqueued game mode.
        /// <para/>
        /// Requires <see cref="UUModuleType.CustomGameModes"/> to be enabled.
        /// </summary>
        /// <param name="customGameMode">The custom game mode to start.</param>
        /// <param name="isForced">Whether to force-start the game mode.</param>
        public virtual void Start(CustomGameMode customGameMode, bool isForced = false)
        {
            if (!isForced && customGameMode.Settings.MinimumPlayers < Server.PlayerCount)
                return;

            if (GameState is not null)
                RemoveComponent(GameState).Destroy();

            AddComponent(enqueuedGameMode.GameState);
            GameState.Start(isForced);
        }

        /// <inheritdoc />
        protected override void PostInitialize_Static()
        {
            if (enqueuedGameMode)
                return;

            IEnumerable<CustomGameMode> auto = CustomGameMode.Get(mode => mode.CanStartAuto);

            if (auto.IsEmpty())
                return;

            enqueuedGameMode = auto.Random();
        }

        /// <inheritdoc/>
        protected override void Tick()
        {
            base.Tick();

            if (GameState.MatchState != UEMatchState.Terminated)
                return;

            if (GameState.Settings.RestartRoundOnEnd)
                Timing.CallDelayed(GameState.Settings.RestartWindupTime, Server.Restart);

            CanEverTick = false;
        }

        /// <inheritdoc />
        protected override void EndPlay_Static()
        {
            Timing.KillCoroutines(queueHandle);
        }

        /// <inheritdoc />
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();

            if (CustomModules.IsModuleLoaded(UUModuleType.CustomRoles))
            {
                CustomRole.ChangedCustomRoleDispatcher.Bind(this, OnChangedCustomRole);

                Exiled.Events.Handlers.Server.EndingRound += OnEndingRound;
            }

            if (CustomModules.IsModuleLoaded(UUModuleType.CustomGameModes))
            {
                Exiled.Events.Handlers.Server.RoundStarted += OnRoundStarted;
            }
        }

        /// <inheritdoc />
        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();

            if (CustomModules.IsModuleLoaded(UUModuleType.CustomRoles))
            {
                CustomRole.ChangedCustomRoleDispatcher.Unbind(this);

                Exiled.Events.Handlers.Server.EndingRound -= OnEndingRound;
            }

            if (CustomModules.IsModuleLoaded(UUModuleType.CustomGameModes))
            {
                Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStarted;
            }
        }

        private void OnRoundStarted()
        {
            EnqueueGameMode(enqueuedGameMode);
        }

        private void OnEndingRound(EndingRoundEventArgs ev)
        {
            if (!ev.IsAllowed)
                return;

            foreach (CustomRole role in CustomRole.Get(cr => cr.TeamsOwnership.Length != 1 && cr.Instances > 1))
            {
                if (role.Owners.First().RoleBehaviour.EvaluateEndingConditions())
                    continue;

                ev.IsAllowed = false;
                return;
            }
        }

        private void OnChangedCustomRole(ChangedCustomRoleEventArgs ev)
        {
            if (!CustomRole.TryGet(ev.Role, out CustomRole role) || role.TeamsOwnership.Count() == 1)
                return;

            Round.IgnoredPlayers.Add(ev.Player);
        }

        private IEnumerator<float> DequeueGameMode()
        {
            for (; ;)
            {
                if (!enqueuedGameMode)
                    yield break;

                if (enqueuedGameMode.Settings.MinimumPlayers < Server.PlayerCount)
                {
                    yield return Timing.WaitForSeconds(DequeueRate);
                    continue;
                }

                Start(enqueuedGameMode, true);
                ClearQueue();
            }
        }
    }
}