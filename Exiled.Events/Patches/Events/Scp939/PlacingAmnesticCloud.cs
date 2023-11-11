// -----------------------------------------------------------------------
// <copyright file="PlacingAmnesticCloud.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp939
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp939;
    using Exiled.Events.Handlers;

    using HarmonyLib;
    using Mirror;
    using PlayerRoles.PlayableScps.Scp939;
    using PlayerRoles.Subroutines;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="Scp939AmnesticCloudAbility.ServerProcessCmd(NetworkReader)" />
    ///     to add the <see cref="Scp939.PlacingAmnesticCloud" /> event.
    /// </summary>
    [EventPatch(typeof(Scp939), nameof(Scp939.PlacingAmnesticCloud))]
    [HarmonyPatch(typeof(Scp939AmnesticCloudAbility), nameof(Scp939AmnesticCloudAbility.ServerProcessCmd))]
    internal static class PlacingAmnesticCloud
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(PlacingAmnesticCloudEventArgs));

            Label jump = generator.DefineLabel();

            int offset = 1;
            int index = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Stloc_1) + offset;

            newInstructions[index].labels.Add(jump);

            newInstructions.InsertRange(index, new[]
            {
                // Player::Get(Owner)
                new CodeInstruction(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Scp939AmnesticCloudAbility), nameof(Scp939AmnesticCloudAbility.Owner))),
                new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),

                // flag (NewState)
                new(OpCodes.Ldloc_0),

                // this.Cooldown.IsReady
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(Scp939AmnesticCloudAbility), nameof(Scp939AmnesticCloudAbility.Cooldown))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(AbilityCooldown), nameof(AbilityCooldown.IsReady))),

                // this._failedCooldown
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(Scp939AmnesticCloudAbility), nameof(Scp939AmnesticCloudAbility._failedCooldown))),

                // true
                new(OpCodes.Ldc_I4_1),

                // PlacingAmnesticCloudEventArgs ev = new(referenceHub, state, isReady, cooldown, isAllowed);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(PlacingAmnesticCloudEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev.LocalIndex),

                // Scp939.OnPlacingAmnesticCloud(ev);
                new(OpCodes.Call, Method(typeof(Scp939), nameof(Scp939.OnPlacingAmnesticCloud))),

                // if (ev.IsAllowed)
                //    goto Jump;
                new(OpCodes.Callvirt, PropertyGetter(typeof(PlacingAmnesticCloudEventArgs), nameof(PlacingAmnesticCloudEventArgs.IsAllowed))),
                new(OpCodes.Brtrue_S, jump),

                // __instance.TargetState = false;
                // return;
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldc_I4_0),
                new(OpCodes.Callvirt, PropertySetter(typeof(Scp939AmnesticCloudAbility), nameof(Scp939AmnesticCloudAbility.TargetState))),
                new(OpCodes.Ret),
            });

            // Remove this.Cooldown.IsReady from if (this.Cooldown.IsReady)
            offset = -12;
            index = newInstructions.FindLastIndex(i => i.operand == (object)Method(typeof(AbilityCooldown), nameof(AbilityCooldown.Trigger))) + offset;
            newInstructions.RemoveRange(index, 2);

            newInstructions.InsertRange(index, new[]
            {
                // ev.IsReady
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(PlacingAmnesticCloudEventArgs), nameof(PlacingAmnesticCloudEventArgs.Cooldown))),
            });

            // Remove this._failedCooldown from this.Cooldown.Trigger(this._failedCooldown)
            offset = -2;
            index = newInstructions.FindLastIndex(i => i.operand == (object)PropertyGetter(typeof(AbilityCooldown), nameof(AbilityCooldown.IsReady))) + offset;
            newInstructions.RemoveRange(index, 3);

            newInstructions.InsertRange(index, new[]
            {
                // ev.Cooldown
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(PlacingAmnesticCloudEventArgs), nameof(PlacingAmnesticCloudEventArgs.IsReady))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
