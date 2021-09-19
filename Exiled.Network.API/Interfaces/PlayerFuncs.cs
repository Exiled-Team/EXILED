// -----------------------------------------------------------------------
// <copyright file="PlayerFuncs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Network.API.Interfaces
{
    using Exiled.Network.API.Models;

    /// <summary>
    /// Player functions.
    /// </summary>
    public abstract class PlayerFuncs
    {
        /// <summary>
        /// Gets or sets the player's nickname.
        /// </summary>
        public virtual string UserName { get; set; }

        /// <summary>
        /// Gets or sets the player's user id.
        /// </summary>
        public virtual string UserID { get; set; }

        /// <summary>
        /// Gets or sets the player's <see cref="int"/>.
        /// </summary>
        public virtual int Role { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player can be tracked.
        /// </summary>
        public virtual bool DoNotTrack { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player has Remote Admin access.
        /// </summary>
        public virtual bool RemoteAdminAccess { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player's overwatch is enabled.
        /// </summary>
        public virtual bool IsOverwatchEnabled { get; set; }

        /// <summary>
        /// Gets or sets the player's IP address.
        /// </summary>
        public virtual string IPAddress { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player is muted.
        /// </summary>
        public virtual bool IsMuted { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player is intercom muted.
        /// </summary>
        public virtual bool IsIntercomMuted { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player has godmode enabled.
        /// </summary>
        public virtual bool IsGodModeEnabled { get; set; }

        /// <summary>
        /// Gets or sets the player's health.
        /// If the health is greater than the <see cref="MaxHealth"/>, the MaxHealth will also be changed to match the health.
        /// </summary>
        public virtual float Health { get; set; }

        /// <summary>
        /// Gets or sets the player's maximum health.
        /// </summary>
        public virtual int MaxHealth { get; set; }

        /// <summary>
        /// Gets or sets the player's group name.
        /// </summary>
        public virtual string GroupName { get; set; }

        /// <summary>
        /// Gets or sets the player's rank color.
        /// </summary>
        public virtual string RankColor { get; set; }

        /// <summary>
        /// Gets or sets the player's rank name.
        /// </summary>
        public virtual string RankName { get; set; }

        /// <summary>
        /// Gets or sets the player's id.
        /// </summary>
        public virtual int PlayerID { get; set; }

        /// <summary>
        /// Gets or sets the player's position.
        /// </summary>
        public virtual Position Position { get; set; }

        /// <summary>
        /// Gets or sets the player's rotation.
        /// </summary>
        public virtual Rotation Rotation { get; set; }

        /// <summary>
        /// Kill player.
        /// </summary>
        public abstract void Kill();

        /// <summary>
        /// Send report message to player.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        public abstract void SendReportMessage(string message);

        /// <summary>
        /// Send remoteadmin message to player.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        public abstract void SendRAMessage(string message);

        /// <summary>
        /// Send console message to player.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        /// <param name="color">Message color.</param>
        public abstract void SendConsoleMessage(string message, string color = "GREEN");

        /// <summary>
        /// Redirect player to other server.
        /// </summary>
        /// <param name="port">Server port.</param>
        public abstract void Redirect(ushort port);

        /// <summary>
        /// Disconnect player from server.
        /// </summary>
        /// <param name="reason">Reason of disconnect.</param>
        public abstract void Disconnect(string reason);

        /// <summary>
        /// Send hint to player.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        /// <param name="duration">The duration of hint.</param>
        public abstract void SendHint(string message, float duration);

        /// <summary>
        /// If player should send position data.
        /// </summary>
        /// <param name="state">State set to true means data about position will be sended via network.</param>
        public abstract void SendPosition(bool state = false);

        /// <summary>
        /// If player should send rotation data.
        /// </summary>
        /// <param name="state">State set to true means data about rotation will be sended via network.</param>
        public abstract void SendRotation(bool state = false);

        /// <summary>
        /// Teleport player.
        /// </summary>
        /// <param name="x">X Cordinate.</param>
        /// <param name="y">Y Cordinate.</param>
        /// <param name="z">Z Cordinate.</param>
        public abstract void Teleport(float x, float y, float z);

        /// <summary>
        /// Set ghostmode.
        /// </summary>
        /// <param name="state">State of godmode.</param>
        public abstract void SetGodmode(bool state = false);

        /// <summary>
        /// Set noclip.
        /// </summary>
        /// <param name="state">State of noclip.</param>
        public abstract void SetNoclip(bool state = false);

        /// <summary>
        /// Clear player inventory.
        /// </summary>
        public abstract void ClearInventory();
    }
}
