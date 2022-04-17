
namespace Exiled.Events.EventArgs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Exiled.API.Features;
    using Exiled.API.Features.Items;

    using InventorySystem.Items.Usables.Scp244;

    /// <summary>
    /// Contains all informations before radio battery charge is changed.
    /// </summary>
    public class UsingScp244EventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UsingScp244EventArgs"/> class.
        /// </summary>
        /// <param name="scp244"><inheritdoc cref="Scp244"/></param>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public UsingScp244EventArgs(Scp244Item scp244, Player player, bool isAllowed = true)
        {
            Scp244 = (Scp244)Item.Get(scp244);
            Player = player;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's using the radio.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the Scp244 instance.
        /// </summary>
        public Scp244 Scp244 { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the radio battery charge can be changed or not.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
