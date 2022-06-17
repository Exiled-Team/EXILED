// -----------------------------------------------------------------------
// <copyright file="ExplodingFlashGrenade.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

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

            if (Exiled.Events.Events.Instance.Config.CanFlashBangsAffectThrower)
            {
                // Remove check if player is thrower. Grenade on self should affect themself.
                int removeSelfCheckOffset = -4;
                int removeSelfCheck = newInstructions.FindIndex(instruction => instruction.LoadsField(Field(typeof(Footprint), nameof(Footprint.Hub)))) + removeSelfCheckOffset;
                newInstructions.RemoveRange(removeSelfCheck, 7);
            }

            int offset = -3;
            int index = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Call && (MethodInfo)i.operand == Method(typeof(FlashbangGrenade), nameof(FlashbangGrenade.ProcessPlayer))) + offset;
            Label returnLabel = generator.DefineLabel();
            LocalBuilder ev = generator.DeclareLocal(typeof(ExplodingGrenadeEventArgs));
            LocalBuilder list = generator.DeclareLocal(typeof(List<ReferenceHub>));
            int instructionsToRemove = 4;
            for (int i = 0; i < instructionsToRemove; i++)
                newInstructions.RemoveAt(index);

            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                // list = ListPool<ReferenceHub>.Shared.Rent();
                new(OpCodes.Ldsfld, Field(typeof(ListPool<ReferenceHub>), nameof(ListPool<ReferenceHub>.Shared))),
                new(OpCodes.Callvirt, Method(typeof(ListPool<ReferenceHub>), nameof(ListPool<ReferenceHub>.Shared.Rent))),
                new(OpCodes.Stloc, list.LocalIndex),
            });

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // list.Add(allHub.Value)
                new(OpCodes.Ldloc, list.LocalIndex),
                new(OpCodes.Ldloca_S, 2),
                new(OpCodes.Call, PropertyGetter(typeof(KeyValuePair<GameObject, ReferenceHub>), nameof(KeyValuePair<GameObject, ReferenceHub>.Value))),
                new(OpCodes.Callvirt, Method(typeof(List<ReferenceHub>), nameof(List<ReferenceHub>.Add))),
            });

            newInstructions.InsertRange(newInstructions.Count - 1, new[]
            {
                // Player player = Player.Get(this.PreviousOwner.Hub)
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[newInstructions.Count - 1]),
                new(OpCodes.Ldfld, Field(typeof(FlashbangGrenade), nameof(FlashbangGrenade.PreviousOwner))),
                new(OpCodes.Ldfld, Field(typeof(Footprint), nameof(Footprint.Hub))),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // this
                new(OpCodes.Ldarg_0),

                // List<Player> players = ConvertHubs(list);
                new(OpCodes.Ldloc, list.LocalIndex),
                new(OpCodes.Call, Method(typeof(ExplodingFlashGrenade), nameof(ConvertHubs))),

                // var ev = new ExplodingGrenadeEventArgs(player, this, players);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ExplodingGrenadeEventArgs))[1]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc, ev.LocalIndex),

                // Handlers.Map.OnExplodingGrenade(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Map), nameof(Handlers.Map.OnExplodingGrenade))),

                // if (!ev.IsAllowed)
                //    return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(ExplodingGrenadeEventArgs), nameof(ExplodingGrenadeEventArgs.IsAllowed))),
                new(OpCodes.Brfalse, returnLabel),

                // ProcessPlayers(this, ev.TargetsToAffect);
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldloc, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ExplodingGrenadeEventArgs), nameof(ExplodingGrenadeEventArgs.TargetsToAffect))),
                new(OpCodes.Call, Method(typeof(ExplodingFlashGrenade), nameof(ProcessPlayers))),

                // ListPool<ReferenceHub>.Shared.Return(list);
                new(OpCodes.Ldsfld, Field(typeof(ListPool<ReferenceHub>), nameof(ListPool<ReferenceHub>.Shared))),
                new(OpCodes.Ldloc, list.LocalIndex),
                new(OpCodes.Callvirt, Method(typeof(ListPool<ReferenceHub>), nameof(ListPool<ReferenceHub>.Shared.Return))),

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
                if(Exiled.Events.Events.Instance.Config.CanFlashBangsAffectThrower && Player.Get(grenade.PreviousOwner.Hub) == player)
                {
                    grenade.ProcessPlayer(player.ReferenceHub);
                }
                else if (HitboxIdentity.CheckFriendlyFire(grenade.PreviousOwner.Role, player.ReferenceHub.characterClassManager.CurClass))
                {
                    grenade.ProcessPlayer(player.ReferenceHub);
                }
            }
        }
    }
}
