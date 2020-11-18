// -----------------------------------------------------------------------
// <copyright file="ExplodingFragGrenade.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
#pragma warning disable SA1313
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    using CustomPlayerEffects;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using GameCore;

    using Grenades;

    using HarmonyLib;

    using UnityEngine;

    /// <summary>
    /// Patches <see cref="FragGrenade.ServersideExplosion()"/>.
    /// Adds the <see cref="Handlers.Map.OnExplodingGrenade"/> event.
    /// </summary>
    [HarmonyPatch(typeof(FragGrenade), nameof(FragGrenade.ServersideExplosion))]
    internal static class ExplodingFragGrenade
    {
        private static bool Prefix(FragGrenade __instance)
        {
            try
            {
                Player thrower = Player.Get(__instance.thrower.gameObject);

                Dictionary<Player, float> players = new Dictionary<Player, float>();

                Vector3 position = ((EffectGrenade)__instance).transform.position;

                float humanMultiplier = ConfigFile.ServerConfig.GetFloat("human_grenade_multiplier", 0.7f);
                float scpMultiplier = ConfigFile.ServerConfig.GetFloat("scp_grenade_multiplier", 1f);

                foreach (GameObject obj2 in PlayerManager.players)
                {
                    if (ServerConsole.FriendlyFire || obj2 == __instance.thrower.gameObject ||
                        obj2.GetComponent<WeaponManager>().GetShootPermission(__instance.throwerTeam, false))
                    {
                        PlayerStats component = obj2.GetComponent<PlayerStats>();
                        if ((component != null) && component.ccm.InWorld)
                        {
                            players.Add(Player.Get(obj2), (float)(__instance.damageOverDistance.Evaluate(Vector3.Distance(position, component.transform.position)) * (component.ccm.IsHuman() ? humanMultiplier : scpMultiplier)));
                        }
                    }
                }

                var ev = new ExplodingGrenadeEventArgs(thrower, players, true, __instance.gameObject);

                Handlers.Map.OnExplodingGrenade(ev);

                return ev.IsAllowed;
            }
            catch (Exception exception)
            {
                API.Features.Log.Error($"Exiled.Events.Patches.Events.Map.ExplodingFragGrenade: {exception}\n{exception.StackTrace}");

            ///////////////////////////////////////////////

            // Get the index of the penultimate instruction;
            var index = newInstructions.Count - 2;

            // Get the count to find the previous index
            var oldCount = newInstructions.Count;

            // var ev = new ExplodingGrenadeEventArgs(players, true, grenadeGameObject, true);
            //
            // Handlers.Map.OnExplodingGrenade(ev);
            //
            // if (!ev.IsAllowed)
            //   return result;
            //
            // foreach (var player in players)
            // {
            //   foreachBody;
            //   hurtPlayerInstructions;
            //   enableEffectsInstructions;
            // }
            var explodingGrenadeEvent = new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Grenade), nameof(Grenade.throwerGameObject))),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(GameObject) })),
                new CodeInstruction(OpCodes.Ldloc_S, players.LocalIndex),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Component), nameof(Component.gameObject))),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(ExplodingGrenadeEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Map), nameof(Handlers.Map.OnExplodingGrenade))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ExplodingGrenadeEventArgs), nameof(ExplodingGrenadeEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse_S, returnLabel),
            };

            var foreachStart = new[]
            {
                new CodeInstruction(OpCodes.Ldloc_S, players.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(Dictionary<Player, float>), nameof(Dictionary<Player, float>.GetEnumerator))),
                new CodeInstruction(OpCodes.Stloc_S, playerEnumerator.LocalIndex),

                new CodeInstruction(OpCodes.Br_S, foreachFirstLabel).WithBlocks(new ExceptionBlock(ExceptionBlockType.BeginExceptionBlock)),
                new CodeInstruction(OpCodes.Ldloca_S, playerEnumerator.LocalIndex).WithLabels(foreachSecondLabel),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(Dictionary<Player, float>.Enumerator), nameof(Dictionary<Player, float>.Enumerator.Current))),
                new CodeInstruction(OpCodes.Stloc_S, playerKeyValuePair.LocalIndex),
            };

            // Fill local variables of hurtPlayerInstructions
            //
            // damage = playerKeyValuePair.Value;
            // playerStats = playerKeyValuePair.Key.ReferenceHub.playerStats;
            // gameObject = playerKeyValuePair.Key.ReferenceHub.gameObject;
            var foreachBody = new[]
            {
                new CodeInstruction(OpCodes.Ldloca_S, playerKeyValuePair.LocalIndex),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(KeyValuePair<Player, float>), nameof(KeyValuePair<Player, float>.Value))),
                new CodeInstruction(OpCodes.Stloc_S, 13),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(KeyValuePair<Player, float>), nameof(KeyValuePair<Player, float>.Key))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.ReferenceHub))),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(ReferenceHub), nameof(ReferenceHub.playerStats))),
                new CodeInstruction(OpCodes.Stloc_S, 12),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Component), nameof(Component.gameObject))),
                new CodeInstruction(OpCodes.Stloc_S, 11),
            };

            var foreachEnd = new[]
            {
                new CodeInstruction(OpCodes.Ldloca_S, playerEnumerator.LocalIndex).WithLabels(foreachFirstLabel),
                new CodeInstruction(OpCodes.Call, Method(typeof(Dictionary<Player, float>.Enumerator), nameof(Dictionary<Player, float>.Enumerator.MoveNext))),
                new CodeInstruction(OpCodes.Brtrue_S, foreachSecondLabel),
                new CodeInstruction(OpCodes.Leave_S, returnLabel),

                // --- Clean up ---
                new CodeInstruction(OpCodes.Ldloca_S, playerEnumerator.LocalIndex).WithBlocks(new ExceptionBlock(ExceptionBlockType.BeginFinallyBlock)),
                new CodeInstruction(OpCodes.Constrained, typeof(Dictionary<Player, float>.Enumerator)),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(IDisposable), nameof(IDisposable.Dispose))),
                new CodeInstruction(OpCodes.Endfinally).WithBlocks(new ExceptionBlock(ExceptionBlockType.EndExceptionBlock)),
            };

            // Insert all instructions.
            newInstructions.InsertRange(index, explodingGrenadeEvent
                .Concat(foreachStart)
                .Concat(foreachBody)
                .Concat(hurtPlayerInstructions)
                .Concat(enableEffectsInstructions)
                .Concat(foreachEnd));

            // Add the starting labels to the first injected instruction.
            // Calculate the difference and get the valid index - is better and easy than using a list
            newInstructions[index].MoveLabelsFrom(newInstructions[newInstructions.Count - oldCount + index]);

            // Add the return label to the penultimate instruction.
            newInstructions[newInstructions.Count - 2].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
