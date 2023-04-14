using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.Events.EventArgs.Interfaces;

namespace Exiled.Events.EventArgs.Scp0492
{
    /// <summary>
    /// Contains all information before zombie attacks.
    /// </summary>
    public class AttackingEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AttackingEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="target"><inheritdoc cref="Target"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public AttackingEventArgs(ReferenceHub player, ReferenceHub target, bool isAllowed = true)
        {
            Player = API.Features.Player.Get(player);
            Target = API.Features.Player.Get(target);
            IsAllowed = isAllowed;
        }

        /// <inheritdoc />
        public API.Features.Player Player { get; }

        /// <summary>
        /// Gets the player who was attacked.
        /// </summary>
        public API.Features.Player Target { get; }

        /// <inheritdoc />
        public bool IsAllowed { get; set; } = true;
    }
}
