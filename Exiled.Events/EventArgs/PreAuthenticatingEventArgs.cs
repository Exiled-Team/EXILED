// -----------------------------------------------------------------------
// <copyright file="PreAuthenticatingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;
    using LiteNetLib;
    using LiteNetLib.Utils;

    /// <summary>
    /// Contains all informations before pre-autenticating a player.
    /// </summary>
    public class PreAuthenticatingEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PreAuthenticatingEventArgs"/> class.
        /// </summary>
        /// <param name="userId"><inheritdoc cref="UserId"/></param>
        /// <param name="request"><inheritdoc cref="Request"/></param>
        /// <param name="readerStartPosition"><inheritdoc cref="ReaderStartPosition"/></param>
        /// <param name="flags"><inheritdoc cref="Flags"/></param>
        /// <param name="country"><inheritdoc cref="Country"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public PreAuthenticatingEventArgs(string userId, ConnectionRequest request, int readerStartPosition, byte flags, string country, bool isAllowed = true)
        {
            UserId = userId;
            Request = request;
            ReaderStartPosition = readerStartPosition;
            Flags = flags;
            Country = country;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player's user id.
        /// </summary>
        public string UserId { get; private set; }

        /// <summary>
        /// Gets the reader starting position.
        /// </summary>
        public int ReaderStartPosition { get; private set; }

        /// <summary>
        /// Gets the flags.
        /// </summary>
        public byte Flags { get; private set; }

        /// <summary>
        /// Gets the player's country.
        /// </summary>
        public string Country { get; private set; }

        /// <summary>
        /// Gets the connection request.
        /// </summary>
        public ConnectionRequest Request { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the player can be authenticated or not.
        /// </summary>
        public bool IsAllowed { get; private set; }

        /// <summary>
        /// Delays the connection.
        /// </summary>
        /// <param name="seconds">The delay in seconds.</param>
        /// <param name="isForced">Indicates whether the player has to be rejected forcefully or not.</param>
        public void Delay(byte seconds, bool isForced = false)
        {
            if (seconds < 1 && seconds > 25)
                throw new Exception("Delay duration must be between 1 and 25 seconds.");

            Reject(RejectionReason.Delay, string.Empty, isForced, 0, seconds);
        }

        /// <summary>
        /// Reject the player and redirects him to another server port.
        /// </summary>
        /// <param name="port">The new server port.</param>
        /// <param name="isForced">Indicates whether the player has to be rejected forcefully or not.</param>
        public void Redirect(ushort port, bool isForced = false) => Reject(RejectionReason.Redirect, string.Empty, isForced, 0, 0, port);

        /// <summary>
        /// Reject a player who's trying to authenticate.
        /// </summary>
        /// <param name="writer">The <see cref="NetDataWriter"/> instance.</param>
        /// <param name="isForced">Indicates whether the player has to be rejected forcefully or not.</param>
        public void Reject(NetDataWriter writer, bool isForced = false)
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
        /// Reject a player who's trying to authenticate.
        /// </summary>
        /// <param name="rejectionType">The rejection type.</param>
        /// <param name="reason">The rejection reason.</param>
        /// <param name="isForced">Indicates whether the player has to be rejected forcefully or not.</param>
        /// <param name="expiration">The ban expiration date.</param>
        public void Reject(RejectionReason rejectionType, string reason = null, bool isForced = false, DateTime expiration = default)
        {
            Reject(rejectionType, reason, isForced, expiration.Ticks);
        }

        /// <summary>
        /// Reject a player who's trying to authenticate.
        /// </summary>
        /// <param name="rejectionType">The rejection type.</param>
        /// <param name="reason">The rejection reason.</param>
        /// <param name="isForced">Indicates whether the player has to be rejected forcefully or not.</param>
        /// <param name="expiration">The ban expiration ticks.</param>
        /// <param name="seconds">The delay in seconds.</param>
        /// <param name="port">The redirection port.</param>
        public void Reject(RejectionReason rejectionType, string reason = null, bool isForced = false, long expiration = 0, byte seconds = 0, ushort port = 0)
        {
            if (string.IsNullOrEmpty(reason) && reason.Length > 400)
                throw new Exception("Reason can't be longer than 400 characters.");

            if (!IsAllowed)
                return;

            IsAllowed = false;

            NetDataWriter rejectData = new NetDataWriter();

            rejectData.Put((byte)rejectionType);

            if (rejectionType == RejectionReason.Banned)
                rejectData.Put(expiration);

            rejectData.Put(reason);

            if (rejectionType == RejectionReason.Delay)
                rejectData.Put(seconds);

            if (rejectionType == RejectionReason.Redirect)
                rejectData.Put(port);

            if (isForced)
                Request.RejectForce(rejectData);
            else
                Request.Reject(rejectData);
        }
    }
}
