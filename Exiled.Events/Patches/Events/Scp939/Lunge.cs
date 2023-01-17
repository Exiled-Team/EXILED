// -----------------------------------------------------------------------
// <copyright file="Lunge.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.API.Features;
using Exiled.API.Features.Pools;
using PlayerRoles.PlayableScps.Subroutines;

namespace Exiled.Events.Patches.Events.Scp939
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs.Scp939;
    using Exiled.Events.Handlers;
    using HarmonyLib;
    using Mirror;
    using PlayerRoles;
    using PlayerRoles.FirstPersonControl;
    using PlayerRoles.PlayableScps.Scp939;
    using RelativePositioning;
    using UnityEngine;
    using Utils.Networking;

    using static HarmonyLib.AccessTools;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
    /// <summary>
    ///     Patches <see cref="Scp939LungeAbility.ServerProcessCmd(NetworkReader)" />
    ///     to add the <see cref="Scp939" /> event.
    /// </summary>
    [HarmonyPatch]
    internal static class Lunge
    {

        // [HarmonyPatch(typeof(Scp939LungeAbility), nameof(Scp939LungeAbility.OnGrounded))]
        // [HarmonyTranspiler]
        // private static IEnumerable<CodeInstruction> OnLungeMiss(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        // {
        //     List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);
        //
        //     Label returnLabel = generator.DefineLabel();
        //
        //     newInstructions.InsertRange(
        //         0,
        //         new[]
        //         {
        //             // Scp939ProcessLunge, NetworkReader
        //             new CodeInstruction(OpCodes.Ldarg_0),
        //             new(OpCodes.Call, Method(typeof(Lunge), nameof(ProcessLungeMiss))),
        //             new(OpCodes.Br, returnLabel),
        //         });
        //
        //     newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);
        //
        //     for (int z = 0; z < newInstructions.Count; z++)
        //         yield return newInstructions[z];
        //
        //     ListPool<CodeInstruction>.Pool.Return(newInstructions);
        // }
        //
        // private static void ProcessLungeMiss(Scp939LungeAbility instance)
        // {
        //     Log.Info($"Processing lunge miss {instance.State}");
        //     if (!instance.HasAuthority || instance.State != Scp939LungeState.Triggered)
        //     {
        //         return;
        //     }
        //     // instance.ServerSendRpc(false);
        //     instance.State = ((instance.CurPos.Position.y < instance.TriggerPos.Position.y - instance._harshLandingHeight) ? Scp939LungeState.LandHarsh : Scp939LungeState.LandRegular);
        // }

        [HarmonyPatch(typeof(Scp939LungeAbility), nameof(Scp939LungeAbility.ServerProcessCmd))]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> OnLungeHit(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();

            // Right after base call.
            int index = newInstructions.FindIndex(instruction => instruction.Calls(Method(typeof(ScpSubroutineBase), nameof(ScpSubroutineBase.ServerProcessCmd)))) + 1;
            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Scp939ProcessLunge, NetworkReader
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new(OpCodes.Call, Method(typeof(Lunge), nameof(ProcessLunge))),
                    new(OpCodes.Br, returnLabel),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private static void ProcessLunge(Scp939LungeAbility instance, NetworkReader reader)
        {
            Vector3 vector = reader.ReadRelativePosition().Position;
            ReferenceHub referenceHub = reader.ReadReferenceHub();
            RelativePosition relativePosition = reader.ReadRelativePosition();
            Log.Info($"What is current state {instance.State}");
            if (instance.State != Scp939LungeState.Triggered)
            {
                if (!instance.IsReady)
                {
                    return;
                }

            }

            LungingEventArgs ev = new(instance.Owner, instance.State);
            Handlers.Scp939.OnLunging(ev);
            if (ev.IsAllowed)
            {
                instance.State = ev.State;
                return;
            }

            instance.TriggerLunge();

            HumanRole humanRole;
            if (referenceHub == null || (humanRole = referenceHub.roleManager.CurrentRole as HumanRole) == null)
            {
                return;
            }

            FirstPersonMovementModule fpcModule = humanRole.FpcModule;
            using (new FpcBacktracker(referenceHub, relativePosition.Position))
            {
                using (new FpcBacktracker(instance.Owner, fpcModule.Position, Quaternion.identity))
                {
                    Vector3 vector2 = fpcModule.Position - instance.ScpRole.FpcModule.Position;
                    if (vector2.SqrMagnitudeIgnoreY() > instance._overallTolerance * instance._overallTolerance)
                    {
                        return;
                    }

                    if (vector2.y > instance._overallTolerance || vector2.y < -instance._bottomTolerance)
                    {
                        return;
                    }
                }
            }

            using (new FpcBacktracker(instance.Owner, vector, Quaternion.identity))
            {
                vector = instance.ScpRole.FpcModule.Position;
            }

            Transform transform = referenceHub.transform;
            Vector3 position = fpcModule.Position;
            Quaternion rotation = transform.rotation;
            Vector3 vector3 = new(vector.x, position.y, vector.z);
            transform.forward = -instance.Owner.transform.forward;
            fpcModule.Position = vector3;
            bool flag = referenceHub.playerStats.DealDamage(new Scp939DamageHandler(instance.ScpRole, Scp939DamageType.LungeTarget));
            float num = flag ? 1f : 0f;
            if (!flag || referenceHub.IsAlive())
            {
                fpcModule.Position = position;
                transform.rotation = rotation;
            }

            foreach (ReferenceHub referenceHub2 in ReferenceHub.AllHubs)
            {
                HumanRole humanRole2;
                if (!(referenceHub2 == referenceHub) && (humanRole2 = referenceHub2.roleManager.CurrentRole as HumanRole) != null
                                                     && (humanRole2.FpcModule.Position - vector3).sqrMagnitude <= instance._secondaryRangeSqr
                                                     && referenceHub2.playerStats.DealDamage(new Scp939DamageHandler(instance.ScpRole, Scp939DamageType.LungeSecondary)))
                {
                    flag = true;
                    num = Mathf.Max(num, 0.6f);
                }
            }

            if (flag)
            {
                Hitmarker.SendHitmarker(instance.Owner, num);
            }

            instance.State = Scp939LungeState.LandHit;
            return;
        }
    }
}