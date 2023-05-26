// -----------------------------------------------------------------------
// <copyright file="ActivatingSense.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp049
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Pools;
    using Exiled.API.Features.Roles;
    using Exiled.Events.EventArgs.Scp049;
    using Exiled.Events.Handlers;
    using HarmonyLib;

    using PlayerRoles;
    using PlayerRoles.PlayableScps.Scp049;
    using PlayerRoles.PlayableScps.Subroutines;

    using static HarmonyLib.AccessTools;

    using ExiledEvents = Exiled.Events.Events;
    using Player = API.Features.Player;
    using Scp049Role = Exiled.API.Features.Roles.Scp049Role;

    /// <summary>
    ///     Patches <see cref="Scp049SenseAbility.ServerProcessCmd" />.
    ///     Adds the <see cref="Handlers.Scp049.ActivatingSense" /> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp049SenseAbility), nameof(Scp049SenseAbility.ServerProcessCmd))]
    internal static class ActivatingSense
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder targetPlayer = generator.DeclareLocal(typeof(Player));
            LocalBuilder targetRefereceHub = generator.DeclareLocal(typeof(ReferenceHub));

            LocalBuilder ev = generator.DeclareLocal(typeof(ActivatingSenseEventArgs));

            Label skipLabel = generator.DefineLabel();
            Label nextCheck = generator.DefineLabel();
            Label newPlayerLabel = generator.DefineLabel();
            Label returnLabel = generator.DefineLabel();

            int offset = 1;
            int index = newInstructions.FindIndex(x => x.operand == (object)PropertySetter(typeof(Scp049SenseAbility), nameof(Scp049SenseAbility.Target))) + offset;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // if ((Player player = Player.Get(this.Target) is null) GOTO skipLabel;
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Scp049SenseAbility), nameof(Scp049SenseAbility.Target))),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, targetPlayer.LocalIndex),
                new(OpCodes.Brfalse, skipLabel),

                // if (player.Role.Type != RoleTypeId.Tutorial) GOTO NextCheck;
                new(OpCodes.Ldloc_S, targetPlayer.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.Role))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Role), nameof(Role.Type))),
                new(OpCodes.Ldc_I4_S, (int)RoleTypeId.Tutorial),
                new(OpCodes.Bne_Un_S, nextCheck),

                // if (ExiledEvents.Instance.Config.CanScp049SenseTutorial) GOTO returnLabel;
                new(OpCodes.Call, PropertyGetter(typeof(ExiledEvents), nameof(ExiledEvents.Instance))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ExiledEvents), nameof(ExiledEvents.Config))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Config), nameof(Config.CanScp049SenseTutorial))),
                new(OpCodes.Brfalse_S, newPlayerLabel),

                // if (Scp049Role.TurnedPlayers.Contains(player)) GOTO returnLabel;
                new CodeInstruction(OpCodes.Call, Method(typeof(Scp049Role), nameof(Scp049Role.TurnedPlayers))).WithLabels(nextCheck),
                new(OpCodes.Ldloc_S, targetPlayer.LocalIndex),
                new(OpCodes.Callvirt, Method(typeof(HashSet<Player>), nameof(HashSet<Player>.Contains))),
                new(OpCodes.Brtrue_S, skipLabel),

                // Scp049SenseAbility.CanFindTarget(out targetRefereceHub)
                new CodeInstruction(OpCodes.Ldarg_0).WithLabels(newPlayerLabel),
                new(OpCodes.Stloc_S, targetRefereceHub.LocalIndex),
                new(OpCodes.Callvirt, Method(typeof(Scp049SenseAbility), nameof(Scp049SenseAbility.CanFindTarget))),

                // this.Target = targetRefereceHub;
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldloc_S, targetRefereceHub.LocalIndex),
                new(OpCodes.Callvirt, PropertySetter(typeof(Scp049SenseAbility), nameof(Scp049SenseAbility.Target))),

                // Event Section
                // this.Owner
                new CodeInstruction(OpCodes.Ldarg_0).WithLabels(skipLabel),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Scp049SenseAbility), nameof(Scp049SenseAbility.Owner))),

                // this.Target
                new CodeInstruction(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Scp049SenseAbility), nameof(Scp049SenseAbility.Target))),

                // true
                new(OpCodes.Ldc_I4_1),

                // ActivatingSenseEventArgs ev = new(...)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ActivatingSenseEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev.LocalIndex),

                // Handlers.Scp049.OnActivatingSense(ev);
                new(OpCodes.Call, Method(typeof(Scp049), nameof(Scp049.OnActivatingSense))),

                // if (!ev.IsAllowed)
                //     return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(ActivatingSenseEventArgs), nameof(ActivatingSenseEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, returnLabel),

                // this.Target = ev.Target;
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ActivatingSenseEventArgs), nameof(ActivatingSenseEventArgs.Target))),
                new(OpCodes.Callvirt, PropertySetter(typeof(Scp049SenseAbility), nameof(Scp049SenseAbility.Target))),
            });

            offset = -1;
            index = newInstructions.FindIndex(x => x.operand == (object)Method(typeof(AbilityCooldown), nameof(AbilityCooldown.Trigger))) + offset;
            newInstructions.RemoveAt(index);
            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ActivatingSenseEventArgs), nameof(ActivatingSenseEventArgs.Cooldown))),
            });

            offset = -1;
            index = newInstructions.FindLastIndex(x => x.operand == (object)Method(typeof(AbilityCooldown), nameof(AbilityCooldown.Trigger))) + offset;
            newInstructions.RemoveAt(index);
            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ActivatingSenseEventArgs), nameof(ActivatingSenseEventArgs.Duration))),
            });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}