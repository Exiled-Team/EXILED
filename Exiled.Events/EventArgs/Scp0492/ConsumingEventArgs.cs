using Exiled.API.Features;
using Exiled.Events.EventArgs.Interfaces;

namespace Exiled.Events.EventArgs.Scp0492
{
    /// <summary>
    /// Contains all information before zombie gets bonuses from his Consume ability
    /// </summary>
    public class ConsumingEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsumingEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="ragdoll"><inheritdoc cref="Ragdoll"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public ConsumingEventArgs(API.Features.Player player, Ragdoll ragdoll, bool isAllowed = true)
        {
            Player = player;
            Ragdoll = ragdoll;
            IsAllowed = isAllowed;
        }

        /// <inheritdoc />
        public API.Features.Player Player { get; }

        /// <summary>
        /// Gets or sets the corpse on which was activated ability.
        /// </summary>
        public Ragdoll Ragdoll { get; set; }

        /// <inheritdoc />
        public bool IsAllowed { get; set; }
    }
}