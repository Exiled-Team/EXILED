// -----------------------------------------------------------------------
// <copyright file="Scp173BeingLooked.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;

    using HarmonyLib;

    using MapGeneration;

    using NorthwoodLib.Pools;

    using PlayableScps;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="PlayableScps.Scp173.UpdateObservers"/>.
    /// </summary>
    [HarmonyPatch(typeof(PlayableScps.Scp173), nameof(PlayableScps.Scp173.UpdateObservers))]
    internal static class Scp173BeingLooked
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label jne = generator.DefineLabel();
            Label cnt = generator.DefineLabel();
            Label isTutorial = generator.DefineLabel();

            int addCheckOffset = 4;
            int addCheck = newInstructions.FindLastIndex(instruction => instruction.Calls(Method(typeof(Physics), nameof(Physics.Linecast), new[] { typeof(Vector3), typeof(Vector3), typeof(int) }))) + addCheckOffset;

            LocalBuilder player = generator.DeclareLocal(typeof(Player));

            newInstructions.InsertRange(addCheck, new CodeInstruction[]
            {
                new(OpCodes.Ldloc_3),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Dup),
                new(OpCodes.Stloc, player.LocalIndex),
                new(OpCodes.Brfalse_S, jne),
                new(OpCodes.Ldloc, player.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.Role))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(API.Features.Roles.Role), nameof(API.Features.Roles.Role.Type))),
                new(OpCodes.Ldc_I4_S, (int)RoleType.Tutorial),
                new(OpCodes.Beq_S, isTutorial),
                new(OpCodes.Call, PropertyGetter(typeof(API.Features.Scp173), nameof(API.Features.Scp173.TurnedPlayers))),
                new(OpCodes.Ldloc_3),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Callvirt, Method(typeof(HashSet<Player>), nameof(HashSet<Player>.Contains))),
                new(OpCodes.Brtrue_S, jne),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(Exiled.Events.Events), nameof(Exiled.Events.Events.Instance))).WithLabels(isTutorial),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Exiled.Events.Events), nameof(Exiled.Events.Events.Config))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Config), nameof(Exiled.Events.Events.Config.CanTutorialBlockScp173))),
                new(OpCodes.Brfalse_S, cnt),
            });

            int offset = -3;
            int defaultLogic = newInstructions.FindIndex(addCheck, instruction => instruction.LoadsField(Field(typeof(PlayableScps.Scp173), nameof(PlayableScps.Scp173._observingPlayers)))) + offset;

            int continueBr = newInstructions.FindIndex(addCheck, instruction => instruction.opcode == OpCodes.Br_S);

            newInstructions[defaultLogic].labels.Add(jne);
            newInstructions[continueBr].labels.Add(cnt);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }

    ///// <summary>
    ///// Patches <see cref="PlayableScps.Scp173.UpdateObservers"/>.
    ///// </summary>
    //[HarmonyPatch(typeof(PlayableScps.Scp173), nameof(PlayableScps.Scp173.UpdateObservers))]

    //internal static class Scp173BeingLooked
    //{
    //    [HarmonyPrefix]
    //    private static bool dumb(PlayableScps.Scp173 __instance)
    //    {

    //            int count = __instance._observingPlayers.Count;
    //            foreach (KeyValuePair<GameObject, ReferenceHub> allHub in ReferenceHub.GetAllHubs())
    //            {
    //                ReferenceHub value = allHub.Value;
    //                CharacterClassManager characterClassManager = value.characterClassManager;

    //                if (characterClassManager.CurClass == RoleType.Spectator || value == __instance.Hub || characterClassManager.IsAnyScp())
    //                {

    //                    if (__instance._observingPlayers.Contains(value))
    //                    {
    //                        __instance._observingPlayers.Remove(value);
    //                    }

    //                    continue;
    //                }

    //                Vector3 realModelPosition = __instance.Hub.playerMovementSync.RealModelPosition;
    //                bool flag = false;
    //                RoomIdentifier roomIdentifier = RoomIdUtils.RoomAtPosition(__instance.Hub.playerMovementSync.RealModelPosition);
    //                if (VisionInformation.GetVisionInformation(value, realModelPosition, -2f, (roomIdentifier != null && roomIdentifier.Zone == FacilityZone.Surface) ? 80f : 40f, checkFog: false, checkLineOfSight: false, __instance.Hub.localCurrentRoomEffects).IsLooking && (!Physics.Linecast(realModelPosition + new Vector3(0f, 1.5f, 0f), value.PlayerCameraReference.position, VisionInformation.VisionLayerMask) || !Physics.Linecast(realModelPosition + new Vector3(0f, -1f, 0f), value.PlayerCameraReference.position, VisionInformation.VisionLayerMask)))
    //                {
    //                    flag = true;
    //                }

    //                Player temp = Player.Get(value);

    //                if (temp != null)
    //                {
    //                    if (temp.Role.Type == RoleType.Tutorial && !Exiled.Events.Events.Instance.Config.CanTutorialBlockScp173)
    //                    {
    //                        continue;
    //                    }
    //                }

    //                if (flag)
    //                    {
    //                        if (!__instance._observingPlayers.Contains(value))
    //                        {
    //                            __instance._observingPlayers.Add(value);
    //                        }
    //                    }
    //                else if (__instance._observingPlayers.Contains(value))
    //                {
    //                    __instance._observingPlayers.Remove(value);
    //                }
    //            }

    //            __instance._isObserved = (__instance._observingPlayers.Count > 0 || __instance.StareAtDuration > 0f);
    //            if (count != __instance._observingPlayers.Count && __instance._blinkCooldownRemaining > 0f)
    //            {
    //                GameCore.Console.AddDebugLog("SCP173", $"Adjusting blink cooldown. Initial observers: {count}. " + $"New observers: {__instance._observingPlayers.Count}.", MessageImportance.LessImportant);
    //                GameCore.Console.AddDebugLog("SCP173", $"Current blink cooldown: {__instance._blinkCooldownRemaining}", MessageImportance.LeastImportant);
    //                __instance._blinkCooldownRemaining = Mathf.Max(0f, __instance._blinkCooldownRemaining + ((float)(__instance._observingPlayers.Count - count) * (__instance.BreakneckSpeedsActive ? 0f : 0f)));
    //                GameCore.Console.AddDebugLog("SCP173", $"New blink cooldown: {__instance._blinkCooldownRemaining}", MessageImportance.LeastImportant);
    //                if (__instance._blinkCooldownRemaining <= 0f)
    //                {
    //                __instance.BlinkReady = true;
    //                }
    //            }

    //            return false;
    //    }
    //}
}
