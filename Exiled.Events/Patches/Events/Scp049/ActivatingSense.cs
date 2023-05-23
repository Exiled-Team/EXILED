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

    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.API.Features.Roles;
    using Exiled.Events.EventArgs.Scp049;
    using HarmonyLib;
    using PlayerRoles.PlayableScps.Scp049;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="Scp049SenseAbility.ServerProcessCmd" />.
    ///     Adds the <see cref="Handlers.Scp049.ActivatingSense" /> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp049SenseAbility), nameof(Scp049SenseAbility.ServerProcessCmd))]
    public class ActivatingSense
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label continueLabel = generator.DefineLabel();
            Label evLabel = generator.DefineLabel();
            Label turnedCheck = generator.DefineLabel();
            Label cantSense = generator.DefineLabel();

            LocalBuilder target = generator.DeclareLocal(typeof(Player));

            int offset = 3;
            int index = newInstructions.FindIndex(x => x.opcode == OpCodes.Ldnull) + offset;

            newInstructions[index].labels.Add(cantSense);

            // Player target = Player.Get(this.Target);
            //
            // if ((player.Role.Type == RoleType.Tutorial && !Exiled.Events.Events.Instance.Config.CanScp049SenseTutorial) || Exiled.API.Features.Roles.Scp049Role.TurnedPlayers.Contains(target))
            //      return;
            newInstructions.InsertRange(
                index,
                new[]
                {
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Scp049SenseAbility), nameof(Scp049SenseAbility.Target))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, target.LocalIndex),

                    new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.Role))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Role), nameof(Role.Type))),
                    new(OpCodes.Ldc_I4_S, 14),
                    new(OpCodes.Bne_Un_S, turnedCheck),

                    new(OpCodes.Callvirt, PropertyGetter(typeof(Exiled.Events.Events), nameof(Exiled.Events.Events.Instance))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Exiled.Events.Events), nameof(Exiled.Events.Events.Config))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Config), nameof(Config.CanScp049SenseTutorial))),
                    new(OpCodes.Brfalse_S, cantSense),

                    new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(API.Features.Roles.Scp049Role), nameof(API.Features.Roles.Scp049Role.TurnedPlayers))).WithLabels(turnedCheck),
                    new(OpCodes.Ldloc_S, target.LocalIndex),
                    new(OpCodes.Call, Method(typeof(HashSet<Player>), nameof(HashSet<Player>.Contains))),
                    new(OpCodes.Brfalse_S, evLabel),
                });

            offset = -2;
            index = newInstructions.FindIndex(x => x.Is(OpCodes.Ldfld, Field(typeof(ReferenceHub), nameof(ReferenceHub.roleManager)))) + offset;

            newInstructions[index].labels.Add(continueLabel);

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player player = Player.Get(this.Owner);
                    new CodeInstruction(OpCodes.Ldarg_0).WithLabels(evLabel),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Scp049SenseAbility), nameof(Scp049SenseAbility.Owner))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // target
                    new(OpCodes.Ldloc_S, target.LocalIndex),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // ActivatingSenseEventArgs ev = new(player, target, true);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ActivatingSenseEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Scp049.OnActivatingSense(ev);
                    new(OpCodes.Call, Method(typeof(Handlers.Scp049), nameof(Handlers.Scp049.OnActivatingSense))),

                    // if (!ev.IsAllowed)
                    //      return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ActivatingSenseEventArgs), nameof(ActivatingSenseEventArgs.IsAllowed))),
                    new(OpCodes.Brtrue_S, continueLabel),

                    new(OpCodes.Ret),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
