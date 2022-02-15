// -----------------------------------------------------------------------
// <copyright file="ExplodingFlashGrenade.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.Patches.Events.Map
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    using SEXiled.API.Features;
    using SEXiled.Events.EventArgs;

    using Footprinting;

    using HarmonyLib;

    using InventorySystem.Items.ThrowableProjectiles;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="FlashbangGrenade.PlayExplosionEffects()"/>.
    /// Adds the <see cref="Handlers.Map.OnExplodingGrenade"/> event.
    /// </summary>
    [HarmonyPatch(typeof(FlashbangGrenade), nameof(FlashbangGrenade.PlayExplosionEffects))]
    internal static class ExplodingFlashGrenade
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);
            int offset = -3;
            int index = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Call && (MethodInfo)i.operand == Method(typeof(FlashbangGrenade), nameof(FlashbangGrenade.ProcessPlayer))) + offset;
            Label returnLabel = generator.DefineLabel();
            LocalBuilder ev = generator.DeclareLocal(typeof(ExplodingGrenadeEventArgs));
            LocalBuilder list = generator.DeclareLocal(typeof(List<ReferenceHub>));
            int instructionsToRemove = 4;
            for (int i = 0; i < instructionsToRemove; i++)
                newInstructions.RemoveAt(index);

            newInstructions.InsertRange(0, new[]
            {
                // list = ListPool<ReferenceHub>.Shared.Rent();
                new CodeInstruction(OpCodes.Ldsfld, Field(typeof(ListPool<ReferenceHub>), nameof(ListPool<ReferenceHub>.Shared))),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(ListPool<ReferenceHub>), nameof(ListPool<ReferenceHub>.Shared.Rent))),
                new CodeInstruction(OpCodes.Stloc, list.LocalIndex),
            });

            newInstructions.InsertRange(index, new[]
            {
                // list.Add(allHub.Value)
                new CodeInstruction(OpCodes.Ldloc, list.LocalIndex),
                new CodeInstruction(OpCodes.Ldloca_S, 2),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(KeyValuePair<GameObject, ReferenceHub>), nameof(KeyValuePair<GameObject, ReferenceHub>.Value))),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(List<ReferenceHub>), nameof(List<ReferenceHub>.Add))),
            });

            newInstructions.InsertRange(newInstructions.Count - 1, new[]
            {
                // Player player = Player.Get(this.PreviousOwner.Hub)
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[newInstructions.Count - 1]),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(FlashbangGrenade), nameof(FlashbangGrenade.PreviousOwner))),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Footprint), nameof(Footprint.Hub))),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // this
                new CodeInstruction(OpCodes.Ldarg_0),

                // List<Player> players = ConvertHubs(list);
                new CodeInstruction(OpCodes.Ldloc, list.LocalIndex),
                new CodeInstruction(OpCodes.Call, Method(typeof(ExplodingFlashGrenade), nameof(ConvertHubs))),

                // var ev = new ExplodingGrenadeEventArgs(player, this, players);
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(ExplodingGrenadeEventArgs))[1]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc, ev.LocalIndex),

                // Handlers.Map.OnExplodingGrenade(ev);
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Map), nameof(Handlers.Map.OnExplodingGrenade))),

                // if (!ev.IsAllowed)
                //    return;
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ExplodingGrenadeEventArgs), nameof(ExplodingGrenadeEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse, returnLabel),

                // ProcessPlayers(this, ev.TargetsToAffect);
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldloc, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ExplodingGrenadeEventArgs), nameof(ExplodingGrenadeEventArgs.TargetsToAffect))),
                new CodeInstruction(OpCodes.Call, Method(typeof(ExplodingFlashGrenade), nameof(ProcessPlayers))),

                // ListPool<ReferenceHub>.Shared.Return(list);
                new CodeInstruction(OpCodes.Ldsfld, Field(typeof(ListPool<ReferenceHub>), nameof(ListPool<ReferenceHub>.Shared))),
                new CodeInstruction(OpCodes.Ldloc, list.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(ListPool<ReferenceHub>), nameof(ListPool<ReferenceHub>.Shared.Return))),

                // return;
                new CodeInstruction(OpCodes.Ret).WithLabels(returnLabel),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        private static List<Player> ConvertHubs(List<ReferenceHub> hubs) => hubs.Select(Player.Get).ToList();

        private static void ProcessPlayers(FlashbangGrenade grenade, List<Player> players)
        {
            foreach (Player player in players)
            {
                if (HitboxIdentity.CheckFriendlyFire(grenade.PreviousOwner.Role, player.ReferenceHub.characterClassManager.CurClass))
                {
                    grenade.ProcessPlayer(player.ReferenceHub);
                }
            }
        }
    }
}
