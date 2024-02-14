// -----------------------------------------------------------------------
// <copyright file="PreAuthenticatingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using System;

    using Interfaces;

    using LiteNetLib;

    using PluginAPI.Events;

    /// <summary>
    /// Contains all information before pre-authenticating a player.
    /// </summary>
    public class PreAuthenticatingEventArgs : IExiledEvent
    {
        private PreauthCancellationData cachedPreauthData = PreauthCancellationData.Accept();

        /// <summary>
        /// Initializes a new instance of the <see cref="PreAuthenticatingEventArgs"/> class.
        /// </summary>
        /// <param name="userId"><inheritdoc cref="UserId"/></param>
        /// <param name="ipAddress"><inheritdoc cref="IpAddress"/></param>
        /// <param name="expiration"><inheritdoc cref="Expiration"/></param>
        /// <param name="flags"><inheritdoc cref="Flags"/></param>
        /// <param name="country"><inheritdoc cref="Country"/></param>
        /// <param name="signature"><inheritdoc cref="Signature"/></param>
        /// <param name="request"><inheritdoc cref="Request"/></param>
        /// <param name="readerStartPosition"><inheritdoc cref="ReaderStartPosition"/></param>
        public PreAuthenticatingEventArgs(
            string userId,
            string ipAddress,
            long expiration,
            CentralAuthPreauthFlags flags,
            string country,
            byte[] signature,
            ConnectionRequest request,
            int readerStartPosition)
        {
            UserId = userId;
            IpAddress = ipAddress;
            Expiration = expiration;
            Flags = flags;
            Country = country;
            Signature = signature;
            Request = request;
            ReaderStartPosition = readerStartPosition;
        }

        /// <summary>
        /// Gets the player's user id.
        /// </summary>
        public string UserId { get; }

        /// <summary>
        /// Gets the player's IP address.
        /// </summary>
        public string IpAddress { get; }

        /// <summary>
        /// Gets the request's expiration.
        /// </summary>
        public long Expiration { get; }

        /// <summary>
        /// Gets the flags.
        /// </summary>
        public CentralAuthPreauthFlags Flags { get; }

        /// <summary>
        /// Gets the player's country.
        /// </summary>
        public string Country { get; }

        /// <summary>
        /// Gets the request's signature.
        /// </summary>
        public byte[] Signature { get; }

        /// <summary>
        /// Gets the reader starting position for reading the preauth.
        /// </summary>
        public int ReaderStartPosition { get; }

        /// <summary>
        /// Gets the connection request.
        /// </summary>
        public ConnectionRequest Request { get; }

        /// <summary>
        /// Gets a value indicating whether the player can be authenticated or not.
        /// </summary>
        public bool IsAllowed { get; private set; } = true;

        /// <summary>
        /// Gets or sets the cached <see cref="CachedPreauthData"/> that is returned back to the NwPluginAPI.
        /// </summary>
        internal PreauthCancellationData CachedPreauthData
        {
            get => cachedPreauthData;
            set
            {
                cachedPreauthData = value;
                IsAllowed = false;
            }
        }

        /// <summary>
        /// Delays a pre-authentincating player.
        /// </summary>
        /// <param name="seconds">The seconds of delay.</param>
        /// <param name="isForced">Indicates whether the delay is forced or not.</param>
        public void Delay(byte seconds, bool isForced) =>
            CachedPreauthData = PreauthCancellationData.RejectDelay(seconds, isForced);

        /// <summary>
        /// Redirects a pre-authentincating player.
        /// </summary>
        /// <param name="port">The redirection port.</param>
        /// <param name="isForced">Indicates whether the redirection is forced or not.</param>
        public void Redirect(ushort port, bool isForced) =>
            CachedPreauthData = PreauthCancellationData.RejectRedirect(port, isForced);

        /// <summary>
        /// Rejects a pre-authentincating banned player.
        /// </summary>
        /// <param name="banReason">The ban reason.</param>>
        /// <param name="expiration">The ban <see cref="DateTime"/> expiration.</param>
        /// <param name="isForced">Indicates whether the rejection is forced or not.</param>
        public void RejectBanned(string banReason, DateTime expiration, bool isForced) =>
            CachedPreauthData = PreauthCancellationData.RejectBanned(banReason, expiration, isForced);

        /// <summary>
        /// Rejects a pre-authentincating banned player.
        /// </summary>
        /// <param name="banReason">The ban reason.</param>
        /// <param name="expiration">The ban expiration.</param>
        /// <param name="isForced">Indicates whether the rejection is forced or not.</param>
        public void RejectBanned(string banReason, long expiration, bool isForced) =>
            CachedPreauthData = PreauthCancellationData.RejectBanned(banReason, expiration, isForced);

        /// <summary>
        /// Rejects a pre-authentincating player.
        /// </summary>
        /// <param name="customReason">The rejection custom reason.</param>
        /// <param name="isForced">Indicates whether the rejection is forced or not.</param>
        public void Reject(string customReason, bool isForced) =>
            CachedPreauthData = PreauthCancellationData.Reject(customReason, isForced);

        /// <summary>
        /// Rejects a pre-authentincating player.
        /// </summary>
        /// <param name="reason">The <see cref="RejectionReason"/>.</param>
        /// <param name="isForced">Indicates whether the rejection is forced or not.</param>
        public void Reject(RejectionReason reason, bool isForced) =>
            CachedPreauthData = PreauthCancellationData.Reject(reason, isForced);

        /*
        /// <summary>
        /// Delays the connection.
        /// </summary>
        /// <param name="seconds">The delay in seconds.</param>
        /// <param name="isForced">Indicates whether the player has to be rejected forcefully or not.</param>
        public void Delay(byte seconds, bool isForced)
        {
            if (seconds is < 1 or > 25)
                throw new ArgumentOutOfRangeException(nameof(seconds), "Delay duration must be between 1 and 25 seconds.");

            Reject(RejectionReason.Delay, isForced, null, 0, seconds);
        }

        /// <summary>
        /// Rejects the player and redirects them to another server port.
        /// </summary>
        /// <param name="port">The new server port.</param>
        /// <param name="isForced">Indicates whether the player has to be rejected forcefully or not.</param>
        public void Redirect(ushort port, bool isForced) => Reject(RejectionReason.Redirect, isForced, null, 0, 0, port);

        /// <summary>
        /// Rejects a player who's trying to authenticate.
        /// </summary>
        /// <param name="banReason">The ban reason.</param>
        /// <param name="expiration">The ban expiration time.</param>
        /// <param name="isForced">Indicates whether the player has to be rejected forcefully or not.</param>
        public void RejectBanned(string banReason, DateTime expiration, bool isForced) => Reject(RejectionReason.Banned, isForced, banReason, expiration.Ticks);

        /// <summary>
        /// Rejects a player who's trying to authenticate.
        /// </summary>
        /// <param name="banReason">The ban reason.</param>
        /// <param name="expiration">The ban expiration time in .NET Ticks.</param>
        /// <param name="isForced">Indicates whether the player has to be rejected forcefully or not.</param>
        public void RejectBanned(string banReason, long expiration, bool isForced) => Reject(RejectionReason.Banned, isForced, banReason, expiration);

        /// <summary>
        /// Rejects a player who's trying to authenticate.
        /// </summary>
        /// <param name="writer">The <see cref="NetDataWriter" /> instance.</param>
        /// <param name="isForced">Indicates whether the player has to be rejected forcefully or not.</param>
        public void Reject(NetDataWriter writer, bool isForced)
        {
            if (!IsAllowed)
                return;

            IsAllowed = false;

            if (isForced)
                Request.RejectForce(writer);
            else
                Request.Reject(writer);
        }

        /// <summary>
        /// Rejects a player who's trying to authenticate.
        /// </summary>
        /// <param name="rejectionReason">The custom rejection reason.</param>
        /// <param name="isForced">Indicates whether the player has to be rejected forcefully or not.</param>
        public void Reject(string rejectionReason, bool isForced) => Reject(RejectionReason.Custom, isForced, rejectionReason);

        /// <summary>
        /// Rejects a player who's trying to authenticate.
        /// </summary>
        /// <param name="rejectionReason">The rejection reason.</param>
        /// <param name="isForced">Indicates whether the player has to be rejected forcefully or not.</param>
        /// <param name="customReason">The custom rejection reason (Banned and Custom reasons only).</param>
        /// <param name="expiration">The ban expiration ticks (Banned reason only).</param>
        /// <param name="seconds">The delay in seconds (Delay reason only).</param>
        /// <param name="port">The redirection port (Redirect reason only).</param>
        public void Reject(RejectionReason rejectionReason, bool isForced, string customReason = null, long expiration = 0, byte seconds = 0, ushort port = 0)
        {
            if (customReason is not null && (customReason.Length > 400))
                throw new ArgumentOutOfRangeException(nameof(rejectionReason), "Reason can't be longer than 400 characters.");

            if (!IsAllowed)
                return;

            IsAllowed = false;

            NetDataWriter rejectData = new();

            rejectData.Put((byte)rejectionReason);

            switch (rejectionReason)
            {
                case RejectionReason.Banned:
                    rejectData.Put(expiration);
                    rejectData.Put(customReason);
                    break;

                case RejectionReason.Custom:
                    rejectData.Put(customReason);
                    break;

                case RejectionReason.Delay:
                    rejectData.Put(seconds);
                    break;

                case RejectionReason.Redirect:
                    rejectData.Put(port);
                    break;
            }

            if (isForced)
                Request.RejectForce(rejectData);
            else
                Request.Reject(rejectData);
        }

        /// <summary>
        /// Disallows the connection without sending any reason. Should only be used when the connection has already been
        /// terminated by the plugin itself.
        /// </summary>
        public void Disallow() => IsAllowed = false;
        */
    }
}