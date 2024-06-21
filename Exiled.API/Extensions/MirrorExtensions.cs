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

    using Features;
    using Features.Core.Generic.Pools;
    using InventorySystem.Items.Firearms;
    using Mirror;
    using PlayerRoles;
    using PlayerRoles.FirstPersonControl;
    using PlayerRoles.PlayableScps.Scp049.Zombies;
    using PlayerRoles.Subroutines;
    using RelativePositioning;
    using Respawning;
    using UnityEngine;

    /// <summary>
    /// A set of extensions for <see cref="Mirror"/> Networking.
    /// </summary>
    public static class MirrorExtensions
    {
        private static readonly Dictionary<Type, MethodInfo> WriterExtensionsValue = new();
        private static readonly Dictionary<string, ulong> SyncVarDirtyBitsValue = new();
        private static readonly Dictionary<string, string> RpcFullNamesValue = new();
        private static readonly ReadOnlyDictionary<Type, MethodInfo> ReadOnlyWriterExtensionsValue = new(WriterExtensionsValue);
        private static readonly ReadOnlyDictionary<string, ulong> ReadOnlySyncVarDirtyBitsValue = new(SyncVarDirtyBitsValue);
        private static readonly ReadOnlyDictionary<string, string> ReadOnlyRpcFullNamesValue = new(RpcFullNamesValue);
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
                    foreach (MethodInfo method in typeof(NetworkWriterExtensions).GetMethods().Where(x => !x.IsGenericMethod && x.GetCustomAttribute(typeof(ObsoleteAttribute)) == null && (x.GetParameters()?.Length == 2)))
                        WriterExtensionsValue.Add(method.GetParameters().First(x => x.ParameterType != typeof(NetworkWriter)).ParameterType, method);

                    Type fuckNorthwood = Assembly.GetAssembly(typeof(RoleTypeId)).GetType("Mirror.GeneratedNetworkCode");
                    foreach (MethodInfo method in fuckNorthwood.GetMethods().Where(x => !x.IsGenericMethod && (x.GetParameters()?.Length == 2) && (x.ReturnType == typeof(void))))
                        WriterExtensionsValue.Add(method.GetParameters().First(x => x.ParameterType != typeof(NetworkWriter)).ParameterType, method);

                    foreach (Type serializer in typeof(ServerConsole).Assembly.GetTypes().Where(x => x.Name.EndsWith("Serializer")))
                    {
                        foreach (MethodInfo method in serializer.GetMethods().Where(x => (x.ReturnType == typeof(void)) && x.Name.StartsWith("Write")))
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

                        if (!SyncVarDirtyBitsValue.ContainsKey($"{property.ReflectedType.Name}.{property.Name}"))
                            SyncVarDirtyBitsValue.Add($"{property.ReflectedType.Name}.{property.Name}", bytecodes[bytecodes.LastIndexOf((byte)OpCodes.Ldc_I8.Value) + 1]);
                    }
                }

                return ReadOnlySyncVarDirtyBitsValue;
            }
        }

        /// <summary>
        /// Gets Rpc's FullName <see cref="string"/> corresponding to <see cref="StringExtensions"/>(format:classname.methodname).
        /// </summary>
        public static ReadOnlyDictionary<string, string> RpcFullNames
        {
            get
            {
                if (RpcFullNamesValue.Count == 0)
                {
                    foreach (MethodInfo method in typeof(ServerConsole).Assembly.GetTypes()
                        .SelectMany(x => x.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                        .Where(m => m.GetCustomAttributes(typeof(ClientRpcAttribute), false).Length > 0 || m.GetCustomAttributes(typeof(TargetRpcAttribute), false).Length > 0))
                    {
                        MethodBody methodBody = method.GetMethodBody();

                        if (methodBody is null)
                            continue;

                        byte[] bytecodes = methodBody.GetILAsByteArray();

                        if (!RpcFullNamesValue.ContainsKey($"{method.ReflectedType.Name}.{method.Name}"))
                            RpcFullNamesValue.Add($"{method.ReflectedType.Name}.{method.Name}", method.Module.ResolveString(BitConverter.ToInt32(bytecodes, bytecodes.IndexOf((byte)OpCodes.Ldstr.Value) + 1)));
                    }
                }

                return ReadOnlyRpcFullNamesValue;
            }
        }

        /// <summary>
        /// Gets a <see cref="NetworkBehaviour.SetSyncVarDirtyBit(ulong)"/>'s <see cref="MethodInfo"/>.
        /// </summary>
        public static MethodInfo SetDirtyBitsMethodInfo => setDirtyBitsMethodInfoValue ??= typeof(NetworkBehaviour).GetMethod(nameof(NetworkBehaviour.SetSyncVarDirtyBit));

        /// <summary>
        /// Gets a NetworkServer.SendSpawnMessage's <see cref="MethodInfo"/>.
        /// </summary>
        public static MethodInfo SendSpawnMessageMethodInfo => sendSpawnMessageMethodInfoValue ??= typeof(NetworkServer).GetMethod("SendSpawnMessage", BindingFlags.NonPublic | BindingFlags.Static);

        /// <summary>
        /// Plays a beep sound that only the target <paramref name="player"/> can hear.
        /// </summary>
        /// <param name="player">Target to play sound to.</param>
        public static void PlayBeepSound(this Player player) => SendFakeTargetRpc(player, ReferenceHub.HostHub.networkIdentity, typeof(AmbientSoundPlayer), nameof(AmbientSoundPlayer.RpcPlaySound), 7);

        /// <summary>
        /// Set <see cref="Player.CustomInfo"/> on the <paramref name="target"/> player that only the <paramref name="player"/> can see.
        /// </summary>
        /// <param name="player">Only this player can see info.</param>
        /// <param name="target">Target to set info.</param>
        /// <param name="info">Setting info.</param>
        public static void SetPlayerInfoForTargetOnly(this Player player, Player target, string info) => player.SendFakeSyncVar(target.ReferenceHub.networkIdentity, typeof(NicknameSync), nameof(NicknameSync.Network_customPlayerInfoString), info);

        /// <summary>
        /// Plays a gun sound that only the <paramref name="player"/> can hear.
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
                ShooterHub = player.ReferenceHub,
                ShooterPosition = new RelativePosition(position),
            };

            player.Connection.Send(message);
        }

        /// <summary>
        /// Sets <see cref="Room.Color"/> of a <paramref name="room"/> that only the <paramref name="target"/> player can see.
        /// </summary>
        /// <param name="room">Room to modify.</param>
        /// <param name="target">Only this player can see room color.</param>
        /// <param name="color">Color to set.</param>
        public static void SetRoomColorForTargetOnly(this Room room, Player target, Color color) => target.SendFakeSyncVar(room.RoomLightControllerNetIdentity, typeof(RoomLightController), nameof(RoomLightController.NetworkOverrideColor), color);

        /// <summary>
        /// Sets <see cref="Player.DisplayNickname"/> of a <paramref name="player"/> that only the <paramref name="target"/> player can see.
        /// </summary>
        /// <param name="target">Only this player can see the name changed.</param>
        /// <param name="player">Player that will desync the CustomName.</param>
        /// <param name="name">Nickname to set.</param>
        public static void SetName(this Player target, Player player, string name)
        {
            target.SendFakeSyncVar(player.NetworkIdentity, typeof(NicknameSync), nameof(NicknameSync.Network_displayName), name);
        }

        /// <summary>
        /// Change <see cref="Player"/> character model for appearance.
        /// It will continue until <see cref="Player"/>'s <see cref="RoleTypeId"/> changes.
        /// </summary>
        /// <param name="player">Player to change.</param>
        /// <param name="type">Model type.</param>
        /// <param name="skipJump">Whether or not to skip the little jump that works around an invisibility issue.</param>
        /// <param name="unitId">The UnitNameId to use for the player's new role, if the player's new role uses unit names. (is NTF).</param>
        public static void ChangeAppearance(this Player player, RoleTypeId type, bool skipJump = false, byte unitId = 0) => ChangeAppearance(player, type, Player.List.Where(x => x != player), skipJump, unitId);

        /// <summary>
        /// Change <see cref="Player"/> character model for appearance.
        /// It will continue until <see cref="Player"/>'s <see cref="RoleTypeId"/> changes.
        /// </summary>
        /// <param name="player">Player to change.</param>
        /// <param name="type">Model type.</param>
        /// <param name="playersToAffect">The players who should see the changed appearance.</param>
        /// <param name="skipJump">Whether or not to skip the little jump that works around an invisibility issue.</param>
        /// <param name="unitId">The UnitNameId to use for the player's new role, if the player's new role uses unit names. (is NTF).</param>
        public static void ChangeAppearance(this Player player, RoleTypeId type, IEnumerable<Player> playersToAffect, bool skipJump = false, byte unitId = 0)
        {
            if (!player.IsConnected || !RoleExtensions.TryGetRoleBase(type, out PlayerRoleBase roleBase))
                return;

            bool isRisky = type.GetTeam() is Team.Dead || player.IsDead;

            NetworkWriterPooled writer = NetworkWriterPool.Get();
            writer.WriteUShort(38952);
            writer.WriteUInt(player.NetId);
            writer.WriteRoleType(type);

            if (roleBase is HumanRole humanRole && humanRole.UsesUnitNames)
            {
                if (player.Role.Base is not HumanRole)
                    isRisky = true;
                writer.WriteByte(unitId);
            }

            if (roleBase is FpcStandardRoleBase fpc)
            {
                if (player.Role.Base is not FpcStandardRoleBase playerfpc)
                    isRisky = true;
                else
                    fpc = playerfpc;

                ushort value = 0;
                fpc?.FpcModule.MouseLook.GetSyncValues(0, out value, out ushort _);
                writer.WriteRelativePosition(player.RelativePosition);
                writer.WriteUShort(value);
            }

            if (roleBase is ZombieRole)
            {
                if (player.Role.Base is not ZombieRole)
                    isRisky = true;

                writer.WriteUShort((ushort)Mathf.Clamp(Mathf.CeilToInt(player.MaxHealth), ushort.MinValue, ushort.MaxValue));
            }

            foreach (Player target in playersToAffect)
            {
                if (target != player || !isRisky)
                    target.Connection.Send(writer.ToArraySegment());
                else
                    Log.Error($"Prevent Seld-Desync of {player.Nickname} with {type}");
            }

            NetworkWriterPool.Return(writer);

            // To counter a bug that makes the player invisible until they move after changing their appearance, we will teleport them upwards slightly to force a new position update for all clients.
            if (!skipJump)
                player.Position += Vector3.up * 0.25f;
        }

        /// <summary>
        /// Send CASSIE announcement that only <see cref="Player"/> can hear.
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
        /// </summary>
        /// <param name="player">Target to send.</param>
        /// <param name="words">The message to be reproduced.</param>
        /// <param name="translation">The translation should be show in the subtitles.</param>
        /// <param name="makeHold">Same on <see cref="Cassie.MessageTranslated(string, string, bool, bool, bool)"/>'s isHeld.</param>
        /// <param name="makeNoise">Same on <see cref="Cassie.MessageTranslated(string, string, bool, bool, bool)"/>'s isNoisy.</param>
        /// <param name="isSubtitles">Same on <see cref="Cassie.MessageTranslated(string, string, bool, bool, bool)"/>'s isSubtitles.</param>
        public static void MessageTranslated(this Player player, string words, string translation, bool makeHold = false, bool makeNoise = true, bool isSubtitles = true)
        {
            StringBuilder announcement = StringBuilderPool.Pool.Get();

            string[] cassies = words.Split('\n');
            string[] translations = translation.Split('\n');

            for (int i = 0; i < cassies.Length; i++)
                announcement.Append($"{translations[i].Replace(' ', ' ')}<size=0> {cassies[i]} </size><split>");

            string message = StringBuilderPool.Pool.ToStringReturn(announcement);

            foreach (RespawnEffectsController controller in RespawnEffectsController.AllControllers)
            {
                if (controller != null)
                {
                    SendFakeTargetRpc(player, controller.netIdentity, typeof(RespawnEffectsController), nameof(RespawnEffectsController.RpcCassieAnnouncement), message, makeHold, makeNoise, isSubtitles);
                }
            }
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
            if (!target.IsConnected)
                return;

            NetworkWriterPooled writer = NetworkWriterPool.Get();
            NetworkWriterPooled writer2 = NetworkWriterPool.Get();
            MakeCustomSyncWriter(behaviorOwner, targetType, null, CustomSyncVarGenerator, writer, writer2);
            target.Connection.Send(new EntityStateMessage
            {
                netId = behaviorOwner.netId,
                payload = writer.ToArraySegment(),
            });

            NetworkWriterPool.Return(writer);
            NetworkWriterPool.Return(writer2);
            void CustomSyncVarGenerator(NetworkWriter targetWriter)
            {
                targetWriter.WriteULong(SyncVarDirtyBits[$"{targetType.Name}.{propertyName}"]);
                WriterExtensions[value.GetType()]?.Invoke(null, new object[2] { targetWriter, value });
            }
        }

        /// <summary>
        /// Force resync to client's <see cref="SyncVarAttribute"/>.
        /// </summary>
        /// <param name="behaviorOwner"><see cref="NetworkIdentity"/> of object that owns <see cref="NetworkBehaviour"/>.</param>
        /// <param name="targetType"><see cref="NetworkBehaviour"/>'s type.</param>
        /// <param name="propertyName">Property name starting with Network.</param>
        public static void ResyncSyncVar(NetworkIdentity behaviorOwner, Type targetType, string propertyName) => SetDirtyBitsMethodInfo.Invoke(behaviorOwner.gameObject.GetComponent(targetType), new object[] { SyncVarDirtyBits[$"{targetType.Name}.{propertyName}"] });

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
            if (!target.IsConnected)
                return;

            NetworkWriterPooled writer = NetworkWriterPool.Get();

            foreach (object value in values)
                WriterExtensions[value.GetType()].Invoke(null, new[] { writer, value });

            RpcMessage msg = new()
            {
                netId = behaviorOwner.netId,
                componentIndex = (byte)GetComponentIndex(behaviorOwner, targetType),
                functionHash = (ushort)RpcFullNames[$"{targetType.Name}.{rpcName}"].GetStableHashCode(),
                payload = writer.ToArraySegment(),
            };

            target.Connection.Send(msg);

            NetworkWriterPool.Return(writer);
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
            if (!target.IsConnected)
                return;

            NetworkWriterPooled writer = NetworkWriterPool.Get();
            NetworkWriterPooled writer2 = NetworkWriterPool.Get();
            MakeCustomSyncWriter(behaviorOwner, targetType, customAction, null, writer, writer2);
            target.ReferenceHub.networkIdentity.connectionToClient.Send(new EntityStateMessage() { netId = behaviorOwner.netId, payload = writer.ToArraySegment() });
            NetworkWriterPool.Return(writer);
            NetworkWriterPool.Return(writer2);
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

            foreach (Player player in Player.List)
            {
                player.Connection.Send(objectDestroyMessage, 0);
                SendSpawnMessageMethodInfo.Invoke(null, new object[] { identity, player.Connection });
            }
        }

        /// <summary>
        /// Sends a <see cref="SubroutineMessage"/>.
        /// </summary>
        /// <param name="subroutineBase">Base <see cref="SubroutineBase"/> instance.</param>
        /// <param name="applyingChanges">Action that will apply needed changes to a <paramref name="subroutineBase"/>.</param>
        /// <param name="toAll">Should message be sent to everybody or to <see cref="SubroutineBase.Role"/> only.</param>
        /// <typeparam name="T">A type of <see cref="SubroutineBase"/>.</typeparam>
        public static void SendRpc<T>(this T subroutineBase, Action<T> applyingChanges, bool toAll = true)
            where T : SubroutineBase
        {
            applyingChanges(subroutineBase);

            SubroutineMessage msg = new(subroutineBase, true);

            if (toAll)
                NetworkServer.SendToAll(msg);
            else
                subroutineBase.Role._lastOwner.connectionToClient.Send(msg);
        }

        // Get components index in identity.(private)
        private static int GetComponentIndex(NetworkIdentity identity, Type type)
        {
            return Array.FindIndex(identity.NetworkBehaviours, (x) => x.GetType() == type);
        }

        // Make custom writer(private)
        private static void MakeCustomSyncWriter(NetworkIdentity behaviorOwner, Type targetType, Action<NetworkWriter> customSyncObject, Action<NetworkWriter> customSyncVar, NetworkWriter owner, NetworkWriter observer)
        {
            ulong value = 0;
            NetworkBehaviour behaviour = null;

            // Get NetworkBehaviors index (behaviorDirty use index)
            for (int i = 0; i < behaviorOwner.NetworkBehaviours.Length; i++)
            {
                if (behaviorOwner.NetworkBehaviours[i].GetType() == targetType)
                {
                    behaviour = behaviorOwner.NetworkBehaviours[i];
                    value = 1UL << (i & 31);
                    break;
                }
            }

            // Write target NetworkBehavior's dirty
            Compression.CompressVarUInt(owner, value);

            // Write init position
            int position = owner.Position;
            owner.WriteByte(0);
            int position2 = owner.Position;

            // Write custom sync data
            if (customSyncObject is not null)
                customSyncObject(owner);
            else
                behaviour.SerializeObjectsDelta(owner);

            // Write custom syncvar
            customSyncVar?.Invoke(owner);

            // Write syncdata position data
            int position3 = owner.Position;
            owner.Position = position;
            owner.WriteByte((byte)(position3 - position2 & 255));
            owner.Position = position3;

            // Copy owner to observer
            if (behaviour.syncMode != SyncMode.Observers)
                observer.WriteBytes(owner.ToArraySegment().Array, position, owner.Position - position);
        }
    }
}
