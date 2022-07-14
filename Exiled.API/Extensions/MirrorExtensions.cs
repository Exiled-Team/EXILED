// -----------------------------------------------------------------------
// <copyright file="MirrorExtensions.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Text;

    using Exiled.API.Features;

    using InventorySystem.Items.Firearms;

    using Mirror;

    using NorthwoodLib.Pools;

    using Respawning;

    using UnityEngine;

    /// <summary>
    /// A set of extensions for <see cref="Mirror"/> Networking.
    /// </summary>
    public static class MirrorExtensions
    {
        private static readonly Dictionary<Type, MethodInfo> WriterExtensionsValue = new();
        private static readonly Dictionary<string, ulong> SyncVarDirtyBitsValue = new();
        private static readonly ReadOnlyDictionary<Type, MethodInfo> ReadOnlyWriterExtensionsValue = new(WriterExtensionsValue);
        private static readonly ReadOnlyDictionary<string, ulong> ReadOnlySyncVarDirtyBitsValue = new(SyncVarDirtyBitsValue);
        private static MethodInfo setDirtyBitsMethodInfoValue;
        private static MethodInfo sendSpawnMessageMethodInfoValue;

        /// <summary>
        /// Gets <see cref="MethodInfo"/> corresponding to <see cref="Type"/>.
        /// </summary>
        public static ReadOnlyDictionary<Type, MethodInfo> WriterExtensions
        {
            get
            {
                if (WriterExtensionsValue.Count == 0)
                {
                    foreach (MethodInfo method in typeof(NetworkWriterExtensions).GetMethods().Where(x => !x.IsGenericMethod && x.GetParameters()?.Length == 2))
                        WriterExtensionsValue.Add(method.GetParameters().First(x => x.ParameterType != typeof(NetworkWriter)).ParameterType, method);

                    foreach (MethodInfo method in typeof(GeneratedNetworkCode).GetMethods().Where(x => !x.IsGenericMethod && x.GetParameters()?.Length == 2 && x.ReturnType == typeof(void)))
                        WriterExtensionsValue.Add(method.GetParameters().First(x => x.ParameterType != typeof(NetworkWriter)).ParameterType, method);

                    foreach (Type serializer in typeof(ServerConsole).Assembly.GetTypes().Where(x => x.Name.EndsWith("Serializer")))
                    {
                        foreach (MethodInfo method in serializer.GetMethods().Where(x => x.ReturnType == typeof(void) && x.Name.StartsWith("Write")))
                            WriterExtensionsValue.Add(method.GetParameters().First(x => x.ParameterType != typeof(NetworkWriter)).ParameterType, method);
                    }
                }

                return ReadOnlyWriterExtensionsValue;
            }
        }

        /// <summary>
        /// Gets a all DirtyBit <see cref="ulong"/> from <see cref="StringExtensions"/>(format:classname.methodname).
        /// </summary>
        public static ReadOnlyDictionary<string, ulong> SyncVarDirtyBits
        {
            get
            {
                if (SyncVarDirtyBitsValue.Count == 0)
                {
                    foreach (PropertyInfo property in typeof(ServerConsole).Assembly.GetTypes()
                        .SelectMany(x => x.GetProperties())
                        .Where(m => m.Name.StartsWith("Network")))
                    {
                        MethodInfo setMethod = property.GetSetMethod();
                        if (setMethod is null)
                            continue;
                        MethodBody methodBody = setMethod.GetMethodBody();
                        if (methodBody is null)
                            continue;
                        byte[] bytecodes = methodBody.GetILAsByteArray();
                        if (!SyncVarDirtyBitsValue.ContainsKey($"{property.Name}"))
                            SyncVarDirtyBitsValue.Add($"{property.Name}", bytecodes[bytecodes.LastIndexOf((byte)OpCodes.Ldc_I8.Value) + 1]);
                    }
                }

                return ReadOnlySyncVarDirtyBitsValue;
            }
        }

        /// <summary>
        /// Gets a <see cref="NetworkBehaviour.SetDirtyBit(ulong)"/>'s <see cref="MethodInfo"/>.
        /// </summary>
        public static MethodInfo SetDirtyBitsMethodInfo =>
            setDirtyBitsMethodInfoValue ??= typeof(NetworkBehaviour).GetMethod(nameof(NetworkBehaviour.SetDirtyBit));

        /// <summary>
        /// Gets a NetworkServer.SendSpawnMessage's <see cref="MethodInfo"/>.
        /// </summary>
        public static MethodInfo SendSpawnMessageMethodInfo =>
            sendSpawnMessageMethodInfoValue ??= typeof(NetworkServer).GetMethod("SendSpawnMessage", BindingFlags.NonPublic | BindingFlags.Static);

        /// <summary>
        /// Shaking target <see cref="Player"/> window.
        /// </summary>
        /// <param name="player">Target to shake.</param>
        public static void Shake(this Player player) => AlphaWarheadController.Host.TargetRpcShake(player.Connection, false, true);

        /// <summary>
        /// Play beep sound to <see cref="Player"/>.
        /// </summary>
        /// <param name="player">Target to play.</param>
        public static void PlayBeepSound(this Player player) => SendFakeTargetRpc(player, ReferenceHub.HostHub.networkIdentity, typeof(AmbientSoundPlayer), nameof(AmbientSoundPlayer.RpcPlaySound), 7);

        /// <summary>
        /// Set <see cref="NicknameSync.Network_customPlayerInfoString"/> that only <see cref="Player"/> can see.
        /// </summary>
        /// <param name="player">Only this player can see info.</param>
        /// <param name="target">Target to set info.</param>
        /// <param name="info">Setting info.</param>
        public static void SetPlayerInfoForTargetOnly(this Player player, Player target, string info) => player.SendFakeSyncVar(target.ReferenceHub.networkIdentity, typeof(NicknameSync), nameof(NicknameSync.Network_customPlayerInfoString), info);

        /// <summary>
        /// Plays gun sound.
        /// </summary>
        /// <param name="player">Target to play.</param>
        /// <param name="position">Position to play on.</param>
        /// <param name="itemType">Weapon' sound to play.</param>
        /// <param name="volume">Sound's volume to set.</param>
        /// <param name="audioClipId">GunAudioMessage's audioClipId to set (default = 0).</param>
        public static void PlayGunSound(this Player player, Vector3 position, ItemType itemType, byte volume, byte audioClipId = 0)
        {
            GunAudioMessage message = new()
            {
                Weapon = itemType,
                AudioClipId = audioClipId,
                MaxDistance = volume,
                ShooterNetId = 0U,
            };

            Vector3 to = position - player.Position;
            float angle = Vector3.Angle(Vector3.forward, to);
            if (Vector3.Dot(to.normalized, Vector3.left) > 0f)
                angle = 360f - angle;
            message.ShooterDirection = (byte)Mathf.RoundToInt(angle / 1.44f);
            message.ShooterRealDistance = (byte)Mathf.RoundToInt(Mathf.Min(to.magnitude, 255f));

            player.Connection.Send(message);
        }

        /// <summary>
        /// Set <see cref="FlickerableLightController.Network_warheadLightColor"/> that only <see cref="Player"/> can see.
        /// </summary>
        /// <param name="room">Room to modify.</param>
        /// <param name="target">Only this player can see room color.</param>
        /// <param name="color">Color to set.</param>
        public static void SetRoomColorForTargetOnly(this Room room, Player target, Color color)
        {
            target.SendFakeSyncVar(room.FlickerableLightControllerNetIdentity, typeof(FlickerableLightController), nameof(FlickerableLightController.Network_warheadLightColor), color);
            target.SendFakeSyncVar(room.FlickerableLightControllerNetIdentity, typeof(FlickerableLightController), nameof(FlickerableLightController.Network_warheadLightOverride), true);
        }

        /// <summary>
        /// Set <see cref="FlickerableLightController.Network_lightIntensityMultiplier"/> that only <see cref="Player"/> can see.
        /// </summary>
        /// <param name="room">Room to modify.</param>
        /// <param name="target">Only this player can see room color.</param>
        /// <param name="multiplier">Light intensity multiplier to set.</param>
        public static void SetRoomLightIntensityForTargetOnly(this Room room, Player target, float multiplier)
        {
            target.SendFakeSyncVar(room.FlickerableLightControllerNetIdentity, typeof(FlickerableLightController), nameof(FlickerableLightController.Network_lightIntensityMultiplier), multiplier);
        }

        /// <summary>
        /// Change <see cref="Player"/> character model for appearance.
        /// It will continue until <see cref="Player"/>'s <see cref="RoleType"/> changes.
        /// </summary>
        /// <param name="player">Player to change.</param>
        /// <param name="type">Model type.</param>
        public static void ChangeAppearance(this Player player, RoleType type)
        {
            foreach (Player target in Player.List.Where(x => x != player))
                SendFakeSyncVar(target, player.ReferenceHub.networkIdentity, typeof(CharacterClassManager), nameof(CharacterClassManager.NetworkCurClass), (sbyte)type);
        }

        /// <summary>
        /// Send CASSIE announcement that only <see cref="Player"/> can hear.
        /// It will continue until <see cref="Player"/>'s <see cref="RoleType"/> changes.
        /// </summary>
        /// <param name="player">Target to send.</param>
        /// <param name="words">Announcement words.</param>
        /// <param name="makeHold">Same on <see cref="Cassie.Message(string, bool, bool, bool)"/>'s isHeld.</param>
        /// <param name="makeNoise">Same on <see cref="Cassie.Message(string, bool, bool, bool)"/>'s isNoisy.</param>
        /// <param name="isSubtitles">Same on <see cref="Cassie.Message(string, bool, bool, bool)"/>'s isSubtitles.</param>
        public static void PlayCassieAnnouncement(this Player player, string words, bool makeHold = false, bool makeNoise = true, bool isSubtitles = false)
        {
            foreach (RespawnEffectsController controller in RespawnEffectsController.AllControllers)
            {
                if (controller != null)
                {
                    SendFakeTargetRpc(player, controller.netIdentity, typeof(RespawnEffectsController), nameof(RespawnEffectsController.RpcCassieAnnouncement), words, makeHold, makeNoise, isSubtitles);
                }
            }
        }

        /// <summary>
        /// Send CASSIE announcement with custom subtitles for translation that only <see cref="Player"/> can hear and see it.
        /// It will continue until <see cref="Player"/>'s <see cref="RoleType"/> changes.
        /// </summary>
        /// <param name="player">Target to send.</param>
        /// <param name="words">The message to be reproduced.</param>
        /// <param name="translation">The translation should be show in the subtitles.</param>
        /// <param name="makeHold">Same on <see cref="Cassie.MessageTranslated(string, string, bool, bool, bool)"/>'s isHeld.</param>
        /// <param name="makeNoise">Same on <see cref="Cassie.MessageTranslated(string, string, bool, bool, bool)"/>'s isNoisy.</param>
        /// <param name="isSubtitles">Same on <see cref="Cassie.MessageTranslated(string, string, bool, bool, bool)"/>'s isSubtitles.</param>
        public static void MessageTranslated(this Player player, string words, string translation, bool makeHold = false, bool makeNoise = true, bool isSubtitles = true)
        {
            StringBuilder annoucement = StringBuilderPool.Shared.Rent();
            string[] cassies = words.Split('\n');
            string[] translations = translation.Split('\n');
            for (int i = 0; i < cassies.Length; i++)
                annoucement.Append($"{translations[i]}<size=0> {cassies[i].Replace(' ', ' ')} </size><split>");

            foreach (RespawnEffectsController controller in RespawnEffectsController.AllControllers)
            {
                if (controller != null)
                {
                    SendFakeTargetRpc(player, controller.netIdentity, typeof(RespawnEffectsController), nameof(RespawnEffectsController.RpcCassieAnnouncement), annoucement, makeHold, makeNoise, isSubtitles);
                }
            }

            StringBuilderPool.Shared.Return(annoucement);
        }

        /// <summary>
        /// Changes the <see cref="Player"/>'s walking speed. Negative values will invert the player's controls.
        /// </summary>
        /// <param name="player">Player to change.</param>
        /// <param name="multiplier">Speed multiplier.</param>
        /// <param name="useCap">Allow <paramref name="multiplier"></paramref> values to be larger than safe amount.</param>
        [Obsolete("Use Player.WalkingSpeed", false)] // TODO : Put this Internal or private
        public static void ChangeWalkingSpeed(this Player player, float multiplier, bool useCap = true)
        {
            if (useCap)
                multiplier = Mathf.Clamp(multiplier, -2f, 2f);
            SendFakeSyncVar(player, ServerConfigSynchronizer.Singleton.netIdentity, typeof(ServerConfigSynchronizer), nameof(ServerConfigSynchronizer.Singleton.NetworkHumanWalkSpeedMultiplier), multiplier);
        }

        /// <summary>
        /// Changes the <see cref="Player"/>'s running speed. Negative values will invert the player's controls.
        /// </summary>
        /// <param name="player">Player to change.</param>
        /// <param name="multiplier">Speed multiplier.</param>
        /// <param name="useCap">Allow <paramref name="multiplier"></paramref> values to be larger than safe amount.</param>
        [Obsolete("Use Player.RunningSpeed", false)] // TODO : Put this Internal or private
        public static void ChangeRunningSpeed(this Player player, float multiplier, bool useCap = true)
        {
            if (useCap)
                multiplier = Mathf.Clamp(multiplier, -1.4f, 1.4f);
            SendFakeSyncVar(player, ServerConfigSynchronizer.Singleton.netIdentity, typeof(ServerConfigSynchronizer), nameof(ServerConfigSynchronizer.Singleton.NetworkHumanSprintSpeedMultiplier), multiplier);
        }

        /// <summary>
        /// Send fake values to client's <see cref="SyncVarAttribute"/>.
        /// </summary>
        /// <param name="target">Target to send.</param>
        /// <param name="behaviorOwner"><see cref="NetworkIdentity"/> of object that owns <see cref="NetworkBehaviour"/>.</param>
        /// <param name="targetType"><see cref="NetworkBehaviour"/>'s type.</param>
        /// <param name="propertyName">Property name starting with Network.</param>
        /// <param name="value">Value of send to target.</param>
        public static void SendFakeSyncVar(this Player target, NetworkIdentity behaviorOwner, Type targetType, string propertyName, object value)
        {
            void CustomSyncVarGenerator(NetworkWriter targetWriter)
            {
                targetWriter.WriteUInt64(SyncVarDirtyBits[$"{propertyName}"]);
                WriterExtensions[value.GetType()]?.Invoke(null, new[] { targetWriter, value });
            }

            PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
            PooledNetworkWriter writer2 = NetworkWriterPool.GetWriter();
            MakeCustomSyncWriter(behaviorOwner, targetType, null, CustomSyncVarGenerator, writer, writer2);
            target.ReferenceHub.networkIdentity.connectionToClient.Send(new UpdateVarsMessage() { netId = behaviorOwner.netId, payload = writer.ToArraySegment() });
            NetworkWriterPool.Recycle(writer);
            NetworkWriterPool.Recycle(writer2);
        }

        /// <summary>
        /// Force resync to client's <see cref="SyncVarAttribute"/>.
        /// </summary>
        /// <param name="behaviorOwner"><see cref="NetworkIdentity"/> of object that owns <see cref="NetworkBehaviour"/>.</param>
        /// <param name="targetType"><see cref="NetworkBehaviour"/>'s type.</param>
        /// <param name="propertyName">Property name starting with Network.</param>
        public static void ResyncSyncVar(NetworkIdentity behaviorOwner, Type targetType, string propertyName) => SetDirtyBitsMethodInfo.Invoke(behaviorOwner.gameObject.GetComponent(targetType), new object[] { SyncVarDirtyBits[$"{propertyName}"] });

        /// <summary>
        /// Send fake values to client's <see cref="ClientRpcAttribute"/>.
        /// </summary>
        /// <param name="target">Target to send.</param>
        /// <param name="behaviorOwner"><see cref="NetworkIdentity"/> of object that owns <see cref="NetworkBehaviour"/>.</param>
        /// <param name="targetType"><see cref="NetworkBehaviour"/>'s type.</param>
        /// <param name="rpcName">Property name starting with Rpc.</param>
        /// <param name="values">Values of send to target.</param>
        public static void SendFakeTargetRpc(Player target, NetworkIdentity behaviorOwner, Type targetType, string rpcName, params object[] values)
        {
            PooledNetworkWriter writer = NetworkWriterPool.GetWriter();

            foreach (object value in values)
                WriterExtensions[value.GetType()].Invoke(null, new[] { writer, value });

            RpcMessage msg = new()
            {
                netId = behaviorOwner.netId,
                componentIndex = GetComponentIndex(behaviorOwner, targetType),
                functionHash = (targetType.FullName.GetStableHashCode() * 503) + rpcName.GetStableHashCode(),
                payload = writer.ToArraySegment(),
            };
            target.Connection.Send(msg, 0);
            NetworkWriterPool.Recycle(writer);
        }

        /// <summary>
        /// Send fake values to client's <see cref="SyncObject"/>.
        /// </summary>
        /// <param name="target">Target to send.</param>
        /// <param name="behaviorOwner"><see cref="NetworkIdentity"/> of object that owns <see cref="NetworkBehaviour"/>.</param>
        /// <param name="targetType"><see cref="NetworkBehaviour"/>'s type.</param>
        /// <param name="customAction">Custom writing action.</param>
        /// <example>
        /// EffectOnlySCP207.
        /// <code>
        ///  MirrorExtensions.SendCustomSync(player, player.ReferenceHub.networkIdentity, typeof(PlayerEffectsController), (writer) => {
        ///   writer.WriteUInt64(1ul);                                           // DirtyObjectsBit
        ///   writer.WriteUInt32(1);                                             // DirtyIndexCount
        ///   writer.WriteByte((byte)SyncList&lt;byte&gt;.Operation.OP_SET);     // Operations
        ///   writer.WriteUInt32(17);                                            // EditIndex
        ///   writer.WriteByte(1);                                               // Value
        ///  });
        /// </code>
        /// </example>
        public static void SendFakeSyncObject(Player target, NetworkIdentity behaviorOwner, Type targetType, Action<NetworkWriter> customAction)
        {
            PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
            PooledNetworkWriter writer2 = NetworkWriterPool.GetWriter();
            MakeCustomSyncWriter(behaviorOwner, targetType, customAction, null, writer, writer2);
            target.ReferenceHub.networkIdentity.connectionToClient.Send(new UpdateVarsMessage() { netId = behaviorOwner.netId, payload = writer.ToArraySegment() });
            NetworkWriterPool.Recycle(writer);
            NetworkWriterPool.Recycle(writer2);
        }

        /// <summary>
        /// Edit <see cref="NetworkIdentity"/>'s parameter and sync.
        /// </summary>
        /// <param name="identity">Target object.</param>
        /// <param name="customAction">Edit function.</param>
        public static void EditNetworkObject(NetworkIdentity identity, Action<NetworkIdentity> customAction)
        {
            customAction.Invoke(identity);

            ObjectDestroyMessage objectDestroyMessage = new()
            {
                netId = identity.netId,
            };
            foreach (Player ply in Player.List)
            {
                ply.Connection.Send(objectDestroyMessage, 0);
                SendSpawnMessageMethodInfo.Invoke(null, new object[] { identity, ply.Connection });
            }
        }

        // Get components index in identity.(private)
        private static int GetComponentIndex(NetworkIdentity identity, Type type)
        {
            return Array.FindIndex(identity.NetworkBehaviours, (x) => x.GetType() == type);
        }

        // Make custom writer(private)
        private static void MakeCustomSyncWriter(NetworkIdentity behaviorOwner, Type targetType, Action<NetworkWriter> customSyncObject, Action<NetworkWriter> customSyncVar, NetworkWriter owner, NetworkWriter observer)
        {
            byte behaviorDirty = 0;
            NetworkBehaviour behaviour = null;

            // Get NetworkBehaviors index (behaviorDirty use index)
            for (int i = 0; i < behaviorOwner.NetworkBehaviours.Length; i++)
            {
                if (behaviorOwner.NetworkBehaviours[i].GetType() == targetType)
                {
                    behaviour = behaviorOwner.NetworkBehaviours[i];
                    behaviorDirty = (byte)i;
                    break;
                }
            }

            // Write target NetworkBehavior's dirty
            owner.WriteByte(behaviorDirty);

            // Write init position
            int position = owner.Position;
            owner.WriteInt32(0);
            int position2 = owner.Position;

            // Write custom sync data
            if (customSyncObject is not null)
                customSyncObject.Invoke(owner);
            else
                behaviour.SerializeObjectsDelta(owner);

            // Write custom syncvar
            customSyncVar?.Invoke(owner);

            // Write syncdata position data
            int position3 = owner.Position;
            owner.Position = position;
            owner.WriteInt32(position3 - position2);
            owner.Position = position3;

            // Copy owner to observer
            if (behaviour.syncMode != SyncMode.Observers)
            {
                ArraySegment<byte> arraySegment = owner.ToArraySegment();
                observer.WriteBytes(arraySegment.Array, position, owner.Position - position);
            }
        }
    }
}
