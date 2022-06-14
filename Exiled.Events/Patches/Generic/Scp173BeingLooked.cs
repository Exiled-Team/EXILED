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

    ///// <summary>
    ///// Patches <see cref="PlayableScps.Scp173.UpdateObservers"/>.
    ///// </summary>
    // [HarmonyPatch(typeof(PlayableScps.Scp173), nameof(PlayableScps.Scp173.UpdateObservers))]
    // internal static class Scp173BeingLookedPrefix
    // {
    //    [HarmonyPrefix]
    //    private static bool ProcessObservers(PlayableScps.Scp173 __instance)
    //    {
    //        try
    //        {
    //            HashSet<uint> hashSet = HashSetPool<uint>.Shared.Rent();
    //            int count = __instance._observingPlayers.Count;
    //            Player scp173Player = Player.Get(__instance.Hub);
    //            API.Features.Scp173.TurnedPlayers.Remove(scp173Player);
    //            foreach (KeyValuePair<GameObject, ReferenceHub> pairedData in ReferenceHub.GetAllHubs())
    //            {
    //                ReferenceHub currentPlayerReferenceHub = pairedData.Value;
    //                CharacterClassManager characterClassManager = currentPlayerReferenceHub.characterClassManager;
    //                if (characterClassManager.CurClass == RoleType.Spectator || currentPlayerReferenceHub == __instance.Hub || characterClassManager.IsAnyScp())
    //                {
    //                    if (__instance._observingPlayers.Contains(currentPlayerReferenceHub))
    //                    {
    //                        __instance._observingPlayers.Remove(currentPlayerReferenceHub);
    //                    }
    //                }
    //                else
    //                {
    //                    Vector3 realModelPosition = __instance.Hub.playerMovementSync.RealModelPosition;
    //                    bool flag = false;
    //                    Player player = Player.Get(pairedData.Key);
    //                    if (player != null)
    //                    {
    //                        if (player?.Role?.Type == RoleType.Tutorial)
    //                        {
    //                            if (!Exiled.Events.Events.Instance?.Config?.CanTutorialBlockScp173 ?? false)
    //                            {
    //                                if (__instance._observingPlayers.Contains(currentPlayerReferenceHub))
    //                                {
    //                                    __instance._observingPlayers.Remove(currentPlayerReferenceHub);
    //                                }
    //                                continue;
    //                            }
    //                        }
    //                        else if (API.Features.Scp173.TurnedPlayers.Contains(player))
    //                        {
    //                            if (__instance._observingPlayers.Contains(currentPlayerReferenceHub))
    //                            {
    //                                __instance._observingPlayers.Remove(currentPlayerReferenceHub);
    //                            }

    // continue;
    //                        }
    //                    }

    // RoomIdentifier roomIdentifier = RoomIdUtils.RoomAtPosition(__instance.Hub.playerMovementSync.RealModelPosition);
    //                    if (VisionInformation.GetVisionInformation(currentPlayerReferenceHub, realModelPosition, -2f, (roomIdentifier != null && roomIdentifier.Zone == FacilityZone.Surface) ? 80f : 40f, false, false, __instance.Hub.localCurrentRoomEffects, 0).IsLooking && (!Physics.Linecast(realModelPosition + new Vector3(0f, 1.5f, 0f), currentPlayerReferenceHub.PlayerCameraReference.position, VisionInformation.VisionLayerMask) || !Physics.Linecast(realModelPosition + new Vector3(0f, -1f, 0f), currentPlayerReferenceHub.PlayerCameraReference.position, VisionInformation.VisionLayerMask)))
    //                    {
    //                        flag = true;
    //                    }

    // if (flag)
    //                    {
    //                        if (!__instance._observingPlayers.Contains(currentPlayerReferenceHub))
    //                        {
    //                            __instance._observingPlayers.Add(currentPlayerReferenceHub);
    //                        }
    //                    }
    //                    else if (__instance._observingPlayers.Contains(currentPlayerReferenceHub))
    //                    {
    //                        __instance._observingPlayers.Remove(currentPlayerReferenceHub);
    //                    }
    //                }
    //            }

    // __instance._isObserved = __instance._observingPlayers.Count > 0 || __instance.StareAtDuration > 0f;
    //            if (count != __instance._observingPlayers.Count && __instance._blinkCooldownRemaining > 0f)
    //            {
    //                GameCore.Console.AddDebugLog("SCP173", string.Format("Adjusting blink cooldown. Initial observers: {0}. ", count) + string.Format("New observers: {0}.", __instance._observingPlayers.Count), MessageImportance.LessImportant, false);
    //                GameCore.Console.AddDebugLog("SCP173", string.Format("Current blink cooldown: {0}", __instance._blinkCooldownRemaining), MessageImportance.LeastImportant, false);
    //                __instance._blinkCooldownRemaining = Mathf.Max(0f, __instance._blinkCooldownRemaining + ((__instance._observingPlayers.Count - count) * (__instance.BreakneckSpeedsActive ? 0f : 0f)));
    //                GameCore.Console.AddDebugLog("SCP173", string.Format("New blink cooldown: {0}", __instance._blinkCooldownRemaining), MessageImportance.LeastImportant, false);
    //                if (__instance._blinkCooldownRemaining <= 0f)
    //                {
    //                    __instance.BlinkReady = true;
    //                }
    //            }
    //            HashSetPool<uint>.Shared.Return(hashSet);
    //            return false;
    //        }
    //        catch (Exception generic)
    //        {
    //            Log.Error($"Unable to do Scp173BeingLooked {generic}\n");
    //        }

    // return true;
    //    }
    // }

    /// <summary>
    /// Patches <see cref="PlayableScps.Scp173.UpdateObservers"/>.
    /// </summary>
    [HarmonyPatch(typeof(PlayableScps.Scp173), nameof(PlayableScps.Scp173.UpdateObservers))]
    internal static class Scp173BeingLooked
    {
        /// <summary>
        /// Checks if the current player is to be skipped.
        /// </summary>
        /// <param name="instance"> Scp173 instance <see cref="PlayableScps.Scp173"/>. </param>
        /// <param name="curPlayerGameObj"> Current player game object. </param>
        /// <param name="curPlayerHub"> Current player referencehub. </param>
        /// <returns> True if to be skipped, false if not. </returns>
        public static bool SkipPlayer(ref PlayableScps.Scp173 instance, GameObject curPlayerGameObj, ReferenceHub curPlayerHub)
        {
            Player player = Player.Get(curPlayerGameObj);
            if (player is not null)
            {
                if (player?.Role?.Type == RoleType.Tutorial)
                {
                    if (!Exiled.Events.Events.Instance?.Config?.CanTutorialBlockScp173 ?? false)
                    {
                        instance._observingPlayers.Remove(curPlayerHub);
                        return true;
                    }
                }
                else if (API.Features.Scp173.TurnedPlayers.Contains(player))
                {
                    instance._observingPlayers.Remove(curPlayerHub);
                    return true;
                }
            }

            return false;
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label jne = generator.DefineLabel();
            Label cnt = generator.DefineLabel();
            Label isTutorial = generator.DefineLabel();

            LocalBuilder currentPlayer = generator.DeclareLocal(typeof(Player));
            LocalBuilder scp173Player = generator.DeclareLocal(typeof(Player));
            LocalBuilder turnedPlayers = generator.DeclareLocal(typeof(HashSet<Player>));

            int removeTurnedPeanutOffset = 2;
            int removeTurnedPeanut = newInstructions.FindIndex(instruction => instruction.Calls(PropertyGetter(typeof(HashSet<ReferenceHub>), nameof(HashSet<ReferenceHub>.Count)))) + removeTurnedPeanutOffset;

            newInstructions.InsertRange(removeTurnedPeanut, new CodeInstruction[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(PlayableScp), nameof(PlayableScp.Hub))),
                new(OpCodes.Callvirt, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Stloc, scp173Player.LocalIndex),
                new(OpCodes.Call, PropertyGetter(typeof(API.Features.Scp173), nameof(API.Features.Scp173.TurnedPlayers))),
                new(OpCodes.Ldloc, scp173Player.LocalIndex),
                new(OpCodes.Callvirt, Method(typeof(HashSet<Player>), nameof(HashSet<Player>.Remove))),
                new(OpCodes.Pop),
            });

            int skipPlayerCheckOffset = 2;
            int skipPlayerCheck = newInstructions.FindIndex(instruction => instruction.Calls(PropertyGetter(typeof(PlayerMovementSync), nameof(PlayerMovementSync.RealModelPosition)))) + skipPlayerCheckOffset;

            newInstructions.InsertRange(skipPlayerCheck, new CodeInstruction[]
            {
                new(OpCodes.Ldarga, 0),
                new(OpCodes.Ldloca_S, 2),
                new(OpCodes.Callvirt, PropertyGetter(typeof(KeyValuePair<GameObject, ReferenceHub>), nameof(KeyValuePair<GameObject, ReferenceHub>.Key))),
                new(OpCodes.Ldloc_3),
                new(OpCodes.Call, Method(typeof(Scp173BeingLooked), nameof(Scp173BeingLooked.SkipPlayer), new[] { typeof(API.Features.Scp173).MakeByRefType(), typeof(GameObject), typeof(ReferenceHub) })),

                // If true, skip adding to watching
                new(OpCodes.Brtrue, cnt),
            });

            int continueBr = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Br);

            newInstructions[continueBr].labels.Add(cnt);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
