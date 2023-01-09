// -----------------------------------------------------------------------
// <copyright file="Lunge.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.Events.EventArgs.Scp939;
using Exiled.Events.Handlers;
using PlayerRoles.PlayableScps.Scp939;
using static PlayerList;

namespace Exiled.Events.Patches.Events.Scp939
{
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
    using Exiled.Events.EventArgs.Scp939;
    using Exiled.Events.Handlers;
    using HarmonyLib;
    using Mirror;
    using PlayerRoles.FirstPersonControl;
    using PlayerRoles;
    using PlayerRoles.PlayableScps.Scp939;
    using RelativePositioning;
    using UnityEngine;
    using Utils.Networking;

    /// <summary>
    ///     Patches <see cref="Scp939LungeAbility.ServerProcessCmd(NetworkReader)" />
    ///     to add the <see cref="Scp939.Lunging" /> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp939LungeAbility), nameof(Scp939LungeAbility.ServerProcessCmd))]
    internal static class Lunge
    {
        private static bool Prefix(Scp939LungeAbility __instance, NetworkReader reader)
        {
            __instance.ServerProcessCmd(reader);
            Vector3 vector = reader.ReadRelativePosition().Position;
            ReferenceHub referenceHub = reader.ReadReferenceHub();
            RelativePosition relativePosition = reader.ReadRelativePosition();
            if (__instance.State != Scp939LungeState.Triggered)
            {
                if (__instance.State != Scp939LungeState.Triggered && !__instance.IsReady)
                    return false;

                LungingEventArgs ev = new(__instance.Owner, __instance.IsReady);
                Scp939.OnLunging(ev);
                if (ev.IsAllowed)
                    return false;

                __instance.TriggerLunge();
            }

            HumanRole humanRole;
            if (referenceHub == null || (humanRole = referenceHub.roleManager.CurrentRole as HumanRole) == null)
            {
                return false;
            }

            FirstPersonMovementModule fpcModule = humanRole.FpcModule;
            using (new FpcBacktracker(referenceHub, relativePosition.Position, 0.4f))
            {
                using (new FpcBacktracker(__instance.Owner, fpcModule.Position, Quaternion.identity, 0.1f, 0.15f))
                {
                    Vector3 vector2 = fpcModule.Position - __instance.ScpRole.FpcModule.Position;
                    if (vector2.SqrMagnitudeIgnoreY() > __instance._overallTolerance * __instance._overallTolerance)
                    {
                        return false;
                    }

                    if (vector2.y > __instance._overallTolerance || vector2.y < -__instance._bottomTolerance)
                    {
                        return false;
                    }
                }
            }

            using (new FpcBacktracker(__instance.Owner, vector, Quaternion.identity, 0.1f, 0.15f))
            {
                vector = __instance.ScpRole.FpcModule.Position;
            }

            Transform transform = referenceHub.transform;
            Vector3 position = fpcModule.Position;
            Quaternion rotation = transform.rotation;
            Vector3 vector3 = new(vector.x, position.y, vector.z);
            transform.forward = -__instance.Owner.transform.forward;
            fpcModule.Position = vector3;
            bool flag = referenceHub.playerStats.DealDamage(new Scp939DamageHandler(__instance.ScpRole, Scp939DamageType.LungeTarget));
            float num = flag ? 1f : 0f;
            if (!flag || referenceHub.IsAlive())
            {
                fpcModule.Position = position;
                transform.rotation = rotation;
            }

            foreach (ReferenceHub referenceHub2 in ReferenceHub.AllHubs)
            {
                HumanRole humanRole2;
                if (!(referenceHub2 == referenceHub) && (humanRole2 = referenceHub2.roleManager.CurrentRole as HumanRole) != null && (humanRole2.FpcModule.Position - vector3).sqrMagnitude <= __instance._secondaryRangeSqr && referenceHub2.playerStats.DealDamage(new Scp939DamageHandler(__instance.ScpRole, Scp939DamageType.LungeSecondary)))
                {
                    flag = true;
                    num = Mathf.Max(num, 0.6f);
                }
            }

            if (flag)
            {
                Hitmarker.SendHitmarker(__instance.Owner, num);
            }

            __instance.State = Scp939LungeState.LandHit;
            return false;
        }
    }
}