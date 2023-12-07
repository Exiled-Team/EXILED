// -----------------------------------------------------------------------
// <copyright file="EBehaviour.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Core
{
    using Exiled.API.Features;
    using Exiled.API.Features.DynamicEvents;

    /// <summary>
    /// <see cref="EBehaviour"/> is a versatile component designed to enhance the functionality of playable characters.
    /// <br>It can be easily integrated with various types of playable characters, making it a valuable tool for user-defined playable character behaviours.</br>
    /// </summary>
    public abstract class EBehaviour : EActor
    {
        /// <summary>
        /// Gets or sets the owner of the <see cref="EBehaviour"/>.
        /// </summary>
        public virtual Player Owner { get; protected set; }

        /// <inheritdoc/>
        protected override void PostInitialize()
        {
            base.PostInitialize();

            Owner = Player.Get(Base);
            if (!Owner)
            {
                Destroy();
                return;
            }
        }

        /// <inheritdoc/>
        protected override void Tick()
        {
            base.Tick();

            BehaviourUpdate_Implementation();
        }

        /// <inheritdoc/>
        protected override void OnEndPlay()
        {
            base.OnEndPlay();

            if (!Owner)
                return;
        }

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();

            DynamicEventManager.CreateFromTypeInstance(this);
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();

            DynamicEventManager.DestroyFromTypeInstance(this);
        }

        /// <summary>
        /// Fired every tick.
        /// <para>Code affecting the <see cref="EBehaviour"/>'s base implementation should be placed here.</para>
        /// </summary>
        protected virtual void BehaviourUpdate()
        {
        }

        /// <summary>
        /// Checks whether the given <see cref="Player"/> is the <see cref="Owner"/>.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to check.</param>
        /// <returns><see langword="true"/> if the <see cref="Player"/> is the <see cref="Owner"/>; otherwise, <see langword="false"/>.</returns>
        protected virtual bool Check(Player player) => player is not null && Owner == player;

        /// <inheritdoc cref="BehaviourUpdate"/>
        private protected virtual void BehaviourUpdate_Implementation()
        {
            if (!Owner)
            {
                Destroy();
                return;
            }

            BehaviourUpdate();
        }
    }
}