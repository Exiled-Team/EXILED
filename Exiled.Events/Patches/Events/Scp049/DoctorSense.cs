using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Exiled.Events.EventArgs.Scp049;
using HarmonyLib;
using Mirror;
using NorthwoodLib.Pools;
using PlayerRoles;
using PlayerRoles.PlayableScps;
using PlayerRoles.PlayableScps.Scp049;
using PlayerRoles.PlayableScps.Subroutines;
using PluginAPI.Core;
using UnityEngine;
using Utils.Networking;

namespace Exiled.Events.Patches.Events.Scp049
{
    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="Scp049ResurrectAbility.ServerComplete" />.
    ///     Adds the <see cref="Handlers.Scp049.FinishingRecall" /> event.
    /// </summary>

    [HarmonyPatch(typeof(Scp049SenseAbility), nameof(Scp049SenseAbility.ServerProcessCmd))]
    public class DoctorSense
    {

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            // Immediately return
            Label returnLabel = generator.DefineLabel();

            newInstructions.InsertRange(
                0,
                new[]
                {
                    // Scp049SenseAbility, NetworkReader
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldarg_1),
                    // Returns DoctorSenseEventArgs
                    new(OpCodes.Call, Method(typeof(DoctorSense), nameof(Scp049Sense))),
                    // If !ev.IsAllowed, return
                    new(OpCodes.Br, returnLabel),

                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        /// <summary>
        /// Basically rewrites the ServerProcessCmd - It really is not worth it to not do this
        /// </summary>
        /// <param name="senseAbility"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static void Scp049Sense(Scp049SenseAbility senseAbility, NetworkReader reader)
        {
            DoctorSenseEventArgs ev = new DoctorSenseEventArgs(API.Features.Player.Get(senseAbility.Owner), API.Features.Player.Get(reader.ReadReferenceHub()), reader, senseAbility);
            Handlers.Scp049.OnDoctorSense(ev);
            if (!ev.IsAllowed)
            {
                return;
            }

            // If users want to force event
            if (!ev.BypassChecks)
            {
                if (!senseAbility.Cooldown.IsReady || !senseAbility.Duration.IsReady)
                {
                    return;
                }
            }

            senseAbility.HasTarget = false;
            senseAbility.Target = ev.Target?.ReferenceHub;

            // If no targets, allow custom cooldown
            if (senseAbility.Target == null)
            {
                senseAbility.Cooldown.Trigger(ev.Cooldown);
                senseAbility.ServerSendRpc(true);
                return;
            }

            HumanRole humanRole;
            if ((humanRole = senseAbility.Target.roleManager.CurrentRole as HumanRole) == null)
            {
                return;
            }

            float radius = humanRole.FpcModule.CharController.radius;
            Vector3 cameraPosition = humanRole.CameraPosition;
            senseAbility._distanceThreshold = ev.Distance;
            if (!VisionInformation.GetVisionInformation(senseAbility.Owner, senseAbility.Owner.PlayerCameraReference, cameraPosition, radius, senseAbility._distanceThreshold, true, true, 0).IsLooking)
            {
                return;
            }

            senseAbility.Duration.Trigger(ev.Duration);
            senseAbility.HasTarget = true;
            senseAbility.ServerSendRpc(true);
        }

    }
}