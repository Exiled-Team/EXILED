// -----------------------------------------------------------------------
// <copyright file="Scp173BeingLooked.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;
    using System.Text;

    using Exiled.API.Features;

    using HarmonyLib;

    using InventorySystem.Items.Firearms.Modules;

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
        public static LayerMask Scp173CurMask;

        public static LayerMask Scp173Mask {
            get
            {
                if (Scp173CurMask == 0)
                {
                    Scp173CurMask = LayerMask.GetMask(new string[]
                    {
                        "Default",
                        "Glass",
                        "CCTV",
                        "Door",
                        "Locker"
                    });
                }
                return Scp173CurMask;
            }
        }

        [HarmonyPrefix]
        private static bool ProcessObservers(PlayableScps.Scp173 __instance)
        {
            try
            {
                HashSet<uint> hashSet = HashSetPool<uint>.Shared.Rent();
                int count = __instance._observingPlayers.Count;
                Player scp173Player = Player.Get(__instance.Hub);
                API.Features.Scp173.TurnedPlayers.Remove(scp173Player);
                foreach (KeyValuePair<GameObject, ReferenceHub> pairedData in ReferenceHub.GetAllHubs())
                {
                    ReferenceHub currentPlayerReferenceHub = pairedData.Value;
                    CharacterClassManager characterClassManager = currentPlayerReferenceHub.characterClassManager;
                    if (characterClassManager.CurClass == RoleType.Spectator || currentPlayerReferenceHub == __instance.Hub || characterClassManager.IsAnyScp())
                    {
                        if (__instance._observingPlayers.Contains(currentPlayerReferenceHub))
                        {
                            __instance._observingPlayers.Remove(currentPlayerReferenceHub);
                        }
                    }
                    else
                    {
                        Vector3 realModelPosition = __instance.Hub.playerMovementSync.RealModelPosition;
                        bool flag = false;
                        Player player = Player.Get(pairedData.Key);
                        if (player != null)
                        {
                            if (player?.Role?.Type == RoleType.Tutorial)
                            {
                                if (!Exiled.Events.Events.Instance?.Config?.CanTutorialBlockScp173 ?? false)
                                {
                                    if (__instance._observingPlayers.Contains(currentPlayerReferenceHub))
                                    {
                                        __instance._observingPlayers.Remove(currentPlayerReferenceHub);
                                    }
                                    continue;
                                }
                            }
                            else if (API.Features.Scp173.TurnedPlayers.Contains(player))
                            {
                                if (__instance._observingPlayers.Contains(currentPlayerReferenceHub))
                                {
                                    __instance._observingPlayers.Remove(currentPlayerReferenceHub);
                                }

                                continue;
                            }
                        }

                        RoomIdentifier roomIdentifier = RoomIdUtils.RoomAtPosition(__instance.Hub.playerMovementSync.RealModelPosition);
                        if (VisionInformation.GetVisionInformation(currentPlayerReferenceHub, realModelPosition, -2f, (roomIdentifier != null && roomIdentifier.Zone == FacilityZone.Surface) ? 80f : 40f, false, false, __instance.Hub.localCurrentRoomEffects, 0).IsLooking && (!Physics.Linecast(realModelPosition + new Vector3(0f, 1.5f, 0f), currentPlayerReferenceHub.PlayerCameraReference.position, VisionInformation.VisionLayerMask) || !Physics.Linecast(realModelPosition + new Vector3(0f, -1f, 0f), currentPlayerReferenceHub.PlayerCameraReference.position, VisionInformation.VisionLayerMask)))
                        {
                            flag = true;
                        }

                        if (flag)
                        {
                            if (!__instance._observingPlayers.Contains(currentPlayerReferenceHub))
                            {
                                __instance._observingPlayers.Add(currentPlayerReferenceHub);
                            }
                        }
                        else if (__instance._observingPlayers.Contains(currentPlayerReferenceHub))
                        {
                            __instance._observingPlayers.Remove(currentPlayerReferenceHub);
                        }
                    }
                }

                __instance._isObserved = __instance._observingPlayers.Count > 0 || __instance.StareAtDuration > 0f;
                if (count != __instance._observingPlayers.Count && __instance._blinkCooldownRemaining > 0f)
                {
                    GameCore.Console.AddDebugLog("SCP173", string.Format("Adjusting blink cooldown. Initial observers: {0}. ", count) + string.Format("New observers: {0}.", __instance._observingPlayers.Count), MessageImportance.LessImportant, false);
                    GameCore.Console.AddDebugLog("SCP173", string.Format("Current blink cooldown: {0}", __instance._blinkCooldownRemaining), MessageImportance.LeastImportant, false);
                    __instance._blinkCooldownRemaining = Mathf.Max(0f, __instance._blinkCooldownRemaining + ((__instance._observingPlayers.Count - count) * (__instance.BreakneckSpeedsActive ? 0f : 0f)));
                    GameCore.Console.AddDebugLog("SCP173", string.Format("New blink cooldown: {0}", __instance._blinkCooldownRemaining), MessageImportance.LeastImportant, false);
                    if (__instance._blinkCooldownRemaining <= 0f)
                    {
                        __instance.BlinkReady = true;
                    }
                }
                HashSetPool<uint>.Shared.Return(hashSet);
                return false;
            }
            catch (Exception generic)
            {
                Log.Error($"Unable to do Scp173BeingLooked {generic}\n");
            }

            return true;
        }
    }

    ///// <summary>
    ///// Patches <see cref="PlayableScps.Scp173.UpdateObservers"/>.
    ///// </summary>
    //[HarmonyPatch(typeof(PlayableScps.Scp173), nameof(PlayableScps.Scp173.UpdateObservers))]
    //internal static class Scp173BeingLooked
    //{
    //    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    //    {
    //        List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

    //        Label jne = generator.DefineLabel();
    //        Label cnt = generator.DefineLabel();
    //        Label isTutorial = generator.DefineLabel();

    //        int addCheckOffset = 4;
    //        int addCheck = newInstructions.FindLastIndex(instruction => instruction.Calls(Method(typeof(Physics), nameof(Physics.Linecast), new[] { typeof(Vector3), typeof(Vector3), typeof(int) }))) + addCheckOffset;

    //        LocalBuilder player = generator.DeclareLocal(typeof(Player));

    //        LocalBuilder turnedPlayers = generator.DeclareLocal(typeof(HashSet<Player>));

    //        newInstructions.InsertRange(addCheck, new CodeInstruction[]
    //        {
    //            // Player.get(current player in all hub)
    //            new(OpCodes.Ldloc_3),
    //            new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
    //            new(OpCodes.Dup),
    //            new(OpCodes.Stloc, player.LocalIndex),

    //            // Player is null (Generic static ref hub for example)
    //            new(OpCodes.Brfalse_S, jne),

    //            // Player.role == Tutorial
    //            new(OpCodes.Ldloc, player.LocalIndex),
    //            new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.Role))),
    //            new(OpCodes.Callvirt, PropertyGetter(typeof(API.Features.Roles.Role), nameof(API.Features.Roles.Role.Type))),
    //            new(OpCodes.Ldc_I4_S, (int)RoleType.Tutorial),

    //            // Skip to checking if tutorial should be blocked
    //            new(OpCodes.Beq_S, isTutorial),

    //            // Scp173.TurnedPlayers.Remove(Current 173)
    //            new(OpCodes.Call, PropertyGetter(typeof(API.Features.Scp173), nameof(API.Features.Scp173.TurnedPlayers))),
    //            new(OpCodes.Stloc, turnedPlayers.LocalIndex),
    //            new(OpCodes.Ldloc, turnedPlayers.LocalIndex),
    //            new(OpCodes.Ldarg_0),
    //            new(OpCodes.Ldfld, Field(typeof(PlayableScp), nameof(PlayableScp.Hub))),
    //            new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
    //            new(OpCodes.Callvirt, Method(typeof(HashSet<Player>), nameof(HashSet<Player>.Remove))),

    //            // Scp173.TurnedPlayers.Contains(Current looking player)
    //            new(OpCodes.Ldloc, turnedPlayers.LocalIndex),
    //            new(OpCodes.Ldloc, player.LocalIndex),
    //            new(OpCodes.Callvirt, Method(typeof(HashSet<Player>), nameof(HashSet<Player>.Contains))),

    //            // If true, skip adding to watching
    //            new(OpCodes.Brtrue_S, cnt),

    //            // If the player is tutorial, and allowed to not block, skip adding to watching
    //            new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(Exiled.Events.Events), nameof(Exiled.Events.Events.Instance))).WithLabels(isTutorial),
    //            new(OpCodes.Callvirt, PropertyGetter(typeof(Exiled.Events.Events), nameof(Exiled.Events.Events.Config))),
    //            new(OpCodes.Callvirt, PropertyGetter(typeof(Config), nameof(Exiled.Events.Events.Config.CanTutorialBlockScp173))),
    //            new(OpCodes.Brfalse_S, cnt),
    //        });

    //        int offset = -3;
    //        int defaultLogic = newInstructions.FindIndex(addCheck, instruction => instruction.LoadsField(Field(typeof(PlayableScps.Scp173), nameof(PlayableScps.Scp173._observingPlayers)))) + offset;

    //        int continueBr = newInstructions.FindIndex(addCheck, instruction => instruction.opcode == OpCodes.Br_S);

    //        newInstructions[defaultLogic].labels.Add(jne);
    //        newInstructions[continueBr].labels.Add(cnt);

    //        for (int z = 0; z < newInstructions.Count; z++)
    //            yield return newInstructions[z];

    //        ListPool<CodeInstruction>.Shared.Return(newInstructions);
    //    }
    //}
}
