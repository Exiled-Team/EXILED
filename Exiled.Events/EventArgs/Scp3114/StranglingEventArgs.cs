namespace Exiled.Events.EventArgs.Scp3114
{
    using API.Features;
    using Exiled.API.Features.Roles;
    using Interfaces;

    /// <summary>
    ///     Contains all information before SCP-3114 strangling.
    /// </summary>
    public class StranglingEventArgs : IScp3114Event, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StranglingEventArgs" /> class.
        /// </summary>
        /// <param name="player">The player initiating the strangling action.</param>
        /// <param name="target">The player being targeted for strangling.</param>
        /// <param name="isAllowed">Indicates whether the action is allowed. Default is true.</param>
        public StranglingEventArgs(Player target, Player player, bool isAllowed = true)
        {
            Player = player;
            Scp3114 = player.Role.As<Scp3114Role>();
            Target = target;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's getting strangled.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the player who's getting strangled.
        /// </summary>
        public Player Target { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the recall can begin.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        /// Gets the Scp3114 player.
        /// </summary>
        public Scp3114Role Scp3114 { get; }
    }
}