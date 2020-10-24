// -----------------------------------------------------------------------
// <copyright file="PickingUpScp330.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1118
#pragma warning disable SA1313
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using SCPSL.Halloween;

    /// <summary>
    /// Patches <see cref="SCPSL.Halloween.Scp330.ServerInteract(ReferenceHub, byte)"/>.
    /// Adds the <see cref="Handlers.Player.PickingUpScp330"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp330), nameof(Scp330.ServerInteract))]
    internal static class PickingUpScp330
    {
        private static bool Prefix(Scp330 __instance, ReferenceHub ply, byte colliderId)
        {
            try
            {
                if (ply.characterClassManager.IsAnyScp())
                {
                    return false;
                }

                if (ply.inventory.IsFull)
                {
                    return false;
                }

                Scp330.Usage usage;
                if (!__instance._usages.TryGetValue(ply.playerId, out usage))
                {
                    usage = new Scp330.Usage
                    {
                        Role = ply.characterClassManager.CurClass,
                        Uses = 0,
                    };
                }

                if (usage.Severed)
                {
                    return false;
                }

                Player p = Player.Get(ply);

                int index = UnityEngine.Random.Range(0, __instance._candyHints.Keys.Count);
                KeyValuePair<ItemType, string> keyValuePair = __instance._candyHints.ElementAt(index);

                PickingUpScp330EventArgs ev = new PickingUpScp330EventArgs(p, usage.Uses + 1, keyValuePair.Key, true);

                Handlers.Player.OnPickingUpScp330(ev);

                if (!ev.IsAllowed)
                {
                    return false;
                }

                usage.Uses = ev.Usage;

                if ((usage.Uses > 2 && ev.IsSevere) || ev.IsSevere)
                {
                    usage.Severed = true;
                    ply.GetComponent<ConsumableAndWearableItems>().CompleteCancelUsage();
                    if (ply.inventory.curItem != ItemType.None)
                    {
                        ply.inventory.DropCurrentItem();
                    }

                    ply.playerEffectsController.EnableEffect<CustomPlayerEffects.Amnesia>(0f, false);
                    ply.playerEffectsController.EnableEffect<CustomPlayerEffects.Exsanguination>(0f, false);
                    ply.playerEffectsController.EnableEffect<CustomPlayerEffects.Hemorrhage>(0f, false);
                    ply.playerEffectsController.EnableEffect<CustomPlayerEffects.Disarmed>(0f, false);
                    ply.hints.Show(new Hints.TextHint(
                    "Both of your hands sever at the wrists and fall to the floor.",
                    new Hints.HintParameter[] { new Hints.StringHintParameter(string.Empty) },
                    new Hints.HintEffect[] { Hints.HintEffectPresets.FadeOut(0.5f, 2f, 1f) },
                    5f));
                    if (__instance._handLossAudio != null)
                    {
                        __instance.RpcRemoveHands(ply.playerId);
                    }

                    ply.characterClassManager.RpcPlaceBlood(ply.transform.position, 0, 3f);
                    __instance.SpawnHands(ply);
                }
                else
                {
                    ply.inventory.AddNewItem(ev.ItemId, -4.65664672E+11f, 0, 0, 0);
                    if (ev.ItemId.IsSCP330())
                    {
                        string hint = __instance._candyHints[ev.ItemId];
                        ply.hints.Show(
                        new Hints.TextHint(
                        hint,
                        new Hints.HintParameter[] { new Hints.StringHintParameter(string.Empty), },
                        new Hints.HintEffect[] { Hints.HintEffectPresets.FadeOut(0.5f, 2f, 1f) },
                        5f));
                    }
                }

                __instance._usages[ply.playerId] = usage;
                return false;
            }
            catch (Exception e)
            {
                Exiled.API.Features.Log.Error($"Exiled.Events.Patches.Events.Player.PickingUpScp330: {e}\n{e.StackTrace}");

                return true;
            }
        }
    }
}
