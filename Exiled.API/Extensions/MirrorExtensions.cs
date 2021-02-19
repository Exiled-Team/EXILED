// -----------------------------------------------------------------------
// <copyright file="MirrorExtensions.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Extensions
{
    using System;
    using System.Reflection;

    using Exiled.API.Features;

    using Mirror;

    using UnityEngine;

    /// <summary>
    /// A set of extensions for <see cref="Mirror"/> Networking.
    /// </summary>
    public static class MirrorExtensions
    {
        /// <summary>
        /// Send fake values to client's <see cref="Mirror.SyncVarAttribute"/>.
        /// </summary>
        /// <param name="target">Target to send.</param>
        /// <param name="behaviorOwner"><see cref="Mirror.NetworkIdentity"/> of object that owns <see cref="Mirror.NetworkBehaviour"/>.</param>
        /// <param name="targetType"><see cref="Mirror.NetworkBehaviour"/>'s type.</param>
        /// <param name="propertyName">Property name starting with Network.</param>
        /// <param name="value">Value of send to target.</param>
        public static void SendFakeSyncVar(this Player target, NetworkIdentity behaviorOwner, Type targetType, string propertyName, object value)
        {
            Action<NetworkWriter> customSyncVarGenerator = (targetWriter) =>
            {
                targetWriter.WritePackedUInt64(GetDirtyBit(targetType, propertyName));
                GetWriteExtension(value)?.Invoke(null, new object[] { targetWriter, value });
            };

            NetworkWriter writer = NetworkWriterPool.GetWriter();
            NetworkWriter writer2 = NetworkWriterPool.GetWriter();
            MakeCustomSyncWriter(behaviorOwner, targetType, null, customSyncVarGenerator, writer, writer2);
            NetworkServer.SendToClientOfPlayer(target.ReferenceHub.networkIdentity, new UpdateVarsMessage() { netId = behaviorOwner.netId, payload = writer.ToArraySegment() });
            NetworkWriterPool.Recycle(writer);
            NetworkWriterPool.Recycle(writer2);
        }

        /// <summary>
        /// Send fake values to client's <see cref="Mirror.ClientRpcAttribute"/>.
        /// </summary>
        /// <param name="target">Target to send.</param>
        /// <param name="behaviorOwner"><see cref="Mirror.NetworkIdentity"/> of object that owns <see cref="Mirror.NetworkBehaviour"/>.</param>
        /// <param name="targetType"><see cref="Mirror.NetworkBehaviour"/>'s type.</param>
        /// <param name="rpcName">Property name starting with Rpc.</param>
        /// <param name="values">Values of send to target.</param>
        public static void SendFakeTargetRpc(this Player target, NetworkIdentity behaviorOwner, Type targetType, string rpcName, object[] values)
        {
            NetworkWriter writer = NetworkWriterPool.GetWriter();

            foreach (var value in values)
                GetWriteExtension(value)?.Invoke(null, new object[] { writer, value });

            var msg = new RpcMessage
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
        /// Send fake values to client's <see cref="Mirror.SyncObject"/>.
        /// </summary>
        /// <param name="target">Target to send.</param>
        /// <param name="behaviorOwner"><see cref="Mirror.NetworkIdentity"/> of object that owns <see cref="Mirror.NetworkBehaviour"/>.</param>
        /// <param name="targetType"><see cref="Mirror.NetworkBehaviour"/>'s type.</param>
        /// <param name="customAction">Custom writing action.</param>
        public static void SendFakeSyncObject(this Player target, NetworkIdentity behaviorOwner, Type targetType, Action<NetworkWriter> customAction)
        {
            NetworkWriter writer = NetworkWriterPool.GetWriter();
            NetworkWriter writer2 = NetworkWriterPool.GetWriter();
            MakeCustomSyncWriter(behaviorOwner, targetType, customAction, null, writer, writer2);
            NetworkServer.SendToClientOfPlayer(target.ReferenceHub.networkIdentity, new UpdateVarsMessage() { netId = behaviorOwner.netId, payload = writer.ToArraySegment() });
            NetworkWriterPool.Recycle(writer);
            NetworkWriterPool.Recycle(writer2);
        }

        /// <summary>
        /// Get <see cref="System.Type"/> index in <see cref="Mirror.NetworkIdentity"/>.
        /// </summary>
        /// <param name="identity">Target <see cref="Mirror.NetworkIdentity"/>.</param>
        /// <param name="type">Components type.</param>
        /// <returns>Returns type index.</returns>
        private static int GetComponentIndex(NetworkIdentity identity, Type type)
        {
            return Array.FindIndex(identity.NetworkBehaviours, (x) => x.GetType() == type);
        }

        /// <summary>
        /// Get property dirtybit in <see cref="Mirror.NetworkBehaviour"/>.
        /// </summary>
        /// <param name="targetType">Target type.</param>
        /// <param name="propertyName">Target property name.</param>
        /// <returns>Returns property dirtybit.</returns>
        private static ulong GetDirtyBit(Type targetType, string propertyName)
        {
            var bytecodes = targetType.GetProperty(propertyName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)?.GetSetMethod().GetMethodBody().GetILAsByteArray();
            return bytecodes[Array.FindLastIndex(bytecodes, x => x == System.Reflection.Emit.OpCodes.Ldc_I8.Value) + 1];
        }

        /// <summary>
        /// Get networkconverter from type.
        /// </summary>
        /// <param name="value">Target type object.</param>
        /// <returns>Returns <see cref="System.Reflection.MethodInfo"/>.</returns>
        private static MethodInfo GetWriteExtension(object value)
        {
            Type type = value.GetType();
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.String:
                    return typeof(NetworkWriterExtensions).GetMethod(nameof(NetworkWriterExtensions.WriteString));
                case TypeCode.Boolean:
                    return typeof(NetworkWriterExtensions).GetMethod(nameof(NetworkWriterExtensions.WriteBoolean));
                case TypeCode.Int16:
                    return typeof(NetworkWriterExtensions).GetMethod(nameof(NetworkWriterExtensions.WriteInt16));
                case TypeCode.Int32:
                    return typeof(NetworkWriterExtensions).GetMethod(nameof(NetworkWriterExtensions.WritePackedInt32));
                case TypeCode.UInt16:
                    return typeof(NetworkWriterExtensions).GetMethod(nameof(NetworkWriterExtensions.WriteUInt16));
                case TypeCode.Byte:
                    return typeof(NetworkWriterExtensions).GetMethod(nameof(NetworkWriterExtensions.WriteByte));
                case TypeCode.SByte:
                    return typeof(NetworkWriterExtensions).GetMethod(nameof(NetworkWriterExtensions.WriteSByte));
                case TypeCode.Single:
                    return typeof(NetworkWriterExtensions).GetMethod(nameof(NetworkWriterExtensions.WriteSingle));
                case TypeCode.Double:
                    return typeof(NetworkWriterExtensions).GetMethod(nameof(NetworkWriterExtensions.WriteDouble));
                default:
                    if (type == typeof(Vector3))
                        return typeof(NetworkWriterExtensions).GetMethod(nameof(NetworkWriterExtensions.WriteVector3));
                    if (type == typeof(Vector2))
                        return typeof(NetworkWriterExtensions).GetMethod(nameof(NetworkWriterExtensions.WriteVector2));
                    if (type == typeof(GameObject))
                        return typeof(NetworkWriterExtensions).GetMethod(nameof(NetworkWriterExtensions.WriteGameObject));
                    if (type == typeof(Quaternion))
                        return typeof(NetworkWriterExtensions).GetMethod(nameof(NetworkWriterExtensions.WriteQuaternion));
                    if (type == typeof(BreakableWindow.BreakableWindowStatus))
                        return typeof(BreakableWindowStatusSerializer).GetMethod(nameof(BreakableWindowStatusSerializer.WriteBreakableWindowStatus));
                    if (type == typeof(Grenades.RigidbodyVelocityPair))
                        return typeof(Grenades.RigidbodyVelocityPairSerializer).GetMethod(nameof(Grenades.RigidbodyVelocityPairSerializer.WriteRigidbodyVelocityPair));
                    if (type == typeof(ItemType))
                        return typeof(NetworkWriterExtensions).GetMethod(nameof(NetworkWriterExtensions.WritePackedInt32));
                    if (type == typeof(PlayerMovementSync.RotationVector))
                        return typeof(RotationVectorSerializer).GetMethod(nameof(RotationVectorSerializer.WriteRotationVector));
                    if (type == typeof(Pickup.WeaponModifiers))
                        return typeof(WeaponModifiersSerializer).GetMethod(nameof(WeaponModifiersSerializer.WriteWeaponModifiers));
                    if (type == typeof(Offset))
                        return typeof(OffsetSerializer).GetMethod(nameof(OffsetSerializer.WriteOffset));
                    return null;
            }
        }

        // Make custom writer(private)
        private static void MakeCustomSyncWriter(NetworkIdentity behaviorOwner, Type targetType, Action<NetworkWriter> customSyncObject, Action<NetworkWriter> customSyncVar, NetworkWriter owner, NetworkWriter observer)
        {
            ulong dirty = 0ul;
            ulong dirty_o = 0ul;
            NetworkBehaviour behaviour = null;
            for (int i = 0; i < behaviorOwner.NetworkBehaviours.Length; i++)
            {
                behaviour = behaviorOwner.NetworkBehaviours[i];
                if (behaviour.GetType() == targetType)
                {
                    dirty |= 1UL << i;
                    if (behaviour.syncMode == SyncMode.Observers)
                        dirty_o |= 1UL << i;
                }
            }

            owner.WritePackedUInt64(dirty);
            observer.WritePackedUInt64(dirty & dirty_o);

            int position = owner.Position;
            owner.WriteInt32(0);
            int position2 = owner.Position;

            if (customSyncObject != null)
                customSyncObject.Invoke(owner);
            else
                behaviour.SerializeObjectsDelta(owner);

            customSyncVar?.Invoke(owner);

            int position3 = owner.Position;
            owner.Position = position;
            owner.WriteInt32(position3 - position2);
            owner.Position = position3;

            if (dirty_o != 0ul)
            {
                ArraySegment<byte> arraySegment = owner.ToArraySegment();
                observer.WriteBytes(arraySegment.Array, position, owner.Position - position);
            }
        }
    }
}
