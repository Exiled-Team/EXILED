namespace Exiled.Events.EventArgs.Player
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;
    using RemoteAdmin;

    /// <summary>
    ///     Contains all information before a player send RA command.
    /// </summary>
    public class SendingCommandEventArgs : IDeniableEvent, IPlayerEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SendingCommandEventArgs" /> class.
        /// </summary>
        /// <param name="command">
        ///     <inheritdoc cref="Command" />
        /// </param>
        /// <param name="args">
        ///     <inheritdoc cref="Args" />
        /// </param>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="errorText">
        ///     <inheritdoc cref="ErrorText" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public SendingCommandEventArgs(string command, string[] args, Player player, string errorText, bool isAllowed)
        {
            Command = command;
            Args = args;
            Player = player;
            ErrorText = errorText;
            IsAllowed = isAllowed;
        }
        /// <summary>
        ///     Gets the command.
        /// </summary>
        public string Command { get; }
        /// <summary>
        ///     Gets the arguments.
        /// </summary>
        public string[] Args { get; }
        /// <summary>
        ///     Gets the player who's sending the command.
        /// </summary>
        public Player Player { get; }
        /// <summary>
        ///     Gets or sets the text that will be displayed if unallowed.
        /// </summary>
        public string ErrorText { get; set; }
        /// <summary>
        ///     Gets or sets a value indicating whether or not the command will be sent.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}