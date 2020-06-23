using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exiled.Events.EventArgs
{
    using System;
    using Exiled.API.Features;
    using UnityEngine;

    /// <summary>
    /// Contains all informations before a player is infected.
    /// </summary>
    public class FinishingRecallEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FinishingRecallEventArgs"/> class.
        /// </summary>
        /// <param name="target"><inheritdoc cref="Target"/></param>
        /// <param name="scp049"><inheritdoc cref="Scp049"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public FinishingRecallEventArgs(Player target, Player scp049, bool isAllowed = true)
        {
            Target = target;
            Scp049 = scp049;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's getting infected.
        /// </summary>
        public Player Target { get; private set; }

        /// <summary>
        /// Gets the player who is SCP049.
        /// </summary>
        public Player Scp049 { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the event can be executed or not.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
