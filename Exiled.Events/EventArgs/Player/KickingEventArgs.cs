// -----------------------------------------------------------------------
// <copyright file="KickingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using System.Reflection;

    using API.Features;

    using Interfaces;

    /// <summary>
    ///     Contains all information before kicking a player from the server.
    /// </summary>
    public class KickingEventArgs : IPlayerEvent, IDeniableEvent
    {
        private bool isAllowed;
        private Player issuer;
        private Player target;

        /// <summary>
        ///     Initializes a new instance of the <see cref="KickingEventArgs" /> class.
        /// </summary>
        /// <param name="target">
        ///     <inheritdoc cref="Target" />
        /// </param>
        /// <param name="issuer">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="reason">
        ///     <inheritdoc cref="Reason" />
        /// </param>
        /// <param name="fullMessage">
        ///     <inheritdoc cref="FullMessage" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public KickingEventArgs(Player target, Player issuer, string reason, string fullMessage, bool isAllowed = true)
        {
            Target = target;
            Player = issuer;
            Reason = reason;
            FullMessage = fullMessage;
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets or sets the ban target.
        /// </summary>
        public Player Target
        {
            get => target;
            set
            {
                if (value is null || target == value)
                    return;

                if (Events.Instance.Config.ShouldLogBans && target is not null)
                    LogBanChange(Assembly.GetCallingAssembly().GetName().Name, $" changed the banned player from user {target.Nickname} ({target.UserId}) to {value.Nickname} ({value.UserId})");

                target = value;
            }
        }

        /// <summary>
        ///     Gets or sets the kick reason.
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        ///     Gets or sets the full kick message.
        /// </summary>
        public string FullMessage { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not action is taken against the target.
        /// </summary>
        public bool IsAllowed
        {
            get => isAllowed;
            set
            {
                if (isAllowed == value)
                    return;

                if (Events.Instance.Config.ShouldLogBans)
                    LogBanChange(Assembly.GetCallingAssembly().GetName().Name, $" {(value ? "allowed" : "denied")} banning user with ID: {Target.UserId}");

                isAllowed = value;
            }
        }

        /// <summary>
        ///     Gets or sets the ban issuer.
        /// </summary>
        public Player Player
        {
            get => issuer;
            set
            {
                if (value is null || issuer == value)
                    return;

                if (Events.Instance.Config.ShouldLogBans && issuer is not null)
                    LogBanChange(Assembly.GetCallingAssembly().GetName().Name, $" changed the ban issuer from user {issuer.Nickname} ({issuer.UserId}) to {value.Nickname} ({value.UserId})");

                issuer = value;
            }
        }

        /// <summary>
        ///     Logs the kick, anti-backdoor protection from malicious plugins.
        /// </summary>
        /// <param name="assemblyName">The name of the calling assembly.</param>
        /// <param name="message">The message to be logged.</param>
        protected void LogBanChange(string assemblyName, string message)
        {
            if (assemblyName != "Exiled.Events")
            {
                lock (ServerLogs.LockObject)
                {
                    Log.Warn($"[ANTI-BACKDOOR]: {assemblyName} {message} - {TimeBehaviour.FormatTime("yyyy-MM-dd HH:mm:ss.fff zzz")}");
                }
            }

            ServerLogs._state = ServerLogs.LoggingState.Write;
        }
    }
}