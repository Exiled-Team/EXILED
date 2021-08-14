// -----------------------------------------------------------------------
// <copyright file="NormalPlayer.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Network.API.Models
{
    using System;
    using Exiled.API.Features;
    using Exiled.Network.API.Interfaces;
    using Mirror;

    /// <summary>
    /// Normal exiled player.
    /// </summary>
    public class NormalPlayer : PlayerFuncs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NormalPlayer"/> class.
        /// </summary>
        /// <param name="player">Player.</param>
        public NormalPlayer(Player player)
        {
            this.player = player;
        }

        private readonly Player player;

        /// <inheritdoc/>
        public override string UserName => player.Nickname;

        /// <inheritdoc/>
        public override string UserID => player.UserId;

        /// <inheritdoc/>
        public override int Role => (int)player.Role;

        /// <inheritdoc/>
        public override bool DoNotTrack => player.DoNotTrack;

        /// <inheritdoc/>
        public override string GroupName => player.GroupName;

        /// <inheritdoc/>
        public override string RankColor => player.RankColor;

        /// <inheritdoc/>
        public override string RankName => player.RankName;

        /// <inheritdoc/>
        public override string IPAddress => player.IPAddress;

        /// <inheritdoc/>
        public override bool IsGodModeEnabled => player.IsGodModeEnabled;

        /// <inheritdoc/>
        public override float Health => player.Health;

        /// <inheritdoc/>
        public override int MaxHealth => player.MaxHealth;

        /// <inheritdoc/>
        public override bool IsIntercomMuted => player.IsIntercomMuted;

        /// <inheritdoc/>
        public override bool IsMuted => player.IsMuted;

        /// <inheritdoc/>
        public override bool IsOverwatchEnabled => player.IsOverwatchEnabled;

        /// <inheritdoc/>
        public override bool RemoteAdminAccess => player.RemoteAdminAccess;

        /// <inheritdoc/>
        public override int PlayerID => player.Id;

        /// <inheritdoc/>
        public override Position Position
        {
            get
            {
                return new Position() { X = player.Position.x, Y = player.Position.y, Z = player.Position.z };
            }

            set
            {
                Position = value;
            }
        }

        /// <inheritdoc/>
        public override Rotation Rotation
        {
            get
            {
                return new Rotation() { X = player.Rotation.x, Y = player.Rotation.y, Z = player.Rotation.z };
            }

            set
            {
                Rotation = value;
            }
        }

        /// <inheritdoc/>
        public override void Disconnect(string reason)
        {
            ServerConsole.Disconnect(player.GameObject, reason);
        }

        /// <inheritdoc/>
        public override void Kill()
        {
            player.Kill();
        }

        /// <inheritdoc/>
        public override void Redirect(ushort port)
        {
            SendClientToServer(player, port);
        }

        /// <inheritdoc/>
        public override void SendConsoleMessage(string message, string color = "GREEN")
        {
            player.SendConsoleMessage(message, color);
        }

        /// <inheritdoc/>
        public override void SendRAMessage(string message)
        {
            player.RemoteAdminMessage(message, true, "NP");
        }

        /// <inheritdoc/>
        public override void SendReportMessage(string message)
        {
            player.SendConsoleMessage("[REPORTING] " + message, "GREEN");
        }

        /// <inheritdoc/>
        public override void SendHint(string message, float duration)
        {
            player.ShowHint(message, duration);
        }

        /// <inheritdoc/>
        public override void SendPosition(bool state = false)
        {
            if (!player.SessionVariables.ContainsKey("SP"))
                player.SessionVariables.Add("SP", state);
            player.SessionVariables["SP"] = state;
        }

        /// <inheritdoc/>
        public override void SendRotation(bool state = false)
        {
            if (!player.SessionVariables.ContainsKey("SR"))
                player.SessionVariables.Add("SR", state);
            player.SessionVariables["SR"] = state;
        }

        /// <inheritdoc/>
        public override void Teleport(float x, float y, float z)
        {
            player.Position = new UnityEngine.Vector3(x, y, z);
        }

        /// <inheritdoc/>
        public override void SetGodmode(bool state = false)
        {
            player.IsGodModeEnabled = state;
        }

        /// <inheritdoc/>
        public override void SetNoclip(bool state = false)
        {
            player.NoClipEnabled = state;
        }

        /// <inheritdoc/>
        public override void ClearInventory()
        {
            player.ClearInventory();
        }

        private void SendClientToServer(Player hub, ushort port)
        {
            var serverPS = hub.ReferenceHub.playerStats;
            NetworkWriter writer = NetworkWriterPool.GetWriter();
            writer.WriteSingle(1f);
            writer.WriteUInt16(port);
            RpcMessage msg = new RpcMessage
            {
                netId = serverPS.netId,
                componentIndex = serverPS.ComponentIndex,
                functionHash = GetMethodHash(typeof(PlayerStats), "RpcRoundrestartRedirect"),
                payload = writer.ToArraySegment(),
            };
            hub.Connection.Send<RpcMessage>(msg, 0);
            NetworkWriterPool.Recycle(writer);
        }

        private int GetMethodHash(Type invokeClass, string methodName)
        {
            return (invokeClass.FullName.GetStableHashCode() * 503) + methodName.GetStableHashCode();
        }
    }
}
