// -----------------------------------------------------------------------
// <copyright file="Attacking.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp106
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp106;
    using HarmonyLib;
    using PlayerRoles.PlayableScps.Scp106;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp106Attack.ServerShoot"/>
    /// to add <see cref="Handlers.Scp106.Attacking"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Scp106), nameof(Handlers.Scp106.Attacking))]
    [HarmonyPatch(typeof(Scp106Attack), nameof(Scp106Attack.ServerShoot))]
    internal class Attacking
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int offset = 3;
            int index = newInstructions.FindIndex(x => x.Calls(Method(typeof(Scp106Attack), nameof(Scp106Attack.VerifyShot)))) + offset;
            Label ret = generator.DefineLabel();

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // Player player = Player.Get(this.Owner);
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Scp106Attack), nameof(Scp106Attack.Owner))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // Player target = Player.Get(this._targetHub);
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(Scp106Attack), nameof(Scp106Attack._targetHub))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // AttackingEventArgs ev = new(player, target, true);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(AttackingEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Scp106.OnAttacking(ev);
                    new(OpCodes.Call, Method(typeof(Handlers.Scp106), nameof(Handlers.Scp106.OnAttacking))),

                    // if (!ev.IsAllowed)
                    //      return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(AttackingEventArgs), nameof(AttackingEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, ret),
                });
            newInstructions[newInstructions.Count - 1].labels.Add(ret);
            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}