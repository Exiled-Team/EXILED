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
    public class InfectPlayerArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InfectPlayerArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="Scp049"><inheritdoc cref="SCP049"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public InfectPlayerArgs(Player player, Player Scp049, bool isAllowed = true)
        {
            Player = player;
            SCP049 = Scp049;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's getting infected.
        /// </summary>
        public Player Player { get; private set; }

        /// <summary>
        /// Gets the player who is SCP049.
        /// </summary>
        public Player SCP049 { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the event can be executed or not.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
