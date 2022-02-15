// -----------------------------------------------------------------------
// <copyright file="InteractingGeneratorEvents.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.Patches.Events.Player
{
#pragma warning disable SA1313
    using System;

    using SEXiled.Events.EventArgs;
    using SEXiled.Events.Handlers;

    using HarmonyLib;

    using Interactables.Interobjects.DoorUtils;

    using InventorySystem.Items.Keycards;

    using MapGeneration.Distributors;

    /// <summary>
    /// Patches <see cref="Scp079Generator.ServerInteract(ReferenceHub, byte)"/>.
    /// Adds the <see cref="Player.ActivatingGenerator"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp079Generator), nameof(Scp079Generator.ServerInteract))]
    internal static class InteractingGeneratorEvents
    {
        private static bool Prefix(Scp079Generator __instance, ReferenceHub ply, byte colliderId)
        {
            try
            {
                if ((__instance._cooldownStopwatch.IsRunning && __instance._cooldownStopwatch.Elapsed.TotalSeconds <
                    __instance._targetCooldown) || (colliderId != 0 && !__instance.HasFlag(__instance._flags, Scp079Generator.GeneratorFlags.Open)))
                    return false;
                __instance._cooldownStopwatch.Stop();
                switch (colliderId)
                {
                    case 0:
                        if (__instance.HasFlag(__instance._flags, Scp079Generator.GeneratorFlags.Unlocked))
                        {
                            if (__instance.HasFlag(__instance._flags, Scp079Generator.GeneratorFlags.Open))
                            {
                                ClosingGeneratorEventArgs closingGenEvent =
                                    new ClosingGeneratorEventArgs(API.Features.Player.Get(ply), __instance);
                                Player.OnClosingGenerator(closingGenEvent);
                                if (!closingGenEvent.IsAllowed)
                                {
                                    __instance.RpcDenied();
                                    break;
                                }
                            }
                            else
                            {
                                OpeningGeneratorEventArgs openingGenEvent =
                                    new OpeningGeneratorEventArgs(API.Features.Player.Get(ply), __instance);
                                Player.OnOpeningGenerator(openingGenEvent);
                                if (!openingGenEvent.IsAllowed)
                                {
                                    __instance.RpcDenied();
                                    break;
                                }
                            }

                            __instance.ServerSetFlag(Scp079Generator.GeneratorFlags.Open, !__instance.HasFlag(__instance._flags, Scp079Generator.GeneratorFlags.Open));
                            __instance._targetCooldown = __instance._doorToggleCooldownTime;
                            break;
                        }

                        bool flag =
                            (!(ply.inventory.CurInstance != null) ||
                             !(ply.inventory.CurInstance is KeycardItem curInstance2)
                                ? 0
                                : (curInstance2.Permissions.HasFlagFast(__instance._requiredPermission) ? 1 : 0)) != 0;
                        UnlockingGeneratorEventArgs unlockingEvent = new UnlockingGeneratorEventArgs(API.Features.Player.Get(ply), __instance, flag);
                        Player.OnUnlockingGenerator(unlockingEvent);

                        if (unlockingEvent.IsAllowed)
                            __instance.ServerSetFlag(Scp079Generator.GeneratorFlags.Unlocked, true);
                        else
                            __instance.RpcDenied();
                        __instance._targetCooldown = __instance._unlockCooldownTime;
                        break;
                    case 1:
                        if ((ply.characterClassManager.IsHuman() || __instance.Activating) && !__instance.Engaged)
                        {
                            if (__instance.Activating)
                            {
                                StoppingGeneratorEventArgs stoppingGen = new StoppingGeneratorEventArgs(API.Features.Player.Get(ply), __instance);
                                Player.OnStoppingGenerator(stoppingGen);
                                if (!stoppingGen.IsAllowed)
                                {
                                    __instance.RpcDenied();
                                    break;
                                }
                            }
                            else
                            {
                                ActivatingGeneratorEventArgs activatingEvent =
                                    new ActivatingGeneratorEventArgs(API.Features.Player.Get(ply), __instance);
                                Player.OnActivatingGenerator(activatingEvent);
                                if (!activatingEvent.IsAllowed)
                                {
                                    __instance.RpcDenied();
                                    break;
                                }
                            }

                            __instance.Activating = !__instance.Activating;
                            if (__instance.Activating)
                                __instance._leverStopwatch.Restart();
                            __instance._targetCooldown = __instance._doorToggleCooldownTime;
                            break;
                        }

                        break;
                    case 2:
                        if (__instance.Activating && !__instance.Engaged)
                        {
                            StoppingGeneratorEventArgs stoppingGen = new StoppingGeneratorEventArgs(API.Features.Player.Get(ply), __instance);
                            Player.OnStoppingGenerator(stoppingGen);
                            if (!stoppingGen.IsAllowed)
                            {
                                __instance.RpcDenied();
                                break;
                            }

                            __instance.ServerSetFlag(Scp079Generator.GeneratorFlags.Activating, false);
                            __instance._targetCooldown = __instance._unlockCooldownTime;
                            break;
                        }

                        break;
                    default:
                        __instance._targetCooldown = 1f;
                        break;
                }

                __instance._cooldownStopwatch.Restart();
            }
            catch (Exception exception)
            {
                API.Features.Log.Error(
                    $"SEXiled.Events.Patches.Events.Player.InteractingGeneratorEvent: {exception}\n{exception.StackTrace}");

                return true;
            }

            return false;
        }
    }
}
